using System;
using System.IO;
using Avalonia.NETCoreMVVMApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Avalonia.NETCoreMVVMApp.DatabaseStuff
{
    public class DataContext : DbContext
    {
        public DbSet<DataViewModel> Datas { get; set; }

        public string DbPath { get; private set; }

        public DataContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var test = $"{path}{System.IO.Path.DirectorySeparatorChar}test.db";
            DbPath = Path.Combine(path, "test.db");
            Database.EnsureCreated();
            Console.WriteLine(Equals(DbPath, test));
        }
        
        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}