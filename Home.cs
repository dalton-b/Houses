using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Houses
{
    public class Home
    {
        double _latitude;
        double _longitude;
        string _address;
        string _city;
        string _state;
        string _url;
        double _price;
        string _date;

        public Home(double latitude, double longitude, string address, string city, 
            string state, string url, double price, string date)
        {
            _latitude = latitude;
            _longitude = longitude;
            _address = address;
            _city = city;
            _state = state;
            _url = url;
            _price = price;
            _date = date;
        }

        //For debugging purposes
        public void disp()
        {
            Debug.WriteLine("Latitude: " + _latitude);
            Debug.WriteLine("Longitude: " + _longitude);
            Debug.WriteLine("Address: " + _address);
            Debug.WriteLine("City: " + _city);
            Debug.WriteLine("State: " + _state);
            //Debug.WriteLine("URL: " + _url);
            Debug.WriteLine("Price: " + _price);
            Debug.WriteLine("Date: " + _date);

        }

        public double Latitude
        {
            get
            {
                return _latitude;
            }

            set
            {
                _latitude = value;
            }
        }

        public double Longitude
        {
            get
            {
                return _longitude;
            }

            set
            {
                _longitude = value;
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }

            set
            {
                _address = value;
            }
        }

        public string City
        {
            get
            {
                return _city;
            }

            set
            {
                _city = value;
            }
        }

        public string State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
            }
        }

        public string Url
        {
            get
            {
                return _url;
            }

            set
            {
                _url = value;
            }
        }

        public double Price
        {
            get
            {
                return _price;
            }

            set
            {
                _price = value;
            }
        }

        public string Date
        {
            get
            {
                return _date;
            }

            set
            {
                _date = value;
            }
        }
    }
}
