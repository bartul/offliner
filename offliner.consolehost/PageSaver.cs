using System;
using System.Net;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Offliner;

namespace Offliner 
{
    public class PageSaver
    {

        public async Task SaveAsync(PageInfo page, IProgress<SaveProgressInfo> progress) 
        {
            var output = Path.Combine(Environment.CurrentDirectory, "_output\\");
            if (!Directory.Exists(output)) { Directory.CreateDirectory(output); }
            
            var content = page.Content;
            foreach(var href in page.HRefs) { content = content.Replace($"href=\"{href}\"", $"href=\"{href.ToHtmlFilename()}\""); }
            var filename = Path.Combine(output, page.Url.ToHtmlFilename());
            byte[] encodedText = Encoding.Unicode.GetBytes(content);

            using (FileStream sourceStream = new FileStream(filename,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
            progress.Report(new SaveProgressInfo(filename, true));
        }
        public async Task SaveAsync(PageInfo[] pages, IProgress<SaveProgressInfo> progress) 
        {
            var tasks = from page in pages
                        select this.SaveAsync(page, progress);    
            await Task.WhenAll(tasks); 
        }

    }

    public class SaveProgressInfo
    {
        public SaveProgressInfo(string filename, bool isDone) { Filename = filename; IsDone = isDone; }
        
        public string Filename { get; }
        public bool IsDone { get; }            
    }
    
    public static class PageSaverExtensions
    {
        public static string ToHtmlFilename(this string value)
        {
            // TODO: do replace smarter with Regex("[-/]|[\n]{2}") and for all Path.GetInvalidPathChars()
            return value.Replace(':', '-').Replace('/', '-') + ".html";
        }
    }
}   