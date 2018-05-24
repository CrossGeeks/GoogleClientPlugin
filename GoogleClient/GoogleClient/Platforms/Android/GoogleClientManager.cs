using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Plugin.GoogleClient.Shared;
using Debug = System.Diagnostics.Debug;
using Object = Java.Lang.Object;

namespace Plugin.GoogleClient
{
    /// <summary>
    /// Implementation for GoogleClient
    /// </summary>
    public class GoogleClientManager : Object, IGoogleClientManager, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        // Class Debug Tag
        static string Tag = typeof(GoogleClientManager).FullName;
        static int AuthActivityID = Tag.GetHashCode() % Int16.MaxValue;
        public static GoogleApiClient GoogleApiClient { get; set; }
        public static Activity CurrentActivity { get; set; }
        static TaskCompletionSource<GoogleResponse<GoogleUser>> _loginTcs;


        internal GoogleClientManager()
        {
            GoogleSignInOptions googleSignInOptions =
                new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestEmail()
                    .Build();

            GoogleApiClient = new GoogleApiClient.Builder(Application.Context)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, googleSignInOptions)
                .AddScope(new Scope(Scopes.Profile))
                .Build();
        }

        public static void Initialize(Activity activity)
        {
            CurrentActivity = activity;
        }

        static EventHandler<GoogleClientResultEventArgs<GoogleUser>> _onLogin;
        public event EventHandler<GoogleClientResultEventArgs<GoogleUser>> OnLogin
        {
            add => _onLogin += value;
            remove => _onLogin -= value;
        }

        public async Task<GoogleResponse<GoogleUser>> LoginAsync()
        {
            _loginTcs = new TaskCompletionSource<GoogleResponse<GoogleUser>>();
            Intent intent = Auth.GoogleSignInApi.GetSignInIntent(GoogleApiClient);
            CurrentActivity?.StartActivityForResult(intent, AuthActivityID);
            GoogleApiClient.Connect();

            return await _loginTcs.Task;
        }

        static EventHandler _onLogout;
        public event EventHandler OnLogout
        {
            add => _onLogout += value;
            remove => _onLogout -= value;
        }

        protected virtual void OnLogoutCompleted(EventArgs e)
        {
            _onLogout?.Invoke(this, e);
        }

        public void Logout()
        {
            Auth.GoogleSignInApi.SignOut(GoogleApiClient);
            GoogleApiClient.Disconnect();

            // Log the state of the client
            Debug.WriteLine(Tag + ": The user has logged out succesfully? " + !GoogleApiClient.IsConnected);

            // Send the logout result to the receivers
            OnLogoutCompleted(EventArgs.Empty);
        }

        public bool IsLoggedIn { get; }


		static EventHandler<GoogleClientErrorEventArgs> _onError;
        public event EventHandler<GoogleClientErrorEventArgs> OnError
        {
            add => _onError += value;
            remove => _onError -= value;
        }



        protected virtual void OnGoogleClientError(GoogleClientErrorEventArgs e)
        {
            _onError?.Invoke(this, e);
        }

        public static void OnAuthCompleted(int requestCode, Result resultCode, Intent data)
        {

            if (requestCode == AuthActivityID)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);

                GoogleUser googleUser = null;

                // Log the result of the authentication
                Debug.WriteLine(Tag + ": Is the user authenticated? " + result.IsSuccess);

                if (result.IsSuccess)
                {
                    GoogleSignInAccount userAccount = result.SignInAccount;
                    googleUser = new GoogleUser
                    {
                        Name = userAccount.DisplayName,
                        Email = userAccount.Email,
                        Picture = new Uri((userAccount.PhotoUrl != null ? $"{userAccount.PhotoUrl}" : $"https://autisticdating.net/imgs/profile-placeholder.jpg"))
                    };

                    var googleArgs =
                        new GoogleClientResultEventArgs<GoogleUser>(googleUser, GoogleActionStatus.Completed, result.Status.StatusMessage);

                    // Send the result to the receivers
                    _onLogin?.Invoke(CrossGoogleClient.Current, googleArgs);
                    _loginTcs.TrySetResult(new GoogleResponse<GoogleUser>(googleArgs));
                }
                else
                {
					GoogleClientErrorEventArgs errorEventArgs = new GoogleClientErrorEventArgs();

                    switch (error.Code)
                    {
                        case -1:
                            errorEventArgs.Error = GoogleClientErrorType.SignInUnknownError;
                            errorEventArgs.Message = GoogleClientBaseException.SignInUnknownErrorMessage;
                            _loginTcs.TrySetException(new GoogleClientSignInUnknownErrorException());
                            break;
                        case -2: 
                            errorEventArgs.Error = GoogleClientErrorType.SignInKeychainError;
                            errorEventArgs.Message = GoogleClientBaseException.SignInKeychainErrorMessage;
                            _loginTcs.TrySetException(new GoogleClientSignInKeychainErrorException());
                            break;
                        case -3:
                            errorEventArgs.Error = GoogleClientErrorType.NoSignInHandlersInstalledError;
                            errorEventArgs.Message = GoogleClientBaseException.SignInNoSignInHandlersInstalledErrorMessage;
                            _loginTcs.TrySetException(new GoogleClientSignInNoSignInHandlersInstalledErrorException());
                            break;
                        case -4:
                            errorEventArgs.Error = GoogleClientErrorType.SignInHasNoAuthInKeychainError;
                            errorEventArgs.Message = GoogleClientBaseException.SignInUnknownErrorMessage;
                            _loginTcs.TrySetException(new GoogleClientSignInHasNoAuthInKeychainErrorException());
                            break;
                        case -5:
                            errorEventArgs.Error = GoogleClientErrorType.SignInCanceledError;
                            errorEventArgs.Message = GoogleClientBaseException.SignInCanceledErrorMessage;
                            _loginTcs.TrySetException(new GoogleClientSignInCanceledErrorException());
                            break;
                        default:
                            errorEventArgs.Error = GoogleClientErrorType.SignInDefaultError;
                            errorEventArgs.Message = GoogleClientBaseException.SignInDefaultErrorMessage;
                            _loginTcs.TrySetException(new GoogleClientBaseException());
                            break;
                    }

                    OnGoogleClientError(errorEventArgs);
                }

            }

        }

        public void OnConnected(Bundle connectionHint)
        {
            Debug.WriteLine(Tag + ": Connection to the client succesfull");
        }

        public void OnConnectionSuspended(int cause)
        {
            Debug.WriteLine(Tag + ": Connection to the client was suspended");
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Debug.WriteLine(Tag + ": Connection to the client failed with error <" + result.ErrorCode + "> " + result.ErrorMessage);
        }

    }
}
