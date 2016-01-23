using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Offliner;

namespace Offliner
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Let's go!");
            
            var regexString =  "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            var regex = new ThreadLocal<Regex>(() => new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase));

            var progress = new Progress<DownloadProgressInfo>((p) => Console.WriteLine($"{p.Page.Url} - {p.Page.HRefs.Count()} links like '{p.Page.HRefs.LastOrDefault()}' - Done!!!"));
            Task.WaitAll(
                DownloadAsync("http://www.index.hr", progress),
                DownloadAsync("http://www.slobodnadalmacija.hr", progress),
                DownloadAsync("http://www.forum.tm", progress),
                DownloadAsync("http://www.ina.hr", progress),
                DownloadAsync("http://www.hrt.hr", progress),
                DownloadAsync("http://www.jutarnji.hr", progress),
                DownloadAsync("http://www.net.hr", progress)
            );
            
            Console.WriteLine("Press any key...");
            Console.Read();
        }


        public static async Task<string> DownloadAsync(string url, IProgress<DownloadProgressInfo> progress) {
            var regexString =  "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            var regex = new ThreadLocal<Regex>(() => new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase));

            var content = await new WebClient().DownloadStringTaskAsync(url);
            var hrefs = regex.Value.Matches(content).Values().Where(href => !string.IsNullOrWhiteSpace(href) && href.StartsWith(url)).ToArray();
            
            progress.Report(new DownloadProgressInfo(new PageInfo(url, content, hrefs), true));
            return content; 
        }
        public class DownloadProgressInfo
        {
            public DownloadProgressInfo(PageInfo page, bool isDone) { Page = page; IsDone = isDone; }
            
            public PageInfo Page { get; }
            public bool IsDone { get; }
        }
        public class PageInfo {
            public PageInfo(string url, string content, string[] hrefs) { Url = url; Content = content; HRefs = hrefs; }
            
            public string Url { get; }
            public string Content { get; }
            public string[] HRefs { get; }
        }  
        
        
        public static IEnumerable<string> Values(this MatchCollection matches){
            foreach (Match match in matches) yield return match.Groups[1].Value;
        }
    }
}
