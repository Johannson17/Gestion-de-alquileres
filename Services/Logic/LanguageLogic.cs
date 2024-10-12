using Services.Dao.Contracts;
using Services.Factory;
using System;
using System.Collections.Generic;

namespace Services.Logic
{
    public static class LanguageLogic
    {
        private static Dictionary<string, string> translationsCache = new Dictionary<string, string>();

        /// <summary>
        /// Traduce una clave especificada.
        /// </summary>
        /// <param name="key">Clave a traducir.</param>
        /// <returns>Texto traducido.</returns>
        public static string Translate(string key)
        {
            if (translationsCache.ContainsKey(key))
            {
                return translationsCache[key];
            }

            throw new KeyNotFoundException($"La clave '{key}' no existe en las traducciones cargadas.");
        }

        /// <summary>
        /// Guarda una traducción para una clave específica y genera un nuevo archivo de idioma.
        /// </summary>
        /// <param name="key">Clave de traducción.</param>
        /// <param name="translation">Traducción.</param>
        /// <param name="newLanguageFile">El nombre del nuevo archivo de idioma a crear.</param>
        public static void SaveTranslation(string key, string translation, string newLanguageFile)
        {
            translationsCache[key] = translation;

            // Llama al DAO para persistir los cambios en un nuevo archivo de idioma
            var languageRepository = FactoryDao.CreateRepository<ILanguageRepository>();
            languageRepository.SaveTranslation(key, translation, newLanguageFile);
        }

        /// <summary>
        /// Agrega una nueva traducción a un archivo de idioma.
        /// </summary>
        /// <param name="language">El idioma al que pertenece la clave.</param>
        /// <param name="key">Clave a agregar.</param>
        /// <param name="value">Valor de la clave a agregar.</param>
        public static void AddTranslation(string language, string key, string value)
        {
            if (!translationsCache.ContainsKey(key))
            {
                translationsCache[key] = value;

                // Llama al DAO para agregar la traducción en el archivo
                var languageRepository = FactoryDao.CreateRepository<ILanguageRepository>();
                languageRepository.AddTranslation(language, key, value);
            }
            else
            {
                throw new InvalidOperationException($"La clave '{key}' ya existe en el idioma '{language}'.");
            }
        }

        /// <summary>
        /// Guarda las traducciones modificadas en un archivo de idioma existente.
        /// </summary>
        public static void SaveTranslations(Dictionary<string, string> translations, string languageFile)
        {
            var languageRepository = FactoryDao.CreateRepository<ILanguageRepository>();
            foreach (var translation in translations)
            {
                languageRepository.SaveTranslation(translation.Key, translation.Value, languageFile);
            }
        }

        /// <summary>
        /// Guarda las traducciones en un nuevo archivo de idioma.
        /// </summary>
        public static void SaveTranslationsToNewFile(Dictionary<string, string> translations, string newLanguageFile)
        {
            var languageRepository = FactoryDao.CreateRepository<ILanguageRepository>();
            foreach (var translation in translations)
            {
                languageRepository.SaveTranslation(translation.Key, translation.Value, newLanguageFile);
            }
        }

        /// <summary>
        /// Recarga las traducciones de un archivo de idioma en la caché.
        /// </summary>
        public static void ReloadLanguages(string language)
        {
            translationsCache.Clear();

            // Llama al DAO para cargar las traducciones del archivo especificado
            var languageRepository = FactoryDao.CreateRepository<ILanguageRepository>();
            var translations = languageRepository.LoadAllTranslations(language);

            foreach (var translation in translations)
            {
                translationsCache[translation.Key] = translation.Value;
            }
        }

        /// <summary>
        /// Carga todas las traducciones para un idioma específico.
        /// </summary>
        /// <param name="language">Idioma para cargar las traducciones.</param>
        /// <returns>Diccionario con las traducciones cargadas.</returns>
        public static Dictionary<string, string> LoadAllTranslations(string language)
        {
            // Llama al DAO para cargar todas las traducciones de un archivo de idioma
            var languageRepository = FactoryDao.CreateRepository<ILanguageRepository>();
            return languageRepository.LoadAllTranslations(language);
        }

        /// <summary>
        /// Obtiene una lista de todas las claves de idioma disponibles en la caché.
        /// </summary>
        /// <returns>Lista de claves de traducciones en la caché.</returns>
        public static List<string> GetLanguages()
        {
            return new List<string>(translationsCache.Keys);
        }
    }
}