using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

// If you wish to create your custom UserProfile just replace the Google Client Google User with your own UserProfile Model
namespace GoogleClientSample.Models
{
    public class UserProfile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
