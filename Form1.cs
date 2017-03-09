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
            Spider spider = new Houses.Spider();

            //Set up the map(???) and center it on Harford
            gmap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.SetPositionByKeywords("Hartford, Connecticut");

            string domain = "http://www.apartmentguide.com/apartments/Connecticut/Manchester/";

            HashSet<string> siteHash = spider.getWebsiteList(domain);

            HashSet<Home> homeHash = new HashSet<Home>();
            List<Home> homes = new List<Home>();


            //Pick out the important bits and add it to the database
            foreach(string s in siteHash)
            {
                string webAddress = domain + s;
                string source = spider.getHTML(webAddress);
                homeHash.Add(spider.parseHTML(source));
            }

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


            gmap.Overlays.Add(pins);


        }

        

    }
}
