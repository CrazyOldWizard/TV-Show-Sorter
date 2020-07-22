using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TV_Show_Sorter
{
    public class Cleaning
    {

        public string[] junkStrings = {
            "HEVC",
            "x265",
            "x264",
            "BluRay",
            "Blu-Ray",
            "Blu Ray",
            "ISO",
            "AAC",
            "AC3",
            "(Dual-Audio)",
            "Dual-Audio",
            "DualAudio",
            "Dual.Audio",
            "Dual Audio",
            "10bit",
            "10-Bit",
            "10 Bit",
            "10.Bit",
            "1080p",
            "720p",
            "480p",
            "DTS",
            "TrueHD",
            "TRUE-HD",
            "-iAHD",
            "iAHD",
            "LPCM",
            "PCM",
            "264",
            "265",
            "DVD",
            "MKED",
            "WEB-DL",
            "WEBDL",
            "WEB DL",
            "WEB.RIP",
            "WEB.DL",
            "WEBRIP",
            "WEB-Rip",
            "WEB RIP",
            "8BIT",
            "8-BIT",
            "8 Bit",
            "8.bit",
            "RARBG",
            "REMUX",
            "DTS-HD",
            "DTS",
            "5.1",
            "-PHM",
            "ENG SUB",
            "ENG-SUB",
            "ENG.SUB",
            "ENG-DUB",
            "ENG.DUB",
            "ENGLISH-DUB",
            "ENG",
            "JAP SUB",
            "JAP-SUB",
            "JAP.DUB",
            "JAP",
            "DUB",
            "Hi-DEF",
            "2.0",
            "DDP-5.1",
            "DDP",
            "AMZN",
            "EVO",
            "DD5.1",
            "HD",
        };

        public string CleanName(string fileNameNoExt)
        {
            foreach(var junk in junkStrings)
            {
                if(fileNameNoExt.IndexOf(junk,StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fileNameNoExt = fileNameNoExt.Replace(junk, "");
                }
            }
            fileNameNoExt = fileNameNoExt.Trim();
            fileNameNoExt = fileNameNoExt.Replace(".", "");
            if(fileNameNoExt.EndsWith("-") || fileNameNoExt.EndsWith("_"))
            {
                fileNameNoExt = fileNameNoExt.Remove(fileNameNoExt.Length - 1);
            }
            fileNameNoExt = fileNameNoExt.Trim();

            return fileNameNoExt;
        }


    }
}
