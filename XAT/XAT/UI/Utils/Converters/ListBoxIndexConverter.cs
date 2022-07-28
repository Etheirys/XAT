using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace XAT.UI.Utils.Converters;

public class ListBoxIndexConverter : IValueConverter
{
    public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
    {
        int index = 0;

        ListBoxItem item = (ListBoxItem)value;
        ListBox? listView = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
        if(listView != null)
            index = listView.ItemContainerGenerator.IndexFromContainer(item);
        return index.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
