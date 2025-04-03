using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CustomerApp.Services
{
    public class AuthService: BindableObject, Singleton<AuthService>
    {
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
            var r = await HttpService.PostJsonAsync($"{HttpService.BaseAPIUrl}/user/login", new AuthObject() { name=name, password=password });
            UserObject user;
            string? stringContent = null;
            try
            {
                stringContent = await r.Content.ReadAsStringAsync();
                user = JsonSerializer.Deserialize<UserObject>(stringContent)!;
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
                Debug.WriteLine("Got an error during login", ex.ToString());
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
#pragma warning restore CS8618 
    }
}
