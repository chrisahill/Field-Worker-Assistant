using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Layers;
using FieldWorkerAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace FieldWorkerAssistant.Converters
{
    public class ServiceItemsToFeatures : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<Graphic> features = new List<Graphic>();

            if ((value is IEnumerable<ServiceItemViewModel>))
            {
                var serviceItems = (IEnumerable<ServiceItemViewModel>)value;
                foreach (var item in serviceItems)
                {
                    if (item.Service.Feature is Graphic)
                    {
                        features.Add((Graphic)item.Service.Feature);
                    }
                    else if (item.Service.Feature is GdbFeature)
                    {
                        var g = ((GdbFeature)item.Service.Feature).AsGraphic();
                        if (g.Attributes.ContainsKey("GlobalID"))
                            g.Attributes.Remove("GlobalID");
                        features.Add(g);
                    }
                }
            }
            return features;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
