# Google Client Plugin for Xamarin iOS and Android

[![Build Status](https://dev.azure.com/CrossGeeks/Plugins/_apis/build/status/GoogleClient/GoogleClient%20Plugin%20CI%20Pipeline?branchName=master)](https://dev.azure.com/CrossGeeks/Plugins/_build/latest?definitionId=3&branchName=master)

Cross platform plugin for handling Google authentication.

<p align="center">
<img src="images/googleclient.gif" height="400" width="240" title="Google Client"/>
</p>

### Setup
* Available on Nuget: https://www.nuget.org/packages/Plugin.GoogleClient
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Plugin.GoogleClient.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.GoogleClient)
* Install into your .NETStandard project and Client projects.
* Create your [Google Firebase Console Platform](docs/GoogleFirebaseConsoleSetup.md) application.
* Follow the [Android](docs/AndroidSetup.md) and [iOS](docs/iOSSetup.md) guides
* Check out [Getting Started](docs/GettingStarted.md)

**Platform Support**

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|iOS 8+|
|Xamarin.Android|API 15+|

### API Usage

Call **CrossGoogleClient.Current** from any project to gain access to API.

## Features

- Authentication

## Documentation

Here you will find detailed documentation on setting up and using the Google Client Plugin for Xamarin

* [Google Firebase Console Setup](docs/GoogleFirebaseConsoleSetup.md) 
* [Android Setup](docs/AndroidSetup.md)
* [iOS Setup](docs/iOSSetup.md)

### Google Client Sample Application
* [Google Client Sample App](samples)

### References
* [Documentation References](docs/References.md)
* Google Sign In SDK Bindings by [Xamarin](https://github.com/xamarin):

    - [Xamarin.GooglePlayServices.Auth](https://www.nuget.org/packages/Xamarin.GooglePlayServices.Auth/)
    
    - [Xamarin.Google.iOS.SignIn](https://www.nuget.org/packages/Xamarin.Google.iOS.SignIn/)

### Contributors

* [Luis Pujols](https://github.com/pujolsluis)
* [Rendy Del Rosario](https://github.com/rdelrosario)
* [RomaRudyak](https://github.com/RomaRudyak)
* [Thiago Carvalho](https://github.com/stealthcold)
* [Nathalia](https://github.com/natsoragge)
* [NGumby](https://github.com/NGumby)
