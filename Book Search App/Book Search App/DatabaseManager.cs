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

        public async Task<ObservableCollection<Author>> InitAuthorsTable()
        {
            await aconn.CreateTableAsync<Author>();
            var saved = await ReadOperations.GetAllWithChildrenAsync<Author>(aconn);
            return new ObservableCollection<Author>(saved);
        }

        /// <summary>
        ///Async returning the number of rows changed with the given type
        /// </summary>
        /// <param name="T">Type</param>
        /// <returns>Number of rows updated as an int</returns>
        public async void UpdateInfo<T>(T info)
        {
            await aconn.UpdateWithChildrenAsync(info);
        }


        ///</summary>
        ///Adds information about a book/author to the DB. Does nothing if it already exists.
        ///<summary>
        public async Task<bool> AddInfo<T>(T info)
        {
            try {
                await aconn.InsertWithChildrenAsync(info);
            } catch (SQLiteException e) {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Gets the saved work with the given OpenLibrary key.
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

        /// <summary>
        /// Finds an author using the given OpenLibrary key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Author> GetSavedAuthor(string key)
        {
            try {
                var results = await aconn.GetAllWithChildrenAsync<Author>(a => a.key.Equals(key));

                if (results.Count == 0)
                    return null;

                return results[0];
            }
            catch (SQLiteException se) {
                return null;
            }
            catch (InvalidOperationException ie) {
                return null;
            }
        }



        //Deletes all saved data about works
        public async Task DeleteWorksTable()
        {
            await aconn.DeleteAllAsync<WorkInfo>();
        }

        //Deletes all saved data about authors
        public async Task DeleteAuthorsTable()
        {
            await aconn.DeleteAllAsync<Author>();
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

        /// <summary>
        /// Deletes work using OpenLibrary key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Deletes author using UID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAuthor(int id)
        {
            try {
                return await aconn.DeleteAsync<Author>(id);
            }
            catch (NotSupportedException e) {
                return 0;
            }
        }

        /// <summary>
        /// Delete author using OpenLibrary key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<int> DeleteAuthor(string key)
        {
            try {
                string query = "DELETE FROM Author WHERE key='" + key + "';";
                return await aconn.ExecuteAsync(query);
            }
            catch (NotSupportedException e) {
                return 0;
            }
        }



        //Drops works table. To be used when the table is malformed.
        public async Task DropWorksTable()
        {
            await aconn.DropTableAsync<WorkInfo>();
        }

        //Drops authors table. To be used when the table is malformed.
        public async Task DropAuthorsTable()
        {
            await aconn.DropTableAsync<Author>();
        }
    }
}
