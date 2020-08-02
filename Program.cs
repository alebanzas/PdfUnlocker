using RawPrint;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PdfUnlocker
{
    class Program
    {
        static void Main(string[] args)
        {
            //Microsoft Print to PDF

            //foreach (string printerName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            //{
            //    Console.WriteLine(printerName);
            //}
            //Console.ReadLine();
            //return;


            Console.WriteLine("Path to files:");
            string originalFolder = Console.ReadLine();

            string[] filePaths = Directory.GetFiles(originalFolder, "*.pdf", SearchOption.AllDirectories);

            Parallel.ForEach(filePaths, (f) =>
            {
                string path = Path.GetDirectoryName(f);
                string filename = Path.GetFileName(f);

                ProcessFile(path, filename);
            });

            Console.WriteLine("Done! Key to exit.");
            Console.ReadLine();


            return;
        }

        private static void ProcessFile(string path, string filename)
        {
            PrintDoc(path, filename);
            
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string sourceFileName = Path.Combine(
                                documents,
                                string.Join(".", new[] { GetUnlockedFileName(filename), "pdf" }));
            string destFileName = Path.Combine(
                                path,
                                string.Join(".", new[] { GetUnlockedFileName(filename), "pdf" }));

            int ok = 0;
            while (ok < 360)
            {
                while (!File.Exists(sourceFileName) && ok < 360)
                {
                    Thread.Sleep(1000);
                    ok++;
                }
                try
                {
                    File.Move(sourceFileName, destFileName);
                    ok = 1000;
                }
                catch (Exception e)
                {
                    ok++;
                    Thread.Sleep(1000);
                }
            }
            if(ok != 1000)
            {
                Console.WriteLine("ERROR:" + destFileName);
            }
            else
            {
                Console.WriteLine("OK: " + destFileName);
            }
        }

        private static void PrintDoc(string path, string filename)
        {
            // Absolute path to your PDF to print (with filename)
            string Filepath = Path.Combine(path, filename);
            string PrinterName = "PDFCreator";
            IPrinter printer = new Printer();
            printer.PrintRawFile(PrinterName, Filepath, GetUnlockedFileName(filename));
        }

        private static string GetUnlockedFileName(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename) + "_unlocked";
        }
    }
}
