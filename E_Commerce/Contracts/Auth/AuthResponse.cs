﻿namespace E_Commerce.Contracts.Auth
{
    public record AuthResponse(string Id, string? Email, string FirstName, string LastName, string Token, int ExpiresIn);
    
}
