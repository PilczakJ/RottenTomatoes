using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using RottenTomatoes_PCL;

namespace RottenTomatoes
{
	public class Review
	{
        [JsonProperty("critic")]
		public string name{ get; set; }
        [JsonProperty("date")]
		public DateTime date { get; set; }
        [JsonProperty("freshness")]
		public string freshness {get;set;}
        [JsonProperty("publication")]
		public string publication {get;set;}
        [JsonProperty("quote")]
		public string quote {get;set;}
        [JsonProperty("links")]
		public ReviewLink links{ get; set; }
        
	}
    
	public class ReviewLink
	{
        [JsonProperty("review")]
		public string review{ get; set; }
	}
    
	public class FullCast
	{
        [JsonProperty("id")]
		public string id { get; set; }

        [JsonProperty("name")]
		public string name { get; set; }

        [JsonProperty("characters")]
		public string[] characters { get; set; }
	}

	[Activity (Label = "MoviePage",Theme = "@android:style/Theme.Light")]			
	public class MoviePage : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MoviePageLayout);

			//set the title bar to rotten tomatoes green
			View titleView = Window.FindViewById(Android.Resource.Id.Title);
			if (titleView != null) {
				IViewParent parent = titleView.Parent;
				if (parent != null && parent is View) {
					View parentView = (View)parent;
					parentView.SetBackgroundColor (Color.Rgb (61, 132, 4));
				}
			}

			//get passed data
			string id = Intent.GetStringExtra ("id") ?? "Data not available";
			string title = Intent.GetStringExtra ("title") ?? "Data not available";
			string mpaa_rating = Intent.GetStringExtra ("mpaa_rating") ?? "Data not available";
			string director = Intent.GetStringExtra ("director") ?? "Data not available";
			int critic_score = Intent.GetIntExtra ("score",0);
			int release_day = Intent.GetIntExtra ("release_date_day",0);
			int release_month = Intent.GetIntExtra ("release_date_month",0);
			int release_year = Intent.GetIntExtra ("release_date_year",0);
			string synopsis = Intent.GetStringExtra ("synopsis") ?? "Data not available";
			int runtime = Intent.GetIntExtra ("runtime",0);
			int audience_score = Intent.GetIntExtra ("audience_score",0);
			string reviewsURL = Intent.GetStringExtra ("reviews_url");
			int numActors = Intent.GetIntExtra ("num_actors",0);
			string thumbnail = Intent.GetStringExtra ("thumbnail_url") ?? "Data not available";
			string critic_consensus = Intent.GetStringExtra ("critic_consensus") ?? "Data not available";
			List<FullCast> cast = getFullCast("http://api.rottentomatoes.com/api/public/v1.0/movies/"+id+"/cast.json?apikey=p72922sy9n3a7e6ke8syyukx");

			//set title
			TextView titlebar = FindViewById (Resource.Id.title_bar) as TextView;
			titlebar.Text = title;

			//set reviewer/audience scores/icons
			ImageView reviewer_icon = FindViewById (Resource.Id.imageView2) as ImageView;
			if (critic_score >= 60) {
				reviewer_icon.SetImageResource (Resource.Drawable.fresh);
				reviewer_icon.Tag = "fresh";
			} else {
				reviewer_icon.SetImageResource (Resource.Drawable.rotten);
				reviewer_icon.Tag = "rotten";
			}

			ImageView audience_icon = FindViewById (Resource.Id.imageView3) as ImageView;
			audience_icon.SetImageResource (Resource.Drawable.popcorn);

			TextView criticScore = FindViewById (Resource.Id.textView2) as TextView;
			TextView audienceScore = FindViewById (Resource.Id.textView3) as TextView;
			criticScore.Text = critic_score + "%";
			audienceScore.Text = audience_score + "%";


			//set first two actors
			TextView first_two_actors = FindViewById (Resource.Id.textView6) as TextView;
			first_two_actors.Text = cast [0].name + ", " + cast [1].name;

			//set release date
			TextView release_date = FindViewById (Resource.Id.textView7) as TextView;
			release_date.Text = "In theaters " + release_month + "/" + release_day + "/" + release_year;

			//runtime
			int hours = runtime / 60;
			int minutes = runtime - (hours * 60);
			TextView ratingRuntime = FindViewById (Resource.Id.textView8) as TextView;
			ratingRuntime.Text = mpaa_rating + ", " + hours + " hr. " + minutes + " min.";

			//synopsis
			TextView synopsisText = FindViewById (Resource.Id.synopsis) as TextView;
			synopsisText.Text = "Synopsis: " + synopsis;

			//director
			TextView directorText = FindViewById (Resource.Id.director) as TextView;
			directorText.Text = "Director: " + director;

			//mpaa rating
			TextView mpaaText = FindViewById (Resource.Id.mpaa_rating) as TextView;
			mpaaText.Text = "Rated: " + mpaa_rating;

			//runtime (movie info)
			TextView runTime = FindViewById (Resource.Id.runtime) as TextView;
			runTime.Text = "Running Time: " + hours + " hr. " + minutes + " min.";

			//genre
			TextView genreText = FindViewById (Resource.Id.genre) as TextView;
			genreText.Text = "Genre: ";

			List<string> genres = getGenres ("http://api.rottentomatoes.com/api/public/v1.0/movies/" + id + ".json?apikey=p72922sy9n3a7e6ke8syyukx");
			if (genres.Count > 1)
				for (int i = 0; i < genres.Count; i++)
					genreText.Text += genres [i] + ", ";
			else
				genreText.Text += genres [0];

			//thumbnail
			Bitmap img = GetImageBitmapFromUrl (thumbnail);
			ImageView thumbnailPic = FindViewById (Resource.Id.imageView1) as ImageView;
			thumbnailPic.SetImageBitmap (img);
				
			//theater release
			TextView theaterRelease = FindViewById (Resource.Id.release_date) as TextView;
			theaterRelease.Text = "Theater Release: " + release_month + "/" + release_day + "/" + release_year;

			//actor list
			TextView actor1 = FindViewById (Resource.Id.actor1) as TextView;
			actor1.Text = cast [0].name;
			actor1.SetTextColor (Color.Blue);
			actor1.Click += delegate {
				var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [0].name.Replace (" ", "_") + "/");
				var intent = new Intent (Intent.ActionView, uri); 
				StartActivity (intent);     
			};

			TextView characters1 = FindViewById (Resource.Id.characters1) as TextView;
			characters1.Text = "";
			if(cast[0].characters.Length > 0)
				for (int i = 0; i < cast [0].characters.Length; i++)
					characters1.Text += (cast [0].characters [i] + ", ");
					
			if (cast.Count > 1) {
				TextView actor2 = FindViewById (Resource.Id.actor2) as TextView;
				actor2.Text = cast [1].name;
				actor2.SetTextColor (Color.Blue);
				actor2.Click += delegate {
					var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [1].name.Replace (" ", "_") + "/");
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};

				TextView characters2 = FindViewById (Resource.Id.characters2) as TextView;
				characters2.Text = "";
				if (cast [1].characters.Length > 0)
					for (int i = 0; i < cast [1].characters.Length; i++)
						characters2.Text += (cast [1].characters [i] + ", ");
			}
					
			if (cast.Count > 2) {
				TextView actor3 = FindViewById (Resource.Id.actor3) as TextView;
				actor3.Text = cast [2].name;
				actor3.SetTextColor (Color.Blue);
				actor3.Click += delegate {
					var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [2].name.Replace (" ", "_") + "/");
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};

				TextView characters3 = FindViewById (Resource.Id.characters3) as TextView;
				characters3.Text = "";
				if (cast [2].characters.Length > 0)
					for (int i = 0; i < cast [2].characters.Length; i++)
						characters3.Text += (cast [2].characters [i] + ", ");
			}
					
			if (cast.Count > 3) {
				TextView actor4 = FindViewById (Resource.Id.actor4) as TextView;
				actor4.Text = cast [3].name;
				actor4.SetTextColor (Color.Blue);
				actor4.Click += delegate {
					var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [3].name.Replace (" ", "_") + "/");
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};

				TextView characters4 = FindViewById (Resource.Id.characters4) as TextView;
				characters4.Text = "";
				if (cast [3].characters.Length > 0)
					for (int i = 0; i < cast [3].characters.Length; i++)
						characters4.Text += (cast [3].characters [i] + ", ");
			}

			if (cast.Count > 4) {
				TextView actor5 = FindViewById (Resource.Id.actor5) as TextView;
				actor5.Text = cast [4].name;
				actor5.SetTextColor (Color.Blue);
				actor5.Click += delegate {
					var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [4].name.Replace (" ", "_") + "/");
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};

				TextView characters5 = FindViewById (Resource.Id.characters5) as TextView;
				characters5.Text = "";
				if (cast [4].characters.Length > 0)
					for (int i = 0; i < cast [4].characters.Length; i++)
						characters5.Text += (cast [4].characters [i] + ", ");
			}
					
			if (cast.Count > 5) {
				TextView actor6 = FindViewById (Resource.Id.actor6) as TextView;
				actor6.Text = cast [5].name;
				actor6.SetTextColor (Color.Blue);
				actor6.Click += delegate {
					var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [5].name.Replace (" ", "_") + "/");
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};

				TextView characters6 = FindViewById (Resource.Id.characters6) as TextView;
				characters6.Text = "";
				if (cast [5].characters.Length > 0)
					for (int i = 0; i < cast [0].characters.Length; i++)
						characters6.Text += (cast [0].characters [i] + ", ");
			}
					
			if (cast.Count > 6) {
				TextView actor7 = FindViewById (Resource.Id.actor7) as TextView;
				actor7.Text = cast [6].name;
				actor7.SetTextColor (Color.Blue);
				actor7.Click += delegate {
					var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [6].name.Replace (" ", "_") + "/");
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};

				TextView characters7 = FindViewById (Resource.Id.characters7) as TextView;
				characters7.Text = "";
				if (cast [6].characters.Length > 0)
					for (int i = 0; i < cast [6].characters.Length; i++)
						characters7.Text += (cast [6].characters [i] + ", ");
			}
					
			if (cast.Count > 7) {
				TextView actor8 = FindViewById (Resource.Id.actor8) as TextView;
				actor8.Text = cast [7].name;
				actor8.SetTextColor (Color.Blue);
				actor8.Click += delegate {
					var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [7].name.Replace (" ", "_") + "/");
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};

				TextView characters8 = FindViewById (Resource.Id.characters8) as TextView;
				characters8.Text = "";
				if (cast [7].characters.Length > 0)
					for (int i = 0; i < cast [7].characters.Length; i++)
						characters8.Text += (cast [7].characters [i] + ", ");
			}
					
			if (cast.Count > 8) {
				TextView actor9 = FindViewById (Resource.Id.actor9) as TextView;
				actor9.Text = cast [8].name;
				actor9.SetTextColor (Color.Blue);
				actor9.Click += delegate {
					var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [8].name.Replace (" ", "_") + "/");
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};

				TextView characters9 = FindViewById (Resource.Id.characters9) as TextView;
				characters9.Text = "";
				if (cast [8].characters.Length > 0)
					for (int i = 0; i < cast [8].characters.Length; i++)
						characters9.Text += (cast [8].characters [i] + ", ");
			}
					
			if (cast.Count > 9) {
				TextView actor10 = FindViewById (Resource.Id.actor10) as TextView;
				actor10.Text = cast [9].name;
				actor10.SetTextColor (Color.Blue);
				actor10.Click += delegate {
					var uri = Android.Net.Uri.Parse ("http://www.rottentomatoes.com/mobile/celebrity/" + cast [9].name.Replace (" ", "_") + "/");
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};

				TextView characters10 = FindViewById (Resource.Id.characters10) as TextView;
				characters10.Text = "";
				if (cast [9].characters.Length > 0)
					for (int i = 0; i < cast [9].characters.Length; i++)
						characters10.Text += (cast [9].characters [i] + ", ");
			}
					

			//critic consensus
			TextView criticConsensus = FindViewById (Resource.Id.consensus) as TextView;
			criticConsensus.Text = "Consensus: " + critic_consensus;

			//make each review (freshness, critic, quote, and make link to the full review)

			List<Review> reviews = getReviews ("http://api.rottentomatoes.com/api/public/v1.0/movies/" + id + "/reviews.json?review_type=top_critic&page_limit=20&page=1&country=us&apikey=p72922sy9n3a7e6ke8syyukx");
			if (reviews.Count > 0) {
				ImageView freshness1 = FindViewById (Resource.Id.icon1) as ImageView;
				if (reviews [0].freshness == "fresh")
					freshness1.SetImageResource (Resource.Drawable.fresh);
				else
					freshness1.SetImageResource (Resource.Drawable.rotten);
				TextView critic1 = FindViewById (Resource.Id.reviewer1) as TextView;
				TextView review1 = FindViewById (Resource.Id.review1) as TextView;
				critic1.Text = reviews [0].name;
				review1.Text = reviews [0].quote+"..";
				review1.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [0].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}

			if (reviews.Count > 1) {
				ImageView freshness2 = FindViewById (Resource.Id.icon2) as ImageView;
				if (reviews [1].freshness == "fresh")
					freshness2.SetImageResource (Resource.Drawable.fresh);
				else
					freshness2.SetImageResource (Resource.Drawable.rotten);
				TextView critic2 = FindViewById (Resource.Id.reviewer2) as TextView;
				TextView review2 = FindViewById (Resource.Id.review2) as TextView;
				critic2.Text = reviews [1].name;
				review2.Text = reviews [1].quote+"..";
				review2.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [1].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}

			if (reviews.Count > 2) {
				ImageView freshness3 = FindViewById (Resource.Id.icon3) as ImageView;
				if (reviews [2].freshness == "fresh")
					freshness3.SetImageResource (Resource.Drawable.fresh);
				else
					freshness3.SetImageResource (Resource.Drawable.rotten);
				TextView critic3 = FindViewById (Resource.Id.reviewer3) as TextView;
				TextView review3 = FindViewById (Resource.Id.review3) as TextView;
				critic3.Text = reviews [2].name;
				review3.Text = reviews [2].quote+"..";
				review3.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [2].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}

			if (reviews.Count > 3) {
				ImageView freshness4 = FindViewById (Resource.Id.icon4) as ImageView;
				if (reviews [3].freshness == "fresh")
					freshness4.SetImageResource (Resource.Drawable.fresh);
				else
					freshness4.SetImageResource (Resource.Drawable.rotten);
				TextView critic4 = FindViewById (Resource.Id.reviewer4) as TextView;
				TextView review4 = FindViewById (Resource.Id.review4) as TextView;
				critic4.Text = reviews [3].name;
				review4.Text = reviews [3].quote+"..";
				review4.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [3].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}
			if (reviews.Count > 4) {
				ImageView freshness5 = FindViewById (Resource.Id.icon5) as ImageView;
				if (reviews [4].freshness == "fresh")
					freshness5.SetImageResource (Resource.Drawable.fresh);
				else
					freshness5.SetImageResource (Resource.Drawable.rotten);
				TextView critic5 = FindViewById (Resource.Id.reviewer5) as TextView;
				TextView review5 = FindViewById (Resource.Id.review5) as TextView;
				critic5.Text = reviews [4].name;
				review5.Text = reviews [4].quote+"..";
				review5.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [4].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}

			if (reviews.Count > 5) {
				ImageView freshness6 = FindViewById (Resource.Id.icon6) as ImageView;
				if (reviews [5].freshness == "fresh")
					freshness6.SetImageResource (Resource.Drawable.fresh);
				else
					freshness6.SetImageResource (Resource.Drawable.rotten);
				TextView critic6 = FindViewById (Resource.Id.reviewer6) as TextView;
				TextView review6 = FindViewById (Resource.Id.review6) as TextView;
				critic6.Text = reviews [5].name;
				review6.Text = reviews [5].quote+"..";
				review6.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [7].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}
			if (reviews.Count > 6) {
				ImageView freshness7 = FindViewById (Resource.Id.icon7) as ImageView;
				if (reviews [6].freshness == "fresh")
					freshness7.SetImageResource (Resource.Drawable.fresh);
				else
					freshness7.SetImageResource (Resource.Drawable.rotten);
				TextView critic7 = FindViewById (Resource.Id.reviewer7) as TextView;
				TextView review7 = FindViewById (Resource.Id.review7) as TextView;
				critic7.Text = reviews [6].name;
				review7.Text = reviews [6].quote+"..";
				review7.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [6].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}

			if (reviews.Count > 7) {
				ImageView freshness8 = FindViewById (Resource.Id.icon8) as ImageView;
				if (reviews [7].freshness == "fresh")
					freshness8.SetImageResource (Resource.Drawable.fresh);
				else
					freshness8.SetImageResource (Resource.Drawable.rotten);
				TextView critic8 = FindViewById (Resource.Id.reviewer8) as TextView;
				TextView review8 = FindViewById (Resource.Id.review8) as TextView;
				critic8.Text = reviews [7].name;
				review8.Text = reviews [7].quote+"..";
				review8.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [7].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}
			if (reviews.Count > 8) {
				ImageView freshness9 = FindViewById (Resource.Id.icon9) as ImageView;
				if (reviews [8].freshness == "fresh")
					freshness9.SetImageResource (Resource.Drawable.fresh);
				else
					freshness9.SetImageResource (Resource.Drawable.rotten);
				TextView critic9 = FindViewById (Resource.Id.reviewer9) as TextView;
				TextView review9 = FindViewById (Resource.Id.review9) as TextView;
				critic9.Text = reviews [8].name;
				review9.Text = reviews [8].quote+"..";
				review9.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [8].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}

			if (reviews.Count > 9) {
				ImageView freshness10 = FindViewById (Resource.Id.icon10) as ImageView;
				if (reviews [9].freshness == "fresh")
					freshness10.SetImageResource (Resource.Drawable.fresh);
				else
					freshness10.SetImageResource (Resource.Drawable.rotten);
				TextView critic10 = FindViewById (Resource.Id.reviewer10) as TextView;
				TextView review10 = FindViewById (Resource.Id.review10) as TextView;
				critic10.Text = reviews [9].name;
				review10.Text = reviews [9].quote+"..";
				review10.Click += delegate {
					var uri = Android.Net.Uri.Parse (reviews [9].links.review);
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);     
				};
			}

		}

        public static async Task<JObject> getGenreJson(string url)
        {
            var client = new Client<string[]>(url);
            return await client.getResult();
        }
        public static List<string> makeGenreList(JObject json)
        {
            List<string> genres = new List<string>();
            JArray jGenres = (JArray)json["genres"];
            for (int i = 0; i < jGenres.Count; i++)
            {
                genres.Add((string)jGenres[i]);

            }
            return genres;
        }
        public List<string> getGenres(string url)
        {
            try
            {
                JObject obj = getGenreJson(url).Result;
                return makeGenreList(obj);
            }
            catch
            {
                Console.WriteLine("invalid json");
                return new List<string>();
            }
        }
        /*
		/// <summary>
		/// Gets the genres
		/// </summary>
		/// <returns>The movies.</returns>
		/// <param name="url">URL.</param>
		public List<string> getGenres(string url)
		{
			//make a list
			List<string> genres = new List<string>();

			//make a result string
			string result = string.Empty;

			//make a web request to the given url
			WebRequest request = WebRequest.Create (url);
			request.Timeout = 30000;

			try {
				using (var response = request.GetResponse () as HttpWebResponse) {
					if (response.StatusCode == HttpStatusCode.OK) {
						//get a stream of the info
						var recieveStream = response.GetResponseStream ();
						if (recieveStream != null) {
							var stream = new StreamReader (recieveStream);
							result = stream.ReadToEnd ();
						}
					}
					//make a json object of the parsed results
					JObject json = JObject.Parse (result);
					//get the array of genres
					JArray jGenres = (JArray)json["genres"];
					for(int i = 0;i<jGenres.Count;i++)
					{
						genres.Add((string)jGenres[i]);

					}

					
					//set the values for movies from the json 
				}
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
			return genres;

		}
         * */

        public static async Task<JObject> getCastJson(string url)
        {
            var client = new Client<FullCast[]>(url);
            return await client.getResult();
        }

        List<FullCast> makeCastList(JObject json)
        {
            List<FullCast> cast = new List<FullCast>();

			JArray jCast = (JArray)json ["cast"];


			for(int i = 0;i<jCast.Count;i++)
			{
				JObject cObject = (JObject) jCast[i];
				cast.Add(new FullCast());
				cast[i].id = (string)cObject["id"];
				cast[i].name = (string)cObject["name"];
				cast[i].characters = (string[]) cObject["characters"].ToObject<string[]>();
			}
            return cast;
        }
        public List<FullCast> getFullCast(string url)
        {
            try
            {
                JObject obj = getCastJson(url).Result;
                return makeCastList(obj);
            }
            catch
            {
                Console.WriteLine("invalid json");
                return new List<FullCast>();
            }

            
        }
        /*
		/// <summary>
		/// Gets the full cast
		/// </summary>
		/// <returns>The movies.</returns>
		/// <param name="url">URL.</param>
		public List<FullCast> getFullCast(string url)
		{
			//make a list
			List<FullCast> cast = new List<FullCast> ();

			//make a result string
			string result = string.Empty;

			//make a web request to the given url
			WebRequest request = WebRequest.Create (url);
			request.Timeout = 30000;

			try {
				using (var response = request.GetResponse () as HttpWebResponse) {
					if (response.StatusCode == HttpStatusCode.OK) {
						//get a stream of the info
						var recieveStream = response.GetResponseStream ();
						if (recieveStream != null) {
							var stream = new StreamReader (recieveStream);
							result = stream.ReadToEnd ();
						}
					}
					//make a json object of the parsed results
					JObject json = JObject.Parse (result);
					//get the array of genres
					JArray jCast = (JArray)json ["cast"];


					for(int i = 0;i<jCast.Count;i++)
					{
						JObject cObject = (JObject) jCast[i];
						cast.Add(new FullCast());
						cast[i].id = (string)cObject["id"];
						cast[i].name = (string)cObject["name"];
						cast[i].characters = (string[]) cObject["characters"].ToObject<string[]>();
					}

					//set the values for movies from the json 
				}
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
			return cast;

		}
         * */

		/// <summary>
		/// Gets the image bitmap from URL.
		/// </summary>
		/// <returns>The image bitmap from URL.</returns>
		/// <param name="url">URL.</param>
		private Bitmap GetImageBitmapFromUrl(string url)
		{
			Bitmap imageBitmap = null;

			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}

			return imageBitmap;
		}

        public static async Task<JObject> getReviewJson(string url)
        {
            var client = new Client<Review[]>(url);
            return await client.getResult();
        }


        List<Review> makeReviewList(JObject json)
        {
            List<Review> reviews = new List<Review>();

            JArray jReviews = (JArray)json["reviews"];

            //set the values for movies from the json 
            for (int i = 0; i < jReviews.Count; i++)
            {
                //set mObject to the movie
                JObject mObject = (JObject)jReviews[i];

                //change the data for movies at the index
                reviews.Add(new Review());
                reviews[i].name = (string)mObject["critic"];
                reviews[i].date = (DateTime)mObject["date"].ToObject<DateTime>(); ;
                reviews[i].freshness = (string)mObject["freshness"];
                reviews[i].publication = (string)mObject["publication"];
                reviews[i].quote = (string)mObject["quote"];
                reviews[i].links = (ReviewLink)mObject["links"].ToObject<ReviewLink>();

            }
            return reviews;
        }
        public List<Review> getReviews(string url)
        {
            try
            {
                JObject obj = getCastJson(url).Result;
                return makeReviewList(obj);
            }
            catch
            {
                Console.WriteLine("invalid json");
                return new List<Review>();
            }


        }
        /*
		/// <summary>
		/// Gets the review data
		/// </summary>
		/// <returns>The movies.</returns>
		/// <param name="url">URL.</param>
		public List<Review> getReviews(string url)
		{
			//make a list
			List<Review> reviews = new List<Review> ();

			//make a result string
			string result = string.Empty;

			//make a web request to the given url
			WebRequest request = WebRequest.Create (url);
			request.Timeout = 30000;

			try {
				using (var response = request.GetResponse () as HttpWebResponse) {
					if (response.StatusCode == HttpStatusCode.OK) {
						//get a stream of the info
						var recieveStream = response.GetResponseStream ();
						if (recieveStream != null) {
							var stream = new StreamReader (recieveStream);
							result = stream.ReadToEnd ();
						}
					}
					//make a json object of the parsed results
					JObject json = JObject.Parse (result);
					//make an array out of the movies from the object
					JArray jReviews = (JArray)json ["reviews"];

					//set the values for movies from the json 
					for (int i = 0; i < jReviews.Count; i++) {
						//set mObject to the movie
						JObject mObject = (JObject)jReviews [i];

						//change the data for movies at the index
						reviews.Add (new Review ());
						reviews[i].name = (string)mObject["critic"];
						reviews[i].date = (DateTime)mObject["date"].ToObject<DateTime>();;
						reviews[i].freshness = (string)mObject["freshness"];
						reviews[i].publication = (string)mObject["publication"];
						reviews[i].quote = (string)mObject["quote"];
						reviews[i].links = (ReviewLink)mObject["links"].ToObject<ReviewLink>();

					}
				}
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
			return reviews;

		}
         * */


	}
}


