public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest req);
    Task LogoutAsync(string tokenJti, DateTime expiresAt);
}
