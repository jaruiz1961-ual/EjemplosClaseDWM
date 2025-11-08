using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace PasoParametrosBlazor.Client
{
    public class AppState
    {
        private string _valor;
        public string ValorCompartido
        {
            get => _valor;
            set
            {
                if (_valor != value)
                {
                    _valor = value;
                    OnChange?.Invoke();
                }
            }
        }

        private ClaseDato _datoCascada = new ClaseDato();
        public ClaseDato DatoCascada
        {
            get => _datoCascada;
            set
            {
                if (_datoCascada != value)
                {
                    _datoCascada = value;
                    OnChange?.Invoke();
                }
            }
        }

        // Evento para avisar a los componentes cuando cambia el valor
        public event Action? OnChange;
    }
    
    [TypeConverter(typeof(ClaseDatoConverter))]
    public class ClaseDato
    {
        public string Valor1 { get; set; }
        public string Valor2 { get; set; }
        public string Valor3 { get; set; }
    }
    public class ClaseDatoConverter : TypeConverter
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
                return JsonSerializer.Deserialize<ClaseDato>(s);
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
            if (destinationType == typeof(string) && value is ClaseDato dato)
            {
                return JsonSerializer.Serialize(dato);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
