using System;
using System.Diagnostics;
using System.IO;

namespace ExeVersion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                Console.Error.WriteLine("Missing parameter for exe path");
                PrintUsage();
                Environment.Exit(2);
            }

            try
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(args[0]);
                Console.Out.Write(versionInfo.ProductVersion);
            }
            catch(FileNotFoundException)
            {
                Console.Error.WriteLine("File " + args[0] + " not found.");
                PrintUsage();
                Environment.Exit(2);
            }
        }

        private static void PrintUsage()
        {
            Console.Error.WriteLine("Usage: ExeVersion <path>");
            Console.Error.WriteLine("Example: ExeVersion C:\\Windows\\System32\\notepad.exe");
        }
    }
}
