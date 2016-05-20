// <copyright file="Program.cs" company="GenericEventHandler">
//     Copyright (c) GenericEventHandler all rights reserved. Licensed under the Mit license.
// </copyright>
namespace ScrapProject
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The main program for the scrap project
    /// </summary>
    internal static class Program
    {
        private static void Encode(string filename)
        {
            using (Bitmap b = new Bitmap("beach.png"))
            {
                var width = b.Width;
                var height = b.Height;
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.Clear(Color.White);

                    var si = new FileInfo(filename);
                    using (var fs = si.OpenRead())
                    {
                        var x = 0;
                        var y = 0;
                        var byt = 0;
                        while (byt >= 0)
                        {
                            byt = fs.ReadByte();
                            byte green;
                            byte red;
                            byte blue;
                            int a;
                            if (byt >= 0)
                            {
                                var pixel = b.GetPixel(x, y);

                                green = pixel.G;
                                red = pixel.R;
                                blue = pixel.B;
                                a = 255 - pixel.A;

                                b.SetPixel(x, y, Color.FromArgb(a, Color.FromArgb(red, green, blue)));
                                x = x + 2;
                                if (x >= width)
                                {
                                    x = 0;
                                    y = y + 1;
                                    if (y >= height)
                                    {
                                        throw new OutOfMemoryException("not enought room");
                                    }
                                }
                            }
                        }
                    }
                }

                b.Save(string.Format(CultureInfo.CurrentCulture, "{0}.x.png", filename), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static void Main(string[] args)
        {
            var asyncCall = new Action(ExtensionMethods.TestAysnc).ExecuteAsync();

            Encode(@"temp.txt");
            var apple = new Fruit { Name = "Granny smith", Color = "Green" };
            apple["Size"] = 3.0;
            apple["Pips"] = 5;

            if (args.Length == 2)
            {
                apple[args[0]] = args[1];
            }

            Console.WriteLine("Writing out all properties, dynamic and hardcoded");
            const string outputFormat = "{0}\t\t\t{1}";
            var ps = apple.GetProperties(true).ToArray();
            if (ps != null)
            {
                foreach (var p in ps)
                {
                    Console.WriteLine(outputFormat, p.Key, p.Value);
                }
            }

            // we can use the property as well.
            dynamic appleX = apple;
            Console.WriteLine("By casting to dynamic we can access apple.Pips {0}\n", appleX.Pips);

            Console.Write("We can access the hard coded properties as well Name {0} and Color {1}\n", apple.Name, apple.Color);
            Console.Write("We can access the hard coded properties as well via dynamic Name {0} and Color {1}\n", appleX.Name, appleX.Color);
            Console.Write("We can also access like this apple[\"Pips\"] {0}\n", apple["Pips"]);
            Console.WriteLine("And access the hard coded as well apple[\"Name\"] {0}\n", apple["Name"]);
            Console.WriteLine("=== Waiting for asyncs to finish. ===\n");

            asyncCall.WaitAndDispose();

            var start = DateTime.Now;
            var path = DirectoryHelper.FindFile("Fruit.cs");
            Console.WriteLine("Found: {0} in {1} ms", path, DateTime.Now.Subtract(start).TotalMilliseconds.ToString());
            Console.WriteLine("---press any key---");
            Console.ReadKey();
        }
    }
}