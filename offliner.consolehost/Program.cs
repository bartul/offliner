using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
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

            var output = Path.Combine(Environment.CurrentDirectory, "_output\\");
            if (Directory.Exists(output)) { Directory.Delete(output, true); }
            
            var firstLevelUrl = "http://www.vergic.com";
            var levelsDeep = 5;
            
            TakeOfflineAsync(firstLevelUrl, levelsDeep).Wait();          
            
            Console.WriteLine("Press any key...");
            Console.Read();
        }
        
        private static ConcurrentDictionary<string, PageInfo> processedPages = new ConcurrentDictionary<string, PageInfo>();
        private static Progress<GrabberProgressInfo> grabProgress = new Progress<GrabberProgressInfo>((p) => Console.WriteLine($"GRABBED {p.Page.Url} - {p.Page.HRefs.Count()} internal links."));
        private static PageGrabber grabber = new PageGrabber();            
        private static Progress<SaveProgressInfo> saveProgress = new Progress<SaveProgressInfo>((p) => Console.WriteLine($"SAVED {p.Filename} - Created and saved to disk."));
        private static PageSaver saver = new PageSaver();

        public static async Task TakeOfflineAsync(string url, int levelsDeep)
        {
            var newUrls = new string[] { url };
            for (var level = 1; level <= levelsDeep && newUrls.Length > 0; level++)
            {
                Console.WriteLine($"LEVEL {level} - Processing.");                
                
                var pages = await grabber.GrabAsync(newUrls, grabProgress)
                    .ContinueWith(async (p) => {
                        var page = p.Result; 
                        await saver.SaveAsync(page, saveProgress);
                        return page;})
                    .Unwrap();
                
                foreach(var page in pages) {
                    processedPages.TryAdd(page.Url, page);
                }
                
                newUrls = (from p in pages
                        from l in p.HRefs
                        where !processedPages.ContainsKey(l) 
                        select l).Distinct().ToArray();
            }           
        }
    }
}
