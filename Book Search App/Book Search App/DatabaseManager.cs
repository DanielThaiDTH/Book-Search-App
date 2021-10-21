using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text;
using SQLite;
using Xamarin.Forms;

namespace Book_Search_App
{
    public class DatabaseManager
    {
        SQLiteAsyncConnection aconn;

        public DatabaseManager()
        {
            aconn = DependencyService.Get<ISQLiteDB>().CreateSQLiteDB("books.db3");
        }

        public DatabaseManager(ISQLiteDB implmentation, string filename)
        {
            aconn = implmentation.CreateSQLiteDB(filename);
        }

        public async Task<ObservableCollection<WorkInfo>> InitWorksTable()
        {
            await aconn.CreateTableAsync<WorkInfo>();
            var savedWorks = await aconn.Table<WorkInfo>().ToListAsync();
            return new ObservableCollection<WorkInfo>(savedWorks);
        }

        //Async returning the number of rows changed with the given WorkInfo
        public async Task<int> UpdateSavedWorks(WorkInfo info)
        {
            return await aconn.UpdateAsync(info);
        }

        //Adds information about a work to the DB. Does nothing if it already exists.
        public async Task<bool> AddSavedWork(WorkInfo info)
        {
            try {
                await aconn.InsertAsync(info);
            } catch (SQLiteException e) {
                return false;
            }
            return true;
        }

        //Deletes all saved data about works
        public async void DeleteWorksTable()
        {
            await aconn.DeleteAllAsync<WorkInfo>();
        }
    }
}
