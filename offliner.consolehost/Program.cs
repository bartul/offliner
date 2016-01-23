using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Offliner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Let's go!");
            
            var progress = new Progress<DownloadProgressInfo>((p) => Console.WriteLine($"{p.Url} - Done!!!"));
            Task.WaitAll(
                DownloadAsync("http://www.index.hr", progress),
                DownloadAsync("http://www.slobodnadalmacija.hr", progress),
                DownloadAsync("http://www.forum.tm", progress),
                DownloadAsync("http://www.tportal.hr", progress),
                DownloadAsync("http://www.hrt.hr", progress),
                DownloadAsync("http://www.jutarnji.hr", progress),
                DownloadAsync("http://www.net.hr", progress)
            );
            
            Console.WriteLine("Press any key...");
            Console.Read();
        }


        public static async Task<string> DownloadAsync(string url, IProgress<DownloadProgressInfo> progress) {
          var content = await new WebClient().DownloadStringTaskAsync(url);
          progress.Report(new DownloadProgressInfo(url, true));
          return content; 
        }  
        public class DownloadProgressInfo
        {
            public DownloadProgressInfo(string url, bool isDone) { Url = url; IsDone = isDone; }
            public string Url { get; }
            public bool IsDone { get; }
        }
    }
}
