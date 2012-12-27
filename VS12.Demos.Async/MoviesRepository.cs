using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AA.Demo.Model
{

    public class MoviesRepository
    {
        #region Sinc
        public static IEnumerable<Movie> GetMovieDataFromXDoc(XDocument doc)
        {
            XNamespace dRss = "http://www.w3.org/2005/Atom";
            XNamespace odata = "http://schemas.microsoft.com/ado/2007/08/dataservices";

            List<XElement> dox = doc.Descendants(dRss + "entry").ToList();
            List<Movie> movieList = new List<Movie>();
            foreach (XElement ele in dox)
            {
                XElement movieSummary = ele.Descendants(dRss + "summary").First();
                XElement movieName = ele.Descendants(odata + "Name").First();
                XElement boxArtUrl = ele.Descendants(odata + "MediumUrl").First();
                Movie tempMovie = new Movie()
                {
                    BoxArtUrl = boxArtUrl.Value,
                    Title = movieName.Value,
                    Summary = movieSummary.Value
                };
                movieList.Add(tempMovie);
            }
            return movieList;
        }
        public static List<Movie> GetMovies()
        {
            string query =
              @"http://odata.netflix.com/Catalog/Titles?" +
              @"$filter=ReleaseYear le 1986 and ReleaseYear ge 1980 ";
            var client = new System.Net.WebClient();
            // client.MaxResponseContentBufferSize = Int32.MaxValue;
            try
            {
                var resp = client.DownloadString(new Uri(query));
                // string content = resp.Content.ReadAsString();
                XDocument xml = XDocument.Parse(resp);
                List<Movie> movieList = GetMovieDataFromXDoc(xml).ToList();
                return movieList;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        #endregion



        #region Async

        public static IEnumerable<XElement> GetMovieDataFromXDocElements(XDocument doc)
        {
            XNamespace dRss = "http://www.w3.org/2005/Atom";
            XNamespace odata = "http://schemas.microsoft.com/ado/2007/08/dataservices";

            return doc.Descendants(dRss + "entry").ToList();
        }

        private static Task GetMovieInfo(XElement movie)
        {
            return Task.Run(() =>
            {
                XNamespace dRss = "http://www.w3.org/2005/Atom";
                XNamespace odata = "http://schemas.microsoft.com/ado/2007/08/dataservices";

                XElement movieSummary = movie.Descendants(dRss + "summary").First();
                XElement movieName = movie.Descendants(odata + "Name").First();
                XElement boxArtUrl = movie.Descendants(odata + "MediumUrl").First();
                Movie tempMovie = new Movie()
                {
                    BoxArtUrl = boxArtUrl.Value,
                    Title = movieName.Value,
                    Summary = movieSummary.Value

                };


            });
        }

        public static Task<List<XElement>> GetMoviesAsync()
        {
            string query =
              @"http://odata.netflix.com/Catalog/Titles?" +
              @"$filter=ReleaseYear le 1986 and ReleaseYear ge 1980 ";
            var client = new System.Net.WebClient();
            return Task.Factory.StartNew<List<XElement>>(() =>
            {
                var resp = client.DownloadString(new Uri(query));
                XDocument xml = XDocument.Parse(resp);

                var counter = 0;
                var movieList = GetMovieDataFromXDocElements(xml);
                return movieList.ToList();
            });



        }

        public async static Task<int> ManageMovieAsync(List<XElement> movies)
        {
            int totalCount = movies.Count;
            int processCount = await Task.Run<int>(async () =>
            {
                int tempCount = 0;
                foreach (var movie in movies)
                {
                    await Task.Delay(100);
                    await GetMovieInfo(movie);
                   
                }

                return tempCount;
            });
            return processCount;
        }

        public async static Task<int> ManageMovieAsync(List<XElement> movies, IProgress<int> progress)
        {
            int totalCount = movies.Count;
            int processCount = await Task.Run<int>(async () =>
            {
                int tempCount = 0;
                foreach (var movie in movies)
                {
                    await Task.Delay(100);
                    await GetMovieInfo(movie);
                    progress.Report(movies.IndexOf(movie));
                }

                return tempCount;
            });
            return processCount;
        }

        public async static Task<int> ManageMovieAsync(List<XElement> movies, CancellationToken token)
        {
            int totalCount = movies.Count;
            int processCount = await Task.Run<int>(async () =>
            {
                int tempCount = 0;
                foreach (var movie in movies)
                {
                    
                    await Task.Delay(100);
                    await GetMovieInfo(movie);
                    token.ThrowIfCancellationRequested();
                }

                return tempCount;
            });
            return processCount;
        }

        public async static Task<int> ManageMovieAsync(List<XElement> movies, IProgress<int> progress,CancellationToken token)
        {
            int totalCount = movies.Count;
            int processCount = await Task.Run<int>(async () =>
            {
                int tempCount = 0;
                foreach (var movie in movies)
                {
                    //await the processing and uploading logic here
                    await Task.Delay(100);
                    await GetMovieInfo(movie);
                    progress.Report(movies.IndexOf(movie));
                    token.ThrowIfCancellationRequested();
                }

                return tempCount;
            });
            return processCount;
        }
        
        


        #endregion
    }
}


