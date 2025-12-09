using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Nitrox.Model.Helper;
using Nitrox.Model.Platforms.Discovery.Models;
using Nitrox.Model.Platforms.OS.Shared;
using Nitrox.Model.Platforms.Store.Interfaces;

namespace Nitrox.Model.Platforms.Store;

public sealed class Goldberg : IGamePlatform
{
    public string Name => "Goldberg";
    public Platform Platform => Platform.GOLDBERG;

    public bool OwnsGame(string gameDirectory)
    {
        if (!Directory.Exists(gameDirectory))
        {
            return false;
        }
        
        //similarly to steam, find steam_api64.dll
        string plugins = Path.Combine(gameDirectory, GameInfo.Subnautica.DataFolder, "Plugins");
        string dllPath1 = Path.Combine(plugins, "x86_64", "steam_api64.dll");
        string dllPath2 = Path.Combine(plugins, "steam_api64.dll");

        string foundDll = null;
        if (File.Exists(dllPath1))
        {
            foundDll = dllPath1;
        }
        else if (File.Exists(dllPath2))
        {
            foundDll = dllPath2;
        }

        if (foundDll == null)
        {
            return false;
        }

        string dllDir = Path.GetDirectoryName(foundDll) ?? plugins;
        string steamSettings = Path.Combine(dllDir, "steam_settings");
        if (Directory.Exists(steamSettings))
        {
            // steam_settings folder is a clear indicator for configured GSE
            Log.Debug("Goldberg detected via steam_settings folder");
            return true;
        }

        // check if DLL is not the standard Steam one
        // there shouldn't be ANY false positives here unless the GSE dll is disguised as the steam one (unlikely)
        // however, results cannot be guaranteed(?) it's just EXTREMELY likely that it will work right
        try
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(foundDll);
            if (versionInfo != null)
            {
                string productName = (versionInfo.ProductName ?? string.Empty).Trim();
                if (productName != "Steam Client API")
                {
                    Log.Debug($"Goldberg detected via steam_api64.dll product name: {productName}");
                    return true;
                }
            }
        }
        catch
        {
            // if inspection fails, assume not GSE
        }

        return false;
    }

    public static async Task<ProcessEx> StartGameAsync(string pathToGameExe, string launchArguments)
    {
        return await Task.FromResult(
            ProcessEx.Start(
                pathToGameExe,
                [(NitroxUser.LAUNCHER_PATH_ENV_KEY, NitroxUser.LauncherPath)],
                Path.GetDirectoryName(pathToGameExe),
                launchArguments
            )
        );
    }
}
