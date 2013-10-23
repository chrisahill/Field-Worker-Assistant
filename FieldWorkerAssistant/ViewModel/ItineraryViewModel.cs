
﻿using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Offline;
using FieldWorkerAssistant.Model;
using FieldWorkerAssitant.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace FieldWorkerAssistant.ViewModel
{
    internal class ItineraryViewModel : INotifyPropertyChanged
    {
        public ItineraryViewModel()
        {
            //int[] ids = new int[] { 1001, 1002, 1003, 1004, 1005 };
            //string[] priorities = new string[] { "High", "Normal", "Low", "Normal", "Normal" };
            //string[] problemDescriptions = new string[] { 
            //    "This is a really big problem need someone on it right away.", 
            //    "This is a normal issue fix when you can.", 
            //    "This is low priority get to it when have nothing to do.", 
            //    "This is a normal issue fix when you can.", 
            //    "This is a normal issue fix when you can." };
            //MapPoint[] coords = new MapPoint[] {
            //    new MapPoint(-13046000, 4037000, SpatialReferences.WebMercator),
            //    new MapPoint(-13045000, 4036000, SpatialReferences.WebMercator),
            //    new MapPoint(-13044000, 4035000, SpatialReferences.WebMercator),
            //    new MapPoint(-13044500, 4034800, SpatialReferences.WebMercator),
            //    new MapPoint(-13045200, 4035800, SpatialReferences.WebMercator) };
            AllServiceItems = new ObservableCollection<ServiceItemViewModel>();

            //for (int i = 0; i < ids.Length; i++)
            //{
            //    Graphic g = new Graphic();
            //    g.Attributes["ServiceRequestID"] = ids[i];
            //    g.Attributes["Severity"] = priorities[i];
            //    g.Attributes["ProblemDescription"] = problemDescriptions[i];
            //    g.Geometry = coords[i];
            //    var viewModel = new ServiceItemViewModel(new ServiceItem(g));
            //    viewModel.PropertyChanged += item_PropertyChanged;
            //    AllServiceItems.Add(viewModel);
            //}

            AllServiceItems.CollectionChanged += AllServiceItems_CollectionChanged;

            IncludedServiceItems = new ObservableCollection<ServiceItemViewModel>();
            IncludedServiceItems.CollectionChanged += IncludedServiceItems_CollectionChanged;

            IncludeAllCommand = new DelegateCommand(includeAll, canIncludeAll);
            ExcludeAllCommand = new DelegateCommand(excludeAll, canExcludeAll);            
            DownloadCommand = new DelegateCommand(download, canDownload);
        }

        void AllServiceItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            raiseCanExecuteChanged();
            OnPropertyChanged("AllServiceItems");
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

            OnPropertyChanged("IncludedServiceItems");
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
        
        public ICommand DownloadCommand { get; private set; }

        private bool m_downloading;
        public bool IsDownloading
        {
            get { return m_downloading; }
            private set
            {
                if (m_downloading != value)
                {
                    m_downloading = value;
                    OnPropertyChanged();
                    raiseCanExecuteChanged();
                }
            }
        }

        public void InitializeServiceItems(IEnumerable<Feature> features)
        {
            if (ExcludeAllCommand.CanExecute(null))
                ExcludeAllCommand.Execute(null);

            ServiceItemViewModel[] allItemsCopy = AllServiceItems.ToArray();
            foreach (var item in allItemsCopy)
                AllServiceItems.Remove(item);

            foreach (var feature in features)
            {
                var serviceItem = new ServiceItem(feature);
                var viewModel = new ServiceItemViewModel(serviceItem);
                AllServiceItems.Add(viewModel);
            }
        }

        private bool canIncludeAll(object parameter)
        {
            var anyExcludedItems = AllServiceItems.Any(item => !IncludedServiceItems.Contains(item));
            return !IsDownloading && anyExcludedItems;
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
            return !IsDownloading && IncludedServiceItems.Count > 0;
        }

        private void excludeAll(object parameter)
        {
            var includedItemsCopy = IncludedServiceItems.ToArray();
            foreach (var item in includedItemsCopy)
                IncludedServiceItems.Remove(item);
        }
        
        private const string FeatureServiceUri = "http://services.arcgis.com/pmcEyn9tLWCoX7Dm/arcgis/rest/services/HackathonSR/FeatureServer";
        private Envelope MapExtent = new Envelope(-13046907.1247363, 4034314.00501996, -13042604.5495666, 4038309.25339177,
                new SpatialReference(3857));

        private bool canDownload(object parameter)
        {
            return !IsDownloading && IncludedServiceItems.Count > 0;
        }

        private async void download(object parameter)
        {
            IsDownloading = true;

            var task = new GeodatabaseTask(new Uri(FeatureServiceUri));
            var layerQueries = new Dictionary<int, LayerQuery>();
            var ids = from item in IncludedServiceItems select (long)item.Service.OBJECTID;
            layerQueries[0] = new LayerQuery() { Where = string.Format("OBJECTID IN ({0})", string.Join(",", ids)) };
            var jobresult = await task.SubmitGenerateGeodatabaseJobAsync(
             new GenerateGeodatabaseParameters(new int[] { 0}, MapExtent.Expand(5)) { SyncModel = SyncModel.PerLayer, LayerQueries = layerQueries },
             async (status, error) => //complete callback
             {
                 HttpClient client = new HttpClient();
                 var result = await client.GetStreamAsync(status.ResultUri);
                 App app = (App)App.Current;
                 var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Replica.geodatabase", CreationCollisionOption.ReplaceExisting);
                 using (var stream = await file.OpenStreamForWriteAsync())
                 {
                     result.CopyTo(stream);
                 }
                 await CreateCachedFeatureLayer(file);

                 IsDownloading = false;

             }, TimeSpan.FromSeconds(2), CancellationToken.None
             //,
             //(status) => //status updates
             //{

             //}
             );
        }

        internal async Task CreateCachedFeatureLayer(StorageFile file)
        {
            var fileCopy = await file.CopyAsync(ApplicationData.Current.LocalFolder,
                System.Guid.NewGuid().ToString() + ".geodatabase");
            var geodatabasePath = fileCopy.Path;
            var cache = await Geodatabase.OpenAsync(geodatabasePath);
            foreach (var source in cache.FeatureTables)
            {
                App app = (App)App.Current;
                CachedFeatureLayer = new ArcGISFeatureLayer(source) { ID = source.Name , Renderer = app.WorkItemsRenderer};
                QueryFilter filter = new QueryFilter() { WhereClause = "1=1" };
                var gdbFeatures = await CachedFeatureLayer.FeatureTable.QueryAsync(filter);
                
                app.RouteViewModel.CachedFeatureLayer = CachedFeatureLayer;
                app.RouteViewModel.GdbFile = file;
                app.RouteViewModel.InitializeServiceItems(gdbFeatures);
                break;
            }
        }

        private ArcGISFeatureLayer m_CachedFeatureLayer;
        public ArcGISFeatureLayer CachedFeatureLayer
        {
            get
            {
                return m_CachedFeatureLayer;
            }
            private set
            {
                if (m_CachedFeatureLayer != value)
                {
                    m_CachedFeatureLayer = value;                    
                 
                    OnPropertyChanged();
                }
            }
        }

        private void raiseCanExecuteChanged()
        {
            ((DelegateCommand)IncludeAllCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ExcludeAllCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DownloadCommand).RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
