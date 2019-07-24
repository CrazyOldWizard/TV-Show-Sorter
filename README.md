# Sort TV Shows

This is a c# program that will create a folder structure based off of the name of the show, then the season number.

## Installation

Download the .exe as well as the config.
 Change the RootDir key in the config to the root folder with the shows you would like to sort.
 Run the .exe


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
