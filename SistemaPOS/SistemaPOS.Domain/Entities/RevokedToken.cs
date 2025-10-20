using System;
namespace SistemaPOS.Domain.Entities
{
    public class RevokedToken
    {
        public string Jti { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}