namespace Application.jwt
{
    public interface IJwtManager
    {
        public Task<string> MakeToken(string email, string password, CancellationToken ct);
    }
}
