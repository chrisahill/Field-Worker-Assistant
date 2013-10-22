using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace FieldWorkerAssistant.Commands
{
    /// <summary>
    /// Selects all the items in a ListBox
    /// </summary>
    class ClearSelectedCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return parameter is ListBox;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                throw new Exception("Command is not in an executable state");

            var listBox = ((ListBox)parameter);
            foreach (var item in listBox.Items)
            {
                if (listBox.SelectedItems.Contains(item))
                    listBox.SelectedItems.Remove(item);
            }
        }
    }
}
