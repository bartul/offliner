using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Offliner 
{
    public class PageGrabber
    {

            private readonly static string regexString =  "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            private readonly static ThreadLocal<Regex> _Regex = new ThreadLocal<Regex>(() => new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase));

            public async Task<PageInfo> GrabAsync(string url, IProgress<GrabberProgressInfo> progress) 
            {
                var content = await new WebClient().DownloadStringTaskAsync(url);
                var hrefs = _Regex.Value.Matches(content).Values()
                    .Where(href => !string.IsNullOrWhiteSpace(href) && href.StartsWith(url) && href.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                    .Distinct()
                    .ToArray();
                
                var page = new PageInfo(url, content, hrefs);
                progress.Report(new GrabberProgressInfo(page, true));
                return page; 
            }
            public async Task<PageInfo[]> GrabAsync(string[] urls, IProgress<GrabberProgressInfo> progress) 
            {
                var pages = from url in urls
                            select this.GrabAsync(url, progress);    
                return await Task.WhenAll<PageInfo>(pages); 
            }

    }
    public class GrabberProgressInfo
    {
        public GrabberProgressInfo(PageInfo page, bool isDone) { Page = page; IsDone = isDone; }
        
        public PageInfo Page { get; }
        public bool IsDone { get; }
    }
    public class PageInfo 
    {
        public PageInfo(string url, string content, string[] hrefs) { Url = url; Content = content; HRefs = hrefs; }
        
        public string Url { get; }
        public string Content { get; }
        public string[] HRefs { get; }
    }  
    
    public static class PageGrabberExtensions
    {
        public static IEnumerable<string> Values(this MatchCollection matches)
        {
            foreach (Match match in matches) yield return match.Groups[1].Value;
        }
    }
}   