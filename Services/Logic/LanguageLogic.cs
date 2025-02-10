using Services.Facade;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace Services.Logic
{
    public static class LanguageLogic
    {
        private static readonly string DefaultLanguage = "es"; // Idioma predeterminado (Español)
        private static string currentLanguage = DefaultLanguage; // Idioma actualmente seleccionado
        private static readonly string googleTranslateApiUrl = "https://translate.googleapis.com/translate_a/single";

        private static readonly Dictionary<string, string> languageMap = new Dictionary<string, string>
        {
            { "Español", "es" },
            { "English", "en" },
            { "Français", "fr" },
            { "Deutsch", "de" },
            { "Italiano", "it" },
            { "Português", "pt" },
            { "Русский", "ru" },
            { "中文", "zh" },
            { "日本語", "ja" },
            { "한국어", "ko" },
            { "العربية", "ar" },
            { "हिन्दी", "hi" },
            { "עברית", "he" },
            { "Türkçe", "tr" },
            { "Svenska", "sv" },
            { "Nederlands", "nl" },
            { "Polski", "pl" },
            { "Українська", "uk" },
            { "ไทย", "th" },
            { "Tiếng Việt", "vi" }
        };

        public static Dictionary<string, string> GetLanguageMap() => new Dictionary<string, string>(languageMap);

        public static List<string> GetAvailableLanguages() => new List<string>(languageMap.Keys);

        public static string Translate(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("La clave no puede ser nula o vacía.", nameof(key));
            }

            // Traducir el texto
            return TranslateWithGoogle(key, currentLanguage);
        }

        public static void SetCurrentLanguage(string language)
        {
            ValidateParameter(language, nameof(language));

            if (!languageMap.ContainsValue(language) && !languageMap.ContainsKey(language))
            {
                throw new InvalidOperationException($"El idioma '{language}' no es válido.");
            }

            currentLanguage = languageMap.ContainsKey(language) ? languageMap[language] : language;

            try
            {
                CultureInfo newCulture = new CultureInfo(currentLanguage);
                CultureInfo.DefaultThreadCurrentCulture = newCulture;
                CultureInfo.DefaultThreadCurrentUICulture = newCulture;
            }
            catch (CultureNotFoundException ex)
            {
                throw new InvalidOperationException($"El idioma '{currentLanguage}' no es válido en el sistema.", ex);
            }
        }

        public static string GetCurrentLanguage()
        {
            foreach (var pair in languageMap)
            {
                if (pair.Value == currentLanguage)
                {
                    return pair.Key;
                }
            }

            return DefaultLanguage;
        }

        private static string TranslateWithGoogle(string text, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text; // No traducir textos vacíos o nulos
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"{googleTranslateApiUrl}?client=gtx&sl=auto&tl={targetLanguage}&dt=t&q={Uri.EscapeDataString(text)}";
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();

                    string result = response.Content.ReadAsStringAsync().Result;
                    return ParseGoogleTranslateResponse(result);
                }
                catch
                {
                    return text; // Retorna el texto original si hay un error
                }
            }
        }

        private static string ParseGoogleTranslateResponse(string response)
        {
            try
            {
                int startIndex = response.IndexOf("\"") + 1;
                int endIndex = response.IndexOf("\"", startIndex);
                return response.Substring(startIndex, endIndex - startIndex);
            }
            catch
            {
                return "Error en la traducción"; // Mensaje de error claro
            }
        }

        private static void ValidateParameter(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentException($"El parámetro '{parameterName}' no puede ser nulo o estar vacío.", parameterName);
            }
        }
    }
}
