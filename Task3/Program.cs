using System;
using System.IO;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите нужную папку");
            string folderPath = Console.ReadLine();

            long size = GetDirectorySize(folderPath);

            try
            {
                Console.WriteLine($"Исходный размер папки: {size} байт");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка {ex}");
            }
            var (deletedFilesCount, freeSpace) = CleanOldFilesAndFolders(folderPath);
            Console.WriteLine($"Освобождено: {size} байт");
            Console.WriteLine($"Текущий размер папки: {freeSpace}");
        }

        static (int,long) CleanOldFilesAndFolders(string folderPath)
        {
            TimeSpan expirationTime = TimeSpan.FromMinutes(30);
            DateTime currentTime = DateTime.Now;

            int deletedFilesCount = 0;
            long freeSpace = 0;

            foreach (var  file in Directory.GetFiles(folderPath))
            {
                DateTime lastAccessTime = File.GetLastAccessTime(file);

                if (currentTime -  lastAccessTime > expirationTime)
                {
                    long fileSize = GetDirectorySize(file);
                    File.Delete(file);
                    Console.WriteLine($"Удален файл: {file}");
                    deletedFilesCount++;
                    freeSpace += fileSize;
                }
            }

            foreach (var directory in Directory.GetDirectories(folderPath))
            {
                DateTime lastAccessTime = Directory.GetLastAccessTime(directory);

                if (currentTime - lastAccessTime > expirationTime)
                {
                    long directorySize = GetDirectorySize(directory);
                    Directory.Delete(directory, true);
                    Console.WriteLine($"Удалена папка: {directory}");
                    deletedFilesCount++;
                    freeSpace += directorySize;
                }
            }
            return (deletedFilesCount, freeSpace);
        }

        static long GetDirectorySize(string folderPath)
        {
            long size = 0;

            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            if(directoryInfo.Exists )
            {
                FileInfo[] files = directoryInfo.GetFiles();
                foreach( FileInfo file in files )
                {
                    size += file.Length;
                }

                DirectoryInfo[] dirs = directoryInfo.GetDirectories();
                foreach( DirectoryInfo dir in dirs )
                {
                    size += GetDirectorySize(dir.FullName);
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"Директория {folderPath} не была найдена");
            }
            return size;
        }
    }
}