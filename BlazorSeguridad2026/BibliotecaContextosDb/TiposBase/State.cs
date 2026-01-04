using System.ComponentModel;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text.Json;

namespace BlazorSeguridad2026.Base.Seguridad
{

  
    [TypeConverter(typeof(State))]
    public class State 
    {
        public int? TenantId { get; set; } = null;
        public string DbKey { get; set; }

        public string ConnectionMode
        {
            get { return cm; }
            set
            {
                 cm = value;
                if (cm == "Api")
                    cm = "Api";
            }
        }

        private string cm;
        public string ApiName { get; set; }
        public Uri DirBase { get; set; }

        public string Token { get; set; }
        public string Status { get; set; }

        public string Culture { get; set; }
        public bool isValid { get
            {
                if (string.IsNullOrEmpty(TenantId.ToString())) return false;
                if (string.IsNullOrEmpty(DbKey)) return false;
                if (string.IsNullOrEmpty(ConnectionMode)) return true;
                if (ConnectionMode.ToLower() == "api" && string.IsNullOrEmpty(ApiName)) return false;
                if (ConnectionMode.ToLower() == "api" && string.IsNullOrEmpty(DirBase.ToString())) return false;
                return true;
            } }
        public bool IsAutenticated {get { return !string.IsNullOrEmpty(Token); } }

        public bool ApplyTenantFilter { get; set; } = true;

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
                return JsonSerializer.Deserialize<State>(s);
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
            if (destinationType == typeof(string) && value is State dato)
            {
                return JsonSerializer.Serialize(dato);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
