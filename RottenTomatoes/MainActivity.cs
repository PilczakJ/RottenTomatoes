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
using Newtonsoft.Json.Linq;
using System.Linq;

namespace RottenTomatoes
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

	public class Movies : Java.Lang.Object
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
	[Activity (Label = "RottenTomatoes", MainLauncher = true, Theme = "@android:style/Theme.Light")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//set the title bar to rotten tomatoes green
			View titleView = Window.FindViewById(Android.Resource.Id.Title);
			if (titleView != null) {
				IViewParent parent = titleView.Parent;
				if (parent != null && parent is View) {
					View parentView = (View)parent;
					parentView.SetBackgroundColor (Color.Rgb (61, 132, 4));
				}
			}

			//get the info on the movies
			string url = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/box_office.json?page_limit=16&page=1&country=us&apikey=p72922sy9n3a7e6ke8syyukx";
			string url2 = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/opening.json?limit=2&country=us&apikey=p72922sy9n3a7e6ke8syyukx";
			string url3 = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/in_theaters.json?page_limit=40&page=1&country=us&apikey=p72922sy9n3a7e6ke8syyukx";
			List<Movies> boxOffice = getMovies(url);
			List<Movies> opening = getMovies(url2);
			List<Movies> inTheaters = getMovies(url3);

			//make the lists using the custom list item layout
			ListView openingList = FindViewById(Resource.Id.listView1) as ListView;
			opening.AddRange (boxOffice);
			opening.AddRange (inTheaters);
			openingList.Adapter = new HomeScreenAdapter (this, opening);
			setListViewHeightBasedOnChildren (openingList);

			openingList.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e) {
				var activity2 = new Intent (this, typeof (MoviePage));
				Movies item = (Movies)openingList.Adapter.GetItem(e.Position);
				activity2.PutExtra("id",item.id);
				activity2.PutExtra("title",item.Title);
				activity2.PutExtra("mpaa_rating",item.mpaa_rating);
				activity2.PutExtra("director",item.abridged_directors);
				activity2.PutExtra("score",item.ratings.critics_score);
				activity2.PutExtra("release_date_day",item.releasedate.theater.Day);
				activity2.PutExtra("release_date_month",item.releasedate.theater.Month);
				activity2.PutExtra("release_date_year",item.releasedate.theater.Year);
				activity2.PutExtra("synopsis",item.synopsis);
				activity2.PutExtra("runtime",item.runtime);
				activity2.PutExtra("audience_score",item.ratings.audience_score);
				activity2.PutExtra("reviews_url",item.links.reviews);
				activity2.PutExtra("thumbnail_url",item.poster.thumbnail);
				int numActors = item.abridged_cast.Length;
				activity2.PutExtra("num_actors",numActors);
				for(int i = 0;i<numActors;i++)
				{
					activity2.PutExtra("actor"+i,item.abridged_cast[i].name);
					activity2.PutExtra("characters"+i,item.abridged_cast[i].characters);
				}
				activity2.PutExtra("critic_consensus",item.critics_consensus);
				StartActivity(activity2);






			};
			/*
			ListView boxOfficeList = FindViewById (Resource.Id.listView2) as ListView;
			boxOfficeList.Adapter = new HomeScreenAdapter (this, boxOffice);
			setListViewHeightBasedOnChildren (boxOfficeList);

			ListView inTheatersList = FindViewById(Resource.Id.listView3) as ListView;
			inTheatersList.Adapter = new HomeScreenAdapter (this, inTheaters);
			setListViewHeightBasedOnChildren (inTheatersList);
			*/

		}
			

		/// <summary>
		/// Sets the list view height based on children.
		/// </summary>
		/// <param name="listView">List view.</param>
		public void setListViewHeightBasedOnChildren(ListView listView)
		{
			try{
				SimpleCursorAdapter listAdapter = (SimpleCursorAdapter)listView.Adapter;
				if (listAdapter == null)
				{
					// pre-condition
					return;
				}

				int totalHeight = 0;
				for (int i = 0; i < listAdapter.Count; i++)
				{
					View listItem = listAdapter.GetView(i, null, listView);
					listItem.Measure(0, 0);
					totalHeight += listItem.MeasuredHeight;
				}

				ViewGroup.LayoutParams _params = listView.LayoutParameters;

				_params.Height = totalHeight + (listView.DividerHeight * (listAdapter.Count - 1));
				if (_params.Height < 250)//to have min height
					_params.Height = 250;//to have min height
				listView.LayoutParameters = _params;
				listView.RequestLayout();
			}
			catch (Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		/// <summary>
		/// Gets the movie data
		/// </summary>
		/// <returns>The movies.</returns>
		/// <param name="url">URL.</param>
		public List<Movies> getMovies(string url)
		{
			//make a list
			List<Movies> movies = new List<Movies> ();

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
					JArray jMovies = (JArray)json ["movies"];

					//set the values for movies from the json 
					for (int i = 0; i < jMovies.Count; i++) {
						//set mObject to the movie
						JObject mObject = (JObject)jMovies [i];

						//change the data for movies at the index
						movies.Add (new Movies ());
						movies [i].id = (string)mObject ["id"];
						movies [i].Title = (string)mObject ["title"];
						movies [i].Year = (int)mObject ["year"];
						movies [i].critics_consensus = (string)mObject ["critics_consensus"];
						movies [i].runtime = (int)mObject ["runtime"];
						movies [i].mpaa_rating = (string)mObject ["mpaa_rating"];
						movies [i].synopsis = (string)mObject ["synopsis"];
						movies [i].abridged_directors = (string)mObject ["abridged_directors"];
						movies [i].studio = (string)mObject ["studio"];
						movies [i].alternate_ids = (string)mObject ["altenate_ids"];

						movies [i].poster = (Posters)mObject ["posters"].ToObject<Posters> ();
						movies [i].ratings = (Ratings)mObject ["ratings"].ToObject<Ratings> ();
						movies [i].releasedate = (ReleaseDates)mObject ["release_dates"].ToObject<ReleaseDates> ();
						movies [i].links = (MovieLinks)mObject ["links"].ToObject<MovieLinks> ();
						movies [i].abridged_cast = (AbridgedCast[])mObject ["abridged_cast"].ToObject<AbridgedCast[]> ();

					}
				}
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
			return movies;

		}

			
	}

	/// <summary>
	/// Custom adapter for list items
	/// </summary>
	public class HomeScreenAdapter : BaseAdapter<Movies> {
		public List<Movies> items{ get; set; }
		Activity context;
		public HomeScreenAdapter(Activity context, List<Movies> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override Movies this[int position]
		{
			get { return items[position]; }
		}

		public override int Count
		{
			get { return items.Count; }
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return items [position];
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
		
			Movies item = items[position];
			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.ListRow, null);

			//title
			view.FindViewById<TextView>(Resource.Id.title).Text = item.Title;
			//first two actors
			view.FindViewById<TextView>(Resource.Id.actors).Text = item.abridged_cast[0].name + ", " + item.abridged_cast[1].name;

			//runtime in hours and minutes plus mpaa_rating
			int hours = item.runtime / 60;
			int minutes = item.runtime - (hours * 60);
			view.FindViewById<TextView> (Resource.Id.ratingRuntime).Text = item.mpaa_rating + ", " + hours + " hr. " + minutes + " min.";

			//freshness
			view.FindViewById<TextView> (Resource.Id.freshness).Text = item.ratings.critics_score + "%";

			//60%+ is fresh, under that is rotten
			if (item.ratings.critics_score >= 60)
				view.FindViewById<ImageView> (Resource.Id.freshnessIcon).SetImageResource (Resource.Drawable.fresh);
			else
				view.FindViewById<ImageView> (Resource.Id.freshnessIcon).SetImageResource (Resource.Drawable.rotten);

			//get the thumbnail
			Bitmap img = GetImageBitmapFromUrl (item.poster.thumbnail);
			view.FindViewById<ImageView> (Resource.Id.thumbnail).SetImageBitmap (img);

			return view;
		}

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
	}


}


