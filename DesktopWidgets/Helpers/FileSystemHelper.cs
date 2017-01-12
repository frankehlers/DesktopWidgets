﻿#region

using IWshRuntimeLibrary;
using File = System.IO.File;

#endregion

namespace DesktopWidgets.Helpers
{
    internal static class FileSystemHelper
    {
        public static string GetShortcutTargetFile(string path)
        {
            if (!File.Exists(path))
            {
                return path;
            }
            var shell = new WshShell();
            var link = (IWshShortcut) shell.CreateShortcut(path);
            return File.Exists(link.TargetPath) ? link.TargetPath : path;
        }
    }
}