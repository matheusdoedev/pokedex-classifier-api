public interface NoSqlDatabasePort
{
	public Task Store(string key, string value);
	public Task<string> GetAsString(string key);
	public Task<bool> Exists(string key);
}