using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Houses
{
    class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
