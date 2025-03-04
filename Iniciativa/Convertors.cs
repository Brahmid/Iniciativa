using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace Iniciativa
{
    public class VisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 1) return Visibility.Collapsed;

            bool isHidden = values[0] is bool hide && hide;            

            if (isHidden)
            {
                return Visibility.Collapsed;  // Skrytí, pokud je `Hide == true`
            }

            return Visibility.Visible; // Jinak zůstane viditelná
        }
               

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Brushes.White;

            bool isHidden = values[0] is bool hide && hide;
            int index = values[1] is int alternationIndex ? alternationIndex : -1;

            if (isHidden)
                return Brushes.Gray; // Pokud je Hide == true, barva je šedá
            if (index == 0)
                return Brushes.LightGreen; // Pokud je první položka, barva je zelená

            return Brushes.White; // Výchozí barva
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NameWithAvatarNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var name = values[0] as string;
            var avatarName = values[1] as string;
            switch (MainWindow.NameDisplayOpt)
            {
                case NameDisplay.Name:
                    return $"{name}";
                    
                case NameDisplay.IdAndName:
                    return $"{name} ({avatarName})";
                    
                case NameDisplay.None:
                    return "";
                default:
                    return "";                
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NameWithAvatarNameVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var name = values[0] as string;           
            switch (MainWindow.NameDisplayOpt)
            {
                case NameDisplay.Name:
                    if(string.IsNullOrEmpty(name) == true)
                    {
                        return Visibility.Hidden;
                    }                    
                    return Visibility.Visible;                   
                   

                case NameDisplay.IdAndName:
                    return Visibility.Visible;

                case NameDisplay.None:
                    return Visibility.Hidden;
                default:
                    return Visibility.Hidden;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class WidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return 100;

            bool isHidden = values[0] is bool hide && hide;
            double width = values[1] is double actualWidth ? actualWidth : 100;

            if (isHidden == true)
                return 0;

            return width; // Defaultní hodnota
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
