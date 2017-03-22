using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Houses
{
    class Database
    {
        string _filePath;
        Excel.Application _xlApp;
        Excel.Workbook _xlWB;
        Excel.Worksheet _xlSheet;


        //Constructor: Opens Excel, the workbook, and the worksheet
        public Database(string filePath)
        {
            _filePath = filePath;
            _xlApp = new Excel.Application();
            //_xlApp.Visible = true;
            _xlWB = _xlApp.Workbooks.Open(filePath);
            _xlSheet = (Excel.Worksheet)_xlWB.Worksheets[1];
        }

        //Add a home object to the sheet
        public void add(Home home)
        {
            int count = 1;

            //Move past nonempty cells
            while(!String.IsNullOrEmpty(_xlSheet.Range["A" + count, "A" + count].Value2))
            {
                count++;
            }
 
            //Add the data to the respective columns
            _xlSheet.Range["A" + count, "A" + count].Value2 = home.Name;
            _xlSheet.Range["B" + count, "B" + count].Value2 = home.Address;
            _xlSheet.Range["C" + count, "C" + count].Value2 = home.City;
            _xlSheet.Range["D" + count, "D" + count].Value2 = home.State;
            _xlSheet.Range["E" + count, "E" + count].Value2 = home.Price;
            _xlSheet.Range["F" + count, "F" + count].Value2 = home.Latitude;
            _xlSheet.Range["G" + count, "G" + count].Value2 = home.Longitude;
            _xlSheet.Range["H" + count, "H" + count].Value2 = home.Url;
            _xlSheet.Range["I" + count, "I" + count].Value2 = home.Date;
            _xlSheet.Range["J" + count, "J" + count].Value2 = home.Id;
            count++;
        }

        //Return a home object given its row index in the Excel sheet
        public Home getByRow(int row)
        {
            string name;
            string address;
            string city;
            string state;
            double price;
            double latitude;
            double longitude;
            string url;
            string date;
            string id;

            name = _xlSheet.Range["A" + row, "A" + row].Value2;
            address = _xlSheet.Range["B" + row, "B" + row].Value2;
            city = _xlSheet.Range["C" + row, "C" + row].Value2;
            state = _xlSheet.Range["D" + row, "D" + row].Value2;
            price = _xlSheet.Range["E" + row, "E" + row].Value2;
            latitude = _xlSheet.Range["F" + row, "F" + row].Value2;
            longitude = _xlSheet.Range["G" + row, "G" + row].Value2;
            url = _xlSheet.Range["H" + row, "H" + row].Value2;
            date = _xlSheet.Range["I" + row, "I" + row].Value2;
            id = _xlSheet.Range["J" + row, "J" + row].Value2;

            return new Home(latitude, longitude, address, city, state, url, price, date, name, id);
        }

        //Return the contents of the sheet as a hashset
        public HashSet<Home> getAsHashSet()
        {
            HashSet<Home> homeHash = new HashSet<Home>();

            int row = 1;

            //Iterate until we find an empty cell
            while (!String.IsNullOrEmpty(_xlSheet.Range["A" + row, "A" + row].Value2))
            {
                string name;
                string address;
                string city;
                string state;
                double price;
                double latitude;
                double longitude;
                string url;
                string date;
                string id;

                name = _xlSheet.Range["A" + row, "A" + row].Value2;
                address = _xlSheet.Range["B" + row, "B" + row].Value2;
                city = _xlSheet.Range["C" + row, "C" + row].Value2;
                state = _xlSheet.Range["D" + row, "D" + row].Value2;
                price = _xlSheet.Range["E" + row, "E" + row].Value2;
                latitude = _xlSheet.Range["F" + row, "F" + row].Value2;
                longitude = _xlSheet.Range["G" + row, "G" + row].Value2;
                url = _xlSheet.Range["H" + row, "H" + row].Value2;
                date = _xlSheet.Range["I" + row, "I" + row].Value2;
                id = _xlSheet.Range["J" + row, "J" + row].Value2;
                row++;

                homeHash.Add(new Home(latitude, longitude, address, city, state, url, price, date, name, id));
            }

            return homeHash;
        }

        //Save and close the Excel file
        public void saveAndClose()
        {
            _xlWB.Save();
            _xlWB.Close();
        }

    }
}
