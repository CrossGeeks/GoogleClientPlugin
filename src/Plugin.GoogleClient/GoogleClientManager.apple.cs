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

        private UIViewController _viewController { get; set; }

        public string ActiveToken { get { return _activeToken; } }
        string _activeToken { get; set; }
        static string _clientId { get; set; }

        public GoogleUser CurrentUser
        {
            get
            {
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
            InitializeClientDelegates();
            InitializeScopes(scopes);
            InitializeClientId(clientId);
        }

        private static void InitializeClientDelegates()
        {

            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            SignIn.SharedInstance.PresentingViewController = vc;
            SignIn.SharedInstance.Delegate = CrossGoogleClient.Current as ISignInDelegate;
        }
        
        private static void InitializeScopes(params string[] scopes)
        {
            if(scopes.Length == 0)
            {
                return;
            }
            var currentScopes = SignIn.SharedInstance.Scopes;
            var initScopes = currentScopes
                .Concat(scopes)
                .Distinct()
                .ToArray();

            SignIn.SharedInstance.Scopes = initScopes;
        }

        private static void InitializeClientId(string clientId = null)
        {
            SignIn.SharedInstance.ClientId = string.IsNullOrWhiteSpace(clientId)
                ? GetClientIdFromGoogleServiceDictionary()
                : clientId;
        }

        private static string GetClientIdFromGoogleServiceDictionary()
        {
            var googleServiceDictionary = NSDictionary.FromFile("GoogleService-Info.plist");
            _clientId = googleServiceDictionary["CLIENT_ID"].ToString();
            return googleServiceDictionary["CLIENT_ID"].ToString();
        }

        static EventHandler<GoogleClientResultEventArgs<GoogleUser>> _onLogin;
        event EventHandler<GoogleClientResultEventArgs<GoogleUser>> IGoogleClientManager.OnLogin
        {
            add => _onLogin += value;
            remove => _onLogin -= value;
        }
        
        static EventHandler _onLogout;
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

            var task = CreateLoginTask();

            UpdatePresentedViewController();
            
            SignIn.SharedInstance.SignInUser();

            return await task;
        }

        public async Task<GoogleResponse<GoogleUser>> SilentLoginAsync()
        {
            if (SignIn.SharedInstance.ClientId == null)
            {
                throw new GoogleClientNotInitializedErrorException(GoogleClientBaseException.ClientNotInitializedErrorMessage);
            }

            var task = CreateLoginTask();

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
            
            return await task;
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

            if (SignIn.SharedInstance.HasPreviousSignIn)
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

        static EventHandler<GoogleClientErrorEventArgs> _onError;
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

        [Export("signInWillDispatch:error:")]
        public void WillDispatch(SignIn signIn, NSError error)
        {
            //myActivityIndicator.StopAnimating();
        }

        [Export("signIn:presentViewController:")]
        public void PresentViewController(SignIn signIn, UIViewController viewController)
        {
            _viewController?.PresentViewController(viewController, true, null);
        }

        [Export("signIn:dismissViewController:")]
        public void DismissViewController(SignIn signIn, UIViewController viewController)
        {
            _viewController?.DismissViewController(true, null);
        }

        private void UpdatePresentedViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            _viewController = viewController;
        }


        private static Task<GoogleResponse<GoogleUser>> CreateLoginTask()
        {
            _loginTcs = new TaskCompletionSource<GoogleResponse<GoogleUser>>();
            
            return _loginTcs.Task;
        }

        private void OnSignInSuccessful(Google.SignIn.GoogleUser user)
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

                _activeToken = user.Authentication.AccessToken;
                System.Console.WriteLine($"Active Token: {_activeToken}");
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
