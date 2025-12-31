using System.Globalization;

namespace BlazorSeguridad2026.Base.Cultures
{
    public static class SupportedCultures
    {
        public const string es_ES = "es-ES";
        public const string it_IT = "it-IT";
        public const string en_US = "en-US";

        public static readonly CultureInfo es_ES_Culture = new(es_ES);
        public static readonly CultureInfo it_IT_Culture = new(it_IT);
        public static readonly CultureInfo en_US_Culture = new(en_US);

        public static readonly CultureInfo DefaultCulture = es_ES_Culture;

        public static readonly List<Tuple<string,string>> CultureCodes = new()
        {
            new Tuple<string,string>(es_ES, "Español (España)"),
            new Tuple<string,string>(it_IT, "Italiano (Italia)"),
            new Tuple<string,string>(en_US, "Inglés (Estados Unidos)")
        };
    
        public static readonly CultureInfo[] Cultures =
        [
            es_ES_Culture,
            it_IT_Culture,
            en_US_Culture
        ];
    }
}
