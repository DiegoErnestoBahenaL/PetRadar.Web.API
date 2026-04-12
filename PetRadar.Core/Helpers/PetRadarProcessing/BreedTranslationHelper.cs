using PetRadar.Core.Data.Entities.Enums;
using System;
using System.Collections.Generic;

namespace PetRadar.Core.Helpers.PetRadarProcessing
{
    /// <summary>
    /// Provides translation of breed names returned by the PetRadarProcessing API
    /// (EfficientNet cat/dog classifiers) from English into Spanish, using the
    /// terminology most commonly recognized in Mexico.
    /// </summary>
    public static class BreedTranslationHelper
    {
        // Keys match the labels in the cat model's class_names.json exactly.
        private static readonly Dictionary<string, string> CatBreedTranslations = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Abyssinian",         "Abisinio" },
            { "American Bobtail",   "Bobtail Americano" },
            { "American Curl",      "American Curl" },
            { "American Shorthair", "Americano de Pelo Corto" },
            { "Bengal",             "Bengalí" },
            { "Birman",             "Sagrado de Birmania" },
            { "Bombay",             "Bombay" },
            { "British Shorthair",  "Británico de Pelo Corto" },
            { "Egyptian Mau",       "Mau Egipcio" },
            { "Exotic Shorthair",   "Exótico de Pelo Corto" },
            { "Maine Coon",         "Maine Coon" },
            { "Manx",               "Manx" },
            { "Norwegian Forest",   "Bosque de Noruega" },
            { "Persian",            "Persa" },
            { "Ragdoll",            "Ragdoll" },
            { "Russian Blue",       "Azul Ruso" },
            { "Scottish Fold",      "Scottish Fold" },
            { "Siamese",            "Siamés" },
            { "Sphynx",             "Esfinge" },
            { "Turkish Angora",     "Angora Turco" },
        };

        // Keys match the labels in the dog model's class_names.json exactly
        // (lowercase with underscores, as exported by the Stanford Dogs-style dataset).
        private static readonly Dictionary<string, string> DogBreedTranslations = new(StringComparer.OrdinalIgnoreCase)
        {
            { "affenpinscher",                  "Affenpinscher" },
            { "afghan_hound",                   "Galgo Afgano" },
            { "african_hunting_dog",            "Perro Salvaje Africano" },
            { "airedale",                       "Airedale Terrier" },
            { "american_staffordshire_terrier", "American Staffordshire Terrier" },
            { "appenzeller",                    "Boyero de Appenzell" },
            { "australian_terrier",             "Terrier Australiano" },
            { "basenji",                        "Basenji" },
            { "basset",                         "Basset Hound" },
            { "beagle",                         "Beagle" },
            { "bedlington_terrier",             "Bedlington Terrier" },
            { "bernese_mountain_dog",           "Boyero de Berna" },
            { "black-and-tan_coonhound",        "Coonhound Negro y Fuego" },
            { "blenheim_spaniel",               "Cavalier King Charles Blenheim" },
            { "bloodhound",                     "Sabueso" },
            { "bluetick",                       "Coonhound Bluetick" },
            { "border_collie",                  "Border Collie" },
            { "border_terrier",                 "Border Terrier" },
            { "borzoi",                         "Galgo Ruso" },
            { "boston_bull",                    "Boston Terrier" },
            { "bouvier_des_flandres",           "Bouvier de Flandes" },
            { "boxer",                          "Bóxer" },
            { "brabancon_griffon",              "Grifón de Bruselas" },
            { "briard",                         "Briard" },
            { "brittany_spaniel",               "Spaniel Bretón" },
            { "bull_mastiff",                   "Bullmastiff" },
            { "cairn",                          "Cairn Terrier" },
            { "cardigan",                       "Welsh Corgi Cardigan" },
            { "chesapeake_bay_retriever",       "Retriever de la Bahía de Chesapeake" },
            { "chihuahua",                      "Chihuahueño" },
            { "chow",                           "Chow Chow" },
            { "clumber",                        "Clumber Spaniel" },
            { "cocker_spaniel",                 "Cocker Spaniel" },
            { "collie",                         "Collie" },
            { "curly-coated_retriever",         "Retriever de Pelo Rizado" },
            { "dandie_dinmont",                 "Dandie Dinmont Terrier" },
            { "dhole",                          "Dhole" },
            { "dingo",                          "Dingo" },
            { "doberman",                       "Dóberman" },
            { "english_foxhound",               "Foxhound Inglés" },
            { "english_setter",                 "Setter Inglés" },
            { "english_springer",               "Springer Spaniel Inglés" },
            { "entlebucher",                    "Boyero de Entlebuch" },
            { "eskimo_dog",                     "Perro Esquimal Americano" },
            { "flat-coated_retriever",          "Retriever de Pelo Liso" },
            { "french_bulldog",                 "Bulldog Francés" },
            { "german_shepherd",                "Pastor Alemán" },
            { "german_short-haired_pointer",    "Braco Alemán de Pelo Corto" },
            { "giant_schnauzer",                "Schnauzer Gigante" },
            { "golden_retriever",               "Golden Retriever" },
            { "gordon_setter",                  "Setter Gordon" },
            { "great_dane",                     "Gran Danés" },
            { "great_pyrenees",                 "Gran Pirineo" },
            { "greater_swiss_mountain_dog",     "Gran Boyero Suizo" },
            { "groenendael",                    "Pastor Belga Groenendael" },
            { "ibizan_hound",                   "Podenco Ibicenco" },
            { "irish_setter",                   "Setter Irlandés" },
            { "irish_terrier",                  "Terrier Irlandés" },
            { "irish_water_spaniel",            "Spaniel de Agua Irlandés" },
            { "irish_wolfhound",                "Lobero Irlandés" },
            { "italian_greyhound",              "Galgo Italiano" },
            { "japanese_spaniel",               "Chin Japonés" },
            { "keeshond",                       "Keeshond" },
            { "kelpie",                         "Kelpie Australiano" },
            { "kerry_blue_terrier",             "Kerry Blue Terrier" },
            { "komondor",                       "Komondor" },
            { "kuvasz",                         "Kuvasz" },
            { "labrador_retriever",             "Labrador Retriever" },
            { "lakeland_terrier",               "Lakeland Terrier" },
            { "leonberg",                       "Leonberger" },
            { "lhasa",                          "Lhasa Apso" },
            { "malamute",                       "Malamute de Alaska" },
            { "malinois",                       "Pastor Belga Malinois" },
            { "maltese_dog",                    "Bichón Maltés" },
            { "mexican_hairless",               "Xoloitzcuintle" },
            { "miniature_pinscher",             "Pinscher Miniatura" },
            { "miniature_poodle",               "Caniche Miniatura" },
            { "miniature_schnauzer",            "Schnauzer Miniatura" },
            { "newfoundland",                   "Terranova" },
            { "norfolk_terrier",                "Norfolk Terrier" },
            { "norwegian_elkhound",             "Elkhound Noruego" },
            { "norwich_terrier",                "Norwich Terrier" },
            { "old_english_sheepdog",           "Viejo Pastor Inglés" },
            { "otterhound",                     "Otterhound" },
            { "papillon",                       "Papillón" },
            { "pekinese",                       "Pekinés" },
            { "pembroke",                       "Welsh Corgi Pembroke" },
            { "pomeranian",                     "Pomerania" },
            { "pug",                            "Pug" },
            { "redbone",                        "Coonhound Redbone" },
            { "rhodesian_ridgeback",            "Rhodesian Ridgeback" },
            { "rottweiler",                     "Rottweiler" },
            { "saint_bernard",                  "San Bernardo" },
            { "saluki",                         "Saluki" },
            { "samoyed",                        "Samoyedo" },
            { "schipperke",                     "Schipperke" },
            { "scotch_terrier",                 "Terrier Escocés" },
            { "scottish_deerhound",             "Lebrel Escocés" },
            { "sealyham_terrier",               "Sealyham Terrier" },
            { "shetland_sheepdog",              "Pastor de Shetland" },
            { "shih-tzu",                       "Shih Tzu" },
            { "siberian_husky",                 "Husky Siberiano" },
            { "silky_terrier",                  "Silky Terrier" },
            { "soft-coated_wheaten_terrier",    "Wheaten Terrier de Pelo Suave" },
            { "staffordshire_bullterrier",      "Staffordshire Bull Terrier" },
            { "standard_poodle",                "Caniche Estándar" },
            { "standard_schnauzer",             "Schnauzer Estándar" },
            { "sussex_spaniel",                 "Sussex Spaniel" },
            { "tibetan_mastiff",                "Mastín Tibetano" },
            { "tibetan_terrier",                "Terrier Tibetano" },
            { "toy_poodle",                     "Caniche Toy" },
            { "toy_terrier",                    "Toy Terrier Inglés" },
            { "vizsla",                         "Vizsla" },
            { "walker_hound",                   "Treeing Walker Coonhound" },
            { "weimaraner",                     "Weimaraner" },
            { "welsh_springer_spaniel",         "Welsh Springer Spaniel" },
            { "west_highland_white_terrier",    "West Highland White Terrier" },
            { "whippet",                        "Whippet" },
            { "wire-haired_fox_terrier",        "Fox Terrier de Pelo Duro" },
            { "yorkshire_terrier",              "Yorkshire Terrier" },
        };

        /// <summary>
        /// Translates a single English breed label (as emitted by the EfficientNet
        /// classifier) into its Spanish (Mexican context) equivalent. If the breed
        /// is unknown, the original label is returned in a human-readable form
        /// (underscores replaced by spaces, title-cased).
        /// </summary>
        public static string TranslateBreed(PetSpeciesEnum species, string englishBreed)
        {
            if (string.IsNullOrWhiteSpace(englishBreed))
                return englishBreed;

            var dictionary = species switch
            {
                PetSpeciesEnum.Cat => CatBreedTranslations,
                PetSpeciesEnum.Dog => DogBreedTranslations,
                _ => null
            };

            if (dictionary is null)
                return Humanize(englishBreed);

            if (dictionary.TryGetValue(englishBreed, out var translation))
                return translation;

            // Tolerate minor formatting differences between the API payload and the
            // dictionary keys (e.g. spaces vs underscores).
            var alternateKey = englishBreed.Contains('_')
                ? englishBreed.Replace('_', ' ')
                : englishBreed.Replace(' ', '_');

            if (dictionary.TryGetValue(alternateKey, out translation))
                return translation;

            return Humanize(englishBreed);
        }

        /// <summary>
        /// Translates every breed reference inside a <see cref="CharacteristicsResponse"/>
        /// in place (top predicted breed and every entry in TopPredictions) and
        /// returns the same instance for chaining.
        /// </summary>
        public static CharacteristicsResponse TranslateCharacteristicsResponse(PetSpeciesEnum species, CharacteristicsResponse response)
        {
            if (response is null)
                return response;

            response.TopPredictedBreed = TranslateBreed(species, response.TopPredictedBreed);

            if (response.TopPredictions is not null)
            {
                foreach (var prediction in response.TopPredictions)
                {
                    prediction.Breed = TranslateBreed(species, prediction.Breed);
                }
            }

            return response;
        }

        private static string Humanize(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return raw;

            var spaced = raw.Replace('_', ' ').Replace('-', ' ');
            return System.Globalization.CultureInfo
                .GetCultureInfo("es-MX")
                .TextInfo
                .ToTitleCase(spaced.ToLowerInvariant());
        }
    }
}