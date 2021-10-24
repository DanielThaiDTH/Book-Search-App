using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Book_Search_App;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            dbm.DeleteWorksTable();
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

        [TestMethod]
        public async Task DeleteWorkTest()
        {
            WorkInfo info = new WorkInfo
            {
                key = "/3",
                Desc = "Description. End.",
                covers = new List<int> { 1, 2, 3 }
            };
            WorkInfo unadded_info = new WorkInfo
            {
                key = "/4",
                Desc = "Description. End.",
                covers = new List<int> { 1, 2, 3 }
            };

            await dbm.AddSavedWork(info);

            //Primary key auto updates, so can be used immediately after adding
            Assert.AreEqual(1, await dbm.DeleteWork(info.UID));
            Assert.AreEqual(0, await dbm.DeleteWork(unadded_info.UID));

            //Info no longer in table
            info = await dbm.GetSavedWork(info.key);
            Assert.IsNull(info);
        }

        [TestMethod]
        public async Task GetWorkTest()
        {
            WorkInfo info = new WorkInfo
            {
                key = "/4",
                Desc = "Description. End.",
                covers = new List<int> { 1, 2, 3 }
            };

            Assert.IsTrue(await dbm.AddSavedWork(info));
            Assert.IsFalse(await dbm.AddSavedWork(info)); //Re-adding will fail
            Assert.AreEqual(info, await dbm.GetSavedWork(info.key));
            Assert.AreNotEqual<WorkInfo>(new WorkInfo(), await dbm.GetSavedWork(info.key));
        }


        [ClassCleanup]
        public static void cleanup()
        {
            dbm.DeleteWorksTable();
        }
    }
}
