[![Build status](https://ci.appveyor.com/api/projects/status/sfxwgf2l87l1g8j7?svg=true)](https://ci.appveyor.com/project/UCNETAdmin/anlab)

[![forthebadge](http://forthebadge.com/images/badges/uses-html.svg)](http://forthebadge.com)
# Anlab

Order and results management system for http://anlab.ucdavis.edu/

# Build + Run

Have a recent version of NodeJS installed (https://nodejs.org/)

Have latest version of .Net Core installed, current v1.1 (https://www.microsoft.com/net/core)

Get the app settings from Box and put them in your secrets location (on MacOSx, `~/.microsoft/usersecrets/f3c999a5-2304-44e0-a49e-f43333bf9ccf`)

### [If you don't have visual studio]
// Go to the Anlab.Mvc website directory 

// first time, or when modules change

`npm install`

`dotnet restore`

// when you are ready to debug locally

`npm run debug`

### [If you have visual studio]
// Just open it up and run the project!
