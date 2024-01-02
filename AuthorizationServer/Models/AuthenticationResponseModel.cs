﻿using System.Text.Json.Serialization;

namespace OIDCDemo.AuthorizationServer.Models
{
    public class AuthenticationResponseModel
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }
        [JsonPropertyName("refresh_token")]
        public required string RefreshToken { get; set; }
        [JsonPropertyName("token_type")]
        public required string TokenType { get; set; }
        [JsonPropertyName("id_token")]
        public required string IdToken { get; set; }
        [JsonPropertyName("state")]
        public string? State { get; set; }
        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }
    }
}
