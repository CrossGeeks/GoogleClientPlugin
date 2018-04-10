using System;

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using GoogleClientSample.Models;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using Xamarin.Forms;

namespace GoogleClientSample.ViewModels
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        public UserProfile User { get; set; } = new UserProfile();
        public string Name
        {
            get => User.Name;
            set => User.Name = value;
        }

        public string Email
        {
            get => User.Email;
            set => User.Email = value;
        }

        public Uri Picture
        {
            get => User.Picture;
            set => User.Picture = value;
        }

        public bool IsLoggedIn { get; set; }

        public ICommand LoginCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        private readonly IGoogleClientManager _googleClientManager;
        public event PropertyChangedEventHandler PropertyChanged;

        public LoginPageViewModel()
        {
            LoginCommand = new Command(LoginAsync);
            LogoutCommand = new Command(Logout);
            _googleClientManager = CrossGoogleClient.Current;
            IsLoggedIn = false;
        }

        public void LoginAsync()
        {
            _googleClientManager.OnLogin += OnLoginCompleted;
            _googleClientManager.LoginAsync();
        }


        private void OnLoginCompleted(object sender, GoogleClientResultEventArgs<GoogleUser> loginEventArgs)
        {
            if (loginEventArgs.Data != null)
            {
                GoogleUser googleUser = loginEventArgs.Data;

                User.Name = googleUser.Name;
                User.Email = googleUser.Email;
                User.Picture = googleUser.Picture;
                

                // Log the current User email
                Debug.WriteLine(User.Email);
                IsLoggedIn = true;
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", loginEventArgs.Message, "OK");
            }

            _googleClientManager.OnLogin -= OnLoginCompleted;

        }

        public void Logout()
        {
            _googleClientManager.OnLogout += OnLogoutCompleted;
            _googleClientManager.Logout();
        }

        private void OnLogoutCompleted(object sender, EventArgs loginEventArgs)
        {
            IsLoggedIn = false;
            User.Email = "Offline";
            _googleClientManager.OnLogout -= OnLogoutCompleted;
        }
    }
}
