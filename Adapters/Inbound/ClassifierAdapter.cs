using Microsoft.AspNetCore.Mvc;

[Controller]
[Route("classifier")]
public class ClassifierAdapter : ClassifierPort
{
	[HttpPost]
	public Task<IResult> PostClassifyPokemonImage([FromBody] PostClassifyPokemonImageDto postClassifyPokemonImageDto)
	{
		throw new NotImplementedException();
	}
}