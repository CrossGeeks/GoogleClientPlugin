# Android Setup

* Install Plugin.GoogleClient package into your Android project.

## Prerequisites
- Compatible Android device that runs Android 4.0+ and includes the Google Play Store or an emulator with an AVD that runs the Google APIs platform based on Android 4.2+ and has Google Play services 11.6+
- The latest version of the Android SDK, including the SDK Tools component. You can get the latest SDK from the Android SDK Manager in Visual Studio.
- Project configured to compile against Android 4.0(Ice Cream Sandwich) or newer.
- The Google Play services SDK:
1. **Visual Studio 2017 Tools > Android > Android SDK Manager.**
2. Scroll to the bottom of the package list and select **Extras > Google Repository.** The page is downloaded to your computer and installed in your SDK environment where the android SDK on you're Visual Studio is Installed.

3. Complete the [Google Developer Platform Setup](GoogleDeveloperPlatformSetup.md) to include the required configuration file to you're projects.

## MainActivity.cs

- On the OnCreate method just after calling base.OnCreate:
```cs
     GoogleClientManager.Initialize(this);
```

## AndroidManifest.xml

Add this permission.
```xml
    <uses-permission android:name="android.permission.INTERNET"/>
```


<= Back to [Table of Contents](../README.md)