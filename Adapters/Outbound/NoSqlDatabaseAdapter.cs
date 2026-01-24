using StackExchange.Redis;

public class NoSqlDatabaseAdapter : NoSqlDatabasePort
{
	private readonly IDatabase _db;

	public NoSqlDatabaseAdapter(IConnectionMultiplexer redis)
	{
		_db = redis.GetDatabase();
	}

	public async Task<bool> Exists(string key)
	{
		return await _db.KeyExistsAsync(key);
	}

	public async Task<string> GetAsString(string key)
	{
		RedisValue value = await _db.StringGetAsync(key);

		return value.ToString();
	}

	public async Task Store(string key, string value)
	{
		await _db.StringSetAsync(key, value);
	}
}