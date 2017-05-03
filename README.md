# Anlab

Order and results management system for http://anlab.ucdavis.edu/

# Build + Run

Have a recent version of NodeJS installed (https://nodejs.org/)

Get the app settings from Box and put them in your secrets location (on MacOSx, `~.microsoft/usersecrets/f3c999a5-2304-44e0-a49e-f43333bf9ccf`)

### [If you don't have visual studio]
// Go to the Anlab.Mvc website directory 

`export ASPNETCORE_ENVIRONMENT=Development`

`npm install`

`dotnet restore`

`dotnet run`

### [If you have visual studio]
// Just open it up and run the project!
