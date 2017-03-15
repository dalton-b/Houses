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
using Excel = Microsoft.Office.Interop.Excel;

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


            //Declare some strings we need for the spider
            string state = "Connecticut";
            //Use a '-' to instead of blank space when typing here
            string city = "Manchester";
            //Cap the search at this number of pages
            int cap = 15;
            string baseUrl = "http://www.apartmentguide.com/apartments/";
            string domain = baseUrl + state + "/" + city + "/";

            Database xlDB = new Database("C:\\Users\\dbassett\\Documents\\Visual Studio 2015\\Projects\\Houses\\HousesDB.xlsx");

            //Retrieve the links on the given page
            //Put them in hashset to eliminate duplicates
            HashSet<string> siteHash = spider.getWebsiteList(domain, cap);

            //Create a hashset to get rid of duplicates, and the list to get rid of nulls
            HashSet<Home> homeHash = new HashSet<Home>();

            //Iterate over each link we grabbed
            foreach (string s in siteHash)
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
            foreach (Home home in homeHash)
            {
                if (home != null)
                {

                    bool isDuplicate = false;
                    foreach (Home DBhome in xlDB.getAsHashSet())
                    {
                        if (DBhome.CompareTo(home) == 0)
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (!isDuplicate)
                    {
                        xlDB.add(home);
                    }
                }
            }

            List<Home> homes = xlDB.getAsHashSet().ToList();
            Debug.WriteLine("homes count: " + homes.Count);

            xlDB.saveAndClose();

            //Price divisions, for determining pin color
            double[] priceDivs = new double[3];

            List<double> prices = new List<double>();

            //Collect and sort all the prices
            for(int i = 0; i < homes.Count; i++)
            {
                if(Double.IsNaN(homes[i].Price) == false)
                {
                    prices.Add(homes[i].Price);
                }
            }
            prices.Sort();

            //Divide up the set of prices into groups of equal parts
            //Use those groups of prices to determine the bounds for each color
            for(int i = 0; i < priceDivs.Length; i++)
            {
                priceDivs[i] = prices[Convert.ToInt32(Math.Round((i+1) * prices.Count / (double) (priceDivs.Length + 1)))];
            }



            //Set up the map and center it
            gmap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.SetPositionByKeywords("Hartford, Connecticut");

            //Create markers
            GMapOverlay markers = new GMapOverlay("markers");

            //Add the overlay to the map
            //This has to come before we create the markers, otherwise
            //the positions are thrown off
            gmap.Overlays.Add(markers);

            for (int i = 0; i < homes.Count; i++)
            {
                //Create a different color marker based on price
                //Green marker
                if (homes[i].Price <= priceDivs[0])
                {
                    //Create a marker
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(homes[i].Latitude, homes[i].Longitude), GMarkerGoogleType.green_small);
                    //Add some info for when the user mouses over the pin
                    marker.ToolTipText = homes[i].Name + "\n" + homes[i].Address + "\n1 Bedroom: $" + homes[i].Price;
                    markers.Markers.Add(marker);
                }
                //Yellow marker
                else if (homes[i].Price > priceDivs[1] && homes[i].Price <= priceDivs[2])
                {
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(homes[i].Latitude, homes[i].Longitude), GMarkerGoogleType.yellow_small);
                    marker.ToolTipText = homes[i].Name + "\n" + homes[i].Address + "\n1 Bedroom: $" + homes[i].Price;
                    markers.Markers.Add(marker);
                }
                //Red marker
                else if (homes[i].Price > priceDivs[2])
                {
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(homes[i].Latitude, homes[i].Longitude), GMarkerGoogleType.red_small);
                    marker.ToolTipText = homes[i].Name + "\n" + homes[i].Address + "\n1 Bedroom: $" + homes[i].Price;
                    markers.Markers.Add(marker);
                }
            }
        }


        



    }
}
