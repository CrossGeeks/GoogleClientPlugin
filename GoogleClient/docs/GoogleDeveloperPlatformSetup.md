# Google Developer Platform Setup Android & iOS
## Android Setup
1. First of all, we will have to create an application on the google developer platform. You can access it [here.](https://console.developers.google.com/cloud-resource-manager)

![Creating Application 1](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/GoogleClient/images/GooglePlatformCreateApp.PNG?raw=true)

## Configuration File
After creating you're project in the Google Developer Platform, we will need a configuration file, you can generate the file from [here.](https://developers.google.com/mobile/add?platform=android&cntapi=signin&cnturl=https:%2F%2Fdevelopers.google.com%2Fidentity%2Fsign-in%2Fandroid%2Fsign-in%3Fconfigured%3Dtrue&cntlbl=Continue%20Adding%20Sign-In)

2. Select your the application name you created on the Google Developer Platform, and insert the package name of your application.

![Creating Application 2](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/GoogleClient/images/ConfigurationFileAndroidStep1.PNG?raw=true)

3. After selecting filling out that information, you will need to enable Google Sign-In, and to do so we will need to provide Google with a SHA-1 certificate.

![Creating Application 3](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/GoogleClient/images/ConfigurationFileAndroidStep2.PNG?raw=true)

For our example applications we will use a debug key, but to publish your app to the store, you will need a different SHA-1 key. You can read more about it [here](https://docs.microsoft.com/en-us/xamarin/android/platform/maps-and-location/maps/obtaining-a-google-maps-api-key?tabs=vswin#Obtaining_your_Signing_Key_Fingerprint). 

For the SHA-1 debug key you will need to modify the command bellow, open a CMD or Terminal and paste the bash:

```bash
Mac: keytool -list -v -keystore /Users/[USERNAME]/.local/share/Xamarin/Mono\ for\ Android/debug.keystore -alias androiddebugkey -storepass android -keypass android

Windows: keytool -list -v -keystore "C:\Users\[USERNAME]\AppData\Local\Xamarin\Mono for Android\debug.keystore" -alias androiddebugkey -storepass android -keypass android

```

4. Finally, you will be able to download the **google-services.json** file and add it to your **Xamarin.Android** project.

![Creating Application 4](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/GoogleClient/images/GoogleConfigurationFileAndroidStep4.PNG?raw=true)


**PDT:** If you replace the google-servies.json file on the sample project and change the Xamarin.Android package name of the project to yours, the android sample project should work.


## iOS Setup
We will also need to create a configuration file for iOS, which you can get [here.](https://developers.google.com/mobile/add?platform=ios&cntapi=signin&cntapp=Default%20Demo%20App&cntpkg=com.google.samples.quickstart.SignInExample&cnturl=https:%2F%2Fdevelopers.google.com%2Fidentity%2Fsign-in%2Fios%2Fstart%3Fconfigured%3Dtrue&cntlbl=Continue%20with%20Try%20Sign-In), the steps are very similar to the one we just did for android, you will select the Application name we initially created and your application bundle id. This time there will be no need to add SHA-1 certificate, so go ahead and enable the Google Sign-In option.


2. Download the **GoogleService-info.plist** file and add it to your **Xamarin.iOS** project.

![Creating Application 5](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/GoogleClient/images/ConfigurationFileiOSStep2.PNG?raw=true)

3. Next we will have to add the URL Scheme to our project. So Open the **GoogleService-info.plist** file and copy the **RESERVED_CLIENT_ID** value.


4. Then open the Xamarin.iOS **info.plist** file, go to the Advanced tab, create a new URL with the editor role and paste it in the URL Scheme Field.



**Note:** To be able to run the iOS project on the simulator, you will have to enable Keychain sharin on the **Entitlements.plist**:



Key chain sharing on iOS, need entitlements and for that we need a valid provision profile, find out more about it [here.](https://developer.apple.com/library/content/documentation/IDEs/Conceptual/AppDistributionGuide/MaintainingProfiles/MaintainingProfiles.html)


After you have completed the integration of the configurations file to your project you're ready for the final steps in the [Android Setup](AndroidSetup.md) and [iOS Setup](iOSSetup.md) guides.

<= Back to [Table of Contents](../README.md)