// <copyright file="Program.cs" company="GenericEventHandler">
//     Copyright (c) GenericEventHandler all rights reserved. Licensed under the Mit license.
// </copyright>

using System;
using System.Drawing;
using System.IO;
using Westwind.Utilities.Dynamic;

namespace ScrapProject
{
    /// <summary>The main program for the scrap project</summary>
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

                b.Save(filename + ".x.png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static void Main(string[] args)
        {
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
            foreach (var p in apple.GetProperties(true))
            {
                Console.WriteLine(outputFormat, p.Key, p.Value);
            }

            // we can use the property as well.
            dynamic appleX = apple;
            Console.WriteLine("By casting to dynamic we can access apple.Pips " + appleX.Pips);

            Console.Write("We can access the hard coded properties as well Name " + apple.Name + " and Color " +
                          apple.Color);
            Console.Write("We can access the hard coded properties as well via dynamic Name " + appleX.Name + " and Color " +
                          appleX.Color);
            Console.Write("We can also access like this apple[\"Pips\"] " + apple["Pips"]);
            Console.Write("And access the hard coded as well apple[\"Name\"] " + apple["Name"]);
            Console.ReadKey();
        }
    }
}