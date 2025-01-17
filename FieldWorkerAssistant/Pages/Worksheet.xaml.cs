﻿using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Navigation;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using FieldWorkerAssistant.Common;

namespace FieldWorkerAssistant.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Worksheet : FieldWorkerAssistant.Common.LayoutAwarePage
    {
        private GdbFeature feature;

        public Worksheet()
        {
            this.InitializeComponent();

            feature = ((App)(App.Current)).SelectedFeature;
            this.DataContext = feature;

            pageDate.Text = DateTime.Now.ToString("M");
            pageTime.Text = DateTime.Now.ToString("h:mm tt");
            StartTimeTextBlock.Text = DateTime.Now.ToString("h:mm tt");
            EndTimeTextBlock.Text = DateTime.Now.AddHours(1).ToString("h:mm tt");


            var clock = new DispatcherTimer {Interval = new TimeSpan(0,0,1,0,0)};
            clock.Tick += (s, e) => { pageTime.Text = DateTime.Now.ToString("h:mm tt"); };
            clock.Start();

            var gr = feature.AsGraphic();
            gr.Attributes.Remove(gr.Attributes.FirstOrDefault(kvp => kvp.Key == "GlobalID"));
            double xmin, xmax, ymin, ymax;
            var point = (MapPoint) gr.Geometry;
            xmin = point.X - 1000;
            xmax = point.X + 1000;
            ymin = point.Y - 1000;
            ymax = point.Y + 1000;
            mySmallMap.InitialExtent = new Envelope(xmin, ymin, xmax, ymax, SpatialReferences.WebMercator);
            mySmallMap.IsHitTestVisible = false;
            var gl = mySmallMap.Layers["SelectedItemsLayer"] as GraphicsLayer;
            gl.Graphics.Add(gr);

        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }      

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            feature.Attributes["Status"] = StatusComboBox.SelectedItem;
            feature.Attributes["ActionTaken"] = ActionTakenTextBox.Text;
            var viewModel = ((App)App.Current).RouteViewModel;
            await viewModel.CachedFeatureLayer.FeatureTable.UpdateAsync(feature);
            viewModel.HasChanges = true;
            if(Frame.CanGoBack)
                Frame.GoBack();            
        }
    }
}
