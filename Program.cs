using System;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using GMap.NET;
using System.Windows.Forms;

namespace Houses
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());  
        }
    }
}
