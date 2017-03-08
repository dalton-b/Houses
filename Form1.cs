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
            gmap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.SetPositionByKeywords("Hartford, Connecticut");


            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.apartmentguide.com/apartments/Connecticut/Simsbury/Mill-Commons/187207/");

            request.CookieContainer = new CookieContainer();
            request.AllowAutoRedirect = false;
            request.UserAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";

            request.Method = "GET";

            string source;
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                source = reader.ReadToEnd();
            }

            //Debug.WriteLine(source);

            Regex latReg = new Regex(@"data-lat=\""(.*?)\""");
            MatchCollection mc = latReg.Matches(source);
            double latitude = Convert.ToDouble(mc[0].Groups[1].Value);
            Debug.WriteLine("Latitude: " + latitude);

            Regex lngReg = new Regex(@"data-lng=\""(.*?)\""");
            mc = lngReg.Matches(source);
            double longitude = Convert.ToDouble(mc[0].Groups[1].Value);
            Debug.WriteLine("Longitude: " + longitude);

            GMapOverlay pins = new GMapOverlay("markers");
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.green_small);
            pins.Markers.Add(marker);
            gmap.Overlays.Add(pins);


        }

    }
}
