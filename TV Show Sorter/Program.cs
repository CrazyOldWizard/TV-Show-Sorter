using System;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using WMPLib;

namespace TV_Show_Sorter
{
    class Program
    {

        public static string RootDir = ConfigurationManager.AppSettings["RootDir"];
        static Regex tvShow = new Regex(@"s\d\d");
        static Regex ShowNameRegex = new Regex(@".*?(?=\ss\d\d)");
        public static string seasonNumber;
        public static string showName;

        private static string ShowMatches(Regex r, Match m)
        {
            string[] names = r.GetGroupNames();
            foreach (var name in names)
            {
                Group grp = m.Groups[name];
                var sNumb = grp.Value.ToString();
                return seasonNumber = sNumb.Substring(1);
            }
            return null;
        }

        private static string ShowName(Regex r, Match m)
        {
            string[] names = r.GetGroupNames();
            foreach (var name in names)
            {
                Group grp = m.Groups[name];
                return showName = grp.Value.ToString();
            }
            return null;
        }


        private static void SearchFolders()
        {
            foreach (string file in Directory.GetFiles(RootDir, "*.*", SearchOption.AllDirectories))
            {
                string fileNoExt = Path.GetFileNameWithoutExtension(file);
                Match matches = tvShow.Match(fileNoExt);
                Match matchName = ShowNameRegex.Match(fileNoExt);

                if (tvShow.IsMatch(fileNoExt))
                {
                    string showName = ShowNameRegex.Match(fileNoExt).ToString();
                    string filename = Path.GetFileName(file);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(fileNoExt + " Is a TV show");
                    Console.ForegroundColor = ConsoleColor.White;
                    ShowMatches(tvShow, matches);
                    ShowName(ShowNameRegex, matchName);
                    Console.WriteLine(showName);
                    string showFolder = Path.Combine(RootDir, showName);
                    string seasonFolder = Path.Combine(showFolder, "Season " + seasonNumber);
                    string newFile = (seasonFolder + "\\" + filename);
                    if (!Directory.Exists(showFolder))
                    {
                        Directory.CreateDirectory(showFolder);
                    }
                    if (!Directory.Exists(seasonFolder))
                    {
                        Directory.CreateDirectory(seasonFolder);
                    }
                    if (!File.Exists(newFile))
                    {
                        File.Move(file, newFile);
                        Console.WriteLine("Moved " + filename + " to " + seasonFolder);
                    }

                }

                else
                {
                    var player = new WindowsMediaPlayer();
                    var clip = player.newMedia(file);

                    Console.WriteLine(fileNoExt + " is not a tv show");
                    Console.WriteLine("Lenght of file is: " + TimeSpan.FromSeconds(clip.duration));

                }
            }
            Console.WriteLine("Searching for files...");
        }



        static void Main()
        {
            while (true)
            {
                SearchFolders();
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
