﻿using MTBrokerMobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MTBrokerMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            BindingContext = new HomePageViewModel(this);

        }
    }
}