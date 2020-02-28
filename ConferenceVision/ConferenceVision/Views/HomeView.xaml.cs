﻿using ConferenceVision.Models;
using ConferenceVision.Utils;
using ConferenceVision.ViewModels;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ConferenceVision.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeView : ContentPage
    {
        HomeViewModel vm;

        public HomeViewModel ViewModel
        {
            get => vm; set
            {
                vm = value;
                BindingContext = vm;
            }
        }

        bool uiInitialized;

        public HomeView()
        {
            InitializeComponent();
            if (vm == null)
                ViewModel = new HomeViewModel();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (!uiInitialized)
            {
                SearchForm.TranslateTo(width, 0, 0);
                uiInitialized = true;
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vm?.OnAppearing();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            //MediaList.DeselectOnTap();
            MessagingCenter.Unsubscribe<CameraView, Memory>(this, "GoToImage");
        }

        async void AddPhoto_Clicked(object sender, System.EventArgs e)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted)
            {
                status = await PermissionUtil.CheckPermissions(Permission.Camera);
            }

            if (status == PermissionStatus.Granted)
            {
                await Navigation.PushModalAsync(new CameraView(), true);
                MessagingCenter.Subscribe<CameraView, Memory>(this, "GoToImage", async (s, arg) =>
                {
                    await Shell.Current.GoToAsync($"details?id={arg.Id}");
                }
                );
            }
        }

        private async void Search_Clicked(object sender, EventArgs e)
        {
            Shell.SetNavBarIsVisible(this, false);
            VisualStateManager.GoToState(MediaList, "Searching");
            SearchContent.IsVisible = true;
            await Task.WhenAny<bool>
            (
              MediaList.FadeTo(0),
              SearchForm.TranslateTo(0, 0, 500, Easing.CubicInOut),
              AddPhotoButton.TranslateTo(0, 150, 500, Easing.CubicInOut),
              SearchContent.FadeTo(0.9)
            );
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Task.WhenAny<bool>
            (

              SearchForm.TranslateTo(Width, 0, 500, Easing.CubicInOut),
              AddPhotoButton.TranslateTo(0, 0, 500, Easing.CubicInOut),
              MediaList.FadeTo(1),
              SearchContent.FadeTo(0)
            );
            SearchContent.IsVisible = false;
            Shell.SetNavBarIsVisible(this, true);
            VisualStateManager.GoToState(MediaList, "Default");

        }

        async void GoToSearchActivityAsync(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//home/homework?content=Activity_ImplementSearch.md&sender=home");
        }

    }
}