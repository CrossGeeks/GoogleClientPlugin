using System;
namespace Plugin.GoogleClient.Shared
{
	
    [Serializable()]
    public class GoogleClientBaseException : Exception
    {
        public static string SignInDefaultErrorMessage = "The Google Sign In could not complete it's process correctly.";
        public static string SignInUnknownErrorMessage = "An unknown error has occured.";
        public static string SignInKeychainErrorMessage = "There was a problem reading or writing to the application keychain.";
        public static string SignInNoSignInHandlersInstalledErrorMessage = "No appropriate applications are installed on the user's device which can handle sign-in.";
        public static string SignInHasNoAuthInKeychainErrorMessage = "There are no auth tokens in the keychain.";
        public static string SignInCanceledErrorMessage = "The Sign In request was cancelled by the user";

        public GoogleClientBaseException() : base() { }
        public GoogleClientBaseException(string message) : base(message) { }
        public GoogleClientBaseException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected GoogleClientBaseException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }

    // Indicates an unknown error has occured.
    [Serializable()]
    public class GoogleClientSignInUnknownErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInUnknownErrorException() : base() { }
        public GoogleClientSignInUnknownErrorException(string message) : base(SignInUnknownErrorMessage) { }
        public GoogleClientSignInUnknownErrorException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected GoogleClientSignInUnknownErrorException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }

    // Indicates a problem reading or writing to the application keychain.
    [Serializable()]
    public class GoogleClientSignInKeychainErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInKeychainErrorException() : base() { }
        public GoogleClientSignInKeychainErrorException(string message) : base(SignInKeychainErrorMessage) { }
        public GoogleClientSignInKeychainErrorException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected GoogleClientSignInKeychainErrorException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }

    // Indicates no appropriate applications are installed on the user's device which can handle
    // sign-in. This code will only ever be returned if using webview and switching to browser have
    // both been disabled.
    [Serializable()]
    public class GoogleClientSignInNoSignInHandlersInstalledErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInNoSignInHandlersInstalledErrorException() : base() { }
        public GoogleClientSignInNoSignInHandlersInstalledErrorException(string message) : base(SignInNoSignInHandlersInstalledErrorMessage) { }
        public GoogleClientSignInNoSignInHandlersInstalledErrorException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected GoogleClientSignInNoSignInHandlersInstalledErrorException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }

    // Indicates there are no auth tokens in the keychain. This error code will be returned by
    // signInSilently if the user has never signed in before with the given scopes, or if they have
    // since signed out.
    [Serializable()]
    public class GoogleClientSignInHasNoAuthInKeychainErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInHasNoAuthInKeychainErrorException() : base() { }
        public GoogleClientSignInHasNoAuthInKeychainErrorException(string message) : base(SignInHasNoAuthInKeychainErrorMessage) { }
        public GoogleClientSignInHasNoAuthInKeychainErrorException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected GoogleClientSignInHasNoAuthInKeychainErrorException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }

    // Indicates the user canceled the sign in request.
    [Serializable()]
    public class GoogleClientSignInCanceledErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInCanceledErrorException() : base() { }
        public GoogleClientSignInCanceledErrorException(string message) : base(SignInCanceledErrorMessage) { }
        public GoogleClientSignInCanceledErrorException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected GoogleClientSignInCanceledErrorException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }


}
