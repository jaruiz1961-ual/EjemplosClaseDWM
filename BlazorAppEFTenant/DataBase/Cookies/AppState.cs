using DataBase.Genericos;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace DataBase.Genericos
{
    [TypeConverter(typeof(AppState))]
    public class AppState 
    {
        public int? TenantId { get; set; } = 0;
        public string DbKey { get; set; }

        public string ConnectionMode { get; set; }
        public string ApiName { get; set; }
        public Uri DirBase { get; set; }

    }
    
    public class AppStateConverter : TypeConverter
    {
        // Permite convertir desde string (JSON)
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        // Convierte un string JSON en ClaseDato
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string s)
            {
                return JsonSerializer.Deserialize<AppState>(s);
            }
            return base.ConvertFrom(context, culture, value);
        }

        // Permite convertir a string (JSON)
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        // Convierte una ClaseDato en string JSON
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is AppState dato)
            {
                return JsonSerializer.Serialize(dato);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
