using Microsoft.AspNetCore.Mvc;

public interface ClassifierPort
{
	public Task<IResult> PostClassifyPokemonImage([FromBody] PostClassifyPokemonImageDto postClassifyPokemonImageDto);
}