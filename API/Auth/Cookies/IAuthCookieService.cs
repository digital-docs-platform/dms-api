namespace API.Auth.Cookies
{
    public interface IAuthCookieService
    {
        public void SetAccessToken(HttpResponse response, string token);
        public void ClearAccessToken(HttpResponse response);

    }
}
