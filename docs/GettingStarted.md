### Getting Started

If developing an application that supports iOS and Android, make sure you installed the NuGet package into your NETStandard project and Client projects.

### Setup
* [Google Firebase Console Setup](GoogleFirebaseConsoleSetup.md)
* [Android Setup](AndroidSetup.md)
* [iOS Setup](iOSSetup.md)

### Login

Here is an example on how to launch the login to the Google Client:

```cs
    CrossGoogleClient.Current.LoginAsync();
```

This method returns a Google User which contains the basic profile of the user that was authenticated, with the following structure:

```cs
    public class GoogleUser
    {
	public string Id { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public Uri Picture { get; set; }
    }
```

### Logout

Here is an example on how to logout of the Google Client:

```cs
    CrossGoogleClient.Current.Logout();
```

### Available Properties
* **AccessToken** (Signed In user access token)
* **IdToken** (Signed In user id token)

### Events

All async methods also trigger events:

Login event:

```cs
  CrossGoogleClient.Current.OnLogin += (s,a)=> 
  {
     switch (a.Status)
      {
         case GoogleActionStatus.Completed:
          //Logged in succesfully
         break;
      }
  };
```


### Exceptions
Types of exceptions the user can handle from the Google Client plugin.
```cs
    // Indicates Google Client Plugin was not initialized correctly on the platform.
    GoogleClientNotInitializedErrorException	

    // Indicates Google Sign In could not complete it's process correctly.
    SignInDefaultErrorException

    // Indicates an unknown error has occured.
    SignInUnknownErrorException

    // Indicates a problem reading or writing to the application keychain.
    SignInKeychainErrorException

    /* Indicates no appropriate applications are installed on the user's device which can handle
    sign-in. This code will only ever be returned if using webview and switching to browser have
    both been disabled. */
    SignInNoSignInHandlersInstalledErrorException

    /* Indicates there are no auth tokens in the keychain. This error code will be returned by
    signInSilently if the user has never signed in before with the given scopes, or if they have
    since signed out. */
    SignInHasNoAuthInKeychainErrorException

    // Indicates the user canceled the sign in request.
    SignInCanceledErrorException

    // Indicates the client attempted to call a method from an API that failed to connect.
    SignInApiNotConnectedErrorException

    // Indicates The client attempted to connect to the service with an invalid account name specified.
    SignInInvalidAccountErrorException

    // Indicates a network error occurred. Retrying should resolve the problem.
    SignInNetworkErrorException

    // Indicates an internal error occurred.
    SignInInternalErrorException

    // Indicates the client attempted to connect to the service but the user is not signed in.
    SignInRequiredErrorException

    // Indicates the sign in attempt didn't succeed with the current account.
    SignInFailedErrorException
```

### Enabling Server Auth Code or RequestIdToken Programmatically (Optional)
If you have a more complex development flow, that integrates CI in which it's better to just set your Server Client ID and RequestIdToken programmatically for the Plugin you can do so, sending the  Web Client Id on each projects Initialize method respectively.

- Relevant Google Documentation about this topic:

[Authenticate with a backend server](https://developers.google.com/identity/sign-in/android/backend-auth)

[Get your backend server's OAuth 2.0 client ID](https://developers.google.com/identity/sign-in/android/start-integrating#get_your_backend_servers_oauth_20_client_id)

#### Android
```cs
    /* 1st Parameter (MainActivity), 2nd Parameter enables the ServerAuthCode 
    3rd Parameter enables the RequestIdToken. 
    (You pass the server web client id to both parameters).
    */
    GoogleClientManager.Initialize(this, "Xewa3121FDvbam", "Xewa3121FDvbam");
```
After applying this changes, you should now be able to see that your AccessToken no longer returns null when you access it on the plugin after a successful login.

#### iOS
```cs
    GoogleClientManager.Initialize("Xewa3121FDvbam");
```


### Silent Login
If you wish to silently login your users if they have already authenticated on their device without the need to interact with the UI from the Google Sign In SDK and simply set your UI on the Authenticated state.

You can now do this simply calling this line of code:
```cs
    CrossGoogleClient.Current.SilentLoginAsync();
```

### Enable Additional Scopes and API's
In case you wish to enable additional Scopes and API’s to access different types of information, you can now do so simply by passing in 2 new parameters containing arrays with the scopes and API’s you want to enable on the Google Plugin Initialize method.

Signature of the initialize methods:
```cs
//Android initialize method.
public static void Initialize(
            Activity activity,
            string serverClientId = null,
            string clientId = null,
            Api[] apis = null,
            string[] scopes = null)
```

```cs
//iOS initialize method.
public static void Initialize(
            string serverClientId = null,
            string[] scopes = null)
```

<= Back to [Table of Contents](../../README.md)
