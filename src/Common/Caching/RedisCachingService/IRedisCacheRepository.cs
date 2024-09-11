namespace RedisCachingService
{
    public interface IRedisCacheRepository
    {
        Task<bool> AddEntityToHashAsync(string key, IDictionary<string, string> dictionary, CancellationToken cancellationToken);
        Task<IDictionary<string, string>> GetEntityFromHashAsync(string hashKey);
        Task<bool> AddEntryToHashAsync(string key, string entryName, string entryValue, CancellationToken cancellationToken);
        Task<string> GetEntryValueFromHashAsync(string key, string entryName);
        Task<bool> AddValueToSetAsync(string key, string value, CancellationToken cancellationToken);
        Task<List<string>> GetValuesFromSetAsync(string key);
        Task<bool> AddStringAsync(string key, string value, CancellationToken cancellationToken);
        Task<string> GetStringAsync(string key);
        Task<bool> DeleteEntryFromHashAsync(string key, string entryName, CancellationToken cancellationToken);
        Task<bool> DeleteEntityFromHashAsync(string key, CancellationToken cancellationToken);
    }
}
