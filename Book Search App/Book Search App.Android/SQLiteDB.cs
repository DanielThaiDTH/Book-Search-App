using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using Book_Search_App;

[assembly: Dependency(typeof(Book_Search_App.Droid.SQLiteDB))]
namespace Book_Search_App.Droid
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