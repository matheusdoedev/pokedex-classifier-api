using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

/// <summary>
/// Controller for AI model inference using the ModelPort
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ModelController : ControllerBase
{
    private readonly ModelPort _modelPort;
    private readonly ILogger<ModelController> _logger;

    public ModelController(ModelPort modelPort, ILogger<ModelController> logger)
    {
        _modelPort = modelPort;
        _logger = logger;
    }

    /// <summary>
    /// Send a prompt to the Ollama model
    /// </summary>
    /// <remarks>
    /// Example request:
    ///
    ///     POST /api/model/prompt
    ///     {
    ///       "prompt": "What are the characteristics of Pikachu?"
    ///     }
    /// </remarks>
    [HttpPost("prompt")]
    public async Task<IActionResult> SendPrompt([FromBody] SendPromptRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt is required");

            var response = await _modelPort.SendPrompt(request.Prompt);
            return Ok(new { response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending prompt to model");
            return StatusCode(500, new { error = "Error processing prompt" });
        }
    }

    /// <summary>
    /// Classify a Pokémon based on image description
    /// </summary>
    /// <remarks>
    /// Example request:
    ///
    ///     POST /api/model/classify-pokemon
    ///     {
    ///       "imageDescription": "Yellow electric mouse with red cheeks and lightning tail"
    ///     }
    /// </remarks>
    [HttpPost("classify-pokemon")]
    public async Task<IActionResult> ClassifyPokemon([FromBody] ClassifyPokemonRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ImageDescription))
                return BadRequest("Image description is required");

            var prompt = $"""
                Classify the following Pokémon based on its description.
                Provide the Pokémon name, type, and characteristics.

                Description: {request.ImageDescription}
                """;

            var classification = await _modelPort.SendPrompt(prompt);
            return Ok(new { classification });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error classifying pokemon");
            return StatusCode(500, new { error = "Error classifying pokemon" });
        }
    }
}

/// <summary>
/// Request model for sending prompts
/// </summary>
public class SendPromptRequest
{
    public required string Prompt { get; set; }
}

/// <summary>
/// Request model for Pokémon classification
/// </summary>
public class ClassifyPokemonRequest
{
    public required string ImageDescription { get; set; }
}
