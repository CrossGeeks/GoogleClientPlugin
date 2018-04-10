using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Plugin.GoogleClient.Shared;

// If you wish to create your custom UserProfile just replace the Google Client Google User with your own UserProfile Model
namespace GoogleClientSample.Models
{
    public class UserProfile :  INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Uri Picture { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
