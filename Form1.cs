using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET;

namespace Houses
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Set up the map(???) and center it on Harford
            gmap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.SetPositionByKeywords("Hartford, Connecticut");

            //Get the HTML
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.apartmentguide.com/apartments/Connecticut/Manchester/Brook-Haven-Apartments/100011973/");
            //I think we need cookies
            request.CookieContainer = new CookieContainer();
            //Not totally sure what this does
            request.AllowAutoRedirect = false;
            //Pretend we're the googlebot by using its user agent
            request.UserAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
            //Not at all sure what this does
            request.Method = "GET";
            //Read the HTML and store it in this string
            string source;
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                source = reader.ReadToEnd();
            }

            List<Home> homes = new List<Home>();

            //Pick out the important bits and add it to the database
            homes.Add(parseHTML(source));
            homes[0].disp();

            //Create markers
            GMapOverlay pins = new GMapOverlay("markers");
            //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.green_small);
            //pins.Markers.Add(marker);
            //gmap.Overlays.Add(pins);


        }

        //Pick out important info and add it to the database
        public Home parseHTML(string source)
        {
            //Latitude
            Regex latReg = new Regex(@"data-lat=\""(.*?)\""");
            MatchCollection latColl = latReg.Matches(source);
            double latitude = Convert.ToDouble(latColl[0].Groups[1].Value);

            //Longitude
            Regex lngReg = new Regex(@"data-lng=\""(.*?)\""");
            MatchCollection lngColl = lngReg.Matches(source);
            double longitude = Convert.ToDouble(lngColl[0].Groups[1].Value);

            //Address
            Regex addressReg = new Regex(@"<span itemprop=\""streetAddress\"">(.*?), </span>");
            MatchCollection addressColl = addressReg.Matches(source);
            string address = addressColl[0].Groups[1].Value;

            //City
            Regex cityReg = new Regex(@"city&quot;:&quot;(.*?)&quot;");
            MatchCollection cityColl = cityReg.Matches(source);
            string city = cityColl[0].Groups[1].Value;

            //State
            Regex stateReg = new Regex(@"state&quot;:&quot;(.*?)&quot;");
            MatchCollection stateColl = stateReg.Matches(source);
            string state = stateColl[0].Groups[1].Value;

            //URL
            Regex urlReg = new Regex(@"href=\'(.*?)\'");
            MatchCollection urlColl = urlReg.Matches(source);
            string url = urlColl[0].Groups[1].Value;

            //Price (average in building for a 1 bedroom)
            Regex priceReg = new Regex(@"&quot;bed_high&quot;:1,&quot;bed_low&quot;:1,&quot;bed_price_high&quot;:(.*?),&quot;bed_price_low&quot;:(.*?),&quot;");
            MatchCollection priceColl = priceReg.Matches(source);
            double priceTotal = 0;
            for(int i = 0; i<priceColl.Count; i++)
            {
                priceTotal += Convert.ToDouble(priceColl[i].Groups[1].Value);
                priceTotal += Convert.ToDouble(priceColl[i].Groups[2].Value);
            }
            double price = Math.Round(priceTotal / (priceColl.Count * 2));

            //Date of search
            string thisDay = DateTime.Today.ToString().Split(' ')[0];

            return new Home(latitude, longitude, address, city, state, url, price, thisDay);
        }

    }
}
