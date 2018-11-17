using System;
using System.Threading.Tasks;
using System.Linq;
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
        public static GoogleApiClient GoogleApiClient { get; private set; }
        public static Activity CurrentActivity { get; set; }
        static TaskCompletionSource<GoogleResponse<GoogleUser>> _loginTcs;
        private static string _serverClientId;
        private static string _clientId;
        private static string[] _initScopes = new string[0];
        private static Api[] _initApis = new Api[0];


        private static readonly string[] DefaultScopes = new []
        {
            Scopes.Profile
        };


        internal GoogleClientManager()
        {
            if(CurrentActivity == null)
            {
                GoogleClientErrorEventArgs errorEventArgs = new GoogleClientErrorEventArgs();
                Exception exception = null;

                errorEventArgs.Error = GoogleClientErrorType.SignInInternalError;
                errorEventArgs.Message = GoogleClientBaseException.SignInInternalErrorMessage;
                exception = new GoogleClientSignInInternalErrorException();

                _loginTcs.TrySetException(exception);
            }

            var gopBuilder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestEmail();
            
			if(!string.IsNullOrWhiteSpace(_serverClientId))
            {
				gopBuilder.RequestServerAuthCode(_serverClientId, false);
            }

			if(!string.IsNullOrWhiteSpace(_clientId))
            {
                gopBuilder.RequestIdToken(_clientId);
            }
            
            foreach (var s in _initScopes)
            {
                gopBuilder.RequestScopes(new Scope(s));
            }

            GoogleSignInOptions googleSignInOptions = gopBuilder.Build();

            var googleApiClientBuilder = new GoogleApiClient.Builder(Application.Context)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, googleSignInOptions);

            foreach(var a in _initApis)
            {
                googleApiClientBuilder.AddApi(a);
            }
            
            GoogleApiClient = googleApiClientBuilder.Build();
        }

        public static void Initialize(
            Activity activity,
            string serverClientId = null,
            string clientId = null,
            Api[] apis = null,
            string[] scopes = null)
        {
            CurrentActivity = activity;
            _serverClientId = serverClientId;
            _clientId = clientId;
            _initApis = apis ?? new Api[0];
            _initScopes = DefaultScopes.Concat(scopes ?? new string[0]).ToArray();
        }

        static EventHandler<GoogleClientResultEventArgs<GoogleUser>> _onLogin;
        public event EventHandler<GoogleClientResultEventArgs<GoogleUser>> OnLogin
        {
            add => _onLogin += value;
            remove => _onLogin -= value;
        }

        public async Task<GoogleResponse<GoogleUser>> LoginAsync()
        {
            Intent intent = Auth.GoogleSignInApi.GetSignInIntent(GoogleApiClient);
            CurrentActivity?.StartActivityForResult(intent, AuthActivityID);

            ConnectClientIfNeeded();
            
            return await CreateLoginTask();
        }

        public async Task<GoogleResponse<GoogleUser>> SilentLoginAsync()
        {
            ConnectClientIfNeeded();
            Auth.GoogleSignInApi
                .SilentSignIn(GoogleClientManager.GoogleApiClient)
                .AsAsync<GoogleSignInResult>()
                .ContinueWith(t => ProcessGoogleSignInResult(t.Result));
            
            return await CreateLoginTask();
        }

        static EventHandler _onLogout;
        public event EventHandler OnLogout
        {
            add => _onLogout += value;
            remove => _onLogout -= value;
        }

        protected virtual void OnLogoutCompleted(EventArgs e) => _onLogout?.Invoke(this, e);

        public void Logout()
        {
            Auth.GoogleSignInApi.SignOut(GoogleApiClient);
            _activeToken = null;
            GoogleApiClient.Disconnect();

            // Log the state of the client
            Debug.WriteLine(Tag + ": The user has logged out succesfully? " + !GoogleApiClient.IsConnected);

            // Send the logout result to the receivers
            OnLogoutCompleted(EventArgs.Empty);
        }

        public bool IsLoggedIn { get; }

        public string ActiveToken { get { return _activeToken; } }
        static string _activeToken { get; set; }

        static EventHandler<GoogleClientErrorEventArgs> _onError;
        public event EventHandler<GoogleClientErrorEventArgs> OnError
        {
            add => _onError += value;
            remove => _onError -= value;
        }

        protected virtual void OnGoogleClientError(GoogleClientErrorEventArgs e) => _onError?.Invoke(this, e);

        public static void OnAuthCompleted(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode != AuthActivityID)
            {
                return;
            }
            
            GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);

           ProcessGoogleSignInResult(result);
        }

        public void OnConnected(Bundle connectionHint) => Debug.WriteLine(Tag + ": Connection to the client successfull");

        public void OnConnectionSuspended(int cause) => Debug.WriteLine(Tag + ": Connection to the client was suspended");

        public void OnConnectionFailed(ConnectionResult result) => Debug.WriteLine(Tag + ": Connection to the client failed with error <" + result.ErrorCode + "> " + result.ErrorMessage);

        private static void OnSignInSuccessful(GoogleSignInResult result)
        {
            GoogleSignInAccount userAccount = result.SignInAccount;
            GoogleUser googleUser = new GoogleUser
            {
                Id = userAccount.Id,
                Name = userAccount.DisplayName,
                Email = userAccount.Email,
                Picture = new Uri((userAccount.PhotoUrl != null ? $"{userAccount.PhotoUrl}" : $"https://autisticdating.net/imgs/profile-placeholder.jpg"))
            };

            _activeToken = result.SignInAccount.IdToken;

            System.Console.WriteLine($"Active Token: {_activeToken}");

            var googleArgs =
                new GoogleClientResultEventArgs<GoogleUser>(googleUser, GoogleActionStatus.Completed, result.Status.StatusMessage);

            // Send the result to the receivers
            _onLogin?.Invoke(CrossGoogleClient.Current, googleArgs);
            _loginTcs.TrySetResult(new GoogleResponse<GoogleUser>(googleArgs));
        }

        private static void OnSignInFailed(GoogleSignInResult result)
        {
            GoogleClientErrorEventArgs errorEventArgs = new GoogleClientErrorEventArgs();
            Exception exception = null;

            switch (result.Status.StatusCode)
            {
                case GoogleSignInStatusCodes.InternalError:
                    errorEventArgs.Error = GoogleClientErrorType.SignInInternalError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInInternalErrorMessage;
                    exception = new GoogleClientSignInInternalErrorException();
                    break;
                case GoogleSignInStatusCodes.ApiNotConnected:
                    errorEventArgs.Error = GoogleClientErrorType.SignInApiNotConnectedError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInApiNotConnectedErrorMessage;
                    exception = new GoogleClientSignInApiNotConnectedErrorException();
                    break;
                case GoogleSignInStatusCodes.NetworkError:
                    errorEventArgs.Error = GoogleClientErrorType.SignInNetworkError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInNetworkErrorMessage;
                    exception = new GoogleClientSignInNetworkErrorException();
                    break;
                case GoogleSignInStatusCodes.InvalidAccount:
                    errorEventArgs.Error = GoogleClientErrorType.SignInInvalidAccountError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInInvalidAccountErrorMessage;
                    exception = new GoogleClientSignInInvalidAccountErrorException();
                    break;
                case GoogleSignInStatusCodes.SignInRequired:
                    errorEventArgs.Error = GoogleClientErrorType.SignInRequiredError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInRequiredErrorMessage;
                    exception = new GoogleClientSignInRequiredErrorErrorException();
                    break;
                case GoogleSignInStatusCodes.SignInFailed:
                    errorEventArgs.Error = GoogleClientErrorType.SignInFailedError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInFailedErrorMessage;
                    exception = new GoogleClientSignInFailedErrorException();
                    break;
                case GoogleSignInStatusCodes.SignInCancelled:
                    errorEventArgs.Error = GoogleClientErrorType.SignInCanceledError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInCanceledErrorMessage;
                    exception = new GoogleClientSignInCanceledErrorException();
                    break;
                default:
                    errorEventArgs.Error = GoogleClientErrorType.SignInDefaultError;
                    errorEventArgs.Message = result.Status.StatusMessage;
                    exception = new GoogleClientBaseException(
                        string.IsNullOrEmpty(result.Status.StatusMessage) 
                            ? GoogleClientBaseException.SignInDefaultErrorMessage 
                            : result.Status.StatusMessage
                        );
                    break;
            }

            _loginTcs.TrySetException(exception);
            _onError?.Invoke(CrossGoogleClient.Current, errorEventArgs);
        }

        private void ConnectClientIfNeeded()
        {
            if(GoogleApiClient.IsConnected)
            {
                return;
            }

            GoogleApiClient.Connect(GoogleApiClient.SignInModeOptional);
        }

        private static Task<GoogleResponse<GoogleUser>> CreateLoginTask()
        {
            _loginTcs = new TaskCompletionSource<GoogleResponse<GoogleUser>>();
            
            return _loginTcs.Task;
        }

        private static void ProcessGoogleSignInResult(GoogleSignInResult result)
        {
             // Log the result of the authentication
            Debug.WriteLine(Tag + ": Is the user authenticated? " + result.IsSuccess);

            if (result.IsSuccess)
            {
                OnSignInSuccessful(result);
                return;
            }
            
            OnSignInFailed(result);
        }
    }
}
