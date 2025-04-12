using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Services
{
    public partial class LocalString : BindableObject
    {
        public static LocalString Empty = new LocalString(new());
        private readonly Dictionary<string, string> _translations;

        public LocalString(Dictionary<string, string> translations)
        {
            _translations = translations;
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(Current));
        }

        public string Current
        {
            get
            {
                var lang = LanguageService.Instance.CurrentLanguage;
                if (_translations.TryGetValue(lang, out var value))
                    return value;

                // fallback to English or first available translation
                if (_translations.TryGetValue("en", out var fallback))
                    return fallback;

                return _translations.Values.FirstOrDefault() ?? string.Empty;
            }
        }
    }
    public sealed partial class LanguageService : BindableObject, Singleton<LanguageService>
    {
        static LanguageService()
        {
            var indexerProp = typeof(LanguageService)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .First(x => x.GetIndexParameters().Length != 0);
            IndexerName = indexerProp.Name;
        }
        public LanguageService()
        {
            SetLanguage(CultureInfo.CurrentUICulture);
        }

        private static string IndexerName;

        private static LanguageService? _instance;
        public static LanguageService Instance => _instance ??= new();

        // Current language code (e.g. "en", "hu")
        public string CurrentLanguage { get; private set; } = "en";
        public void SetLanguage(CultureInfo culture)
        {
            if (culture.Name.StartsWith("en-"))
            {
                CurrentLanguage = "en";
            }
            else
            {
                CurrentLanguage = culture.Name;
            }
            foreach (var v in _localizedStrings.Values)
            {
                v.Refresh();
            }
            OnPropertyChanged(nameof(CurrentLanguage));
        }

        public void Add(string key, Dictionary<string, string> translations)
        {
            _localizedStrings[key] = new LocalString(translations);
            OnPropertyChanged(IndexerName);
        }

        public string Get(string key)
        {
            if (_localizedStrings.TryGetValue(key, out var localString))
                return localString.Current;
            return key; // fallback to key
        }

        public LocalString this[string key] => _localizedStrings.TryGetValue(key, out var localString) ? localString : LocalString.Empty;


        // Language data: key -> LocalString
        private readonly Dictionary<string, LocalString> _localizedStrings = new()
        {
            ["Title"] = new LocalString(new()
            {
                ["hu-HU"] = "Gyorsétterem",
                ["en"] = "Fast Food"
            }),
            ["SearchText"] = new LocalString(new()
            {
                ["hu-HU"] = "Keress ételeink és italaink között...",
                ["en"] = "Search our foods and drinks..."
            }),
            ["Highlights"] = new LocalString(new()
            {
                ["hu-HU"] = "Kiemelt Termékeink",
                ["en"] = "Our Highlights"
            }),
            ["Ingredients"] = new LocalString(new()
            {
                ["hu-HU"] = "Hozzávalók:",
                ["en"] = "Ingredients:"
            }),
            ["Description"] = new LocalString(new()
            {
                ["hu-HU"] = "Leírás",
                ["en"] = "Description"
            }),
            ["ToCart"] = new LocalString(new()
            {
                ["hu-HU"] = "Kosárba",
                ["en"] = "Add"
            }),
            ["OrderDetails"] = new LocalString(new()
            {
                ["hu-HU"] = "Részletek->",
                ["en"] = "Details->"
            }),
            ["LogOut"] = new LocalString(new()
            {
                ["hu-HU"] = "Kijelentkezés",
                ["en"] = "Log out"
            }),
            ["PlaceOrder"] = new LocalString(new()
            {
                ["hu-HU"] = "Rendelés",
                ["en"] = "Place Order"
            }),
            ["CartText"] = new LocalString(new()
            {
                ["hu-HU"] = "Kosár",
                ["en"] = "Cart"
            }),
            ["Username"] = new LocalString(new()
            {
                ["hu-HU"] = "Felhasználónév",
                ["en"] = "Username"
            }),
            ["Password"] = new LocalString(new()
            {
                ["hu-HU"] = "Jelszó",
                ["en"] = "Password"
            }),
            ["NoAccountText"] = new LocalString(new()
            {
                ["hu-HU"] = "Nincs még fiókom->",
                ["en"] = "Not registered->"
            }),
            ["HaveAccountText"] = new LocalString(new()
            {
                ["hu-HU"] = "Van már fiókom->",
                ["en"] = "Already registered->"
            }),
            ["LogIn"] = new LocalString(new()
            {
                ["hu-HU"] = "Bejelentkezés",
                ["en"] = "Login"
            }),
            ["ForgotPassword"] = new LocalString(new()
            {
                ["hu-HU"] = "Elfelejtett jelszó->",
                ["en"] = "Forgotten password->"
            }),
            ["FillOutAllFields"] = new LocalString(new()
            {
                ["hu-HU"] = "Töltsön ki minden mezőt!",
                ["en"] = "Please fill all fields!"
            }),
            ["UnknownError"] = new LocalString(new()
            {
                ["hu-HU"] = "Hiba történt.",
                ["en"] = "An error occured."
            }),
            ["NoInternet"] = new LocalString(new()
            {
                ["hu-HU"] = "Nincs internet",
                ["en"] = "No internet."
            }),
            ["Timeout"] = new LocalString(new()
            {
                ["hu-HU"] = "A kérés lejárt.",
                ["en"] = "The request timed out."
            }),
            ["ErrorTitle"] = new LocalString(new()
            {
                ["hu-HU"] = "Hiba",
                ["en"] = "Error"
            }),
            ["IncorrectUser"] = new LocalString(new()
            {
                ["hu-HU"] = "Hibás felhasználónév vagy jelszó!",
                ["en"] = "Incorrect username or password!"
            }),
            ["ServerError"] = new LocalString(new()
            {
                ["hu-HU"] = "Szerver hiba. Próbálja meg újra!",
                ["en"] = "Server error. Try again later!"
            }),
            ["UserTooShort"] = new LocalString(new()
            {
                ["hu-HU"] = "A névnek legalább 4 karakternek kell lennie.",
                ["en"] = "Username must be at least 4 characters."
            }),
            ["InvalidUsername"] = new LocalString(new()
            {
                ["hu-HU"] = "Érvénytelen felhasználónév.",
                ["en"] = "Invalid username."
            }),
            ["PasswordTooShort"] = new LocalString(new()
            {
                ["hu-HU"] = "Jelszónak legalább 8 karakternek kell lennie!",
                ["en"] = "Password must be at least 8 characters!"
            }),
            ["PasswordNoLower"] = new LocalString(new()
            {
                ["hu-HU"] = "Jelszóban kell lennie kisbetűnek.",
                ["en"] = "Password must contain lowercase letters."
            }),
            ["PasswordNoUpper"] = new LocalString(new()
            {
                ["hu-HU"] = "Jelszóban kell lennie nagybetűnek.",
                ["en"] = "Password must contain uppercase letters."
            }),
            ["PasswordNoDigit"] = new LocalString(new()
            {
                ["hu-HU"] = "Jelszóban kell lennie számnak.",
                ["en"] = "Password must contain digits."
            }),
            ["PasswordNoSpecial"] = new LocalString(new()
            {
                ["hu-HU"] = "Jelszóban kell lennie speciális karakternek.",
                ["en"] = "Password must contain special characters."
            }),
            ["Register"] = new LocalString(new()
            {
                ["hu-HU"] = "Regisztrálás",
                ["en"] = "Register"
            }),
            ["EmailPlaceholder"] = new LocalString(new()
            {
                ["hu-HU"] = "Email",
                ["en"] = "Email"
            }),
            ["InvalidEmail"] = new LocalString(new()
            {
                ["hu-HU"] = "Hibás email.",
                ["en"] = "Invalid email."
            }),
            ["Continue"] = new LocalString(new()
            {
                ["hu-HU"] = "Tovább",
                ["en"] = "Contine"
            }),
            ["GoBack"] = new LocalString(new()
            {
                ["hu-HU"] = "Vissza->",
                ["en"] = "Back->"
            }),
            ["Success"] = new LocalString(new()
            {
                ["hu-HU"] = "Siker",
                ["en"] = "Success"
            }),
            ["OK"] = new LocalString(new()
            {
                ["hu-HU"] = "OK",
                ["en"] = "OK"
            }),
            ["PasswordResetSuccess"] = new LocalString(new()
            {
                ["hu-HU"] = "Jelszó helyreállító email sikeresen elküldve.",
                ["en"] = "Password reset email successfully sent."
            }),
            ["PleaseConnectToInternet"] = new LocalString(new()
            {
                ["hu-HU"] = "Kérem csatlakozzon az internetre!",
                ["en"] = "Please connect to the internet!"
            }),
            ["PageLoadError"] = new LocalString(new()
            {
                ["hu-HU"] = "Adatok betöltése sikertelen.",
                ["en"] = "Failed to load data."
            }),
        };

    }

}
