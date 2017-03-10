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
            //Create the spider object so it can start crawlin' the web
            Spider spider = new Houses.Spider();

            //Set up the map and center it
            gmap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.SetPositionByKeywords("Hartford, Connecticut");

            //The domain we'll be searching. Someday maybe it will be a text box input?
            //string domain = "http://www.apartmentguide.com/apartments/Connecticut/Manchester/";

            string state = "Connecticut";
            string city = "Manchester";
            string baseUrl = "http://www.apartmentguide.com/apartments/";
            string domain = baseUrl + state + "/" + city + "/";

            //Retrieve the links on the given page
            //Put them in hashset to eliminate duplicates
            HashSet<string> siteHash = spider.getWebsiteList(domain);

            //Create a hashset to get rid of duplicates, and the list to get rid of nulls
            HashSet<Home> homeHash = new HashSet<Home>();
            List<Home> homes = new List<Home>();


            //Iterate over each link we grabbed
            foreach(string s in siteHash)
            {
                //Construct the web address
                string webAddress = baseUrl + s;
                Debug.WriteLine("Web address: " + webAddress);
                //Pull the source HTML
                string source = spider.getHTML(webAddress);
                //Pick out the important bits and add it to the 'database'
                homeHash.Add(spider.parseHTML(source));
            }

            //Add them to a list if they're not null
            foreach(Home home in homeHash)
            {
                if(home != null)
                {
                    homes.Add(home);
                    home.disp();
                }
            }


            //Create markers
            GMapOverlay pins = new GMapOverlay("markers");
            for (int i = 0; i < homes.Count; i++)
            {
                //Create a different color pin based on price
                if(homes[i].Price <= 1100)
                {
                    GMarkerGoogle pin = new GMarkerGoogle(new PointLatLng(homes[i].Latitude, homes[i].Longitude), GMarkerGoogleType.green_small);
                    pins.Markers.Add(pin);
                }
                else if(homes[i].Price > 1100 && homes[i].Price <= 1300)
                {
                    GMarkerGoogle pin = new GMarkerGoogle(new PointLatLng(homes[i].Latitude, homes[i].Longitude), GMarkerGoogleType.yellow_small);
                    pins.Markers.Add(pin);
                }
                else if (homes[i].Price > 1300)
                {
                    GMarkerGoogle pin = new GMarkerGoogle(new PointLatLng(homes[i].Latitude, homes[i].Longitude), GMarkerGoogleType.red_small);
                    pins.Markers.Add(pin);
                }
            }
            //Add the pins to the map
            gmap.Overlays.Add(pins);
        }
    }
}
