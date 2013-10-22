using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using FieldWorkerAssistant.Converters;
using FieldWorkerAssistant.Model;
using FieldWorkerAssistant.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace FieldWorkerAssistant.Pages
{
    /// <summary>
    /// Page for specifying the items to include in the itinerary
    /// </summary>
    public sealed partial class PlanItinerary : FieldWorkerAssistant.Common.LayoutAwarePage
    {
        public PlanItinerary()
        {
            this.InitializeComponent();

            var viewModel = ((App)App.Current).ItineraryViewModel;
            this.DataContext = viewModel;

            MainMap.InitialExtent = new Envelope(-13046907.1247363, 4034314.00501996, -13042604.5495666, 4038309.25339177,
                SpatialReferences.WebMercator);

            // Bind source of all items and included items - need to do it in code behind because it can't be 
            // done in XAML
            GraphicsLayer includedItemsLayer = (GraphicsLayer)MainMap.Layers["IncludedItemsLayer"];
            Binding b = new Binding()
            {
                Path = new PropertyPath("IncludedServiceItems"),
                Source = viewModel,
                Converter = new ServiceItemsToFeatures()
            };
            BindingOperations.SetBinding(includedItemsLayer, GraphicsLayer.GraphicsSourceProperty, b);

            FeatureLayer workItemsLayer = (FeatureLayer)MainMap.Layers["WorkItemsLayer"];
            if (workItemsLayer.Graphics.Count > 0)
            {
                viewModel.InitializeServiceItems(workItemsLayer.Graphics);
            }
            else
            {
                workItemsLayer.Graphics.CollectionChanged += (o, e) => 
                    viewModel.InitializeServiceItems(workItemsLayer.Graphics);
            }
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

        private void ItineraryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ServiceItemViewModel item in e.AddedItems)
                item.Included = true;

            foreach (ServiceItemViewModel item in e.RemovedItems)
                item.Included = false;
        }
    }
}
