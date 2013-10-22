using FieldWorkerAssistant.Model;
using FieldWorkerAssitant.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
