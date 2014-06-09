using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RottenTomatoes_PCL
{
    
    public class ReleaseDates
    {
        public DateTime theater { get; set; }
        public DateTime DVD { get; set; }

    }

    public class Ratings
    {
        public string critics_rating { get; set; }
        public int critics_score { get; set; }
        public string audience_rating { get; set; }
        public int audience_score { get; set; }
    }

    public class Posters
    {
        public string thumbnail { get; set; }
        public string profile { get; set; }
        public string detailed { get; set; }
        public string original { get; set; }
    }

    public class AbridgedCast
    {
        public string name { get; set; }
        public string[] characters { get; set; }
    }

    public class MovieLinks
    {
        public string self { get; set; }
        public string alternate { get; set; }
        public string cast { get; set; }
        public string clips { get; set; }
        public string reviews { get; set; }
        public string similar { get; set; }
    }

    public class Movies
    {

        public string id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public string mpaa_rating { get; set; }

        public int runtime { get; set; }

        public string critics_consensus { get; set; }

        public ReleaseDates releasedate { get; set; }

        public Ratings ratings { get; set; }

        public string synopsis { get; set; }

        public Posters poster { get; set; }

        public AbridgedCast[] abridged_cast { get; set; }

        public string abridged_directors { get; set; }

        public string studio { get; set; }

        public string alternate_ids { get; set; }

        public MovieLinks links { get; set; }

    }
    

    public class Client<T> where T:class
    {
        private string _url;
        public Client(string url)
        {
            _url = url;
        }
        public async Task<JObject> getResult()
        {
            var response = await MakeAsyncRequest(_url).ConfigureAwait(continueOnCapturedContext:false);
            //var result /*JsonConvert.DeserializeObject<T>(response)*/;
 
            return response;
        }
        public static Task<JObject> MakeAsyncRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), null);
            return task.ContinueWith(t=>ReadStreamFromResponse(t.Result));
        }

        private static JObject ReadStreamFromResponse(WebResponse response)
        {
            string result = string.Empty;
            List<Movies> movies = new List<Movies>();
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                result = reader.ReadToEnd();
                //return content;
            }

            //make a json object of the whole parsed result
            JObject json = JObject.Parse(result);
            return json;
        } 


    }
    /*
    public class MovieData
    {
        /// <summary>
        /// Gets the movie data
        /// </summary>
        /// <returns>The movies.</returns>
        /// <param name="url">URL.</param>
        public List<Movies> getMovies(string url)
        {
            //make a list
            List<Movies> movies = new List<Movies>();

            //make a result string
            string result = string.Empty;

            //make a web request to the given url
            WebRequest request = WebRequest.Create(url);
            request.Timeout = 30000;
            
            try
            {
                Task<HttpWebResponse> requestTask = Task.Factory.FromAsync<HttpWebResponse>(request.BeginGetResponse, request.EndGetRequestStream, request);
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //get a stream of the info
                        var recieveStream = response.GetResponseStream();
                        if (recieveStream != null)
                        {
                            var stream = new StreamReader(recieveStream);
                            result = stream.ReadToEnd();
                        }
                    }
                    //make a json object of the parsed results
                    JObject json = JObject.Parse(result);
                    //make an array out of the movies from the object
                    JArray jMovies = (JArray)json["movies"];

                    //set the values for movies from the json 
                    for (int i = 0; i < jMovies.Count; i++)
                    {
                        //set mObject to the movie
                        JObject mObject = (JObject)jMovies[i];

                        //change the data for movies at the index
                        movies.Add(new Movies());
                        movies[i].id = (string)mObject["id"];
                        movies[i].Title = (string)mObject["title"];
                        movies[i].Year = (int)mObject["year"];
                        movies[i].critics_consensus = (string)mObject["critics_consensus"];
                        movies[i].runtime = (int)mObject["runtime"];
                        movies[i].mpaa_rating = (string)mObject["mpaa_rating"];
                        movies[i].synopsis = (string)mObject["synopsis"];
                        movies[i].abridged_directors = (string)mObject["abridged_directors"];
                        movies[i].studio = (string)mObject["studio"];
                        movies[i].alternate_ids = (string)mObject["altenate_ids"];

                        movies[i].poster = (Posters)mObject["posters"].ToObject<Posters>();
                        movies[i].ratings = (Ratings)mObject["ratings"].ToObject<Ratings>();
                        movies[i].releasedate = (ReleaseDates)mObject["release_dates"].ToObject<ReleaseDates>();
                        movies[i].links = (MovieLinks)mObject["links"].ToObject<MovieLinks>();
                        movies[i].abridged_cast = (AbridgedCast[])mObject["abridged_cast"].ToObject<AbridgedCast[]>();

                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }
            return movies;

        }

    }
     * */
}
