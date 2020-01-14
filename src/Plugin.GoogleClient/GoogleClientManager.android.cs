using System;
using System.Threading.Tasks;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Plugin.GoogleClient.Shared;
using Android.Gms.Tasks;
using Java.Interop;
using Android.Gms.Auth;

namespace Plugin.GoogleClient
{
    /// <summary>
    /// Implementation for GoogleClient
    /// </summary>
    public class GoogleClientManager : Java.Lang.Object, IGoogleClientManager, IOnCompleteListener
    {
        // Class Debug Tag
        static string Tag = typeof(GoogleClientManager).FullName;
        static int AuthActivityID = 9637;
        public static Activity CurrentActivity { get; set; }
        static TaskCompletionSource<GoogleResponse<GoogleUser>> _loginTcs;
        static string _serverClientId;
        static string _clientId;
        static string[] _initScopes = new string[0];

       GoogleSignInClient mGoogleSignInClient;

        public GoogleUser CurrentUser
        {
            get
            {
                GoogleSignInAccount userAccount = GoogleSignIn.GetLastSignedInAccount(CurrentActivity);
                return userAccount !=null ?new GoogleUser
                {
                    Id = userAccount.Id,
                    Name = userAccount.DisplayName,
                    GivenName = userAccount.GivenName,
                    FamilyName = userAccount.FamilyName,
                    Email = userAccount.Email,
                    Picture = new Uri((userAccount.PhotoUrl != null ? $"{userAccount.PhotoUrl}" : $"https://autisticdating.net/imgs/profile-placeholder.jpg"))
                }: null; 
            }
        }

        static readonly string[] DefaultScopes = new []
        {
            Scopes.Profile
        };


        internal GoogleClientManager()
        {
            if (CurrentActivity == null)
            {
                throw new GoogleClientNotInitializedErrorException(GoogleClientBaseException.ClientNotInitializedErrorMessage);
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

            // Build a GoogleSignInClient with the options specified by gso.
            mGoogleSignInClient = GoogleSignIn.GetClient(CurrentActivity, googleSignInOptions);
        }

        public static void Initialize(
            Activity activity,
            string serverClientId = null,
            string clientId = null,
            string[] scopes = null,
            int requestCode = 9637)
        {
            CurrentActivity = activity;
            _serverClientId = serverClientId;
            _clientId = clientId;
            _initScopes = DefaultScopes.Concat(scopes ?? new string[0]).ToArray();
            AuthActivityID = requestCode;
        }

        static EventHandler<GoogleClientResultEventArgs<GoogleUser>> _onLogin;
        public event EventHandler<GoogleClientResultEventArgs<GoogleUser>> OnLogin
        {
            add => _onLogin += value;
            remove => _onLogin -= value;
        }

        public async Task<GoogleResponse<GoogleUser>> LoginAsync()
        {
            if (CurrentActivity == null || mGoogleSignInClient == null)
            {
                throw new GoogleClientNotInitializedErrorException(GoogleClientBaseException.ClientNotInitializedErrorMessage);
            }

            _loginTcs = new TaskCompletionSource<GoogleResponse<GoogleUser>>();

            GoogleSignInAccount account = GoogleSignIn.GetLastSignedInAccount(CurrentActivity);

 
            if (account!=null)
            {
                OnSignInSuccessful(account);
            }
            else
            {
                Intent intent = mGoogleSignInClient.SignInIntent;
                CurrentActivity?.StartActivityForResult(intent, AuthActivityID);
            }

            return await _loginTcs.Task;
        }

        public async Task<GoogleResponse<GoogleUser>> SilentLoginAsync()
        {

            if (CurrentActivity == null || mGoogleSignInClient == null)
            {
                throw new GoogleClientNotInitializedErrorException(GoogleClientBaseException.ClientNotInitializedErrorMessage);
            }

            _loginTcs = new TaskCompletionSource<GoogleResponse<GoogleUser>>();

            GoogleSignInAccount account = GoogleSignIn.GetLastSignedInAccount(CurrentActivity);

            if (account != null)
            {
                OnSignInSuccessful(account);
            }
            else
            {
                GoogleSignInAccount userAccount = await mGoogleSignInClient.SilentSignInAsync();
                OnSignInSuccessful(userAccount);
            }

            return await _loginTcs.Task;
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
            if (CurrentActivity == null || mGoogleSignInClient == null)
            {
                throw new GoogleClientNotInitializedErrorException(GoogleClientBaseException.ClientNotInitializedErrorMessage);
            }

            if (GoogleSignIn.GetLastSignedInAccount(CurrentActivity)!=null)
            {
                //Auth.GoogleSignInApi.SignOut(GoogleApiClient);
                _activeToken = null;
                mGoogleSignInClient.SignOut();
                //GoogleApiClient.Disconnect();

                // Log the state of the client
                //Debug.WriteLine(Tag + ": The user has logged out succesfully? " + !GoogleApiClient.IsConnected);

                // Send the logout result to the receivers
                OnLogoutCompleted(EventArgs.Empty);
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                if (CurrentActivity == null || mGoogleSignInClient == null)
                {
                    throw new GoogleClientNotInitializedErrorException(GoogleClientBaseException.ClientNotInitializedErrorMessage);
                }
                return GoogleSignIn.GetLastSignedInAccount(CurrentActivity) != null;
            }
        }

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
            GoogleSignIn.GetSignedInAccountFromIntent(data).AddOnCompleteListener(CrossGoogleClient.Current as IOnCompleteListener);
      
        }

        private static void OnSignInSuccessful(GoogleSignInAccount userAccount)
        {
            GoogleUser googleUser = new GoogleUser
            {
                Id = userAccount.Id,
                Name = userAccount.DisplayName,
                GivenName = userAccount.GivenName,
                FamilyName = userAccount.FamilyName,
                Email = userAccount.Email,
                Picture = new Uri((userAccount.PhotoUrl != null ? $"{userAccount.PhotoUrl}" : $"https://autisticdating.net/imgs/profile-placeholder.jpg"))
            };

            _activeToken = userAccount.IdToken;

            var scopes= string.Join(' ',userAccount.GrantedScopes.Select(s => s.ScopeUri).ToArray());

            var accessToken = GoogleAuthUtil.GetToken(Application.Context, userAccount.Account, scopes);
            System.Console.WriteLine($"Active Token: {_activeToken}");
            System.Console.WriteLine($"Access Token: {accessToken}");
            System.Console.WriteLine($"Scopes: {scopes}");

            var googleArgs =
                new GoogleClientResultEventArgs<GoogleUser>(googleUser, GoogleActionStatus.Completed);

            // Send the result to the receivers
            _onLogin?.Invoke(CrossGoogleClient.Current, googleArgs);
            _loginTcs.TrySetResult(new GoogleResponse<GoogleUser>(googleArgs));
        }

        private static void OnSignInFailed(ApiException apiException)
        {
            GoogleClientErrorEventArgs errorEventArgs = new GoogleClientErrorEventArgs();
            Exception exception = null;

            switch (apiException.StatusCode)
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
                    errorEventArgs.Message = apiException.Message;
                    exception = new GoogleClientBaseException(
                        string.IsNullOrEmpty(apiException.Message) 
                            ? GoogleClientBaseException.SignInDefaultErrorMessage 
                            : apiException.Message
                        );
                    break;
            }

            _onError?.Invoke(CrossGoogleClient.Current, errorEventArgs);
            _loginTcs.TrySetException(exception);
        }


        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if(!task.IsSuccessful)
            {
                //Failed
                OnSignInFailed(task.Exception.JavaCast<ApiException>());
                return;
            }
            else
            {
                var userAccount = task.Result.JavaCast<GoogleSignInAccount>();

                OnSignInSuccessful(userAccount);
               
            }
       

        }
    }
}
