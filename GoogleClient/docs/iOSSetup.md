# iOS Setup

* Install Plugin.GoogleClient package into your Android project.

## Prerequisites
1. Complete the [Google Developer Platform Setup](GoogleDeveloperPlatformSetup.md) to include the required configuration file to you're projects.

## AppDelegate.cs
- On the FinishedLaunching method just after calling global::Xamarin.Forms.Forms.Init():
```cs
     CrossGoogleClient.Initialize();
```

## Override OpenUrl method
Override the OpenUrl method from AppDelegate class:
```cs
public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
{
    return CrossGoogleClient.OnOpenUrl(UIApplication app, NSUrl url, NSDictionary options);
}
```


<= Back to [Table of Contents](../../README.md)
