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
            homes.Add(spider.parseHTML(source));
            homes[0].disp();

            //Create markers
            GMapOverlay pins = new GMapOverlay("markers");
            //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.green_small);
            //pins.Markers.Add(marker);
            //gmap.Overlays.Add(pins);


        }

        

    }
}
