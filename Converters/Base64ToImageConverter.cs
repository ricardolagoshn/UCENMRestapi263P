using System.Globalization;

namespace UCENMRestapi263P.Converters
{
    public class Base64ToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string base64String || string.IsNullOrWhiteSpace(base64String))
            {
                return null;
            }

            try
            {
                // Quitar el prefijo "data:image/...;base64," si viene incluido
                int comma = base64String.IndexOf(',');
                if (base64String.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma >= 0)
                {
                    base64String = base64String[(comma + 1)..];
                }

                byte[] imageBytes = System.Convert.FromBase64String(base64String.Trim());

                // Se crea un stream nuevo cada vez que MAUI lo solicita.
                // (Con "using var ms" el stream se cerraba antes de que la imagen se cargara.)
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al convertir Base64 a imagen: {ex.Message}");
                return null;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
