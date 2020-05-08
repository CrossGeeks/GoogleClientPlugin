using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Foundation;
using Google.SignIn;
using Plugin.GoogleClient.Shared;
using UIKit;
using GoogleUser = Plugin.GoogleClient.Shared.GoogleUser;

namespace Plugin.GoogleClient
{
    /// <summary>
    /// Implementation for GoogleClient
    /// </summary>
	/// 
	public class GoogleClientManager : NSObject, IGoogleClientManager, ISignInDelegate
    {
        // Class Debug Tag
        private String Tag = typeof(GoogleClientManager).FullName;

        public string ActiveToken { get { return _activeToken; } }
        string _activeToken { get; set; }
        static string _clientId { get; set; }

        public GoogleUser CurrentUser
        {
            get
            {
                if (SignIn.SharedInstance.HasPreviousSignIn)
                    SignIn.SharedInstance.RestorePreviousSignIn();

                var user = SignIn.SharedInstance.CurrentUser;
                return user!=null? new GoogleUser
                {
                    Id = user.UserId,
                    Name = user.Profile.Name,
                    GivenName = user.Profile.GivenName,
                    FamilyName = user.Profile.FamilyName,
                    Email = user.Profile.Email,
                    Picture = user.Profile.HasImage
                        ? new Uri(user.Profile.GetImageUrl(500).ToString())
                        : new Uri(string.Empty)
                }: null;
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return SignIn.SharedInstance.HasPreviousSignIn;
            }
        }

        /*
        public DateTime TokenExpirationDate { get { return _tokenExpirationDate; } }
        DateTime _tokenExpirationDate { get; set; }
        */
        static TaskCompletionSource<GoogleResponse<GoogleUser>> _loginTcs;

		public static void Initialize(
            string clientId = null,
            params string[] scopes
        )
        {
            SignIn.SharedInstance.Delegate = CrossGoogleClient.Current as ISignInDelegate;
            if (scopes != null && scopes.Length > 0)
            {

                var currentScopes = SignIn.SharedInstance.Scopes;
                var initScopes = currentScopes
                    .Concat(scopes)
                    .Distinct()
                    .ToArray();


                SignIn.SharedInstance.Scopes = initScopes;
            }

            SignIn.SharedInstance.ClientId = string.IsNullOrWhiteSpace(clientId)
                ? GetClientIdFromGoogleServiceDictionary()
                : clientId;
            //SignIn.SharedInstance.ShouldFetchBasicProfile = true;
        }

        static string GetClientIdFromGoogleServiceDictionary()
        {
            var googleServiceDictionary = NSDictionary.FromFile("GoogleService-Info.plist");
            _clientId = googleServiceDictionary["CLIENT_ID"].ToString();
            return googleServiceDictionary["CLIENT_ID"].ToString();
        }

        EventHandler<GoogleClientResultEventArgs<GoogleUser>> _onLogin;
        event EventHandler<GoogleClientResultEventArgs<GoogleUser>> IGoogleClientManager.OnLogin
        {
            add => _onLogin += value;
            remove => _onLogin -= value;
        }
        
        EventHandler _onLogout;
        public event EventHandler OnLogout
        {
            add => _onLogout += value;
            remove => _onLogout -= value;
        }

        public void Login()
        {
            UpdatePresentedViewController();
           
            SignIn.SharedInstance.SignInUser();
        }

        public async Task<GoogleResponse<GoogleUser>> LoginAsync()
        {
            if (SignIn.SharedInstance.ClientId == null)
            {
                throw new GoogleClientNotInitializedErrorException(GoogleClientBaseException.ClientNotInitializedErrorMessage);
            }


            _loginTcs = new TaskCompletionSource<GoogleResponse<GoogleUser>>();

            UpdatePresentedViewController();
            if (CurrentUser == null)
            {
               
                SignIn.SharedInstance.SignInUser();
            }
            else
            {
                SignIn.SharedInstance.CurrentUser.Authentication.GetTokens(async (Authentication authentication, NSError error) =>
                {
                    if (error == null)
                    {
                        _activeToken = authentication.AccessToken;
                        System.Console.WriteLine($"Active Token: {_activeToken}");
                    }

                });


                /* DateTime newDate = TimeZone.CurrentTimeZone.ToLocalTime(
                     new DateTime(2001, 1, 1, 0, 0, 0));
                 _tokenExpirationDate = newDate.AddSeconds(user.Authentication.AccessTokenExpirationDate.SecondsSinceReferenceDate);
                 */
                var googleArgs = new GoogleClientResultEventArgs<GoogleUser>(
                    CurrentUser,
                    GoogleActionStatus.Completed,
                    "the user is authenticated correctly"
                );

                // Log the result of the authentication
                Debug.WriteLine(Tag + ": Authentication " + GoogleActionStatus.Completed);

                // Send the result to the receivers
                _onLogin?.Invoke(this, googleArgs);
                _loginTcs.TrySetResult(new GoogleResponse<GoogleUser>(googleArgs));
            }

            return await _loginTcs.Task;
        }

        public async Task<GoogleResponse<GoogleUser>> SilentLoginAsync()
        {
            if (SignIn.SharedInstance.ClientId == null)
            {
                throw new GoogleClientNotInitializedErrorException(GoogleClientBaseException.ClientNotInitializedErrorMessage);
            }

            //SignIn.SharedInstance.CurrentUser.Authentication.ClientId != _clientId
            _loginTcs = new TaskCompletionSource<GoogleResponse<GoogleUser>>();

            if (SignIn.SharedInstance.HasPreviousSignIn)
                SignIn.SharedInstance.RestorePreviousSignIn();

            var currentUser = SignIn.SharedInstance.CurrentUser;
            var isSuccessful = currentUser != null;

            if(isSuccessful)
            {
                OnSignInSuccessful(currentUser);
            }
            else
            {
                var errorEventArgs = new GoogleClientErrorEventArgs();
                errorEventArgs.Error = GoogleClientErrorType.SignInDefaultError;
                errorEventArgs.Message = GoogleClientBaseException.SignInDefaultErrorMessage;
                _onError?.Invoke(this, errorEventArgs);
                _loginTcs.TrySetException(new GoogleClientBaseException());
            }

            return await _loginTcs.Task;
        }

	public static bool OnOpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var openUrlOptions = new UIApplicationOpenUrlOptions(options);
            return SignIn.SharedInstance.HandleUrl(url);
        }

        protected virtual void OnLoginCompleted(GoogleClientResultEventArgs<GoogleUser> e)
        {
            _onLogin?.Invoke(this, e);
        }

        public void Logout()
        {
            if (SignIn.SharedInstance.ClientId == null)
            {
                throw new GoogleClientNotInitializedErrorException(GoogleClientBaseException.ClientNotInitializedErrorMessage);
            }

            if (IsLoggedIn)
            {
                _activeToken = null;
                SignIn.SharedInstance.SignOutUser();
                // Send the logout result to the receivers
                OnLogoutCompleted(EventArgs.Empty);
            }

        }

        protected virtual void OnLogoutCompleted(EventArgs e)
        {
            _onLogout?.Invoke(this, e);
        }

        EventHandler<GoogleClientErrorEventArgs> _onError;
        public event EventHandler<GoogleClientErrorEventArgs> OnError
        {
            add => _onError += value;
            remove => _onError -= value;
        }


	public void DidSignIn(SignIn signIn, Google.SignIn.GoogleUser user, NSError error)
        {
            var isSuccessful = user != null && error == null;

            if (isSuccessful)
            {
                OnSignInSuccessful(user);
                return;
            }

            GoogleClientErrorEventArgs errorEventArgs = new GoogleClientErrorEventArgs();
            Exception exception = null;
            switch (error.Code)
            {
                case -1:
                    errorEventArgs.Error = GoogleClientErrorType.SignInUnknownError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInUnknownErrorMessage;
                    exception=new GoogleClientSignInUnknownErrorException();
                    break;
                case -2: 
                    errorEventArgs.Error = GoogleClientErrorType.SignInKeychainError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInKeychainErrorMessage;
                    exception = new GoogleClientSignInKeychainErrorException();
                    break;
                case -3:
                    errorEventArgs.Error = GoogleClientErrorType.NoSignInHandlersInstalledError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInNoSignInHandlersInstalledErrorMessage;
                    exception = new GoogleClientSignInNoSignInHandlersInstalledErrorException();
                    break;
                case -4:
                    errorEventArgs.Error = GoogleClientErrorType.SignInHasNoAuthInKeychainError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInUnknownErrorMessage;
                    exception = new GoogleClientSignInHasNoAuthInKeychainErrorException();
                    break;
                case -5:
                    errorEventArgs.Error = GoogleClientErrorType.SignInCanceledError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInCanceledErrorMessage;
                    exception = new GoogleClientSignInCanceledErrorException();
                    break;
                default:
                    errorEventArgs.Error = GoogleClientErrorType.SignInDefaultError;
                    errorEventArgs.Message = GoogleClientBaseException.SignInDefaultErrorMessage;
                    exception = new GoogleClientBaseException();
                    break;
            }

            _onError?.Invoke(this,errorEventArgs);
            _loginTcs.TrySetException(exception);
        }
        
        [Export("signIn:didDisconnectWithUser:withError:")]
        public void DidDisconnect(SignIn signIn, Google.SignIn.GoogleUser user, NSError error)
        {
            // Perform any operations when the user disconnects from app here.
        }


        void UpdatePresentedViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            SignIn.SharedInstance.PresentingViewController = viewController;
        }


        void OnSignInSuccessful(Google.SignIn.GoogleUser user)
        {
            GoogleUser googleUser = new GoogleUser
                {
                    Id = user.UserId,
                    Name = user.Profile.Name,
                    GivenName = user.Profile.GivenName,
                    FamilyName = user.Profile.FamilyName,
                    Email = user.Profile.Email,
                    Picture = user.Profile.HasImage
                        ? new Uri(user.Profile.GetImageUrl(500).ToString())
                        : new Uri(string.Empty)
                };

                 user.Authentication.GetTokens(async (Authentication authentication, NSError error) =>
                {
                    if(error ==null)
                    {
                        _activeToken = authentication.AccessToken;
                        System.Console.WriteLine($"Active Token: {_activeToken}");
                    }
             
                });

             
                /* DateTime newDate = TimeZone.CurrentTimeZone.ToLocalTime(
                     new DateTime(2001, 1, 1, 0, 0, 0));
                 _tokenExpirationDate = newDate.AddSeconds(user.Authentication.AccessTokenExpirationDate.SecondsSinceReferenceDate);
                 */
                var googleArgs = new GoogleClientResultEventArgs<GoogleUser>(
                    googleUser, 
                    GoogleActionStatus.Completed, 
                    "the user is authenticated correctly"
                );

                // Log the result of the authentication
                Debug.WriteLine(Tag + ": Authentication " + GoogleActionStatus.Completed);

                // Send the result to the receivers
                _onLogin?.Invoke(this, googleArgs);
                _loginTcs.TrySetResult(new GoogleResponse<GoogleUser>(googleArgs));
                
        }
    }
}
