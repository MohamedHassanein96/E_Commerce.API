﻿using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Authentication
{
    public class JwtOptions
    {
        [Required]
        public static string SectionName = "Jwt";
        [Required]
        public string Key { get; init; } = string.Empty;
        [Required]
        public string Issuer { get; init; } = string.Empty;
        [Required]
        public string Audience { get; init; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int ExpiryMinutes { get; init; }
    }
}
