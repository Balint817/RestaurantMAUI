using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CustomerApp.Services
{
    public partial class AuthService : BindableObject, Singleton<AuthService>
    {
        public static string? CheckUsername(string name)
        {
            if (name.Length < 4)
            {
                return LanguageService.Instance["UserTooShort"].Current;
            }
            if (!name.All(char.IsLetterOrDigit))
            {
                return LanguageService.Instance["InvalidUsername"].Current;
            }
            return null;
        }
        public static string? CheckPassword(string password)
        {
            if (password.Length < 8)
            {
                return LanguageService.Instance["PasswordTooShort"].Current;
            }
            if (!password.Any(char.IsAsciiLetterLower))
            {
                return LanguageService.Instance["PasswordNoLower"].Current;
            }
            if (!password.Any(char.IsAsciiLetterUpper))
            {
                return LanguageService.Instance["PasswordNoUpper"].Current;
            }
            if (!password.Any(char.IsAsciiDigit))
            {
                return LanguageService.Instance["PasswordNoDigit"].Current;
            }
            if (!password.Any(c => !char.IsAsciiLetterOrDigit(c)))
            {
                return LanguageService.Instance["PasswordNoSpecial"].Current;
            }
            return null;
        }

        static AuthService? _instance;
        public static AuthService Instance => _instance ??= new();
        public HttpService HttpService => HttpService.Instance;

        const string UserInfoKey = "user_info_key";

        private UserObject? _user;

        public UserObject? User
        {
            get { return _user; }
            private set { _user = value; OnPropertyChanged(); }
        }

        public async Task<bool> Init()
        {
            bool isAuth = await LoadUser();
            if (isAuth)
            {
                HttpService.Instance.Authorize(User!.token);
                isAuth = await TestAuth();
                if (!isAuth)
                {
                    RemoveUser();
                }
            }
            return isAuth;
        }

        internal void RemoveUser()
        {
            this.User = null;
            HttpService.Instance.Unauthorize();
            SecureStorage.Remove(UserInfoKey);
        }

        async Task<bool> LoadUser()
        {
            var userString = await SecureStorage.GetAsync(UserInfoKey);
            if (string.IsNullOrEmpty(userString))
                return false;

            UserObject user;
            try
            {
                user = JsonSerializer.Deserialize<UserObject>(userString) ?? throw new NullReferenceException();
            }
            catch (Exception)
            {
                return false;
            }
            await SaveUser(user);
            return true;
        }

        async Task SaveUser(UserObject obj)
        {
            this.User = obj;
            HttpService.Instance.Authorize(obj.token);
            await SecureStorage.SetAsync(UserInfoKey, JsonSerializer.Serialize(obj));
        }

        private async Task<bool> TestAuth()
        {
            var r = await HttpService.GetAsync($"{HttpService.BaseAPIUrl}/token/validate");
            if (r.StatusCode != System.Net.HttpStatusCode.OK)
            {
                RemoveUser();
                return false;
            }
            return true;
        }

        public async Task<bool?> Login(string name, string password)
        {
            var r = await HttpService.PostJsonAsync($"{HttpService.BaseAPIUrl}/user/login", new AuthObject() { name = name, password = password });
            UserObject user;
            string? stringContent = null;
            try
            {
                stringContent = await r.Content.ReadAsStringAsync();
                user = JsonSerializer.Deserialize<UserObject>(stringContent)!;
                if (user?.userId is null)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Got an error during login", ex.ToString());
                if (ex is JsonException)
                {
                    return null;
                }
                return false;
            }
            await SaveUser(user);
            return await Init();
        }
        public async Task<KeyValuePair<bool?, string?>> GoogleLogin(string token)
        {

            var url = $"{HttpService.BaseAPIUrl}/user/google/auth/{token}";
            var r = await HttpService.PostJsonAsync(url, new { });
            UserObject user;
            string? stringContent = null;
            try
            {
                stringContent = await r.Content.ReadAsStringAsync();
                user = JsonSerializer.Deserialize<UserObject>(stringContent)!;
                if (user?.userId is null)
                {
                    if (user.message != null)
                    {
                        return new(false, user.message);
                    }
                    return new(false, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Got an error during google login", ex.ToString());
                if (ex is JsonException)
                {
                    return new(null, null);
                }
                return new(false, "Hiba történt.");
            }
            if (!r.IsSuccessStatusCode)
            {
                return new(false, null);
            }
            
            await SecureStorage.SetAsync(UserInfoKey, JsonSerializer.Serialize(user));
            return new(await Init(), null);
            //return new(await Login(name, password), null);
        }

        public async Task<KeyValuePair<bool?, string?>> Register(string name, string password, string email)
        {
            var r = await HttpService.PostJsonAsync($"{HttpService.BaseAPIUrl}/user/register/customer", new AuthObject() { name = name, password = password, email = email });
            UserObject user;
            string? stringContent = null;
            try
            {
                stringContent = await r.Content.ReadAsStringAsync();
                if (stringContent.ToLower() != "created")
                {
                    user = JsonSerializer.Deserialize<UserObject>(stringContent)!;
                    if (user.message != null)
                    {
                        return new(false, user.message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Got an error during register", ex.ToString());
                if (ex is JsonException)
                {
                    return new(null, null);
                }
                return new(false, "Hiba történt.");
            }
            if (r.StatusCode != System.Net.HttpStatusCode.Created)
            {
                return new(false, null);
            }
            return new(await Login(name, password), null);
        }

        public async Task Logout()
        {
            var r = await HttpService.PostAsync($"{HttpService.BaseAPIUrl}/user/logout", null);
            RemoveUser();
            await ((App)App.Current!).Init();
        }
        static Regex emailRegex = EmailRegex();
        internal static string? CheckEmail(string email)
        {
            return emailRegex.IsMatch(email)
                ? null
                : LanguageService.Instance["InvalidEmail"].Current;
        }

#pragma warning disable CS8618
        class AuthObject
        {
            public required string name { get; set; }
            public required string password { get; set; }
            public string? email { get; set; }
        }
        public class UserObject
        {
            public string role { get; set; }
            public string profilePicture { get; set; }
            public string userId { get; set; }
            public string token { get; set; }
            public string message { get; set; }
        }

        [GeneratedRegex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])")]
        private static partial Regex EmailRegex();

        public static readonly string ForgetPasswordURL = "http://localhost:3000/forgot-password";
        internal async Task<bool?> ForgotPassword(string email)
        {
            var r = await HttpService.PostJsonAsync($"{HttpService.BaseAPIUrl}/user/forgetPassword", new { email = email, url = ForgetPasswordURL });
            UserObject user;
            string? stringContent = null;
            try
            {
                stringContent = await r.Content.ReadAsStringAsync();
                user = JsonSerializer.Deserialize<UserObject>(stringContent)!;
                if (user.message is null)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Got an error during password reset", ex.ToString());
                if (ex is JsonException)
                {
                    return null;
                }
                return false;
            }
            return r.StatusCode == System.Net.HttpStatusCode.OK;
        }
#pragma warning restore CS8618
    }
}
