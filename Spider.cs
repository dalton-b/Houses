using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Houses
{
    class Spider
    {
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
            for (int i = 0; i < priceColl.Count; i++)
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
