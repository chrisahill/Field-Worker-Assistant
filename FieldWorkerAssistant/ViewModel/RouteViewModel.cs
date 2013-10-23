using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Layers;
using FieldWorkerAssistant.Model;
using FieldWorkerAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FieldWorkerAssistant
{
    internal class RouteViewModel : INotifyPropertyChanged
    {

        public RouteViewModel()
        {
            RouteServiceItems = new ObservableCollection<ServiceItemViewModel>();
        }
        public ObservableCollection<ServiceItemViewModel> RouteServiceItems { get; internal set; }

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
    }
}
