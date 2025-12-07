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
        //if found, check for steam_settings directory
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
        return Directory.Exists(steamSettings);
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
