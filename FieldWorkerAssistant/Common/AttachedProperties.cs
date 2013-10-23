using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FieldWorkerAssistant.Common
{
    public class AttachedProperties : DependencyObject
    {
        private static Dictionary<IList, ListBox> m_sourceToListBoxMap = 
            new Dictionary<IList, ListBox>();
 
        public static readonly DependencyProperty SelectedItemsSourceProperty = DependencyProperty.RegisterAttached(
          "SelectedItemsSource",
          typeof(IList),
          typeof(AttachedProperties),
          new PropertyMetadata(null, OnSelectedItemsSourcePropertyChanged)
        );

        public static void SetSelectedItemsSource(ListBox listBox, IList value)
        {
            listBox.SetValue(SelectedItemsSourceProperty, value);
        }

        private static void OnSelectedItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = (ListBox)d;
            var value = (IList)e.NewValue;

            // Hook to SelectionChanged
            listBox.SelectionChanged -= listBox_SelectionChanged;
            listBox.SelectionChanged += listBox_SelectionChanged;

            // Hook to collection changed on source list (if available)
            if (value is INotifyCollectionChanged)
            {
                var collection = (INotifyCollectionChanged)value;
                collection.CollectionChanged -= source_CollectionChanged;
                collection.CollectionChanged += source_CollectionChanged;
            }

            // store association of source with listbox
            m_sourceToListBoxMap[value] = listBox;
        }

        // Sync source collection with listbox's SelectedItems
        static void source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var source = (IList)sender;

            // Get associated ListBox
            ListBox listBox = m_sourceToListBoxMap[source];
            if (listBox == null)
                return;

            if (e.NewItems != null)
            {
                if (listBox.SelectionMode == SelectionMode.Extended ||
                    listBox.SelectionMode == SelectionMode.Multiple)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (!listBox.SelectedItems.Contains(item))
                            listBox.SelectedItems.Add(item);
                    }
                }
                else
                {
                    listBox.SelectedItem = e.NewItems[0];
                }
            }

            if (e.OldItems != null)
            {
                if (listBox.SelectionMode == SelectionMode.Extended ||
                    listBox.SelectionMode == SelectionMode.Multiple)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (listBox.SelectedItems.Contains(item))
                            listBox.SelectedItems.Remove(item);
                    }
                }
                //else if (e.NewItems == null)
                //{
                //    listBox.SelectedItem = null;
                //}
            }
        }

        // Sync with SelectedItemsSource on selection changed
        static void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            IList itemsSource = AttachedProperties.GetSelectedItemsSource(listBox);

            try
            {
                foreach (var item in e.AddedItems)
                    itemsSource.Add(item);

                foreach (var item in e.RemovedItems)
                    itemsSource.Remove(item);
            }
            catch { }
        }

        public static IList GetSelectedItemsSource(ListBox listBox)
        {
            return listBox.GetValue(SelectedItemsSourceProperty) as IList;
        }
    }
}
