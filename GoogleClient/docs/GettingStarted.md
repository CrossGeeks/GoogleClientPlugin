### Getting Started

If developing an application that supports iOS and Android, make sure you installed the NuGet package into your PCL project and Client projects.

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
        public string Name { get; set; }
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
* **ActiveToken** (Signed In user token)


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

### Set ClientId Programmatically (Optional)
If you have a more complex development flow, that integrates CI in which it's better to just set your ClientID programmatically for the Plugin you can do so, sending the ClientId on each projects Initialize method respectively.

#### Android
```cs
    GoogleClientManager.Initialize(this, "Xewa3121FDvbam");
```

#### iOS
```cs
    GoogleClientManager.Initialize("Xewa3121FDvbam");
```

<= Back to [Table of Contents](../../README.md)
