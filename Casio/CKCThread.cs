using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Casio
{
    public class CKCThread
    {
        public string[] mu_posts;
        public int mu_id;
        public string mu_board;
        

        public CKCThread(System.Net.WebClient _wc,int _id, string _board)
        {
            mu_id = _id;
            mu_board = _board;


            JObject thread = JObject.Parse(_wc.DownloadString($"https://kohlchan.net/{mu_board}/res/{mu_id}.json"));
            mu_posts = thread["posts"].Select(p => (string)p["com"]).Where(o => o != null).ToArray();
        }

       
    }
}
