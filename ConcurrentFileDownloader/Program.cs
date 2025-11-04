using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConcurrentFileDownloader
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 1️⃣ List of file URLs to download
            List<string> urls = new List<string>
            {
                "https://www.learningcontainer.com/wp-content/uploads/2020/04/sample-text-file.txt",
                "https://file-examples.com/storage/fe1c4a95dcd812f7077d70f/2017/10/file-example_PNG_500kB.png",
                "https://file-examples.com/storage/febd6895542c2d2d59b3c73/2017/10/file-example_JPG_1MB.jpg",
                "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                "https://jsonplaceholder.typicode.com/posts/1"
            };

            string outputFolder = @"C:\Users\Mohit.Bagul\source\repos\ConcurrentFileDownloader\ConcurrentFileDownloader\Downloads\"; // 📂 Folder to save downloaded files
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            using HttpClient client = new HttpClient();
            List<Task> downloadTasks = new List<Task>();

            Console.WriteLine("Starting concurrent downloads...\n");
            Stopwatch stopwatch = Stopwatch.StartNew();

            // 2️⃣ Create and start a Task for each download
            int fileCounter = 1;
            foreach (string url in urls)
            {
                int currentFileNumber = fileCounter++;
                downloadTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        byte[] data = await client.GetByteArrayAsync(url);
                        string extension = Path.GetExtension(url);
                        if (string.IsNullOrWhiteSpace(extension))
                            extension = ".dat"; // fallback if URL has no extension

                        string filePath = Path.Combine(outputFolder, $"File_{currentFileNumber}{extension}");

                        await File.WriteAllBytesAsync(filePath, data);
                        Console.WriteLine($"✅ Downloaded: {Path.GetFileName(filePath)}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error downloading {url}: {ex.Message}");
                    }
                }));
            }

            // 3️⃣ Wait for all downloads to complete
            await Task.WhenAll(downloadTasks);

            stopwatch.Stop();
            Console.WriteLine($"\n🎉 All downloads completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
