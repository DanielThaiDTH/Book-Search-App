using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Book_Search_App
{
    public class BookInfoManager
    {
        //Search results
        ObservableCollection<BookSearchInfo> search_results;
        public ObservableCollection<BookSearchInfo> Search_Results => search_results;

        ObservableCollection<AuthorWorkSearchInfo> by_author_search_results;
        public ObservableCollection<AuthorWorkSearchInfo> By_Author_Search_Results => by_author_search_results;
        public string PreviousAuthorWorkSearchKey { get; set; }

        ObservableCollection<Author> authors_found;
        public ObservableCollection<Author> Authors_Found
        { 
            get { return authors_found; }
            set
            {
                if (value == null || authors_found.GetType() != value.GetType())
                    return;

                authors_found = value;
            }
        }
        

        //Cache of visted works to reduce the amount of API calls
        ObservableCollection<KeyValuePair<string, WorkInfo>> visited_works;
        public ObservableCollection<KeyValuePair<string, WorkInfo>> VisitedCache => visited_works;
        int cache_count = 0;
        readonly int MAXCACHE = 20;

        //Saved books(works)
        ObservableCollection<WorkInfo> saved_works;
        public ObservableCollection<WorkInfo> Saved_Works
        {
            get { return saved_works; }
            set 
            {
                if (value == null || saved_works.GetType() != saved_works.GetType())
                    return;

                saved_works = value;
            }
        }

        //Saved authors
        ObservableCollection<Author> _authors;
        public ObservableCollection<Author> Authors
        {
            get { return _authors; }
            set
            {
                if (value == null)
                    return;

                _authors = value;
            }
        }

        class PreviousQuery
        {
            public SearchType Option { get; set; }
            public string Query { get; set; }
        }

        PreviousQuery previous;


        public BookInfoManager()
        {
            saved_works = new ObservableCollection<WorkInfo>();
            search_results = new ObservableCollection<BookSearchInfo>();
            by_author_search_results = new ObservableCollection<AuthorWorkSearchInfo>();
            visited_works = new ObservableCollection<KeyValuePair<string, WorkInfo>>();
            _authors = new ObservableCollection<Author>();
            authors_found = new ObservableCollection<Author>();
            previous = new PreviousQuery();
            previous.Query = "";
        }

        public void SetSearchResults(BookSearchResults results)
        {
            search_results.Clear();

            foreach (BookSearchInfo book in results.docs) {
                search_results.Add(book);
            }
        }


        public void SetAuthorSearchResults(AuthorSearchResults results)
        {
            authors_found.Clear();

            foreach (Author author in results.docs) {
                authors_found.Add(author);
            }
        }


        public void SetAuthorWorkSearchResults(AuthorWorkSearchResults results)
        {
            by_author_search_results.Clear();

            foreach (AuthorWorkSearchInfo info in results.entries) {
                //Unused, querying too much images crashes the network manager
                //info.static_cover_url = info.GetImgURL();
                by_author_search_results.Add(info);
            }
        }

        public bool HasBooksFound()
        {
            return Search_Results != null && Search_Results.Count > 0;
        }

        public bool HasAuthorsFound()
        {
            return Authors_Found != null && Authors_Found.Count > 0;
        }


        /// <summary>
        /// Adds a work to the local session and to the database if it doesn't exist.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>True if successfully added</returns>
        public bool AddWork(WorkInfo info)
        {
            if (!Saved_Works.Contains(info)) {
                info.AddTime = DateTime.Now;
                Saved_Works.Add(info);
                App.Database.AddInfo(info);

                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Saves a work into the current session and in the database. Saving into database is async and 
        /// may not complete before function returns.
        /// </summary>
        /// <param name="info">WorkInfo</param>
        /// <returns>True if it removed an existing work, false otherwise.</returns>
        public bool RemoveWork(WorkInfo info)
        {
            if (Saved_Works.Contains(info)) {
                info.AddTime = DateTime.MinValue;
                Saved_Works.Remove(info);
                App.Database.DeleteWork(info.key);

                return true;
            } else {
                return false;
            }
        }


        /// <summary>
        /// Adds an author to the local session and database.
        /// </summary>
        /// <param name="a">an Author</param>
        /// <returns>True if the author us saved, false if it already exists</returns>
        public bool AddAuthor(Author a)
        {
            if (!Authors.Contains(a)) {
                a.AddTime = DateTime.Now;
                Authors.Add(a);
                App.Database.AddInfo(a);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the author is currently stored. If so removes it from the local session 
        /// and database.
        /// </summary>
        /// <param name="a">an Author</param>
        /// <returns>True if found and removed.</returns>
        public bool RemoveAuthor(Author a)
        {
            if (Authors.Contains(a)) {
                a.AddTime = DateTime.MinValue;
                Authors.Remove(a);
                App.Database.DeleteAuthor(a.key);

                return true;
            } else {
                return false;
            }
        }


        /// <summary>
        /// Temporarily stores a work to reduce API calls.
        /// </summary>
        /// <param name="info"></param>
        public void CacheWorkInfo(WorkInfo info)
        {
            bool found = false;
            int foundIdx = -1;

            for (int i = 0; i < visited_works.Count && !found; i++) {
                if (visited_works[i].Key.Equals(info.key)) {
                    found = true;
                    foundIdx = i;
                }
            }

            if (!found) {
                visited_works.Add(new KeyValuePair<string, WorkInfo>(info.key, info));
                if (cache_count == MAXCACHE) {
                    visited_works.RemoveAt(0);
                } else {
                    cache_count++;
                }
            } else {
                KeyValuePair<string, WorkInfo> temp = visited_works[visited_works.Count - 1];
                visited_works[visited_works.Count - 1] = visited_works[foundIdx];
                visited_works[foundIdx] = temp;
            }
        }


        public WorkInfo GetCachedWork(string key)
        {
            WorkInfo old_info = null;

            for (int i = 0; i < visited_works.Count && old_info == null; i++) {
                if (visited_works[i].Key.Equals(key)) {
                    old_info = visited_works[i].Value;
                }
            }

            return old_info;
        }


        public WorkInfo GetSavedWork(string key)
        {
            foreach (WorkInfo info in Saved_Works) {
                if (info.key.Equals(key))
                    return info;
            }

            return null;
        }


        public void RecordPreviousQuery(string query, SearchType option)
        {
            previous.Query = query;
            previous.Option = option;
        }

        public bool IsPreviousQuery(string query, SearchType option, bool ignoreOption = false)
        {
            if (ignoreOption)
                return previous.Query.Equals(query);
            else
                return previous.Query.Equals(query) && previous.Option == option;
        }
    }
}
