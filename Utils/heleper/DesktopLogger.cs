using System;
using System.IO;

namespace AnilTools 
{
    public static class DesktopLogger
    {
        public static void Log(string message)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Bukralog.txt");
            
            if (!File.Exists(path))
            {
                using var stream = File.CreateText(path);
                stream.WriteLine(message);
            }
            else
            {
                using var stream = File.AppendText(path);
                stream.WriteLine(message);
            }
        }
    }
}
