# iOS Setup

* Install Plugin.GoogleClient package into your Android project.

## Prerequisites
1. Complete the [Google Developer Platform Setup](GoogleDeveloperPlatformSetup.md) to include the required configuration file to you're projects.

## AppDelegate.cs
- On the FinishedLaunching method just after calling global::Xamarin.Forms.Forms.Init():
```cs
     var googleServiceDictionary = NSDictionary.FromFile("GoogleService-Info.plist");
     SignIn.SharedInstance.ClientID = googleServiceDictionary["CLIENT_ID"].ToString();
```

## Override OpenUrl method
Override the OpenUrl method from AppDelegate class:
```cs
public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
{
    var openUrlOptions = newUIApplicationOpenUrlOptions(options);
    return SignIn.SharedInstance.HandleUrl(url, openUrlOptions.SourceApplication, openUrlOptions.Annotation);
}
```


<= Back to [Table of Contents](../../README.md)
