using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Prolonger.Services
{
    public static class DownloadManager
    {
        public static async Task Download(string _url, string _fileName)
        {
            using (var client = new WebClient())
            {
               await client.DownloadFileTaskAsync(new Uri(_url), _fileName);     
            }
        }

        public static string GetUrl(string _url)
        {
            List<string> list = new List<string>();       
            try
            {
                WebRequest request = WebRequest.Create(_url);
                var r = request.GetResponse();
                if (r != null)
                {
                    Debug.WriteLine(r);
                    WebResponse response = r;
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line = "";
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Contains("open_data_files"))
                                {
                                    list.Add(line);
                                }
                            }
                        }
                    }
                    response.Close();
                    return TextManager.GetUrl(list[1]);
                }
                else
                {
                    return "Exeption";
                }
            }
            catch
            {
                return "Exeption";
            }
        }

        public static List<string> GetUrlSTAN(string _url)
        {
            List<string> list = new List<string>();
            try
            {
                WebRequest request = WebRequest.Create(_url);
                var r = request.GetResponse();
                if (r != null)
                {
                    Debug.WriteLine(r);
                    WebResponse response = r;
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line = "";
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Contains("open_data_files"))
                                {
                                    string s = TextManager.GetUrlSTAN(line);
                                    if (s != "")
                                    {
                                        list.Add(s);
                                    }                              
                                }
                            }
                        }
                    }
                    response.Close();
                    return list;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


    }
}
