using FieldWorkerAssistant.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace FieldWorkerAssistant.Converters
{
    public class PriorityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is string))
                return null;

            var priority = ((string)value).ToLower();
            if (priority == Constants.HIGH_PRIORITY.ToLower())
                return new SolidColorBrush(Colors.Red);
            else if (priority == Constants.NORMAL_PRIORITY.ToLower())
                return new SolidColorBrush(Colors.Orange);
            else if (priority == Constants.LOW_PRIORITY.ToLower())
                return new SolidColorBrush(Colors.Blue);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!(value is SolidColorBrush))
                return null;

            Color color = ((SolidColorBrush)value).Color;
            if (color == Colors.Red)
                return Constants.HIGH_PRIORITY;
            else if (color == Colors.Orange)
                return Constants.NORMAL_PRIORITY;
            else if (color == Colors.Blue)
                return Constants.LOW_PRIORITY;

            return null;
        }
    }
}
