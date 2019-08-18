# TV Show Sorter

This is a c# program that will create a folder structure based off of the name of the show, then the season number.

## Installation

- Requires Windows Media Player to be installed. (uses WMPLib.dll)
- Download the .exe as well as the config file from the release folder below. 
- https://github.com/CrazyOldWizard/TV-Show-Sorter/tree/master/TV%20Show%20Sorter/bin/Release
- Change the SearchFolder key in the config to the root folder with the shows you would like to sort.
- If you want to have a different output directory, then enable that and set a folder path.
- Run the .exe

## Usage
This tool would sort the following episodes in the root directory...
~~~
└───TV_Shows
        Breaking Bad s01e01.mkv
        Breaking Bad s01e02.mkv
        Breaking Bad s01e03.mkv
        Breaking Bad s02e01.mkv
        Breaking Bad s02e02.mkv
        Breaking Bad s02e03.mkv
~~~


and create a folder structure like the example below.
~~~
└───TV_Shows
    └───Breaking Bad
        ├───Season 01
        │       Breaking Bad s01e01.mkv
        │       Breaking Bad s01e02.mkv
        │       Breaking Bad s01e03.mkv
        │
        └───Season 02
                Breaking Bad s02e01.mkv
                Breaking Bad s02e02.mkv
                Breaking Bad s02e03.mkv
~~~


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
