using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.Tasks.NetworkAnalyst;
using FieldWorkerAssistant.Model;
using FieldWorkerAssitant.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace FieldWorkerAssistant.ViewModel
{
    internal class ItineraryViewModel
    {
        public ItineraryViewModel()
        {
            AllServiceItems = new ObservableCollection<ServiceItemViewModel>
            {
                new ServiceItemViewModel(new ServiceItem{ ServiceRequestID = 1001, Severity = "High", ProblemDescription=" This is a really big problem need someone on it right away."}),
                new ServiceItemViewModel(new ServiceItem{ ServiceRequestID = 1002, Severity = "Normal", ProblemDescription=" This is a normal issue fix when you can."}),
                new ServiceItemViewModel(new ServiceItem{ ServiceRequestID = 1003, Severity = "Low", ProblemDescription=" This is low priority get to it when have nothing to do."}),
                new ServiceItemViewModel(new ServiceItem{ ServiceRequestID = 1004, Severity = "Normal", ProblemDescription=" This is a normal issue fix when you can."}),
                new ServiceItemViewModel(new ServiceItem{ ServiceRequestID = 1005, Severity = "Normal", ProblemDescription=" This is a normal issue fix when you can."}),
            };

            foreach (var item in AllServiceItems)
                item.PropertyChanged += item_PropertyChanged;

            IncludedServiceItems = new ObservableCollection<ServiceItemViewModel>();

            IncludedServiceItems.CollectionChanged += IncludedServiceItems_CollectionChanged;

            IncludeAllCommand = new DelegateCommand(includeAll, canIncludeAll);
            ExcludeAllCommand = new DelegateCommand(excludeAll, canExcludeAll);
            SolveRouteCommand = new DelegateCommand(executeSolveRoute, canExecuteSolveRoute);
        }

        void IncludedServiceItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ServiceItemViewModel item in e.OldItems)
                    item.Included = false;
            }

            if (e.NewItems != null)
            {
                foreach (ServiceItemViewModel item in e.NewItems)
                    item.Included = true;
            }

            raiseCanExecuteChanged();
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (ServiceItemViewModel)sender;
            if (item.Included && !IncludedServiceItems.Contains(item))
                IncludedServiceItems.Add(item);
            else if (!item.Included && IncludedServiceItems.Contains(item))
                IncludedServiceItems.Remove(item);
        }

        public ObservableCollection<ServiceItemViewModel> AllServiceItems { get; internal set; }

        public ObservableCollection<ServiceItemViewModel> IncludedServiceItems { get; internal set; }

        public ICommand IncludeAllCommand { get; private set; }

        public ICommand ExcludeAllCommand { get; private set; }

        public ICommand SolveRouteCommand { get; private set; }

        private bool canExecuteSolveRoute(object parameter)
        {
            return true;
        }

        private async void executeSolveRoute(object parameter)
        {
            Graphic graphicRoute = await SolveRouteOffline();            
            if (graphicRoute != null)
                await AddGraphicLayer(new List<Graphic>() { graphicRoute });
        }

        private async Task GeocodeLocationOffline()
        {
            try
            {
                string filePath = await GetLocalLocatorFilePath();
                LocalLocatorTask locatorTask = new LocalLocatorTask(filePath);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                CancellationToken token = cancellationTokenSource.Token;

                LocatorServiceInfo locatorServiceInfo = await locatorTask.GetInfoAsync(token);
                Dictionary<string, string> inputAddress =
                 new Dictionary<string, string>() { 
                            { locatorServiceInfo.SingleLineAddressField.FieldName, "380 New York St, Redlands, CA 92373" } 
                        };

                IList<LocatorGeocodeResult> results = await locatorTask.GeocodeAsync(inputAddress, new List<string> { "*" }, SpatialReferences.WebMercator, token);
                if (results != null && results.Count > 0)
                    if (results.Any(l => l.Score == 100))
                    {

                    }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task ReverseGeocodeLocationOffline()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;            
            var mp = new MapPoint(-13046156.2143018, 4036527.88969275, new SpatialReference(102100)); // x and y of "380 New York St, Redlands, CA 92373"
            string filePath = await GetLocalLocatorFilePath();
            LocalLocatorTask locatorTask = new LocalLocatorTask(filePath);
            LocatorReverseGeocodeResult result = await locatorTask.ReverseGeocodeAsync(mp, 50, SpatialReferences.WebMercator, token);
            if (result != null && result.AddressFields != null)
            {
                IDictionary<string, string> resultAddress = result.AddressFields;
            }
        }

        private async Task<string> GetLocalLocatorFilePath()
        {
            Uri appResourceUri = new Uri("ms-appx:///Data/Locator/CANV/v101/CaliforniaNevadaLocator.loc");
            StorageFile locFile = await StorageFile.GetFileFromApplicationUriAsync(appResourceUri);
            return locFile.Path;
        }

        private async Task<Graphic> SolveRouteOffline()
        {
            try
            {
                Uri appResourceUri = new Uri("ms-appx:///Data/Network/CaliforniaNevada/RuntimeCANV.geodatabase");
                StorageFile databasePath = await StorageFile.GetFileFromApplicationUriAsync(appResourceUri);
                LocalRouteTask lrt = new LocalRouteTask(databasePath.Path, "RuntimeCANV");                
                RouteParameters routeParams = await lrt.GetDefaultParametersAsync();
                routeParams.ReturnRoutes = true;
                routeParams.ReturnDirections = true;
                routeParams.ReturnStops = true;
                routeParams.Stops = new FeaturesAsFeature(GetStops());
                RouteResult result = await lrt.SolveAsync(routeParams);
                if (result != null && result.Routes != null)
                {
                    return result.Routes[0].RouteGraphic;
                }
            }
            catch (Exception ex)
            {                
                throw;
            }
            return null;
        }

        private List<Graphic> GetStops()
        {
            List<Graphic> stops = new List<Graphic>(3);
            stops.Add(new Graphic() { Geometry = new MapPoint(-13049317.961, 4037281.268) { SpatialReference = new SpatialReference(102100) } }); // Walmart in redlands
            stops.Add(new Graphic() { Geometry = new MapPoint(-13046156.2143018, 4036527.88969275) { SpatialReference = new SpatialReference(102100) } }); // ESRI
            stops.Add(new Graphic() { Geometry = new MapPoint(-13043483.325, 4035523.923) { SpatialReference = new SpatialReference(102100) } }); // Panera Bread in Redlands

            return stops;
        }

        private async Task AddGraphicLayer(List<Graphic> graphicList)
        {
            //TODO
        }

        private bool canIncludeAll(object parameter)
        {
            return AllServiceItems.Where(item => !IncludedServiceItems.Contains(item)).Count() > 0;
        }

        private void includeAll(object parameter)
        {
            foreach (var item in AllServiceItems)
            {
                if (!IncludedServiceItems.Contains(item))
                    IncludedServiceItems.Add(item);
            }
        }

        private bool canExcludeAll(object parameter)
        {
            return IncludedServiceItems.Count > 0;
        }

        private void excludeAll(object parameter)
        {
            var includedItemsCopy = IncludedServiceItems.ToArray();
            foreach (var item in includedItemsCopy)
                IncludedServiceItems.Remove(item);
        }

        private void raiseCanExecuteChanged()
        {
            ((DelegateCommand)IncludeAllCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ExcludeAllCommand).RaiseCanExecuteChanged();
        }
    }
}
