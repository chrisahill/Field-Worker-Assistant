using FieldWorkerAssistant.Model;
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

namespace FieldWorkerAssistant.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PlanRoute : FieldWorkerAssistant.Common.LayoutAwarePage
    {
        public class MockData
        {
            public List<ServiceItem> OnlineServiceItems
            {
                get 
                { 
                    return new List<ServiceItem>
                    {
                        new ServiceItem{ ServiceRequestID = 1001, Severity = "High", ProblemDescription=" This is a really big problem need someone on it right away."},
                        new ServiceItem{ ServiceRequestID = 1002, Severity = "Normal", ProblemDescription=" This is a normal issue fix when you can."},
                        new ServiceItem{ ServiceRequestID = 1003, Severity = "Low", ProblemDescription=" This is low priority get to it when have nothing to do."},
                        new ServiceItem{ ServiceRequestID = 1004, Severity = "Normal", ProblemDescription=" This is a normal issue fix when you can."},
                        new ServiceItem{ ServiceRequestID = 1005, Severity = "Normal", ProblemDescription=" This is a normal issue fix when you can."},
                    };
                }             
             }
        }

        public PlanRoute()
        {
            this.InitializeComponent();
            this.DataContext = new MockData();
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
    }
}
