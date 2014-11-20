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
                              "\n    Get Account Info          gac" +
                              "\n    Create an Album           ca" +
                              "\n    Delete an Album           da" +
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
                    case "gac":
                        GetAccountTest(imgurTools);
                        break;
                    case "ca":
                        CreateAlbumTest(imgurTools);
                        break;
                    case "da":
                        DeleteAlbumTest(imgurTools);
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
                        Console.WriteLine("\nSorry, that command isn't recognized.");
                        break;
                }
            }
        }

        #region Private Test Methods
        private static void GetAlbumTest(Imgur imgur)
        {
            try
            {
                Console.WriteLine("\n=================================================\n" +
                                  "    Get information about an existing album" +
                                  "\n=================================================\n");
                Console.WriteLine("Please type an album ID: ");
                DumpAlbumInfo(imgur.GetAlbum(Console.ReadLine()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
        }

        private static void CreateAlbumTest(Imgur imgur)
        {
            try
            {
                Console.WriteLine("\n=================================================\n" +
                                  "    Create an Imgur Album" +
                                  "\n=================================================\n");
                Console.WriteLine("Give the album a title: ");
                var title = Console.ReadLine();

                Console.WriteLine("\nGive the album a description: ");
                var desc = Console.ReadLine();

                Console.WriteLine("\nSet the album's privacy (public, hidden, secret): ");
                var priv = Console.ReadLine();

                Console.WriteLine("\nSet the album's layout (blog, grid, horizontal, vertical): ");
                var lay = Console.ReadLine();

                DumpAlbumInfo(imgur.CreateAlbum(title, desc, priv, lay));
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
        }

        private static void DeleteAlbumTest(Imgur imgur)
        {
            try
            {
                Console.WriteLine("\n=================================================\n" +
                                  "    Delete an existing album" +
                                  "\n=================================================\n");
                Console.WriteLine("Please type a the delete hash: ");
                imgur.DeleteAlbum(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
        }

        private static void GetImageTest(Imgur imgur)
        {
            try
            {
                Console.WriteLine("\n=================================================\n" +
                                  "    Get information about an existing image" +
                                  "\n=================================================\n");
                Console.WriteLine("Please type an image ID: ");
                DumpImageInfo(imgur.GetImage(Console.ReadLine()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
        }

        private static void DeleteImageTest(Imgur imgur)
        {
            try
            {
                Console.WriteLine("\n=================================================\n" +
                                  "    Delete an existing image" +
                                  "\n=================================================\n");
                Console.WriteLine("Please type a the delete hash: ");
                imgur.DeleteImage(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
        }

        private static void UploadFromWebTest(Imgur imgur)
        {
            try
            {
                Console.WriteLine("\n=================================================\n" +
                                  "    Upload an image from the web" +
                                  "\n=================================================\n");

                Console.WriteLine("Please type the URL for the direct link:");
                var link = Console.ReadLine();

                Console.WriteLine("\nPlease give the image a title:");
                var title = Console.ReadLine();

                Console.WriteLine("\nPlease give the image a description:");
                var desc = Console.ReadLine();

                Console.WriteLine("\nType an album ID if you want to add it to an album:");
                var albId = Console.ReadLine();

                DumpImageInfo(imgur.UploadImageFromWeb(link, title, desc, albId));
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
        }

        private static void UploadFromFileTest(Imgur imgur)
        {
            try
            {
                Console.WriteLine("\n===============================================\n" +
                                  "    Upload an image from a file" +
                                  "\n===============================================\n");

                Console.WriteLine("Please type the file path for the image:");
                var path = Console.ReadLine();

                Console.WriteLine("\nPlease give the image a title:");
                var title = Console.ReadLine();

                Console.WriteLine("\nPlease give the image a description:");
                var desc = Console.ReadLine();

                Console.WriteLine("\nType an album ID if you want to add it to an album:");
                var albId = Console.ReadLine();

                DumpImageInfo(imgur.UploadImageFromFile(path, title, desc, albId));
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
        }

        private static void GetAccountTest(Imgur imgur)
        {
            try
            {
                Console.WriteLine("\n=================================================\n" +
                                  "    Get information about an existing account" +
                                  "\n=================================================\n");
                Console.WriteLine("Please type an account username: ");
                DumpAccountInfo(imgur.GetAccount(Console.ReadLine()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
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

        private static void DumpAccountInfo(ImgurAccount acc)
        {
            Console.WriteLine("\nAccount Info:");
            Console.WriteLine("    ID:          " + acc.ID);
            Console.WriteLine("    Link:        " + acc.Url);
            Console.WriteLine("    Bio:         " + acc.Bio);
            Console.WriteLine("    Reputation:  " + acc.Reputation);
            Console.WriteLine("    Created on:  " + acc.Created);
            Console.WriteLine("    Pro user:    " + acc.ProAccount);
        }

        private static void DumpAlbumInfo(ImgurAlbum alb)
        {
            Console.WriteLine("\nAlbum Info:");
            Console.WriteLine("    Link:        " + alb.Link);
            Console.WriteLine("    Image Count: " + alb.ImagesCount);
            Console.WriteLine("    Date Added:  " + alb.TimeAdded);
            Console.WriteLine("    Views:       " + alb.Views);
            Console.WriteLine("    Delete Hash: " + alb.DeleteHash);
        }
        #endregion
    }
}
