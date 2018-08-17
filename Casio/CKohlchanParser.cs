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
        public static CKCThread[] Parse()
        {
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                JObject[] pages = Enumerable.Range(0, 15).Select(o => JObject.Parse(wc.DownloadString($"https://kohlchan.net/b/{o}.json"))).ToArray();
                return pages.SelectMany(o => o["threads"].Select(p => new CKCThread(wc, (int)p["posts"][0]["no"], "b"))).ToArray();
            }
            catch
            {
                return null;
            }
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
