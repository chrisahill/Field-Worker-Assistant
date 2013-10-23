using Esri.ArcGISRuntime.Layers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using FieldWorkerAssistant.Common;

namespace FieldWorkerAssistant.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Route : FieldWorkerAssistant.Common.LayoutAwarePage
    {
        public Route()
        {
            this.InitializeComponent();
            var viewModel = ((App)App.Current).RouteViewModel;
            this.DataContext = viewModel;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var viewModel = ((App)App.Current).RouteViewModel;
            if (MyMap.Layers.Contains(viewModel.CachedFeatureLayer))
                MyMap.Layers.Remove(viewModel.CachedFeatureLayer);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var viewModel = ((App)App.Current).RouteViewModel;
            if (!MyMap.Layers.Contains(viewModel.CachedFeatureLayer))
                MyMap.Layers.Add(viewModel.CachedFeatureLayer);
        
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
        
        private void Worksheet_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Worksheet));
        }

        private async void MyMap_Tapped(object sender, TappedRoutedEventArgs e)
        {
                 var l = MyMap.Layers[1] as ArcGISFeatureLayer;
            var id = l.HitTest(MyMap, e.GetPosition(MyMap));
            if (id >= 0) //hittest returns the feature ID
            {
                l.ClearSelection();
                l.SelectFeatures(new long[] { id }); //select the found feature
                //Get feature using a query by ID
                var features = await l.FeatureTable.QueryAsync(new long[] { id });
                var feature = features.FirstOrDefault();
                ((App)App.Current).SelectedFeature = feature;
            }
        }
    }
}
