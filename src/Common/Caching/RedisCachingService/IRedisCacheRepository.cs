namespace RedisCachingService
{
    public interface IRedisCacheRepository
    {
        Task AddEntityToHashAsync(string key, IDictionary<string, string> dictionary);
        Task<IDictionary<string, string>> GetEntityFromHashAsync(string hashKey);
        Task AddEntryToHashAsync(string key, string entryName, string entryValue);
        Task<string?> GetEntryValueFromHashAsync(string key, string entryName);
        Task AddValueToSetAsync(string key, string value);
        Task<List<string>> GetValuesFromSetAsync(string key);
        Task AddStringAsync(string key, string value);
        Task<string?> GetStringAsync(string key);
        Task<bool> DeleteEntryFromHashAsync(string key, string entryName);
        Task<bool> DeleteEntityFromHashAsync(string key);
    }
}
