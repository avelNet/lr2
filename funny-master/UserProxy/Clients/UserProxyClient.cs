using System.Net;
using System.Net.Http.Json;
using UserProxy.Interfaces;
using UserProxy.Models;

namespace UserProxy.Clients
{
    public class UserProxyClient : IUserProxy
    {
        private readonly HttpClient _httpClient;

        public UserProxyClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ReadFromJsonAsync читает JSON-ответ и десериализует его в объект указанного типа.
        // В данном случае, мы ожидаем, что ответ будет содержать логическое значение (true или false),
        // которое указывает на существование пользователя с заданным идентификатором.   
        public async Task<bool> ExistsAsync(string id)
        {
            var response = await _httpClient.GetAsync($"users/{Uri.EscapeDataString(id)}/exists");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<UserDto?> GetUserAsync(string id)
        {
            var response = await _httpClient.GetAsync($"users/{Uri.EscapeDataString(id)}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }

        private async Task<UserDto?> SendUserAsync(UserDto user, bool isNewUser)
        {
            PrepareForService(user);

            var flag = isNewUser.ToString().ToLowerInvariant();
            var response = await _httpClient.PostAsJsonAsync($"users/{flag}", user);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserDto>();
        }

        public async Task<UserDto?> CreateUserAsync(UserDto user)
        {
            return await SendUserAsync(user, true);
        }

        public async Task<UserDto?> UpdateUserAsync(UserDto user)
        {
            return await SendUserAsync(user, false);
        }

        private static void PrepareForService(UserDto user)
        {
            if (string.IsNullOrWhiteSpace(user.NewId))
            {
                user.NewId = user.Id;
            }
            if (string.IsNullOrWhiteSpace(user.Department))
            {
                user.Department = "-";
            }
            if (string.IsNullOrWhiteSpace(user.Organization))
            {
                user.Organization = "-";
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                // Guid.NewGuid().ToString("N"); 
                // Генерирует уникальный идентификатор без дефисов, который можно использовать в качестве пароля
                user.Password = "Aa1" + Guid.NewGuid().ToString("N");
            }
            if(user.Roles.Count == 0)
            {
                user.Roles.Add("client");
            }

            var now = DateTime.UtcNow;

            // ??= значение присваивается только если свойство равно null, иначе оно остается без изменений
            user.CertificateFrom ??= now;
            user.CertificateTo ??= now.AddYears(1);
            user.CreatedDatetime ??= now;
            user.LastUpdateDatetime ??= now;
            user.LastPasswordChangeDatetime ??= now;
            user.LastLoginDatetime ??= now;
        }
    }
}
