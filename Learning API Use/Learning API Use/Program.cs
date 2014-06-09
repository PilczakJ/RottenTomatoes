using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using RottenTomatoes_PCL;

namespace Learning_API_Use
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

    public class FlixterMovies
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
    class Program
    {
        private static List<FlixterMovies> getMovies(string url)
        {
            //make the list of movies
            List<FlixterMovies> movies = new List<FlixterMovies>();
            //make a result string
            string result = string.Empty;
            //pick a url for the data to come from

            //make a web request at the url
            WebRequest webRequest = WebRequest.Create(url);
            //2 seconds for timeout
            webRequest.Timeout = 2000;
            try
            {
                //use the web request to get a response
                using (var response = webRequest.GetResponse() as HttpWebResponse)
                {
                    //if the response is OK
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //get the response
                        var recieveStream = response.GetResponseStream();
                        //if there is something there
                        if (recieveStream != null)
                        {
                            //make the result the entirety of the response
                            var stream = new StreamReader(recieveStream);
                            result = stream.ReadToEnd();
                        }
                    }
                    //make a json object of the whole parsed result
                    JObject json = JObject.Parse(result);
                    //make a array of jobjects for the movies
                    JArray jMovies = (JArray)json["movies"];

                    //for each movie
                    for (int i = 0; i < jMovies.Count; i++)
                    {
                        //set mObject to the movie
                        JObject mObject = (JObject)jMovies[i];

                        //change the data for movies at the index
                        movies.Add(new FlixterMovies());
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
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return movies;
        }

        public static async Task<JObject> getBoxOffice(string url)
        {
            var client = new Client<FlixterMovies[]>(url);
            var result = await client.getResult();
            return await client.getResult();
        }

        static List<FlixterMovies> makeMovieList(JObject json)
        {
            List<FlixterMovies> movies = new List<FlixterMovies>();
            JArray jMovies = (JArray)json["movies"];

            //for each movie
            for (int i = 0; i < jMovies.Count; i++)
            {
                //set mObject to the movie
                JObject mObject = (JObject)jMovies[i];

                //change the data for movies at the index
                movies.Add(new FlixterMovies());
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
            return movies;
        }

        static void Main()
        {
            string url = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/box_office.json?page_limit=16&page=1&country=us&apikey=p72922sy9n3a7e6ke8syyukx";
            string url2 = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/opening.json?limit=2&country=us&apikey=p72922sy9n3a7e6ke8syyukx";
            string url3 = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/in_theaters.json?page_limit=40&page=1&country=us&apikey=p72922sy9n3a7e6ke8syyukx";
            
            //List<FlixterMovies> boxOffice = getMovies(url);
            JObject obj = getBoxOffice(url).Result;
            List<FlixterMovies> box_office = makeMovieList(obj);
            obj = getBoxOffice(url2).Result;
            List<FlixterMovies> opening = makeMovieList(obj);
            obj = getBoxOffice(url3).Result;
            List<FlixterMovies> inTheaters = makeMovieList(obj);
            //var client = new Client<List<FlixterMovies>>(url);
            //List<FlixterMovies> opening = getMovies(url2);
            //List<FlixterMovies> inTheaters = getMovies(url3);
            
            
            Console.WriteLine("Opening this week");
            for (int i = 0; i < opening.Count; i++)
            {
                Console.WriteLine("Title: " + opening[i].Title + "   " + "mpaa rating: " + opening[i].mpaa_rating + "   " + "freshness: " + opening[i].ratings.critics_score);
            }
            
            Console.WriteLine("\nTop Box Office");
            for (int i = 0; i < box_office.Count; i++)
            {
                Console.WriteLine("Title: " + box_office[i].Title + "   " + "mpaa rating: " + box_office[i].mpaa_rating + "   " + "freshness: " + box_office[i].ratings.critics_score);
            }
            
            Console.WriteLine("\nAlso In Theaters");
            for (int i = 0; i < inTheaters.Count; i++)
            {
                Console.WriteLine("Title: " + inTheaters[i].Title + "   " + "mpaa rating: " + inTheaters[i].mpaa_rating + "   " + "freshness: " + inTheaters[i].ratings.critics_score);
            }
            
        }





    }


}
