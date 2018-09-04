using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Casio
{
    public class CKCThread
    {
        public CKCPost[] mu_posts;
        public int mu_id;
        public string mu_board;

        public static string[] mu_validBoards = {"b", "pol", "int", "a", "alt", "bus", "c", "co", "d", "danisch", "e", "f", "fb", 
                                                /*"fefe",*/
                                                "fit", "foto", "g", /*"jp",*/ "l", "mali", "med", "mu", "n", "ng", "ph", "prog",
                                                "s", "soz", "trv", "tv", "v", "we", "x", "kohl", "km", "m", "keller", "ernst" };
        

        public CKCThread(System.Net.WebClient _wc,int _id, string _board)
        {
            mu_id = _id;
            mu_board = _board;


            JObject thread = JObject.Parse(_wc.DownloadString($"https://kohlchan.net/{mu_board}/res/{mu_id}.json"));
            mu_posts = thread["posts"].Select(p => new CKCPost(p)).ToArray();
        }
    }

    public class CKCPost
    {
        public int mu_id;
        public string mu_post;
        public string[] mu_files;
        

        public CKCPost(JToken _token)
        {
            mu_id = (int) _token["no"];
            mu_post = (string) _token["com"];
            if (_token["filename"] != null)
            {
                int count = 1 + (_token["extra_files"]?.Children().Count() ?? 0);
                mu_files = new string[count];

                mu_files[0] = (string) _token["filename"] +(string)_token["ext"];
                for(int i = 1; i < count; i++)
                {
                    mu_files[i] = (string)_token["extra_files"][i - 1]["filename"] + (string)_token["extra_files"][i - 1]["ext"];
                }
            }
        }
    }
}
