using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ClassifierAdapter : ControllerBase
{
	private readonly ClassifierInterceptor _classifierInterceptor;
	private readonly ILogger<ClassifierAdapter> _logger;

	public ClassifierAdapter(
			ClassifierInterceptor classifierInterceptor,
			ILogger<ClassifierAdapter> logger)
	{
		_classifierInterceptor = classifierInterceptor ?? throw new ArgumentNullException(nameof(classifierInterceptor));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	[HttpPost("classify")]
	public async Task<IActionResult> PostClassifyPokemonImage([FromBody] PostClassifyPokemonImageDto request)
	{
		if (request is null)
		{
			_logger.LogWarning("Classify request received with null body");
			return BadRequest("Request body cannot be null");
		}

		try
		{
			var result = await _classifierInterceptor.PostClassifyPokemonImage(request);
			return Ok(result);
		}
		catch (ArgumentException ex)
		{
			_logger.LogWarning(ex, "Invalid classification request");
			return BadRequest(ex.Message);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Classification service unavailable");
			return StatusCode(StatusCodes.Status503ServiceUnavailable, "Classification service is unavailable");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error during classification");
			return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
		}
	}
}
