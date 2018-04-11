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

### Logout

Here is an example on how to logout of the Google Client:

```cs
    CrossGoogleClient.Current.Logout();
```

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

<= Back to [Table of Contents](../README.md)