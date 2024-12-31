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
            return string.Format("{0:0.##} {1}", len, sizes[order]);
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
            long totalSize = 0;

            // Add the size of all files in the current directory
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                totalSize += file.Length;
            }

            // Recursively add the size of files in subdirectories
            foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
            {
                totalSize += GetTotalFolderSize(subDirectory);
            }

            return totalSize;
        }

        static void Main(string[] args)
        {
            // get user input
            while (true)
            {
                try
                {
                    Console.Write("\nEnter a file path: ");
                    string? filePath = Console.ReadLine();

                    if (filePath == null) continue;

                    // if (!File.Exists(filePath)) continue;

                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] files = directoryInfo.GetFiles();
                    DirectoryInfo[] directories = directoryInfo.GetDirectories();

                    Console.WriteLine($"Listing Files in {directoryInfo.Name}:");
                    foreach (FileInfo file in files)
                    {
                        Console.WriteLine("---");
                        Console.WriteLine($"{FormatString(file.Name)} {FormatBytes((int)file.Length)}");
                        Console.WriteLine("---");
                    }

                    Console.WriteLine($"\nListing Directories in {directoryInfo.Name}:");
                    foreach (DirectoryInfo directory in directories)
                    {
                        Console.WriteLine("---");
                        Console.WriteLine($"{FormatString(directory.Name)} {FormatBytes(GetTotalFolderSize(directory))}");
                        Console.WriteLine("---");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}