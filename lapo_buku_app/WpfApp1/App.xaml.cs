using DotNetEnv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Env.Load();  // Loads .env file

            // Optional: Test if it's loaded correctly
            Console.WriteLine(Environment.GetEnvironmentVariable("DB_HOST"));
        }
    }
}
