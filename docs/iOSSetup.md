# iOS Setup

* Install Plugin.GoogleClient package into your iOS project.

## Prerequisites
1. Complete the [Google Firebase Console Setup](GoogleFirebaseConsoleSetup.md) to include the required configuration file to you're projects.

## AppDelegate.cs
- On the FinishedLaunching method just after calling global::Xamarin.Forms.Forms.Init():
```cs
     GoogleClientManager.Initialize();
```

## Override OpenUrl method
Override the OpenUrl method from AppDelegate class:
```cs
public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
{
    return GoogleClientManager.OnOpenUrl(app, url, options);
}
```


<= Back to [Table of Contents](../README.md)
