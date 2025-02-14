using System.IO;
using System.Linq;
using System;

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

        static long GetTotalFolderSize(DirectoryInfo directoryInfo)
        {
            try
            {
                long totalSize = 0;

                // Add the size of all files in the current directory
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    totalSize += file.Length;
                }

                // Recursively add the size of files in subdirectories
                foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
                {
                    if (isJunction(subDirectory)) continue;
                    totalSize += GetTotalFolderSize(subDirectory);
                }

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

        static void Main(string[] args)
        {
            // get user input
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

                // if (!File.Exists(filePath)) continue;

                try
                {
                    long totalFileSize = 0;

                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] files = directoryInfo.GetFiles();
                    DirectoryInfo[] directories = directoryInfo.GetDirectories();

                    Console.WriteLine($"Listing Files in {directoryInfo.Name}:", Console.ForegroundColor = ConsoleColor.White);
                    foreach (FileInfo file in files)
                    {
                        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------", Console.ForegroundColor = ConsoleColor.White);
                        Console.Write("| ");

                        totalFileSize += file.Length;

                        try
                        {
                            Console.Write($"{FormatString(file.Name)}", Console.ForegroundColor = ConsoleColor.Yellow);
                            Console.Write($"{FormatBytes(file.Length)}", Console.ForegroundColor = ConsoleColor.Cyan);
                        }
                        catch (Exception ex)
                        {
                            Console.Write($"{ex.Message}", Console.ForegroundColor = ConsoleColor.Red);
                            Console.Write($"{FormatBytes(0)}", Console.ForegroundColor = ConsoleColor.Cyan);
                        }
                        Console.Write(" |\n", Console.ForegroundColor = ConsoleColor.White);
                    }
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------", Console.ForegroundColor = ConsoleColor.White);

                    Console.WriteLine($"\nListing Directories in {directoryInfo.Name}:");
                    foreach (DirectoryInfo directory in directories)
                    {
                        if (isJunction(directory)) continue;

                        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------", Console.ForegroundColor = ConsoleColor.White);
                        Console.Write("| ");
                        try
                        {
                            long folderSize = GetTotalFolderSize(directory);

                            totalFileSize += folderSize;

                            string formattedBytes = FormatBytes(folderSize);
                            Console.Write($"{FormatString(directory.Name)}", Console.ForegroundColor = ConsoleColor.Yellow);
                            Console.Write(formattedBytes, Console.ForegroundColor = ConsoleColor.Cyan);
                        }
                        catch (Exception ex)
                        {
                            Console.Write($"{FormatString($"Access Error: {directory.Name}")}", Console.ForegroundColor = ConsoleColor.Red);
                            Console.Write($"{FormatBytes(0)}", Console.ForegroundColor = ConsoleColor.Cyan);
                        }
                        Console.Write(" |\n", Console.ForegroundColor = ConsoleColor.White);
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
    }
}