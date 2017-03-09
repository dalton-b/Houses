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

            

            HashSet<string> siteHash = spider.getWebsiteList("http://www.apartmentguide.com/apartments/Connecticut/Manchester/");

            List<Home> homes = new List<Home>();

            HashSet<string> visitedHash = new HashSet<string>();

            //Pick out the important bits and add it to the database
            int count = 0;
            foreach(string s in siteHash)
            {
                if(visitedHash.Contains("http://www.apartmentguide.com/apartments/Connecticut/Manchester/" + s) != true)
                {
                    string source = spider.getHTML("http://www.apartmentguide.com/apartments/Connecticut/Manchester/" + s);
                    visitedHash.Add("http://www.apartmentguide.com/apartments/Connecticut/Manchester/" + s);
                    homes.Add(spider.parseHTML(source));
                    homes[count].disp();
                    count++;
                }


            }

            //for(int i = 0; i < homes.Count; i++)
            //{
            //    homes[i].disp();
            //}

            //Create markers
            GMapOverlay pins = new GMapOverlay("markers");
            //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.green_small);
            //pins.Markers.Add(marker);
            //gmap.Overlays.Add(pins);


        }

        

    }
}
