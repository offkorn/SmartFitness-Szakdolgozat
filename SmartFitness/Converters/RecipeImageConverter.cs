using System.Globalization;

namespace SmartFitness.Converters 
{
    public class RecipeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var url = value as string;

          
            if (string.IsNullOrWhiteSpace(url))
            {
             
                return "default_food_icon.svg";
            }

         
            return url;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class RecipeImageScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var url = value as string;

            // Ha a string üres VAGY null -> Default ikon van 
            if (string.IsNullOrWhiteSpace(url))
            {
                return 0.5;
            }

            // Ha van rendes kép URL -> Legyen normál méretű
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
    

}