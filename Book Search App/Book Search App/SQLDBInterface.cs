using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Book_Search_App
{
    public interface ISQLiteDB
    {
        SQLiteAsyncConnection CreateSQLiteDB(string filename);
    }
}
