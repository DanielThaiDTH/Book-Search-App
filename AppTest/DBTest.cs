using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Book_Search_App;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace AppTest
{
    [TestClass]
    public class DBTest
    {
        private static SQLiteDBTestImp dbImp = new SQLiteDBTestImp();
        private static DatabaseManager dbm = null;
        private static ObservableCollection<WorkInfo> work_info;

        [ClassInitialize]
        public static void initialize(TestContext context)
        {
            dbm = new DatabaseManager(dbImp, "test.db3");
        }


        [TestMethod]
        public async Task InitializationTest()
        {
            await dbm.InitWorksTable();
        }

        [TestMethod]
        public async Task InsertWorkTest()
        {
            WorkInfo info = new WorkInfo { 
                key = "/1",
                Desc = "Description. End.",
                covers = new List<int> { 1, 2, 3 } 
            };

            Assert.IsTrue(await dbm.AddSavedWork(info));
            Assert.IsFalse(await dbm.AddSavedWork(info)); //Should fail, work already in DB
        }

        [ClassCleanup]
        public static void cleanup()
        {
            dbm.DeleteWorksTable();
        }
    }
}
