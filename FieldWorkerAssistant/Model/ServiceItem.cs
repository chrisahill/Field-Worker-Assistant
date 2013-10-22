using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FieldWorkerAssistant.Model
{
    public class ServiceItem
    {
        private string[] fieldNames = new string[] 
        {
            "AssignedTo",
            "Type",
            "ProblemDescription",
            "Severity",
            "DateEntered",
            "DateResolved",
            "Status",
            "ActionTaken"
        };

        public ServiceItem(Feature feature)
        {
            //if (fieldNames.Any(name => !feature.Attributes.ContainsKey(name)))
            //    throw new Exception("Service Item is missing a required field");

            Feature = feature;
        }

        public int ServiceRequestID 
        {
            get { return getAttribute<int>(); }
            set { setAttribute(value); }
        }

        public string AssignedTo
        {
            get { return getAttribute<string>(); }
            set { setAttribute(value); }
        }

        public string Type
        {
            get { return getAttribute<string>(); }
            set { setAttribute(value); }
        }
        public string ProblemDescription
        {
            get { return getAttribute<string>(); }
            set { setAttribute(value); }
        }
        public string Severity
        {
            get { return getAttribute<string>(); }
            set { setAttribute(value); }
        }
        public string DateEntered
        {
            get { return getAttribute<string>(); }
            set { setAttribute(value); }
        }
        public string DateResolved
        {
            get { return getAttribute<string>(); }
            set { setAttribute(value); }
        }
        public string Status
        {
            get { return getAttribute<string>(); }
            set { setAttribute(value); }
        }
        public string ActionTaken
        {
            get { return getAttribute<string>(); }
            set { setAttribute(value); }
        }

        public Feature Feature { get; private set; }

        private T getAttribute<T>([CallerMemberName] string attributeName = null)
        {
            return (T)Feature.Attributes[attributeName];
        }

        private void setAttribute(object value, [CallerMemberName] string attributeName = null)
        {
            Feature.Attributes[attributeName] = value;
        }
    }
}
