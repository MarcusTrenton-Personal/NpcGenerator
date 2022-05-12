using System;
using System.Diagnostics;

namespace ExeVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                Console.Error.WriteLine("Missing parameter for exe path");
                Environment.Exit(2);
            }

            var versionInfo = FileVersionInfo.GetVersionInfo(args[0]);
            if(args.Length >= 2 && args[1] == "-Major.Minor")
            {
                string[] parts = versionInfo.FileVersion.Split('.');
                if(parts.Length < 2)
                {
                    Console.Error.WriteLine("Version is malformed with less than 2 parts.");
                    Environment.Exit(2);
                }
                Console.Out.Write(parts[0]+"."+parts[1]);
            }
            else
            {
                Console.Out.Write(versionInfo.FileVersion);
            }
            
        }
    }
}
