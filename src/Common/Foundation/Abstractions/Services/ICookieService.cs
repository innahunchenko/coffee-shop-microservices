namespace Foundation.Abstractions.Services
{
    public interface ICookieService
    {
        void SetData(string key, string value, DateTimeOffset? dateTimeOffset);
        string? GetData(string key);
        void ClearData(string key);
    }
}
