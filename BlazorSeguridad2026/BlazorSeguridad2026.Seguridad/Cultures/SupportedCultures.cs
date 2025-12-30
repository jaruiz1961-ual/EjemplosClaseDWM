using System.Globalization;

namespace BlazorSeguridad2026.Base.Cultures
{
    public static class SupportedCultures
    {
        public const string es_ES = "es-ES";
        public const string it_IT = "it-IT";

        public static readonly CultureInfo es_ES_Culture = new(es_ES);
        public static readonly CultureInfo it_IT_Culture = new(it_IT);

        public static readonly CultureInfo DefaultCulture = es_ES_Culture;

        public static readonly CultureInfo[] Cultures =
        [
            es_ES_Culture,
            it_IT_Culture
        ];
    }
}
