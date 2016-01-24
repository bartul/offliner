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
            

            var progress = new Progress<GrebberProgressInfo>((p) => Console.WriteLine($"{p.Page.Url} - {p.Page.HRefs.Count()} links like '{p.Page.HRefs.LastOrDefault()}' - Done!!!"));
            var grebber = new PageGrabber();            
            
            var firstLevelUrl = "http://www.vergic.com";
            
            var x = grebber.GrabAsync(firstLevelUrl, progress).Result;
            
            foreach (var url in x.HRefs) Console.WriteLine($"Link {url}.");
            
            Console.WriteLine("Press any key...");
            Console.Read();
        }
    }
}
