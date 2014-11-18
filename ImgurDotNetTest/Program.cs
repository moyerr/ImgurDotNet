using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using ImgurDotNet;

namespace ImgurDotNetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n=================================================\n" +
                              "    Welcome to ImgurDotNetTest! Below you will\n" +
                              "    find a list of valid commands to test the\n" +
                              "    ImgurDotNet APIs." +
                              "\n=================================================");
            Console.WriteLine("    Get Album Info:           ga" +
                              "\n    Get Image Info:           gi" +
                              "\n    Upload Image From Web:    uiw" +
                              "\n    Upload Image from File:   uif" +
                              "\n    Delete an Image:          di" +
                              "\n    Quit:                     quit" +
                              "\n=================================================");

            Console.WriteLine("\nFirst, please type your Client ID:");
            var clientId = Console.ReadLine();

            var command = "continue";
            var imgurTools = new Imgur(clientId);

            while (command != "quit")
            {
                Console.WriteLine("\nNext Command: ");
                command = Console.ReadLine();

                switch (command)
                {
                    case "ga":
                        GetAlbumTest(imgurTools);
                        break;
                    case "gi":
                        GetImageTest(imgurTools);
                        break;
                    case "uiw":
                        UploadFromWebTest(imgurTools);
                        break;
                    case "uif":
                        UploadFromFileTest(imgurTools);
                        break;
                    case "di":
                        DeleteImageTest(imgurTools);
                        break;
                    case "quit":
                        Console.WriteLine("\nClosing application...");
                        Thread.Sleep(2000);
                        break;
                    default:
                        Console.WriteLine("Sorry, that command isn't recognized.");
                        break;
                }
            }
        }

        #region Private Test Methods
        private static void GetAlbumTest(Imgur imgur)
        {
            Console.WriteLine("\n=================================================\n" +
                              "    Get information about an existing album" +
                              "\n=================================================\n");
            Console.WriteLine("Please type an album ID: ");
            DumpAlbumInfo(imgur.GetAlbum(Console.ReadLine()));
        }

        private static void GetImageTest(Imgur imgur)
        {
            Console.WriteLine("\n=================================================\n" +
                              "    Get information about an existing image" +
                              "\n=================================================\n");
            Console.WriteLine("Please type an image ID: ");
            DumpImageInfo(imgur.GetImage(Console.ReadLine()));
        }

        private static void DeleteImageTest(Imgur imgur)
        {
            Console.WriteLine("\n=================================================\n" +
                              "            Delete an existing image" +
                              "\n=================================================\n");
            Console.WriteLine("Please type a the delete hash: ");
            imgur.DeleteImage(Console.ReadLine());
        }

        private static void UploadFromWebTest(Imgur imgur)
        {
            Console.WriteLine("\n=================================================\n" +
                              "          Upload an image from the web" +
                              "\n=================================================\n");
            
            Console.WriteLine("Please type the URL for the direct link:");
            var link = Console.ReadLine();

            Console.WriteLine("\nPlease give the image a title:");
            var title = Console.ReadLine();

            Console.WriteLine("\nPlease give the image a description:");
            var desc = Console.ReadLine();

            DumpImageInfo(imgur.UploadImageFromWeb(link, title, desc));
        }

        private static void UploadFromFileTest(Imgur imgur)
        {
            Console.WriteLine("\n===============================================\n" +
                              "          Upload an image from a file" +
                              "\n===============================================\n");

            Console.WriteLine("Please type the file path for the image:");
            var path = Console.ReadLine();

            Console.WriteLine("\nPlease give the image a title:");
            var title = Console.ReadLine();

            Console.WriteLine("\nPlease give the image a description:");
            var desc = Console.ReadLine();

            DumpImageInfo(imgur.UploadImageFromFile(path, title, desc));
        }

        private static void DumpImageInfo(ImgurImage img)
        {
            Console.WriteLine("\nImage Info:");
            Console.WriteLine("    ID:          " + img.ID);
            Console.WriteLine("    Link:        " + img.Link);
            Console.WriteLine("    Image Type:  " + img.Type);
            Console.WriteLine("    Date Added:  " + img.TimeAdded);
            Console.WriteLine("    Views:       " + img.Views);
            Console.WriteLine("    Delete Hash: " + img.DeleteHash);
        }

        private static void DumpAlbumInfo(ImgurAlbum alb)
        {
            Console.WriteLine("\nAlbum Info:");
            Console.WriteLine("    Link:        " + alb.Link);
            Console.WriteLine("    Image Count: " + alb.ImagesCount);
            Console.WriteLine("    Date Added:  " + alb.TimeAdded);
            Console.WriteLine("    Views:       " + alb.Views);
        }
        #endregion
    }
}
