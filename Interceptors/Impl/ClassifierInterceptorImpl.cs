using Microsoft.AspNetCore.Mvc;

public class ClassifierInterceptorImpl : ClassifierInterceptor
{
	public async Task<PostClassifyPokemonImageResponseDto> PostClassifyPokemonImage([FromBody] PostClassifyPokemonImageDto postClassifyPokemonImageDto)
	{
		string imageHash = HashImage(postClassifyPokemonImageDto.Image);
		string? existedResponse = CheckAlreadyExistResponse(imageHash);
		PostClassifyPokemonImageResponseDto responseDto = new("");

		if (existedResponse is not null)
		{
			return new PostClassifyPokemonImageResponseDto(existedResponse);
		}

		// check if already have a response for this image in nosql database

		// if is, return that one
		// send image to model
		// get response and save in nosql database
		// return response
	}

	private string HashImage(string ImageBase64)
	{

	}

	private string? CheckAlreadyExistResponse(string imageHash)
	{

	}
}