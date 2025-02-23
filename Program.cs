using System.IO;
using System.Linq;
using System;
using System.Collections.Concurrent;

namespace FileReader
{
    class Program
    {
        static string FormatBytes(long bytes)
        {
            // format bytes into KB, MB, GB, TB
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            string formattedSize = string.Format("{0:0.##} {1}", len, sizes[order]);

            while (formattedSize.Length < 9)
            {
                formattedSize = " " + formattedSize;
            }

            return formattedSize;
        }

        static string FormatString(string stringToFormat)
        {
            while (stringToFormat.Length < 100)
            {
                stringToFormat += " ";
            }

            return stringToFormat;
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("\nEnter a file path: ", Console.ForegroundColor = ConsoleColor.White);
                string? filePath = Console.ReadLine();

                if (filePath == "clear;")
                {
                    Console.Clear();
                    continue;
                }

                if (filePath == null) continue;

                try
                {
                    long totalFileSize = 0;

                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] files = directoryInfo.GetFiles();
                    DirectoryInfo[] directories = directoryInfo.GetDirectories();

                    var fileTasks = new ConcurrentDictionary<int, string>();
                    var dirTasks = new ConcurrentDictionary<int, string>();

                    Console.WriteLine($"Listing Files in {directoryInfo.Name}:", Console.ForegroundColor = ConsoleColor.White);
                    Parallel.ForEach(files, (file, state, index) =>
                    {
                        fileTasks[(int)index] = GenerateFileOutput(file, ref totalFileSize);
                    });

                    foreach (var item in fileTasks.OrderBy(kvp => kvp.Key))
                    {
                        Console.WriteLine(item.Value, Console.ForegroundColor = ConsoleColor.White);
                    }
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------", Console.ForegroundColor = ConsoleColor.White);

                    Console.WriteLine($"\nListing Directories in {directoryInfo.Name}:");
                    Parallel.ForEach(directories, (directory, state, index) =>
                    {
                        if (isJunction(directory)) return;
                        dirTasks[(int)index] = GenerateDirectoryOutput(directory, ref totalFileSize);
                    });

                    foreach (var item in dirTasks.OrderBy(kvp => kvp.Key))
                    {
                        Console.WriteLine(item.Value, Console.ForegroundColor = ConsoleColor.White);
                    }
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------", Console.ForegroundColor = ConsoleColor.White);
                    Console.WriteLine("");
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------", Console.ForegroundColor = ConsoleColor.White);
                    Console.Write("| ", Console.ForegroundColor = ConsoleColor.White);
                    Console.Write($"{FormatString("Ruff Total Size")}", Console.ForegroundColor = ConsoleColor.Green);
                    Console.Write($"{FormatBytes(totalFileSize)}", Console.ForegroundColor = ConsoleColor.Magenta);
                    Console.Write(" |\n", Console.ForegroundColor = ConsoleColor.White);
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------", Console.ForegroundColor = ConsoleColor.White);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, Console.ForegroundColor = ConsoleColor.Red);
                }
            }
        }

        static string GenerateFileOutput(FileInfo file, ref long totalFileSize)
        {
            totalFileSize += file.Length;
            try
            {
                return string.Format("| {0}{1} |", FormatString(file.Name), FormatBytes(file.Length));
            }
            catch (Exception ex)
            {
                return string.Format("| {0}{1} |", FormatString(ex.Message), FormatBytes(0));
            }
        }

        static string GenerateDirectoryOutput(DirectoryInfo directory, ref long totalFileSize)
        {
            try
            {
                long folderSize = GetTotalFolderSize(directory);
                totalFileSize += folderSize;
                return string.Format("| {0}{1} |", FormatString(directory.Name), FormatBytes(folderSize));
            }
            catch (Exception ex)
            {
                return string.Format("| {0}{1} |", FormatString($"Access Error: {directory.Name}"), FormatBytes(0));
            }
        }

        static long GetTotalFolderSize(DirectoryInfo directoryInfo)
        {
            try
            {
                long totalSize = 0;

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    totalSize += file.Length;
                }

                Parallel.ForEach(directoryInfo.GetDirectories(), (subDirectory) =>
                {
                    if (isJunction(subDirectory)) return;
                    Interlocked.Add(ref totalSize, GetTotalFolderSize(subDirectory));
                });

                return totalSize;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        static bool isJunction(DirectoryInfo dir)
        {
            return dir.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }
    }
}
