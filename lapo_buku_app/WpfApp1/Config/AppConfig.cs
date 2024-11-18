using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp1.Config
{
    public class DbConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    public static class ConfigLoader
    {
        public static DbConfig LoadConfig(string filePath = "appsettings.json")
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Configuration file '{filePath}' not found.");
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<DbConfig>(json);
        }
    }
}
