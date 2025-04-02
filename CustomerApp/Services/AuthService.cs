using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CustomerApp.Services
{
    internal class AuthService: BindableObject, Singleton<AuthService>
    {
        static AuthService _instance;
        public static AuthService Instance => _instance ??= new();
        public HttpService HttpService => HttpService.Instance;

        const string AuthTokenKey = "auth_token";

        private UserObject _user;

        public PartialUserObject User
        {
            get { return _user; }
            set { _user = value as UserObject; OnPropertyChanged(); }
        }

        public async Task<bool> Init()
        {
            var token = await SecureStorage.GetAsync(AuthTokenKey);
            bool isAuth = !string.IsNullOrWhiteSpace(token);
            if (isAuth)
            {
                HttpService.Instance.Authorize(token);
                isAuth = await TestAuth();
                if (!isAuth)
                {
                    HttpService.Instance.Unauthorize();
                }
            }
            return isAuth;
        }

        private async Task<bool> TestAuth()
        {
            var r = await HttpService.GetAsync($"{HttpService.BaseAPIUrl}/token/validate");
            return r.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task Login(string name, string password)
        {
            var r = await HttpService.PostJsonAsync($"{HttpService.BaseAPIUrl}/user/login", new AuthObject() { name=name, password=password });
            var user = await r.Content.ReadFromJsonAsync<UserObject>();
            await SecureStorage.SetAsync(AuthTokenKey, user.token);
            if (await Init())
            {
                User = user;
            }
        }

        class AuthObject
        {
            public required string name { get; set; }
            public required string password { get; set; }
            public string? email { get; set; }
        }

        [JsonPolymorphic]
        [JsonDerivedType(typeof(UserObject))]
        public abstract class PartialUserObject
        {
            public string role { get; set; }
            public string profilePicture { get; set; }
            public string userId { get; set; }
            protected PartialUserObject()
            {
                
            }
        }

        private class UserObject: PartialUserObject
        {
            internal string token { get; set; }
            public UserObject()
            {
                
            }
        }
    }
}
