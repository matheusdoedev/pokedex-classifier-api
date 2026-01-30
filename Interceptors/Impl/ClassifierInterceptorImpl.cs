using Microsoft.AspNetCore.Mvc;

public class ClassifierInterceptorImpl : ClassifierInterceptor
{
	public Task<PostClassifyPokemonImageResponseDto> PostClassifyPokemonImage([FromBody] PostClassifyPokemonImageDto postClassifyPokemonImageDto)
	{
		string hashedImage = HashImage(postClassifyPokemonImageDto.Image);
		string? existedResponse = CheckAlreadyExistResponse(hashedImage);
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