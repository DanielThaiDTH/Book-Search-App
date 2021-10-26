using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Book_Search_App;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;

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
            Assert.AreEqual(results.numFound, 351);
            Assert.AreEqual(results.docs.Count, 22);
        }

        [TestMethod]
        [Description("Changing the return limit changes the amount of results returned. " +
            "Limits restricted from 1 to 100.")]
        public async Task ReturnLimitChangeTest()
        {
            wait(500);
            connector.ReturnLimit = 25;
            results = await connector.searchBooks("algebra");
            Assert.AreEqual(results.docs.Count, connector.ReturnLimit);
            wait(500);
            connector.ReturnLimit = 1000;
            results = await connector.searchBooks("algebra");
            Assert.AreEqual(results.docs.Count, 100);
            wait(500);
            connector.ReturnLimit = 1;
            results = await connector.searchBooks("the lord of the rings");
            Assert.AreEqual(results.docs.Count, connector.ReturnLimit);
            wait(500);
            connector.ReturnLimit = 0;
            connector.ReturnLimit = -50;
            results = await connector.searchBooks("the lord of the rings");
            connector.ReturnLimit = 25;
            Assert.AreEqual(results.docs.Count, 1);
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

            connector.Option = SearchType.REGULAR;
            Assert.AreNotEqual(rs1.numFound, rs2.numFound);
            Assert.AreNotEqual(rs1.numFound, rs3.numFound);
            Assert.AreNotEqual(rs1.numFound, rs4.numFound);
            Assert.AreNotEqual(rs2.numFound, rs3.numFound);
            Assert.AreNotEqual(rs2.numFound, rs4.numFound);
            Assert.AreNotEqual(rs3.numFound, rs4.numFound);
        }

        [TestMethod]
        public async Task TestSearchEditons()
        {
            wait(500);
            List<string> keys = new List<string> { "OL9158229M", "OL1017798M" };
            IDictionary<string, EditionSummary> results = await connector.searchEditions(keys);

            ICollection<string> return_keys = results.Keys;
            Assert.IsTrue(return_keys.Contains("OLID:" + keys[0]));
            Assert.IsTrue(return_keys.Contains("OLID:" + keys[1]));

            Assert.AreEqual(results["OLID:" + keys[0]].title, "Hobbit; o Senhor dos Anéis");

            Assert.AreEqual(results["OLID:" + keys[1]].title, "The adventures of Tom Sawyer");
            Assert.AreEqual(results["OLID:" + keys[1]].author_list[0], "Mark Twain");
            Assert.AreEqual(results["OLID:" + keys[1]].isbn10, "0451526538");
            Assert.AreEqual(results["OLID:" + keys[1]].publish_places_list[0], "New York");
        }

        [TestMethod]
        public async Task QueryEditionTest()
        {
            wait(500);
            string query = "/books/OL1017798M";
            EditionInfo info = await connector.queryEdition(query);

            Assert.AreEqual(info.isbn10, "0451526538");
            Assert.AreEqual(info.publish_places[0], "New York");
            Assert.AreEqual(info.genres[0], "Fiction.");
            Assert.AreEqual(info.covers[0], 11403183);
            Assert.AreEqual(info.title, "The adventures of Tom Sawyer");
        }
    }
}
