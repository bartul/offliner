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
            
            var grebProgress = new Progress<GrebberProgressInfo>((p) => Console.WriteLine($"GRAB {p.Page.Url} - {p.Page.HRefs.Count()} in site links like '{p.Page.HRefs.LastOrDefault()}' - Done!!!"));
            var grebber = new PageGrabber();            
            var saveProgress = new Progress<SaveProgressInfo>((p) => Console.WriteLine($"SAVE {p.Filename} - Created and saved to disk."));
            var saver = new PageSaver();
            
            var firstLevelUrl = "http://www.vergic.com";
            
            grebber
                .GrabAsync(firstLevelUrl, grebProgress)
                .ContinueWith(async (p) => { 
                    var secondLevelGreb = grebber.GrabAsync(p.Result.HRefs, grebProgress);
                    await Task.WhenAll(saver.SaveAsync(p.Result, saveProgress), secondLevelGreb);
                    await saver.SaveAsync(secondLevelGreb.Result, saveProgress);
                    })
                .Wait();
            
            Console.WriteLine("Press any key...");
            Console.Read();
        }
    }
}
