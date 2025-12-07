using System.IO;
using System.Threading.Tasks;
using Nitrox.Model.Helper;
using Nitrox.Model.Platforms.Discovery.Models;
using Nitrox.Model.Platforms.OS.Shared;
using Nitrox.Model.Platforms.Store.Interfaces;

namespace Nitrox.Model.Platforms.Store;
public sealed class Standalone : IGamePlatform
{
    public string Name => "Standalone";
    public Platform Platform => Platform.NONE;

    public bool OwnsGame(string gameDirectory)
    {
        if (!Directory.Exists(gameDirectory))
        {
            return false;
        }

        string executable = Path.Combine(gameDirectory, "Subnautica.exe");
        return File.Exists(executable);
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