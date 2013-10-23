using FieldWorkerAssistant.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FieldWorkerAssistant
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = ((App)Application.Current).RouteViewModel;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void NewRoute_Click(object sender, RoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (!app.LoggedIn) // Show credentials prompt
            {
                var options = new CredentialPickerOptions()
                {
                    TargetName = "FieldWorkerAssistant",
                    AlwaysDisplayDialog = true,
                    Caption = "Sign In",
                    Message = "Sign in to get service items",
                    CredentialSaveOption = CredentialSaveOption.Unselected,
                    CallerSavesCredential = false,
                    AuthenticationProtocol = AuthenticationProtocol.Basic
                };
                var result = await CredentialPicker.PickAsync(options);
                if (result.ErrorCode != 2147943623) // Cancelled
                {
                    app.LoggedIn = true;
                    Frame.Navigate(typeof(PlanItinerary));
                }
            }
            else
            {
                Frame.Navigate(typeof(PlanItinerary));
            }
        }

        private void CurrentRoute_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Route));
        }
    }
}
