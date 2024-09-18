namespace RedisCachingService
{
    public interface IRedisCacheRepository
    {
        Task<bool> AddEntityToHashAsync(string key, IDictionary<string, string> dictionary);
        Task<IDictionary<string, string>> GetEntityFromHashAsync(string hashKey);
        Task<bool> AddEntryToHashAsync(string key, string entryName, string entryValue);
        Task<string> GetEntryValueFromHashAsync(string key, string entryName);
        Task<bool> AddValueToSetAsync(string key, string value);
        Task<List<string>> GetValuesFromSetAsync(string key);
        Task<bool> AddStringAsync(string key, string value);
        Task<string> GetStringAsync(string key);
        Task<bool> DeleteEntryFromHashAsync(string key, string entryName);
        Task<bool> DeleteEntityFromHashAsync(string key);
    }
}
