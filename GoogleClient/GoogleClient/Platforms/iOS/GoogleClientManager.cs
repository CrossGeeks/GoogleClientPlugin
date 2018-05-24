using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Foundation;
using Google.SignIn;
using UIKit;
using GoogleUser = Plugin.GoogleClient.Shared.GoogleUser;

namespace Plugin.GoogleClient
{
    /// <summary>
    /// Implementation for GoogleClient
    /// </summary>
	/// 
	public class GoogleClientManager : NSObject, IGoogleClientManager, ISignInDelegate, ISignInUIDelegate
    {
        // Class Debug Tag
        private String Tag = typeof(GoogleClientManager).FullName;

        private UIViewController _viewController { get; set; }
        static TaskCompletionSource<GoogleResponse<GoogleUser>> _loginTcs;

        public GoogleClientManager()
        {
            SignIn.SharedInstance.UIDelegate = this;
            SignIn.SharedInstance.Delegate = this;
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
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            _viewController = vc;

            SignIn.SharedInstance.SignInUser();
        }

        public async Task<GoogleResponse<GoogleUser>> LoginAsync()
        {
            _loginTcs = new TaskCompletionSource<GoogleResponse<GoogleUser>>();

            var window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            _viewController = viewController;

            SignIn.SharedInstance.SignInUser();

            return await _loginTcs.Task;
        }

        protected virtual void OnLoginCompleted(GoogleClientResultEventArgs<GoogleUser> e)
        {
            _onLogin?.Invoke(this, e);
        }

        public void Logout()
        {
            SignIn.SharedInstance.SignOutUser();

            // Send the logout result to the receivers
            OnLogoutCompleted(EventArgs.Empty);
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



        protected virtual void OnGoogleClientError(GoogleClientErrorEventArgs e)
        {
            _onError?.Invoke(this, e);
        }

        public void DidSignIn(SignIn signIn, Google.SignIn.GoogleUser user, NSError error)
        {
            GoogleUser googleUser = null;

            if (user != null && error == null)
            {
                googleUser = new GoogleUser
                {
                    Name = user.Profile.Name,
                    Email = user.Profile.Email,
                    Picture = user.Profile.HasImage
                        ? new Uri(user.Profile.GetImageUrl(500).ToString())
                        : new Uri(string.Empty)
                };

                var googleArgs =
                    new GoogleClientResultEventArgs<GoogleUser>(googleUser, GoogleActionStatus.Completed, "the user is authenticated correctly");

                // Log the result of the authentication
                Debug.WriteLine(Tag + ": Authentication " + GoogleActionStatus.Completed);

                // Send the result to the receivers
                _onLogin?.Invoke(this, googleArgs);
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

                var googleArgs =
                    new GoogleClientResultEventArgs<GoogleUser>(googleUser, GoogleActionStatus.Error, error?.LocalizedDescription);

                // Log the result of the authentication
                Debug.WriteLine(Tag + ": authentication failed with error " + error?.LocalizedDescription);

                // Send the result to the receivers
                _onLogin?.Invoke(this, googleArgs);
                _loginTcs.TrySetResult(new GoogleResponse<GoogleUser>(googleArgs));
            }

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
    }
}
