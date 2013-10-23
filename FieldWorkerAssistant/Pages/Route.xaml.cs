using Esri.ArcGISRuntime.Geometry;
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
using Esri.ArcGISRuntime.Location;
using FieldWorkerAssistant.Common;
<<<<<<< HEAD
using FieldWorkerAssistant.Converters;
using Esri.ArcGISRuntime.Data;
=======
using FieldWorkerAssistant.ViewModel;
>>>>>>> 4ced8e83619348ce948db4d4aedb18a1f9182e7e

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
            MyMap.LocationDisplay.IsEnabled = true;            
            MyMap.LocationDisplay.CurrentLocation = new LocationInfo()
            {                
                Location = new MapPoint(-13046156.2143018, 4036527.88969275) { SpatialReference = new SpatialReference(102100) }
            };
            MyMap.LocationDisplay.LocationProvider.StartAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var viewModel = ((App)App.Current).RouteViewModel;
<<<<<<< HEAD
            if (MyMap.Layers.Contains(viewModel.CachedGraphicsLayer))
                MyMap.Layers.Remove(viewModel.CachedGraphicsLayer);
=======
            if (MyMap.Layers.Contains(viewModel.CachedFeatureLayer))
                MyMap.Layers.Remove(viewModel.CachedFeatureLayer);
            
            if (MyMap.Layers.Contains(viewModel.RouteLayer))
                MyMap.Layers.Remove(viewModel.RouteLayer);
>>>>>>> 4ced8e83619348ce948db4d4aedb18a1f9182e7e
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
<<<<<<< HEAD
            var app = ((App)App.Current);
            var viewModel = app.RouteViewModel;
            MyMap.InitialExtent = app.DefaultExtent;
            if (!MyMap.Layers.Contains(viewModel.CachedGraphicsLayer))
            {
                MyMap.Layers.Insert(1, viewModel.CachedGraphicsLayer);
                viewModel.CachedGraphicsLayer.Tapped += ServiceItemsLayer_Tapped;
            }

            // Bind selected items layer's graphics source to included items collection - need to do it in 
            // code behind because it can't be done in XAML
            GraphicsLayer includedItemsLayer = (GraphicsLayer)MyMap.Layers["SelectedItemsLayer"];
            Binding b = new Binding()
            {
                Path = new PropertyPath("SelectedRouteServiceItems"),
                Source = viewModel,
                Converter = new ServiceItemsToFeatures()
            };
            BindingOperations.SetBinding(includedItemsLayer, GraphicsLayer.GraphicsSourceProperty, b);
        }

        async void ServiceItemsLayer_Tapped(object sender, GraphicTappedRoutedEventArgs e)
        {
            toggleSelectGraphic(e.Graphic);

            var app = ((App)Application.Current);
            var viewModel = app.RouteViewModel;
            QueryFilter filter = new QueryFilter()
            {
                WhereClause = string.Format("OBJECTID = {0}", e.Graphic.Attributes["OBJECTID"])
            };
            var result = await viewModel.CachedFeatureLayer.FeatureTable.QueryAsync(filter);
            if (result.Count() > 0)
                app.SelectedFeature = (GdbFeature)result.ElementAt(0);
        }

        private void toggleSelectGraphic(Graphic g)
        {
            var viewModel = ((App)Application.Current).RouteViewModel;
            foreach (var item in viewModel.RouteServiceItems)
            {
                object id1 = item.Service.Feature.Attributes["OBJECTID"];
                object id2 = g.Attributes["OBJECTID"];
                bool b = id1 == id2;
            }
            if (viewModel.SelectedRouteServiceItems.Any(
                item => (int)item.Service.Feature.Attributes["OBJECTID"] == (int)g.Attributes["OBJECTID"]))
            {
                var selectedItem = viewModel.SelectedRouteServiceItems.First(item =>
                    (int)item.Service.Feature.Attributes["OBJECTID"] == (int)g.Attributes["OBJECTID"]);
                viewModel.SelectedRouteServiceItems.Remove(selectedItem);
            }
            else if (viewModel.RouteServiceItems.Any(
                item => (int)item.Service.Feature.Attributes["OBJECTID"] == (int)g.Attributes["OBJECTID"]))
            {
                var itemsCopy = viewModel.SelectedRouteServiceItems.ToArray();
                foreach (var item in itemsCopy)
                    viewModel.SelectedRouteServiceItems.Remove(item);
                var newItem = viewModel.RouteServiceItems.First(
                    item => (int)item.Service.Feature.Attributes["OBJECTID"] == (int)g.Attributes["OBJECTID"]);
                viewModel.SelectedRouteServiceItems.Add(newItem);
            }
=======
            var viewModel = ((App)App.Current).RouteViewModel;
            if (!MyMap.Layers.Contains(viewModel.CachedFeatureLayer))
                MyMap.Layers.Add(viewModel.CachedFeatureLayer);
            
            if (!MyMap.Layers.Contains(viewModel.RouteLayer))
                MyMap.Layers.Add(viewModel.RouteLayer);
        
>>>>>>> 4ced8e83619348ce948db4d4aedb18a1f9182e7e
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

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ServiceItemViewModel) e.AddedItems[0];            
            var startPoint = new Graphic {Geometry = MyMap.LocationDisplay.CurrentLocation.Location};
            var endPoint = new Graphic {Geometry = item.Service.Feature.Geometry};        
           
            ((App)App.Current).RouteViewModel.executeSolveRoute(new[] {startPoint,endPoint});
        }
    }
}
