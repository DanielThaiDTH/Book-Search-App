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

        [ClassInitialize]
        public static void initialize(TestContext context)
        {
            dbm = new DatabaseManager(dbImp, "test.db3");
        }


        [TestMethod]
        public async Task InitializationTest()
        {
            await dbm.DropWorksTable();
            await dbm.DropAuthorsTable();
            await dbm.InitWorksTable();
            await dbm.InitAuthorsTable();
            await dbm.DeleteWorksTable();
            await dbm.DeleteAuthorsTable();
        }

        [TestMethod]
        public async Task InsertWorkTest()
        {
            WorkInfo info = new WorkInfo { 
                key = "/1",
                Desc = "Description. End.",
                covers = new List<int> { 1, 2, 3 } 
            };

            Assert.IsTrue(await dbm.AddInfo(info));
            Assert.IsFalse(await dbm.AddInfo(info)); //Should fail, work already in DB
        }

        [TestMethod]
        public async Task DeleteWorkTest()
        {
            WorkInfo info = new WorkInfo
            {
                key = "/3",
                Desc = "Description. End.",
                covers = new List<int> { 1, 2, 3 },
            };
            WorkInfo second_info = new WorkInfo
            {
                key = "/4",
                Desc = "Description. End.",
                covers = new List<int> { 1, 2, 3 }
            };

            await dbm.AddInfo(info);

            //Primary key auto updates, so can be used immediately after adding
            Assert.AreEqual(1, await dbm.DeleteWork(info.UID));
            Assert.AreEqual(0, await dbm.DeleteWork(second_info.UID));

            //Test deletion using OpenLibrary key
            await dbm.AddInfo(second_info);
            Assert.AreEqual(1, await dbm.DeleteWork(second_info.key));

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

            Assert.IsTrue(await dbm.AddInfo(info));
            Assert.IsFalse(await dbm.AddInfo(info)); //Re-adding will fail
            Assert.AreEqual(info, await dbm.GetSavedWork(info.key));
            Assert.AreNotEqual<WorkInfo>(new WorkInfo(), await dbm.GetSavedWork(info.key));
            await dbm.DeleteWork(info.UID);
        }

        [TestMethod]
        public async Task TestworkDeserialization()
        {
            WorkInfo info = new WorkInfo
            {
                key = "/5",
                Desc = "Description. End.",
                covers = new List<int> { 1, 2, 3 },
                subjects = new List<string> { "good", "bad" },
                subject_places = new List<string> { }
            };

            await dbm.AddInfo(info);
            WorkInfo copy = await dbm.GetSavedWork(info.key);
            Assert.AreEqual(info.covers.Count, copy.covers.Count);
            Assert.AreEqual(info.subjects.Count, copy.subjects.Count);
            Assert.AreEqual(info.subject_places.Count, copy.subject_places.Count);

            for (var i = 0; i < info.covers.Count; i++) {
                Assert.AreEqual(info.covers[i], copy.covers[i]);
            }

            for (var i = 0; i < info.subjects.Count; i++) {
                Assert.AreEqual(info.subjects[i], copy.subjects[i]);
            }

            for (var i = 0; i < info.subject_places.Count; i++) {
                Assert.AreEqual(info.subject_places[i], copy.subject_places[i]);
            }
        }

        /*Authors Portion*/
        [TestMethod]
        public async Task AddGetAuthorsTest()
        {
            Author a = new Author
            {
                key = "/1",
                name = "test",
                photos = new List<int> { 1, 2, 3 }
            };

            Assert.IsTrue(await dbm.AddInfo(a));
            Assert.IsFalse(await dbm.AddInfo(a));
            Author copy = await dbm.GetSavedAuthor("/1");
            Assert.AreEqual(copy.name, a.name);
            Assert.AreEqual(copy.photos.Count, a.photos.Count);
            Assert.AreEqual(copy.photos[0], a.photos[0]);
        }


        [ClassCleanup]
        public static void cleanup()
        {
            dbm.DeleteWorksTable();
            dbm.DeleteAuthorsTable();
        }
    }
}
