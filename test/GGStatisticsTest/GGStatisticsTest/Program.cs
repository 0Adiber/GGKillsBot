using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GGStatisticsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string site = dummerScheiss("https://www.gommehd.net/player/index?playerName=Adiber");

            int start = site.IndexOf("id=\"gungame\"");

            if (start < 0) return; //no results

            site = site.Substring(start);
            site = site.Substring(site.IndexOf("score")+7);
            site = site.Substring(0, site.IndexOf("</span>"));

            int kills;
            Int32.TryParse(site.Trim(), out kills);

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ".";

            Console.WriteLine(kills.ToString("#,0", nfi));

            Console.ReadKey();
        }

        public static string dummerScheiss(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            request.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            WebResponse response = request.GetResponse();

            string responseFromServer = "";
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
            }

            response.Close();
            return responseFromServer;
        }
    }
}
