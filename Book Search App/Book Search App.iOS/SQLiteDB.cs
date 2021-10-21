using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(Book_Search_App.iOS.SQLiteDB))]
namespace Book_Search_App.iOS
{
    public class SQLiteDB : ISQLiteDB
    {
        public SQLiteAsyncConnection CreateSQLiteDB(string filename)
        {
            var document_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(document_path, filename);
            return new SQLiteAsyncConnection(path);
        }
    }
}