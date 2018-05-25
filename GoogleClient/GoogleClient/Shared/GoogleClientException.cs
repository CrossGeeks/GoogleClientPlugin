using System;
namespace Plugin.GoogleClient.Shared
{
	public class GoogleClientBaseException : Exception
    {
        public static string SignInDefaultErrorMessage = "The Google Sign In could not complete it's process correctly.";
        public static string SignInUnknownErrorMessage = "An unknown error has occured.";
        public static string SignInKeychainErrorMessage = "There was a problem reading or writing to the application keychain.";
        public static string SignInNoSignInHandlersInstalledErrorMessage = "No appropriate applications are installed on the user's device which can handle sign-in.";
        public static string SignInHasNoAuthInKeychainErrorMessage = "There are no auth tokens in the keychain.";
        public static string SignInCanceledErrorMessage = "The Sign In request was cancelled by the user";
        public static string SignInApiNotConnectedErrorMessage = "API failed to connect.";
        public static string SignInInvalidAccountErrorMessage = "Attempted to connect to the service with an invalid account name specified.";
        public static string SignInNetworkErrorMessage = "A network error occurred. Retrying should resolve the problem.";
        public static string SignInInternalErrorMessage = "An internal error occurred. Retrying should resolve the problem.";


        public GoogleClientBaseException() : base() { }
        public GoogleClientBaseException(string message) : base(message) { }
        public GoogleClientBaseException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates an unknown error has occured.
    public class GoogleClientSignInUnknownErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInUnknownErrorException() : base(SignInUnknownErrorMessage) { }
        public GoogleClientSignInUnknownErrorException(string message) : base(message) { }
        public GoogleClientSignInUnknownErrorException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates a problem reading or writing to the application keychain.
    public class GoogleClientSignInKeychainErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInKeychainErrorException() : base(SignInKeychainErrorMessage) { }
        public GoogleClientSignInKeychainErrorException(string message) : base(message) { }
        public GoogleClientSignInKeychainErrorException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates no appropriate applications are installed on the user's device which can handle
    // sign-in. This code will only ever be returned if using webview and switching to browser have
    // both been disabled.
    public class GoogleClientSignInNoSignInHandlersInstalledErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInNoSignInHandlersInstalledErrorException() : base(SignInNoSignInHandlersInstalledErrorMessage) { }
        public GoogleClientSignInNoSignInHandlersInstalledErrorException(string message) : base(message) { }
        public GoogleClientSignInNoSignInHandlersInstalledErrorException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates there are no auth tokens in the keychain. This error code will be returned by
    // signInSilently if the user has never signed in before with the given scopes, or if they have
    // since signed out.
    public class GoogleClientSignInHasNoAuthInKeychainErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInHasNoAuthInKeychainErrorException() : base(SignInHasNoAuthInKeychainErrorMessage) { }
        public GoogleClientSignInHasNoAuthInKeychainErrorException(string message) : base(message) { }
        public GoogleClientSignInHasNoAuthInKeychainErrorException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates the user canceled the sign in request.
    public class GoogleClientSignInCanceledErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInCanceledErrorException() : base(SignInCanceledErrorMessage) { }
        public GoogleClientSignInCanceledErrorException(string message) : base(message) { }
        public GoogleClientSignInCanceledErrorException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates the user canceled the sign in request.
    public class GoogleClientSignInApiNotConnectedErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInApiNotConnectedErrorException() : base(SignInApiNotConnectedErrorMessage) { }
        public GoogleClientSignInApiNotConnectedErrorException(string message) : base(message) { }
        public GoogleClientSignInApiNotConnectedErrorException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates the user canceled the sign in request.
    public class GoogleClientSignInInvalidAccountErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInInvalidAccountErrorException() : base(SignInInvalidAccountErrorMessage) { }
        public GoogleClientSignInInvalidAccountErrorException(string message) : base(message) { }
        public GoogleClientSignInInvalidAccountErrorException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates the user canceled the sign in request.
    public class GoogleClientSignInNetworkErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInNetworkErrorException() : base(SignInNetworkErrorMessage) { }
        public GoogleClientSignInNetworkErrorException(string message) : base(message) { }
        public GoogleClientSignInNetworkErrorException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates the user canceled the sign in request.
    public class GoogleClientSignInInternalErrorException : GoogleClientBaseException
    {
        public GoogleClientSignInInternalErrorException() : base(SignInInternalErrorMessage) { }
        public GoogleClientSignInInternalErrorException(string message) : base(message) { }
        public GoogleClientSignInInternalErrorException(string message, System.Exception inner) : base(message, inner) { }
    }
}
