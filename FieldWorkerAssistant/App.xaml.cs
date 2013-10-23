using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using FieldWorkerAssistant.Common;
using FieldWorkerAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace FieldWorkerAssistant
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            ItineraryViewModel = new ItineraryViewModel();
            RouteViewModel = new RouteViewModel();

            DefaultExtent = new Envelope(-13046530.4359927, 4034612.33785403, 
                -13042227.860823, 4038825.27604103, SpatialReferences.WebMercator);

            SimpleLineSymbol outline = new SimpleLineSymbol()
            {
                Color = Colors.Black,
                Width = 2
            };

            int markerSize = 28;
            SimpleMarkerSymbol defaultSymbol = new SimpleMarkerSymbol()
            {
                Color = Color.FromArgb(125, 200, 200, 200),
                Size = markerSize,
                Outline = outline
            };
            SimpleMarkerSymbol lowPrioritySymbol = new SimpleMarkerSymbol()
            {
                Color = Color.FromArgb(125, 0, 0, 255),
                Size = markerSize,
                Outline = outline
            };
            SimpleMarkerSymbol mediumPrioritySymbol = new SimpleMarkerSymbol()
            {
                Color = Color.FromArgb(125, 255, 165, 0),
                Size = markerSize,
                Outline = outline
            };
            SimpleMarkerSymbol highPrioritySymbol = new SimpleMarkerSymbol()
            {
                Color = Color.FromArgb(125, 255, 0, 0),
                Size = markerSize,
                Outline = outline
            };

            UniqueValueInfo lowPriorityInfo = new UniqueValueInfo()
            {
                Symbol = lowPrioritySymbol,
                Values = new UniqueValueCollection(Constants.LOW_PRIORITY)
            };
            UniqueValueInfo mediumPriorityInfo = new UniqueValueInfo()
            {
                Symbol = mediumPrioritySymbol,
                Values = new UniqueValueCollection(Constants.NORMAL_PRIORITY)
            };
            UniqueValueInfo highPriorityInfo = new UniqueValueInfo()
            {
                Symbol = highPrioritySymbol,
                Values = new UniqueValueCollection(Constants.HIGH_PRIORITY)
            };

            UniqueValueRenderer uvRenderer = new UniqueValueRenderer();
            uvRenderer.Fields.Add("Severity");
            uvRenderer.Infos.Add(lowPriorityInfo);
            uvRenderer.Infos.Add(mediumPriorityInfo);
            uvRenderer.Infos.Add(highPriorityInfo);

            uvRenderer.DefaultSymbol = defaultSymbol;

            WorkItemsRenderer = uvRenderer;
        }
        internal string FeatureServiceUri = "http://services.arcgis.com/pmcEyn9tLWCoX7Dm/arcgis/rest/services/HackathonSR/FeatureServer";
        internal ItineraryViewModel ItineraryViewModel { get; private set; }
        internal RouteViewModel RouteViewModel { get; private set; }
        internal GdbFeature SelectedFeature { get; set; }        
        internal Renderer WorkItemsRenderer { get; private set; }
        internal Envelope DefaultExtent { get; private set; }

        // Stores whether the user has logged in to the app
        internal bool LoggedIn { get; set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            string path = ApplicationData.Current.LocalFolder.Path;
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("Replica.geodatabase");
                if (file != null) // Offline data exists - allow resumption of route
                {
                    await ItineraryViewModel.CreateCachedFeatureLayer(file);
                }
            }
            catch { }

            if (rootFrame.Content == null)
            {
                App.Current.Resources.Add("WorkItemsRenderer", WorkItemsRenderer);
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
