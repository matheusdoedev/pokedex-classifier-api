using Microsoft.AspNetCore.Mvc;

public interface ClassifierInterceptor
{
	public Task<PostClassifyPokemonImageResponseDto> PostClassifyPokemonImage([FromBody] PostClassifyPokemonImageDto postClassifyPokemonImageDto);
}