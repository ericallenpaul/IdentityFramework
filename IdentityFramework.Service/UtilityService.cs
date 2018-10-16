using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IdentityFramework.Service
{
    public static class UtilityService
    {
        public static void WriteTextFileWithBackup(string FileName, string Directory, int NumberOfFilesToKeep, string FileContents)
        {
            //get the full name and path of the file to be written
            string newestFileName = $"{Directory}/{FileName}(1).txt";
            string oldestFileName = $"{Directory}/{FileName}({NumberOfFilesToKeep}).txt";

            //if the oldest file exists delete it
            if (File.Exists(oldestFileName))
            {
                File.Delete(oldestFileName);
            }

            //rename existing files, deleting the highest number file
            for (int i = NumberOfFilesToKeep - 1; i > 0; i--)
            {
                string fullName = $"{Directory}/{FileName}({i}).txt";
                string moveName = $"{Directory}/{FileName}({i + 1}).txt";

                if (File.Exists(fullName))
                {
                    File.Move(fullName, moveName);
                }
            }

            File.WriteAllText(newestFileName, FileContents);
        }


    }
}
