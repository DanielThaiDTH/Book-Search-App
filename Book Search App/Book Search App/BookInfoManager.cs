using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Book_Search_App
{
    public class BookInfoManager
    {
        
        ObservableCollection<BookSearchInfo> search_results;
        public ObservableCollection<BookSearchInfo> Search_Results => search_results;

        ObservableCollection<Author> authors_found;
        public ObservableCollection<Author> Authors_Found => authors_found;

        //Cache of visted works to reduce the amount of API calls
        ObservableCollection<KeyValuePair<string, WorkInfo>> visited_works;
        public ObservableCollection<KeyValuePair<string, WorkInfo>> VisitedCache => visited_works;
        int cache_count = 0;
        readonly int MAXCACHE = 20;

        //Saved books(works)
        ObservableCollection<WorkInfo> saved_works;
        public ObservableCollection<WorkInfo> Saved_Works => saved_works;

        //Saved authors
        ObservableCollection<Author> _authors;
        public ObservableCollection<Author> Authors => _authors;

        public BookInfoManager()
        {
            saved_works = new ObservableCollection<WorkInfo>();
            search_results = new ObservableCollection<BookSearchInfo>();
            visited_works = new ObservableCollection<KeyValuePair<string, WorkInfo>>();
            _authors = new ObservableCollection<Author>();
            authors_found = new ObservableCollection<Author>();
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


        public bool HasBooksFound()
        {
            return Search_Results != null && Search_Results.Count > 0;
        }

        public bool HasAuthorsFound()
        {
            return Authors_Found != null && Authors_Found.Count > 0;
        }

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
    }
}
