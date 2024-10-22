﻿using System.Linq.Expressions;
using Windows.UI;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.Tasks.NetworkAnalyst;
using Esri.ArcGISRuntime.Tasks.Offline;
using Esri.ArcGISRuntime.WebMap;
using FieldWorkerAssistant.Model;
using FieldWorkerAssistant.ViewModel;
using FieldWorkerAssitant.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;

namespace FieldWorkerAssistant
{
    internal class RouteViewModel : INotifyPropertyChanged
    {
        private LocalRouteTask _routeTask = null;
        public RouteViewModel()
        {
            RouteServiceItems = new ObservableCollection<ServiceItemViewModel>();
            SelectedRouteServiceItems = new ObservableCollection<ServiceItemViewModel>();
            SelectedRouteServiceItems.CollectionChanged += SelectedRouteServiceItems_CollectionChanged;
            //SolveRouteCommand = new DelegateCommand(executeSolveRoute, canExecuteSolveRoute);                          
            SyncCommand = new DelegateCommand(syncCommand, canSyncCommand);
            var v = InitRouteService();
            //Task.WaitAll(new[] {v});
        }

        public async Task InitRouteService()
        {
            //Initialize network            
            var appResourceUri = new Uri("ms-appx:///Data/Network/CaliforniaNevada/RuntimeCANV.geodatabase");                     
            var databasePath = await StorageFile.GetFileFromApplicationUriAsync(appResourceUri);
            _routeTask = new LocalRouteTask(databasePath.Path, "RuntimeCANV");
            routeParams = await _routeTask.GetDefaultParametersAsync();
            routeParams.ReturnRoutes = true;
            routeParams.ReturnDirections = false;
            routeParams.ReturnStops = false;

            IsInitializing = false;
        }

        void SelectedRouteServiceItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("SelectedRouteServiceItems");
        }
        public ObservableCollection<ServiceItemViewModel> RouteServiceItems { get; internal set; }
        public ObservableCollection<ServiceItemViewModel> SelectedRouteServiceItems { get; internal set; }

        private ArcGISFeatureLayer m_CachedFeatureLayer;
        public ArcGISFeatureLayer CachedFeatureLayer
        {
            get
            {
                return m_CachedFeatureLayer;
            }
            internal set
            {
                if (m_CachedFeatureLayer != value)
                {
                    m_CachedFeatureLayer = value;

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Provides access to the cached feature layer as a graphics layer
        /// </summary>
        public GraphicsLayer CachedGraphicsLayer { get; internal set; }
        
        private GraphicsLayer m_RouteLayer;
        public GraphicsLayer RouteLayer
        {
            get
            {
                return m_RouteLayer ?? (m_RouteLayer = new GraphicsLayer 
                { ID = "RouteLayer", 
                    Renderer = new SimpleRenderer
                    {
                        Symbol = new SimpleLineSymbol
                        {
                            Color = Colors.DarkBlue,
                            Style = SimpleLineStyle.Solid,
                            Width = 5
                        }
                    }
                });
            }
            internal set
            {
                if (m_RouteLayer != value)
                {
                    m_RouteLayer = value;

                    OnPropertyChanged();
                }
            }
        }


        private bool m_initializing = true;
        public bool IsInitializing
        {
            get { return m_initializing; }
            private set
            {
                if (m_initializing != value)
                {
                    m_initializing = value;
                    OnPropertyChanged();
                    raiseCanExecuteChanged();
                }
            }
        }

        private bool m_HasChanges;

        public bool HasChanges
        {
            get { return m_HasChanges; }
            internal set
            {
                if (m_HasChanges != value)
                {
                    m_HasChanges = value;
                    OnPropertyChanged();
                    raiseCanExecuteChanged();
                }
            }
        }

        public void InitializeServiceItems(IEnumerable<Feature> features)
        {
            ServiceItemViewModel[] allItemsCopy = RouteServiceItems.ToArray();
            foreach (var item in allItemsCopy)
                RouteServiceItems.Remove(item);

            foreach (var feature in features)
            {
                var serviceItem = new ServiceItem(feature);
                var viewModel = new ServiceItemViewModel(serviceItem);                
                RouteServiceItems.Add(viewModel);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand SolveRouteCommand { get; private set; }
        public ICommand GeocodeCommand { get; private set; }
        public ICommand ReverseGeocodeCommand { get; private set; }
        public ICommand SyncCommand { get; private set; }

        private bool canSyncCommand(object parameter)
        {
            return CachedFeatureLayer != null && CachedFeatureLayer.FeatureTable != null && GdbFile != null && HasChanges;
        }
        private async void syncCommand(object parameter)
        {
            if (!canSyncCommand(parameter))
                return;

            IsSynching = true;

            string serviceTaskUri = ((App)App.Current).FeatureServiceUri;
            var task = new GeodatabaseTask(new Uri(serviceTaskUri));
            SyncGeodatabaseParameters parameters = new SyncGeodatabaseParameters()
            {
                SyncDirection = SyncDirection.Bidirectional
            };
            try
            {
                // Sync is failing for unexplained reasons.  Simulate sync time.
                await Task.Delay(3000);
                var result = await task.SubmitSyncJobAsync(parameters, GdbFile.Path,
                    (status, err) =>
                    {
                        IsSynching = false;
                    },
                    (uploadResult) =>  //delta uploaded
                    {

                    }, TimeSpan.FromSeconds(2),
                    (status) =>
                    { //status updates
                    },
                    CancellationToken.None);
            }
            catch
            {
                IsSynching = false;
            }
        }

        private bool m_IsSynching;
        public bool IsSynching
        {
            get { return m_IsSynching; }
            private set
            {
                if (m_IsSynching != value)
                {
                    m_IsSynching = value;
                    OnPropertyChanged();
                    raiseCanExecuteChanged();
                }
            }
        }

        private void raiseCanExecuteChanged()
        {
            //((DelegateCommand)GeocodeCommand).RaiseCanExecuteChanged();
            //((DelegateCommand)ReverseGeocodeCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SyncCommand).RaiseCanExecuteChanged();
        }


        /// <summary>
        /// Gets the file underlying the <see cref="CachedFeatureLayer"/>
        /// </summary>
        public StorageFile GdbFile { get; internal set; }

        private bool canExecuteSolveRoute(object parameter)
        {
            return true;
        }

        internal async void executeSolveRoute(IEnumerable<Graphic> stops)
        {
            Graphic graphicRoute = await SolveRouteOffline(stops);
            graphicRoute.Geometry = GeometryEngine.Project(graphicRoute.Geometry, SpatialReferences.WebMercator);
            RouteLayer.Graphics.Clear();
            RouteLayer.Graphics.Add(graphicRoute);            
        }

        private bool canExecuteGeocode(object parameter)
        {
            return true;
        }

        private async void executeGeocode(object parameter)
        {
            var v = GeocodeLocationOffline();
        }
        private bool canExecuteReverseGeocode(object parameter)
        {
            return true;
        }

        private async void executeReverseGeocode(object parameter)
        {
           var v = ReverseGeocodeLocationOffline();
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

        private RouteParameters routeParams;
        private async Task<Graphic> SolveRouteOffline(IEnumerable<Graphic> stops)
        {
            try
            {
                //Uri appResourceUri = new Uri("ms-appx:///Data/Network/CaliforniaNevada/RuntimeCANV.geodatabase");
                //StorageFile databasePath = await StorageFile.GetFileFromApplicationUriAsync(appResourceUri);
                //LocalRouteTask lrt = new LocalRouteTask(databasePath.Path, "RuntimeCANV");                
                routeParams.Stops = new FeaturesAsFeature(stops);
                RouteResult result = await _routeTask.SolveAsync(routeParams);
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
    }
}
