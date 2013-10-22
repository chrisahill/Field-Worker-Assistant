using FieldWorkerAssistant.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FieldWorkerAssistant.ViewModel
{
    /// <summary>
    /// Encapsulates an a service item and adds properties for binding to a view
    /// </summary>
    public class ServiceItemViewModel : INotifyPropertyChanged
    {
        public ServiceItemViewModel(ServiceItem serviceItem)
        {
            Service = serviceItem;
        }

        public ServiceItem Service { get; private set; }

        private bool m_included;
        public bool Included 
        {
            get { return m_included; }
            set
            {
                if (m_included != value)
                {
                    m_included = value;
                    OnPropertyChanged();
                }
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
