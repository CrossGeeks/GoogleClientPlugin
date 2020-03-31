# Google Firebase Console Setup Android & iOS
## Android Setup
**1.** First of all, we will have to create an application on the Google Firebase Console. You can access it [here.](https://console.firebase.google.com/)

![Creating Application 1](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/FirebaseConsoleCreateApp.PNG?raw=true)

## Enable Google Authentication
After creating your project, go to the Authentication section in the left hamburger menu of the Firebase Console, and press Select Sign In Method.

![Enable Google Authentication](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/FirebaseAuthSignInMethod.PNG?raw=true)


In the Sign-in provides options, select and enable Google.

![Enable Google Authentication 2](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/FirebaseConsoleEnableAuthGoogle.PNG?raw=true)

## Configuration File
After creating your project and enabling Google Sign In on the Google Firebase Console, we will need a configuration file for our application, you can generate the file from your project settings section (press the settings knob).

![Creating Application 2](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/FirebaseConsoleSettings.PNG?raw=true)

**2.** Select the platform you wish to generate the configuration file for, in this case we will select **Android**.

![Creating Application 2](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/FirebaseConsoleAddToPlatform.PNG?raw=true)

**3.** After selecting Android, we will need to fill out the form with the information of our App, to enable Google Sign-In, and to do so we will need to provide Google with the package name, the name of the App and a SHA-1 certificate.

![Creating Application 3](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/FirebaseConsoleAddToAndroid.PNG?raw=true)

For our example application we will use a debug key, but to publish your app to the store, you will need a different SHA-1 key. You can read more about it [here](https://docs.microsoft.com/en-us/xamarin/android/platform/maps-and-location/maps/obtaining-a-google-maps-api-key?tabs=vswin#Obtaining_your_Signing_Key_Fingerprint). 

For the SHA-1 debug key you will need to modify the command below, open a CMD or Terminal and paste the bash:

**macOS**
```bash
keytool -list -v -keystore /Users/[USERNAME]/.local/share/Xamarin/Mono\ for\ Android/debug.keystore -alias androiddebugkey -storepass android -keypass android
```

**Windows**

```bash
keytool -list -v -keystore "C:\Users\[USERNAME]\AppData\Local\Xamarin\Mono for Android\debug.keystore" -alias androiddebugkey -storepass android -keypass android

```
Other alternative is to adding this method and calling it on the MainActivity OnCreate method to get the SHA 1 printed on the application output:

```cs
  public static void PrintHashKey (Context pContext)
  {
            try {
                PackageInfo info = Android.App.Application.Context.PackageManager.GetPackageInfo (Android.App.Application.Context.PackageName, PackageInfoFlags.Signatures);
                foreach (var signature in info.Signatures) {
                    MessageDigest md = MessageDigest.GetInstance ("SHA");
                    md.Update (signature.ToByteArray ());

                    System.Diagnostics.Debug.WriteLine (BitConverter.ToString(md.Digest ()).Replace ("-", ":"));
                }
            } catch (NoSuchAlgorithmException e) {
                System.Diagnostics.Debug.WriteLine (e);
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine (e);
            }
   }
```

**4.** Finally, you will be able to download the **google-services.json** file and add it to your **Xamarin.Android** project. Make sure build action is GoogleServicesJson

![Creating Application 4](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/ConfigurationFileAndroid.PNG?raw=true)


>**Note:**  
>If you replace the google-servies.json file on the sample project and change the Xamarin.Android package name of the project to yours, the android sample project should work.


## iOS Setup
We will also need to create a configuration file for iOS, which you can get in the settings section of you're project in Firebase Console, the steps are very similar to the one we just did for android, you will select the Application name we initially created and your **application bundle id**. This time there will be no need to add SHA-1 certificate, so go ahead and enable the Google Sign-In option.

You can find your application bundle id by opening the **Info.plist** file in your iOS Project.
![iOS Application bundle id](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/iOSInfoplistBundleID.PNG?raw=true)

Fill out the form with the required information and register the App.

![iOS Application integration](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/FirebaseConsoleAddToiOS.PNG?raw=true)


2. Download the **GoogleService-Info.plist** file and add it to your **Xamarin.iOS** project, **Important:** you must set the Build Action of this file as a BundleResource so the Plugin can find it as part of the app bundle at runtime.

![Creating Application 5](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/ConfigurationFileiOS.PNG?raw=true)

3. Next we will have to add the URL Scheme to our project. So Open the **GoogleService-Info.plist** file and copy the `REVERSED_CLIENT_ID` value.
![Creating Application 6](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/iOSReversedClientID.PNG?raw=true)

4. Then open the Xamarin.iOS **Info.plist** file, go to the Advanced tab, create a new URL with the editor role and paste it in the URL Scheme Field.
![Creating Application 7](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/iOSInfoplist.PNG?raw=true)


>**Note:**  
>To be able to run the iOS project on the simulator, you will have to enable Keychain sharing on the **Entitlements.plist**:

![Creating Application 8](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/images/Entitlementsplist.PNG?raw=true)

Key chain sharing on iOS, need entitlements and for that we need a valid provision profile, find out more about it [here.](https://developer.apple.com/library/content/documentation/IDEs/Conceptual/AppDistributionGuide/MaintainingProfiles/MaintainingProfiles.html)


After you have completed the integration of the configurations file to your project you're ready for the final steps in the [Android Setup](AndroidSetup.md) and [iOS Setup](iOSSetup.md) guides. 

----------
<= Back to [Table of Contents](../../README.md)
