using Core;
using Blazored.SessionStorage;
using System.Net.Http.Json;
// @using System.Net.Http.Json i _Imports.razor gælder Razor-filer, men ikke automatisk dine almindelige .cs-filer.
// Derfor skal vi selv tilføje <using System.Net.Http.Json>; i LoginService.cs, når vi bruger PostAsJsonAsync().


/* HOW TO USE: 
* Gør den tilgængelig i Imports.razor: @using GenbrugEaa.Services
* registrer servicen i program.cs - gør den global for projektet: builder.Services.AddScoped<LoginService>();
* Derfor kan sider kan gøre brug af: 
* * @inject LoginService login
* * Efter injektion kan sider kalde login.UserLogin(username, password) og gøre brug af metoden her for at tjekke et logins gyldighed hos DB. 
*/

namespace GenbrugEaa.Services
{
    public class LoginService
    {
        private readonly HttpClient Http; // opretter et felt, som klassen kan bruge
        private readonly ISessionStorageService sessionStorage; // opretter felt, som klassen kan bruge

        public LoginService(HttpClient http, ISessionStorageService storage) // klassens constructor
        {
            Http = http;
            sessionStorage = storage;
        }

        /* UserLogin():
         * Opretter nyt UserLogin objekt. 
         * Kalder Post API hos LoginController
         */
        public async Task<bool> UserLogin(string username, string password)
        {
            User user = new User { Username = username, Password = password };

            HttpResponseMessage response = await Http.PostAsJsonAsync("api/login", user);

            Console.WriteLine($"Status: {response.StatusCode}");

            return response.IsSuccessStatusCode;
        }

        /* SetUserSession():
         * Hvis et login er gyldigt skal LoginService.SetUserSession(username) kaldes. 
         * gemmer username og bool i sessionStorage - fortæller hvilken bruger og om bruger er logget ind.
         */
        public async Task SetUserSession(string username)
        {
            await sessionStorage.SetItemAsync("username", username);
            await sessionStorage.SetItemAsync("isLoggedIn", true);
        }

        /* Logout():
         * Når bruger logger ud skal LoginService.Logout kaldes
         * Fjerner info om username og om bruger er logget ind fra sessionStorage.
         */
        public async Task Logout()
        {
            await sessionStorage.RemoveItemAsync("username");
            await sessionStorage.RemoveItemAsync("isLoggedIn");
        }

        /* IsLoggedIn():
         * Når sider med begrænset adgang åbnes, skal LoginService.
         * Henter username og isLoggedIn bool fra sessionStorage.
         * Hvis værdier er gyldige returneres true. 
         * Ellers returneres false. 
         */
        public async Task<bool> IsLoggedIn()
        {
            string? username = await sessionStorage.GetItemAsync<string>("username");
            bool isLoggedIn = await sessionStorage.GetItemAsync<bool>("isLoggedIn");
            if (isLoggedIn && username != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
