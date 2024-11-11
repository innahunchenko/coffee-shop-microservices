namespace Foundation.Abstractions.Services
{
    public interface ICookieService
    {
        void SetData(string key, string value, DateTimeOffset dateTimeOffset);
        void SetData(string key, string value);
        string? GetData(string key);
        void ClearData(string key);
    }
}
