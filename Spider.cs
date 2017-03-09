using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Houses
{
    class Spider
    {
        //Given a domain to search, get a list of the links on that page
        public HashSet<string> getWebsiteList(string domain)
        {
            HashSet<string> output = new HashSet<string>();

            //Grab the HTML
            string source = getHTML(domain);

            //Search for urls by using the domain
            // \u0022 is the " character, I was having some issues using a quote within a regex
            Regex urlReg = new Regex(@"href=" + '\u0022' + domain.Replace("http://www.apartmentguide.com", "") + "(.*?)\"");
            MatchCollection urlColl = urlReg.Matches(source);

            //Iterate over all the results and conditionally add them to the output
            for(int i = 0; i < urlColl.Count; i++)
            {
                //Get rid of urls that say "scroll to ID" because they're duplicates
                //Also get rid of urls with too few '/', these all go to irrelevant pages
                if(urlColl[i].Groups[1].Value.Contains("scrollToID")==false && urlColl[i].Groups[1].Value.Split('/').Length >= 3)
                {
                    output.Add(urlColl[i].Groups[1].Value);
                }

            }
            return output;
        }

        public string getHTML(string url)
        {
            //Get the HTML
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
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
            return source;
        }

        //Pick out important info and add it to the database
        public Home parseHTML(string source)
        {
            //If we don't have latitude and longitude, this page doesn't contain the info we want
            //So we just return null
            double latitude = 0;
            double longitude = 0;
            try
            {
                //Latitude
                Regex latReg = new Regex(@"data-lat=\""(.*?)\""");
                MatchCollection latColl = latReg.Matches(source);
                latitude = Convert.ToDouble(latColl[0].Groups[1].Value);

                //Longitude
                Regex lngReg = new Regex(@"data-lng=\""(.*?)\""");
                MatchCollection lngColl = lngReg.Matches(source);
                longitude = Convert.ToDouble(lngColl[0].Groups[1].Value);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return null;
            }
            
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
            double samplesUsed = 0;
            for (int i = 0; i < priceColl.Count; i++)
            {
                //Don't take the price if it's 99999, sometimes they use that as
                //some kind of default value
                if(Convert.ToDouble(priceColl[i].Groups[1].Value) != 99999)
                {
                    priceTotal += Convert.ToDouble(priceColl[i].Groups[1].Value);
                    samplesUsed++;
                }
                if (Convert.ToDouble(priceColl[i].Groups[2].Value) != 99999)
                {
                    priceTotal += Convert.ToDouble(priceColl[i].Groups[2].Value);
                    samplesUsed++;
                }
            }
            //Use the total and the number of prices to get the average
            double price = Math.Round(priceTotal / samplesUsed);

            //Date of search
            string thisDay = DateTime.Today.ToString().Split(' ')[0];

            return new Home(latitude, longitude, address, city, state, url, price, thisDay);
        }
    }
}
