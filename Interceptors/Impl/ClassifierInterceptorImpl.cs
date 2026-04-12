using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

public class ClassifierInterceptorImpl(NoSqlDatabasePort noSqlDatabase, ModelPort modelPort) : ClassifierInterceptor
{
	private readonly NoSqlDatabasePort _noSqlDatabase = noSqlDatabase;
	private readonly ModelPort _modelPort = modelPort;

	public async Task<PostClassifyPokemonImageResponseDto> PostClassifyPokemonImage([FromBody] PostClassifyPokemonImageDto postClassifyPokemonImageDto)
	{
		string imageHash = HashImage(postClassifyPokemonImageDto.Image);
		bool existsResponse = await CheckAlreadyExistResponse(imageHash);
		PostClassifyPokemonImageResponseDto responseDto = new("");

		if (existsResponse)
		{
			string cachedResponse = await _noSqlDatabase.GetAsString(imageHash);
			return new PostClassifyPokemonImageResponseDto(cachedResponse);
		}

		string modelResponse = await SentToModel(postClassifyPokemonImageDto.Image);

		await SaveResponseInStorage(imageHash, modelResponse);
		return new PostClassifyPokemonImageResponseDto(modelResponse);
	}

	private string HashImage(string imageBase64)
	{
		using SHA256 sha256 = SHA256.Create();
		byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(imageBase64));

		return Convert.ToBase64String(hashBytes);
	}

	private async Task<bool> CheckAlreadyExistResponse(string imageHash)
	{
		return await _noSqlDatabase.Exists(imageHash);
	}

	private async Task<string> SentToModel(string imageBase64)
	{
		return await _modelPort.SendPrompt(imageBase64);
	}

	private async Task SaveResponseInStorage(string imageHash, string response)
	{
		await _noSqlDatabase.Store(imageHash, response);
	}
}
