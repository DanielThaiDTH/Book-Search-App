using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Book_Search_App;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Http;

namespace AppTest
{
    [TestClass]
    public class NetworkingManagerTest
    {
        private static SearchResults results = null;
        private static NetworkingManager connector = null;

        [ClassInitialize]
        public static void initialize(TestContext context)
        {
            connector = new NetworkingManager();
        }

        /*Used to prevent overloading the OpenLibrary API*/
        private void wait(int ms)
        {
            try {
                Thread.Sleep(ms);
            } catch (ThreadInterruptedException e) {
                Console.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public async Task ConnectionTest()
        {
            results = await connector.searchBooks("the lord of the rings");
            Assert.IsNotNull(results);
        }

        [TestMethod]
        [Description("Test query 'the lord of the rings' returns the right amount of results. " +
            "If numFound is not equal it maybe that OpenLibrary added or removed a book.")]
        public async Task ResultTest()
        {
            wait(500);
            results = await connector.searchBooks("the lord of the rings");
            Assert.AreEqual(results.numFound, 487);
            Assert.AreEqual(results.docs.Count, 10);
        }

        [TestMethod]
        [Description("Changing the return limit changes the amount of results returned. " +
            "Limits restricted from 1 to 100.")]
        public async Task ReturnLimitChangeTest()
        {
            wait(500);
            connector.ReturnLimit = 50;
            results = await connector.searchBooks("the lord of the rings");
            Assert.AreEqual(results.docs.Count, 50);
            wait(500);
            connector.ReturnLimit = 1000;
            results = await connector.searchBooks("the lord of the rings");
            Assert.AreEqual(results.docs.Count, 100);
            wait(500);
            connector.ReturnLimit = 1;
            results = await connector.searchBooks("the lord of the rings");
            Assert.AreEqual(results.docs.Count, 1);
            wait(500);
            connector.ReturnLimit = 0;
            connector.ReturnLimit = -50;
            results = await connector.searchBooks("the lord of the rings");
            Assert.AreEqual(results.docs.Count, 1);
            connector.ReturnLimit = 10;
        }

        [TestMethod]
        public async Task SearchOptionTest()
        {
            wait(500);
            SearchResults rs1 = await connector.searchBooks("Tolkien");
            wait(500);
            connector.Option = SearchType.AUTHOR;
            SearchResults rs2 = await connector.searchBooks("Tolkien");
            wait(500);
            connector.Option = SearchType.TITLE;
            SearchResults rs3 = await connector.searchBooks("Tolkien");
            wait(500);
            connector.Option = SearchType.ISBN;
            SearchResults rs4 = await connector.searchBooks("Tolkien");

            Assert.AreNotEqual(rs1.numFound, rs2.numFound);
            Assert.AreNotEqual(rs1.numFound, rs3.numFound);
            Assert.AreNotEqual(rs1.numFound, rs4.numFound);
            Assert.AreNotEqual(rs2.numFound, rs3.numFound);
            Assert.AreNotEqual(rs2.numFound, rs4.numFound);
            Assert.AreNotEqual(rs3.numFound, rs4.numFound);
            connector.Option = SearchType.REGULAR;
        }
    }
}
