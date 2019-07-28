using System;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using WMPLib;

namespace TV_Show_Sorter
{
    class Program
    {

        public static string SearchFolder = ConfigurationManager.AppSettings["SearchFolder"];
        public static string TVDestinationFolderConfig = ConfigurationManager.AppSettings["TVDestinationFolder"];
        public static string MovieDestinationFolderConfig = ConfigurationManager.AppSettings["MovieDestinationFolder"];
        public static string TVDestinationFolder;
        public static string MovieDestinationFolder;
        public static string EnableOutFolder = ConfigurationManager.AppSettings["EnableOutFolder"].ToLower();
        public static string MoviesFolder = SearchFolder + "\\" + "Movies";
        public static string FailedToSortFolder = SearchFolder + "\\" + "FailedToSort";

        static Regex tvShow = new Regex(@"s\d\d");
        static Regex ShowNameRegex = new Regex(@".*?(?=\ss\d\d)");
        public static string seasonNumber;
        public static string showName;

        private static void SetDestinationFolder()
        {
            if (EnableOutFolder == "false")
            {
                TVDestinationFolder = SearchFolder;
                MovieDestinationFolder = MoviesFolder;
            }
            else if (EnableOutFolder == "true")
            {
                TVDestinationFolder = TVDestinationFolderConfig;
                MovieDestinationFolder = MovieDestinationFolderConfig;
            }
        }

        public static bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

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
            foreach (string file in Directory.EnumerateFiles(SearchFolder, "*.*", SearchOption.AllDirectories))
            {
                string fileNoExt = Path.GetFileNameWithoutExtension(file);
                Match matches = tvShow.Match(fileNoExt);
                Match matchName = ShowNameRegex.Match(fileNoExt);

                FileAttributes attributes = File.GetAttributes(file);
                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }

                if (tvShow.IsMatch(fileNoExt))
                {
                    string showName = ShowNameRegex.Match(fileNoExt).ToString();
                    string filename = Path.GetFileName(file);
                    ShowMatches(tvShow, matches);
                    ShowName(ShowNameRegex, matchName);
                    string showFolder = Path.Combine(TVDestinationFolder, showName);
                    string seasonFolder = Path.Combine(showFolder, "Season " + seasonNumber);
                    string newFile = (seasonFolder + "\\" + filename);
                    if (File.Exists(newFile))
                    {
                        continue;
                    }
                    if (!Directory.Exists(showFolder))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Creating Show Folder: " + showName);
                        Console.ForegroundColor = ConsoleColor.White;
                        Directory.CreateDirectory(showFolder);
                    }
                    if (!Directory.Exists(seasonFolder))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Creating Season " + seasonNumber + " Folder for " + showName);
                        Console.ForegroundColor = ConsoleColor.White;
                        Directory.CreateDirectory(seasonFolder);
                    }
                    if (!File.Exists(newFile))
                    {
                        Console.WriteLine(fileNoExt + " Is a TV show");
                        if (IsFileReady(file) == true)
                        {
                            File.Move(file, newFile);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Moved " + filename + " to " + seasonFolder);
                            Console.ForegroundColor = ConsoleColor.White;
                            continue;
                        }
                        else
                        {
                            Console.WriteLine(filename + " Is open by another process, skipping...");
                            continue;
                        }
                    }
                }
                else
                {
                    var player = new WindowsMediaPlayer();
                    var clip = player.newMedia(file);
                    var length = clip.duration;

                    if (length >= 4200)
                    {
                        string movieName = Path.GetFileName(file);
                        string newMovie = Path.Combine(MovieDestinationFolder + "\\" + movieName);

                        if (File.Exists(newMovie))
                        {
                            continue;
                        }
                        if(!Directory.Exists(MovieDestinationFolder))
                        {
                            Console.WriteLine("Movie folder doesn't exist, creating...");
                            Directory.CreateDirectory(MovieDestinationFolder);
                        }
                        if (!File.Exists(newMovie))
                        {
                            if (IsFileReady(file) == true)
                            {
                                Console.WriteLine(fileNoExt + " Is a movie, moving to: " + MovieDestinationFolder);
                                File.Move(file, newMovie);
                                continue;
                            }
                        }
                    }

                    else if (IsFileReady(file) == true)
                    {

                        Console.WriteLine(fileNoExt + " Doesn't match search pattern and is not long enough to be a Movie!");
                        Console.WriteLine("Lenght of file is: " + TimeSpan.FromSeconds(clip.duration));
                        string failedSortFile = (FailedToSortFolder + "\\" + Path.GetFileName(file));
                        if (!Directory.Exists(FailedToSortFolder))
                        {
                            Directory.CreateDirectory(FailedToSortFolder);
                        }
                        if (!File.Exists(failedSortFile))
                        {
                            File.Move(file, failedSortFile);
                            continue;
                        }
                    }

                }
            }

        }

        static void Main()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(DateTime.Now.ToString());
                Console.ForegroundColor = ConsoleColor.White;
                SetDestinationFolder();
                Console.WriteLine("Searching for files...");
                SearchFolders();
                Console.WriteLine();
                Console.WriteLine("Finished");
                System.Threading.Thread.Sleep(5000);
                Console.Clear();
            }
        }
    }
}
