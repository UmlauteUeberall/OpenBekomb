using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Casio
{
    public static class CKohlchanParser
    {
        public static CKCThread[] Parse(string _board)
        {
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                List<JObject> pages = fi_GetPages(wc, _board);
                return pages.SelectMany(o => o["threads"].Select(p => new CKCThread(wc, (int)p["posts"][0]["no"], _board))).ToArray();
            }
            catch
            {
                return null;
            }
        }

        private static List<JObject> fi_GetPages(System.Net.WebClient _wc, string _board)
        {
            int counter = 0;
            List<JObject> toReturn = new List<JObject>();

            try
            {
                while (true)
                {
                    toReturn.Add(JObject.Parse(_wc.DownloadString($"https://kohlchan.net/{_board}/{counter}.json")));
                    counter++;
                }
            }
            catch
            {
                
            }

            return toReturn;
        }

        public static CKCThread[] FindWordInThreads(CKCThread[] _threads, string _word)
        {
            Regex r = new Regex(_word, RegexOptions.IgnoreCase);
            return _threads.Where(o => o.mu_posts.LastOrDefault(p => r.IsMatch(p)) != null).ToArray();
        }

        public static int FindCountInThreads(CKCThread[] _threads, string _word)
        {
            Regex r = new Regex(_word, RegexOptions.IgnoreCase);
            return _threads.Sum((o) => o.mu_posts.Count(p => p != null && r.IsMatch(p)));
        }
    }
}
