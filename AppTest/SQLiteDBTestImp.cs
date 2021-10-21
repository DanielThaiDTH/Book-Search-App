using System;
using System.IO;
using SQLite;
using Book_Search_App;

namespace AppTest
{
    public class SQLiteDBTestImp : ISQLiteDB
    {
        public SQLiteAsyncConnection CreateSQLiteDB(string filename)
        {
            var document_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(document_path, filename);
            return new SQLiteAsyncConnection(path);
        }
    }
}
