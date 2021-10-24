using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using SQLiteNetExtensions;
using Xamarin.Forms;
using System.Linq.Expressions;

namespace Book_Search_App
{
    public class DatabaseManager
    {
        SQLiteAsyncConnection aconn;

        public DatabaseManager()
        {
            aconn = DependencyService.Get<ISQLiteDB>().CreateSQLiteDB("books.db3");
        }

        public DatabaseManager(ISQLiteDB implementation, string filename)
        {
            aconn = implementation.CreateSQLiteDB(filename);
        }

        public async Task<ObservableCollection<WorkInfo>> InitWorksTable()
        {
            await aconn.CreateTableAsync<WorkInfo>();
            var savedWorks = await ReadOperations.GetAllWithChildrenAsync<WorkInfo>(aconn);
            return new ObservableCollection<WorkInfo>(savedWorks);
        }


        /// <summary>
        ///Async returning the number of rows changed with the given WorkInfo
        /// </summary>
        /// <param name="info">WorkInfo</param>
        /// <returns>Number of rows updated as an int</returns>
        public async void UpdateSavedWorks(WorkInfo info)
        {
            await aconn.UpdateWithChildrenAsync(info);
        }


        ///</summary>
        ///Adds information about a work to the DB. Does nothing if it already exists.
        ///<summary>
        public async Task<bool> AddSavedWork(WorkInfo info)
        {
            try {
                await aconn.InsertWithChildrenAsync(info);
            } catch (SQLiteException e) {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Gets the saved work with the givne OpenLibrary key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>WorkInfo</returns>
        public async Task<WorkInfo> GetSavedWork(string key)
        {
            try {
                var results = await aconn.GetAllWithChildrenAsync<WorkInfo>(w => w.key.Equals(key));
                
                if (results.Count == 0)
                    return null;

                return results[0];
            } catch (SQLiteException se) {
                return null;
            } catch (InvalidOperationException ie) {
                return null;
            }
        }

        //Deletes all saved data about works
        public async void DeleteWorksTable()
        {
            await aconn.DeleteAllAsync<WorkInfo>();
        }

        /// <summary>
        /// Deletes WorkInfo using UID.
        /// </summary>
        /// <param name="id">UID</param>
        /// <returns>Number of rows deleted</returns>
        public async Task<int> DeleteWork(int id)
        {
            try {
                return await aconn.DeleteAsync<WorkInfo>(id);
            } catch (NotSupportedException e) {
                return 0;
            }
        }


        public async Task<int> DeleteWork(string key)
        {
            try {
                string query = "DELETE FROM Work_Info WHERE key='" + key + "';";
                return await aconn.ExecuteAsync(query);
            }
            catch (NotSupportedException e) {
                return 0;
            }
        }


        //Drops works table. To be used when the table is malformed
        public async void DropWorksTable()
        {
            await aconn.DropTableAsync<WorkInfo>();
        }
    }
}
