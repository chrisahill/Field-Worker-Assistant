using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using FieldWorkerAssistant.Model;
using FieldWorkerAssitant.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        }

        void AllServiceItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
