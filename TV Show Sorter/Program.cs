using System;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using WMPLib;

namespace TV_Show_Sorter
{
    class Program
    {

        //new Microsoft.VisualBasic.Devices.Computer().FileSystem.MoveDirectory(sourceDir, destDir, overwrite);

        public static string SearchFolder = ConfigurationManager.AppSettings["SearchFolder"];
        public static string TVDestinationFolderConfig = ConfigurationManager.AppSettings["TVDestinationFolder"];
        public static string MovieDestinationFolderConfig = ConfigurationManager.AppSettings["MovieDestinationFolder"];
        public static string TVDestinationFolder;
        public static string MovieDestinationFolder;
        public static string EnableOutFolder = ConfigurationManager.AppSettings["EnableOutFolder"].ToLower();
        public static string MoviesFolder = Path.Combine(SearchFolder, "Movies");
        public static string FailedToSortFolder = Path.Combine(SearchFolder, ".FailedToSort");
        public static bool SortFoldersInstead = bool.Parse(ConfigurationManager.AppSettings["SortFoldersInstead"].ToLower());

        static Regex tvShow = new Regex(@"s\d\d", RegexOptions.IgnoreCase);
        static Regex ShowNameRegex = new Regex(@".*?(?=\ss\d\d)", RegexOptions.IgnoreCase);
        public static string seasonNumber;
        public static string showName;

        public static void MsgInfo(string message)
        {
            //Console.Write(DateTime.Now.ToString() + "  ");
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("INFO");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(message);
            Console.WriteLine();

        }
        public static void MsgStatus(string message)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Status");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(message);
            Console.WriteLine();

        }
        public static void MsgError(string message)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Error");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(message);
            Console.WriteLine();

        }

        private static void SetDestinationFolder()
        {
            if (EnableOutFolder == "false")
            {
                if(TVDestinationFolder != SearchFolder)
                {
                    TVDestinationFolder = SearchFolder;
                }
                if(MovieDestinationFolder != MoviesFolder)
                {
                    MovieDestinationFolder = MoviesFolder;
                }
            }
            else if (EnableOutFolder == "true")
            {
                if(TVDestinationFolder != TVDestinationFolderConfig)
                {
                    TVDestinationFolder = TVDestinationFolderConfig;
                }
                if(MovieDestinationFolder != MovieDestinationFolderConfig)
                {
                    MovieDestinationFolder = MovieDestinationFolderConfig;
                }
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

        private static void SortFiles()
        {
            foreach (string file in Directory.EnumerateFiles(SearchFolder, "*.*", SearchOption.AllDirectories))
            {
                if(Path.GetDirectoryName(file) == FailedToSortFolder)
                {
                    continue;
                }
                string fileNoExt = Path.GetFileNameWithoutExtension(file).Replace(".", " ");
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
                    string fileExt = Path.GetExtension(file);
                    string newFile = (seasonFolder + "\\" + filename);
                    if (File.Exists(newFile))
                    {
                        continue;
                    }
                    if (!Directory.Exists(showFolder))
                    {
                        MsgStatus("Creating Show Folder: " + showName);
                        Directory.CreateDirectory(showFolder);
                    }
                    if (!Directory.Exists(seasonFolder))
                    {
                        MsgStatus("Creating Season " + seasonNumber + " Folder for " + showName);
                        Directory.CreateDirectory(seasonFolder);
                    }
                    if (!File.Exists(newFile))
                    {
                        MsgInfo(fileNoExt + " Is a TV show");
                        try
                        {
                            MsgStatus("Moving " + filename + " to " + seasonFolder);
                            File.Move(file, newFile);
                            MsgStatus("Moved " + filename + " to " + seasonFolder);
                            continue;
                        }
                        catch (Exception e)
                        {
                            MsgError(e.Message);
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
                        if (!Directory.Exists(MovieDestinationFolder))
                        {
                            MsgInfo("Movie folder doesn't exist, creating...");
                            Directory.CreateDirectory(MovieDestinationFolder);
                        }
                        if (!File.Exists(newMovie))
                        {
                            try
                            {
                                MsgStatus(fileNoExt + " Is a movie, moving to: " + MovieDestinationFolder);
                                File.Move(file, newMovie);
                                continue;
                            }
                            catch (Exception e)
                            {
                                MsgError(e.Message);
                            }
                        }
                    }

                    else
                    {
                        string failedSortFile = (FailedToSortFolder + "\\" + Path.GetFileName(file));
                        if (!Directory.Exists(FailedToSortFolder))
                        {
                            MsgStatus(FailedToSortFolder + " Does not exist, creating...");
                            Directory.CreateDirectory(FailedToSortFolder);
                        }
                        if (!File.Exists(failedSortFile))
                        {
                            MsgInfo(fileNoExt + " Doesn't match search pattern and is not long enough to be a Movie!");
                            MsgInfo("Lenght of file is: " + TimeSpan.FromSeconds(clip.duration));
                            try
                            {
                                File.Move(file, failedSortFile);
                                MsgStatus("Moved: " + fileNoExt + " To: " + FailedToSortFolder);
                            }
                            catch (Exception e)
                            {
                                MsgError(e.Message);
                            }
                            continue;
                        }
                    }
                }
            }

        }

        private static void SortFolders()
        {
            foreach (string folder in Directory.EnumerateFiles(SearchFolder, "*.*", SearchOption.AllDirectories))
            {
                if (Path.GetDirectoryName(folder) == FailedToSortFolder)
                {
                    continue;
                }
                var dInfo = new DirectoryInfo(folder);
                string folderNameOnly = dInfo.Name.Replace(".", " ");
                Match matches = tvShow.Match(folderNameOnly);
                Match matchName = ShowNameRegex.Match(folderNameOnly);

                var attributes = dInfo.Attributes;
                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }

                if (tvShow.IsMatch(folderNameOnly))
                {
                    string showName = ShowNameRegex.Match(folderNameOnly).ToString();
                    string folderName = folderNameOnly;
                    ShowMatches(tvShow, matches);
                    ShowName(ShowNameRegex, matchName);
                    string showFolder = Path.Combine(TVDestinationFolder, showName);
                    string seasonFolder = Path.Combine(showFolder, "Season " + seasonNumber);
                    string newFolder = Path.Combine(seasonFolder, folderName);
                    if (Directory.Exists(newFolder))
                    {
                        continue;
                    }
                    if (!Directory.Exists(showFolder))
                    {
                        MsgStatus("Creating Show Folder: " + showName);
                        Directory.CreateDirectory(showFolder);
                    }
                    if (!Directory.Exists(seasonFolder))
                    {
                        MsgStatus("Creating Season " + seasonNumber + " Folder for " + showName);
                        Directory.CreateDirectory(seasonFolder);
                    }
                    if (!Directory.Exists(newFolder))
                    {
                        MsgInfo(folderNameOnly + " Is a TV show");
                        try
                        {
                            MsgStatus("Moving " + folderName + " to " + seasonFolder);
                            new Microsoft.VisualBasic.Devices.Computer().FileSystem.MoveDirectory(folder, newFolder, true);
                            MsgStatus("Moved " + folderName + " to " + seasonFolder);
                            continue;
                        }
                        catch (Exception e)
                        {
                            MsgError(e.Message);
                            continue;
                        }
                    }
                }
                else
                {
                    var player = new WindowsMediaPlayer();
                    var clip = player.newMedia(folder);
                    var length = clip.duration;

                    if (length >= 4200)
                    {
                        string movieName = folderNameOnly;
                        string newMovie = Path.Combine(MovieDestinationFolder, movieName);

                        if (Directory.Exists(newMovie))
                        {
                            continue;
                        }
                        if (!Directory.Exists(MovieDestinationFolder))
                        {
                            MsgInfo("Movie folder doesn't exist, creating...");
                            Directory.CreateDirectory(MovieDestinationFolder);
                        }
                        if (!Directory.Exists(newMovie))
                        {
                            try
                            {
                                MsgStatus(folderNameOnly + " Is a movie, moving to: " + MovieDestinationFolder);
                                new Microsoft.VisualBasic.Devices.Computer().FileSystem.MoveDirectory(folder, newMovie, true);
                                continue;
                            }
                            catch (Exception e)
                            {
                                MsgError(e.Message);
                            }
                        }
                    }

                    else
                    {
                        string failedSortFile = Path.Combine(FailedToSortFolder, folderNameOnly);
                        if (!Directory.Exists(FailedToSortFolder))
                        {
                            MsgStatus(FailedToSortFolder + " Does not exist, creating...");
                            Directory.CreateDirectory(FailedToSortFolder);
                        }
                        if (!Directory.Exists(failedSortFile))
                        {
                            MsgInfo(folderNameOnly + " Doesn't match search pattern and is not long enough to be a Movie!");
                            MsgInfo("Lenght of file is: " + TimeSpan.FromSeconds(clip.duration));
                            try
                            {
                                new Microsoft.VisualBasic.Devices.Computer().FileSystem.MoveDirectory(folder, failedSortFile, true);
                                MsgStatus("Moved: " + folderNameOnly + " To: " + FailedToSortFolder);
                            }
                            catch (Exception e)
                            {
                                MsgError(e.Message);
                            }
                            continue;
                        }
                    }
                }
            }

        }

        static void Main()
        {
            SetDestinationFolder();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(DateTime.Now.ToString());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                
                MsgInfo("Searching for files...");
                if(SortFoldersInstead == true)
                {
                    SortFolders();
                }
                else
                {
                    SortFiles();
                }
                
                MsgInfo("Finished");
                System.Threading.Thread.Sleep(5000);
                Console.Clear();
            }
        }
    }
}
