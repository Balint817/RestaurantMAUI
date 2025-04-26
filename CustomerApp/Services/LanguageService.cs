using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
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
        CultureInfo? TryCreateCulture(string cultureName)
        {
            try
            {
                return new CultureInfo(cultureName);
            }
            catch (Exception)
            {
                return null;
            }
        }
        CultureInfo?[] GetInputLanguages()
        {
            try
            {
#if ANDROID
                return [TryCreateCulture(Java.Util.Locale.Default.DisplayLanguage)];
#elif IOS
                return GetIOSLanguages().ToArray();
#endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return [];
        }

#if IOS
        List<CultureInfo?> GetIOSLanguages()
        {
            var languages = Foundation.NSLocale.PreferredLanguages;

            var result = languages?.Select(x => TryCreateCulture(x)).ToList() ?? new();

            var inputModes = UIKit.UITextInputMode.ActiveInputModes;
            if (inputModes != null)
            {
                foreach (UIKit.UITextInputMode mode in inputModes)
                {
                    result.Add(TryCreateCulture(mode.PrimaryLanguage));
                }
            }

            return result;
        }
#endif
        StringWithQualityHeaderValue[]? _cachedLanguages = null;
        public StringWithQualityHeaderValue[] GetLanguages()
        {
            if (_cachedLanguages is not null)
            {
                return _cachedLanguages;
            }
            var resultLanguages = new List<CultureInfo>();
            List<CultureInfo> inputLanguages =
            [
                CultureInfo.CurrentCulture,
                CultureInfo.CurrentCulture.Parent,

                CultureInfo.CurrentUICulture,
                CultureInfo.CurrentUICulture.Parent,

                .. GetInputLanguages(),

                CultureInfo.InstalledUICulture,
                CultureInfo.InstalledUICulture.Parent,

                .. CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).SelectMany(x => (CultureInfo[])[x, x.Parent])
            ];

            foreach (var c in inputLanguages)
            {
                if (c != null && !string.IsNullOrEmpty(c.Name) && !resultLanguages.Contains(c))
                {
                    resultLanguages.Add(c);
                }
            }

            return _cachedLanguages = resultLanguages.DistinctBy(x => x.Name).Select((x, i) => new StringWithQualityHeaderValue(x.Name, Math.Max(1 - (i / 10d), 0))).Where(x => x.Quality != 0).ToArray();
        }
        static LanguageService()
        {
            var indexerProp = typeof(LanguageService)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .First(x => x.GetIndexParameters().Length != 0);
            IndexerName = indexerProp.Name;
        }
        public LanguageService()
        {
            var languages = GetLanguages().ToList();
            if (languages.Count < 0)
            {
                languages.Add(new("en-GB"));
            }
            var firstLanguage = languages[0];
            var culture = new CultureInfo(firstLanguage.Value);
            SetLanguage(culture);
        }

        private static string IndexerName;

        private static LanguageService? _instance;
        public static LanguageService Instance => _instance ??= new();

        // Current language code (e.g. "en", "hu")
        public string CurrentLanguage { get; private set; } = "en";
        public void SetLanguage(CultureInfo culture)
        {
            CurrentLanguage = culture.Name.Split("-")[0].ToLower();
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
                ["hu"] = "Gyorsétterem",
                ["en"] = "Fast Food"
            }),
            ["SearchText"] = new LocalString(new()
            {
                ["hu"] = "Keress ételeink és italaink között...",
                ["en"] = "Search our foods and drinks..."
            }),
            ["Highlights"] = new LocalString(new()
            {
                ["hu"] = "Kiemelt Termékeink",
                ["en"] = "Our Highlights"
            }),
            ["Ingredients"] = new LocalString(new()
            {
                ["hu"] = "Hozzávalók:",
                ["en"] = "Ingredients:"
            }),
            ["Description"] = new LocalString(new()
            {
                ["hu"] = "Leírás",
                ["en"] = "Description"
            }),
            ["ToCart"] = new LocalString(new()
            {
                ["hu"] = "Kosárba",
                ["en"] = "Add"
            }),
            ["OrderDetails"] = new LocalString(new()
            {
                ["hu"] = "Részletek->",
                ["en"] = "Details->"
            }),
            ["LogOut"] = new LocalString(new()
            {
                ["hu"] = "Kijelentkezés",
                ["en"] = "Log out"
            }),
            ["PlaceOrder"] = new LocalString(new()
            {
                ["hu"] = "Rendelés",
                ["en"] = "Place Order"
            }),
            ["CartText"] = new LocalString(new()
            {
                ["hu"] = "Kosár",
                ["en"] = "Cart"
            }),
            ["Username"] = new LocalString(new()
            {
                ["hu"] = "Felhasználónév",
                ["en"] = "Username"
            }),
            ["Password"] = new LocalString(new()
            {
                ["hu"] = "Jelszó",
                ["en"] = "Password"
            }),
            ["NoAccountText"] = new LocalString(new()
            {
                ["hu"] = "Nincs még fiókom->",
                ["en"] = "Not registered->"
            }),
            ["HaveAccountText"] = new LocalString(new()
            {
                ["hu"] = "Van már fiókom->",
                ["en"] = "Already registered->"
            }),
            ["LogIn"] = new LocalString(new()
            {
                ["hu"] = "Bejelentkezés",
                ["en"] = "Login"
            }),
            ["ForgotPassword"] = new LocalString(new()
            {
                ["hu"] = "Elfelejtett jelszó->",
                ["en"] = "Forgotten password->"
            }),
            ["FillOutAllFields"] = new LocalString(new()
            {
                ["hu"] = "Töltsön ki minden mezőt!",
                ["en"] = "Please fill all fields!"
            }),
            ["UnknownError"] = new LocalString(new()
            {
                ["hu"] = "Hiba történt.",
                ["en"] = "An error occured."
            }),
            ["NoInternet"] = new LocalString(new()
            {
                ["hu"] = "Nincs internet",
                ["en"] = "No internet."
            }),
            ["Timeout"] = new LocalString(new()
            {
                ["hu"] = "A kérés lejárt.",
                ["en"] = "The request timed out."
            }),
            ["ErrorTitle"] = new LocalString(new()
            {
                ["hu"] = "Hiba",
                ["en"] = "Error"
            }),
            ["IncorrectUser"] = new LocalString(new()
            {
                ["hu"] = "Hibás felhasználónév vagy jelszó!",
                ["en"] = "Incorrect username or password!"
            }),
            ["ServerError"] = new LocalString(new()
            {
                ["hu"] = "Szerver hiba. Próbálja meg újra!",
                ["en"] = "Server error. Try again later!"
            }),
            ["UserTooShort"] = new LocalString(new()
            {
                ["hu"] = "A névnek legalább 4 karakternek kell lennie.",
                ["en"] = "Username must be at least 4 characters."
            }),
            ["InvalidUsername"] = new LocalString(new()
            {
                ["hu"] = "Érvénytelen felhasználónév.",
                ["en"] = "Invalid username."
            }),
            ["PasswordTooShort"] = new LocalString(new()
            {
                ["hu"] = "Jelszónak legalább 8 karakternek kell lennie!",
                ["en"] = "Password must be at least 8 characters!"
            }),
            ["PasswordNoLower"] = new LocalString(new()
            {
                ["hu"] = "Jelszóban kell lennie kisbetűnek.",
                ["en"] = "Password must contain lowercase letters."
            }),
            ["PasswordNoUpper"] = new LocalString(new()
            {
                ["hu"] = "Jelszóban kell lennie nagybetűnek.",
                ["en"] = "Password must contain uppercase letters."
            }),
            ["PasswordNoDigit"] = new LocalString(new()
            {
                ["hu"] = "Jelszóban kell lennie számnak.",
                ["en"] = "Password must contain digits."
            }),
            ["PasswordNoSpecial"] = new LocalString(new()
            {
                ["hu"] = "Jelszóban kell lennie speciális karakternek.",
                ["en"] = "Password must contain special characters."
            }),
            ["Register"] = new LocalString(new()
            {
                ["hu"] = "Regisztrálás",
                ["en"] = "Register"
            }),
            ["EmailPlaceholder"] = new LocalString(new()
            {
                ["hu"] = "Email",
                ["en"] = "Email"
            }),
            ["InvalidEmail"] = new LocalString(new()
            {
                ["hu"] = "Hibás email.",
                ["en"] = "Invalid email."
            }),
            ["Continue"] = new LocalString(new()
            {
                ["hu"] = "Tovább",
                ["en"] = "Contine"
            }),
            ["GoBack"] = new LocalString(new()
            {
                ["hu"] = "Vissza->",
                ["en"] = "Back->"
            }),
            ["Success"] = new LocalString(new()
            {
                ["hu"] = "Siker",
                ["en"] = "Success"
            }),
            ["OK"] = new LocalString(new()
            {
                ["hu"] = "OK",
                ["en"] = "OK"
            }),
            ["PasswordResetSuccess"] = new LocalString(new()
            {
                ["hu"] = "Jelszó helyreállító email sikeresen elküldve.",
                ["en"] = "Password reset email successfully sent."
            }),
            ["PleaseConnectToInternet"] = new LocalString(new()
            {
                ["hu"] = "Kérem csatlakozzon az internetre!",
                ["en"] = "Please connect to the internet!"
            }),
            ["PageLoadError"] = new LocalString(new()
            {
                ["hu"] = "Adatok betöltése sikertelen.",
                ["en"] = "Failed to load data."
            }),
            ["EmptyCartError"] = new LocalString(new()
            {
                ["hu"] = "Üres a kosár!",
                ["en"] = "The cart is empty!"
            }),
            ["PlaceOrderSuccess"] = new LocalString(new()
            {
                ["hu"] = "Rendelés sikeresen leadva.",
                ["en"] = "Order successfully placed."
            }),
            ["PlaceOrderError"] = new LocalString(new()
            {
                ["hu"] = "Hiba történt a rendelés leadásakor!",
                ["en"] = "An error occured while placing order!"
            }),
            ["GoogleText"] = new LocalString(new()
            {
                ["hu"] = "Google belépés->",
                ["en"] = "Google login->"
            }),
            ["GoogleLoginTitle"] = new LocalString(new()
            {
                ["hu"] = "Google Bejelentkezés",
                ["en"] = "Google Login"
            }),
            //[""] = new LocalString(new()
            //{
            //    ["hu-HU"] = "",
            //    ["en"] = ""
            //}),
        };

    }

}
