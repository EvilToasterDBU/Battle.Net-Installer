# BNet Installer

A command line tool for installing games via Blizzard's Battle.Net application. Windows only.

#### Prerequisites
- .Net Core 2.2

#### Arguments

| Argument | Description |
| ------- | :---- |
| --prod | TACT Product **(Required)** |
| --lang | Game/Asset language **(Required)** |
| --dir | Installation Directory **(Required)** |
| --uid | Agent UID (Required if different to the TACT product) |
| --help | Shows this table |

All products and Agent UIDs can be found [here](https://wowdev.wiki/TACT#Products) however only (green) Active products will work.  
Languages are listed [here](BNetInstaller/Constants/Locale.cs), availability will vary between products.

#### Usage

Example for StarCraft 2:  
TACT Product = `s2`, Agent UID = `s2(_locale)`

`dotnet bnetinstaller.dll --prod s2 --uid s2_enus --lang enus --dir "C:\Test"`