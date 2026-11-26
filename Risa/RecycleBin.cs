using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic.FileIO;

public static class RecycleBinService
{
    public static void MoveToRecycleBin(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException("Файл или папка не найдены", path);

        if (OperatingSystem.IsWindows())
        {
            if (File.Exists(path))
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            else
                FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }
        else if (OperatingSystem.IsLinux())
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "gio",
                ArgumentList = { "trash", path },
                CreateNoWindow = true
            });
            process?.WaitForExit();
        }
        else if (OperatingSystem.IsMacOS())
        {
            var escapedPath = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "osascript",
                ArgumentList = { "-e", $"tell application \"Finder\" to delete POSIX file \"{escapedPath}\"" },
                CreateNoWindow = true
            });
            process?.WaitForExit();
        }
        else
        {
            throw new PlatformNotSupportedException("Удаление в корзину не поддерживается на этой ОС.");
        }
    }
}

