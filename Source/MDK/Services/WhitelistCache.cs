﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace MDK.Services
{
    /// <summary>
    /// A service for refreshing the Space Engineers ingame script whitelist cache file.
    /// </summary>
    public class WhitelistCache
    {
        /// <summary>
        /// Start space engineers with a dedicated plugin designed to update the ingame script whitelist cache file.
        /// </summary>
        public void Refresh()
        {
            var steam = new Steam();
            if (!steam.Exists)
            {
                throw new NotImplementedException();
            }
            var appId = SpaceEngineers.SteamAppId;
            var pluginPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath) ?? ".", "MDKWhitelistExtractor.dll");
            var targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Malware", "SpaceEngineers", "Toolkit");
            var directoryInfo = new DirectoryInfo(targetPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            targetPath = Path.Combine(targetPath, "whitelist.cache");

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Malware\DevKit\SpaceEngineers", "WhitelistPath", targetPath);

            var args = new List<string>
            {
                $"-applaunch {appId}",
                $"-plugin \"{pluginPath}\"",
                "-nosplash",
                "-whitelistcaches",
                $"\"{targetPath}\""
            };

            var process = new Process
            {
                StartInfo =
                {
                    FileName = steam.ExePath,
                    Arguments = string.Join(" ", args)
                },
                EnableRaisingEvents = true
            };
            process.Start();
        }
    }
}
