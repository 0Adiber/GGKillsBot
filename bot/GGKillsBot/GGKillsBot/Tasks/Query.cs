using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GGKillsBot.Tasks
{
    class Query : ITask
    {
        public Query()
        {

        }

        public override bool Exec()
        {
            return true;
        }

        public override void Start()
        {
            player.events.onChat += OnChat;
        }

        public override void Stop()
        {
            player.events.onChat -= OnChat;
        }

        private void OnChat(IPlayer player, IChat message, byte position)
        {
            string msg = message.GetText();

            if (msg.StartsWith("[Clans]"))
            {
                if (!msg.Contains(":")) return;
                string[] parts = msg.Split(new char[] { ':' });
                if (!parts[1].Trim().StartsWith(".kills")) return;

                string[] cmd = parts[1].Trim().Split(new char[] { ' ' });
                if (cmd.Length != 2) return;

                string site = getPageAsString("https://www.gommehd.net/player/index?playerName=" + cmd[1]);

                int start = site.IndexOf("id=\"gungame\"");

                if (start < 0)
                {
                    player.functions.Chat("/cc [KillsBot] Keine Ergebnisse für diesen Namen."); //no results
                    return;
                }

                site = site.Substring(start);
                site = site.Substring(site.IndexOf("score") + 7);
                site = site.Substring(0, site.IndexOf("</span>"));

                int kills;
                Int32.TryParse(site.Trim(), out kills);

                var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                nfi.NumberGroupSeparator = ".";

                player.functions.Chat("/cc [KillsBot] " + cmd[1] + ": " + kills.ToString("#,0", nfi) + " kills.");
            }
        }

        public static string getPageAsString(string url)
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
