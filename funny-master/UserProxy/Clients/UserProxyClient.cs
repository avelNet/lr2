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
        }
    }
}
