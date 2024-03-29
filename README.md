[![Node Version](https://img.shields.io/badge/dynamic/json?label=Node%20Version&query=%24.engines.node&url=https%3A%2F%2Fraw.githubusercontent.com%2Fucdavis%2FAnlab%2Fmaster%2FAnlab.Mvc%2FClientApp%2Fpackage.json)](https://img.shields.io/badge/dynamic/json?label=Node%20Version&query=%24.engines.node&url=https%3A%2F%2Fraw.githubusercontent.com%2Fucdavis%2FAnlab%2Fmaster%2FAnlab.Mvc%2FClientApp%2Fpackage.json)
[![Build Status](https://dev.azure.com/ucdavis/Anlab/_apis/build/status/ucdavis.Anlab?branchName=master)](https://dev.azure.com/ucdavis/Anlab/_build/latest?definitionId=19&branchName=master)

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

`npm install --legacy-peer-deps`

`dotnet restore`

// when you are ready to debug locally

`npm run debug`

### [If you have visual studio]
// Just open it up and run the project!
