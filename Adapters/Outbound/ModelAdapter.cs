using Microsoft.Extensions.Options;
using System.Text.Json;

public class ModelAdapter : ModelPort
{
	private readonly HttpClient _httpClient;
	private readonly IOptions<ModelOptions> _options;
	private readonly ILogger<ModelAdapter> _logger;

	public ModelAdapter(
			HttpClient httpClient,
			IOptions<ModelOptions> options,
			ILogger<ModelAdapter> logger)
	{
		_httpClient = httpClient;
		_options = options;
		_logger = logger;

		ValidateConfiguration();
	}

	public async Task<string> SendPrompt(string prompt)
	{
		ValidatePrompt(prompt);

		try
		{
			var response = await SendRequestToOllama(prompt);
			return ExtractResponseText(response);
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Failed to communicate with Ollama service at {Endpoint}", _options.Value.Endpoint);
			throw new InvalidOperationException("Failed to connect to Ollama service", ex);
		}
		catch (TaskCanceledException ex)
		{
			_logger.LogError(ex, "Request to Ollama service timed out after {TimeoutSeconds} seconds", _options.Value.TimeoutSeconds);
			throw new InvalidOperationException("Ollama service request timed out", ex);
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, "Failed to parse response from Ollama service");
			throw new InvalidOperationException("Invalid response format from Ollama service", ex);
		}
	}

	private void ValidateConfiguration()
	{
		var options = _options.Value;

		if (string.IsNullOrWhiteSpace(options.Endpoint))
		{
			throw new InvalidOperationException("Ollama endpoint is not configured");
		}

		if (string.IsNullOrWhiteSpace(options.ModelName))
		{
			throw new InvalidOperationException("Ollama model name is not configured");
		}
	}

	private void ValidatePrompt(string prompt)
	{
		if (string.IsNullOrWhiteSpace(prompt))
		{
			throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));
		}
	}

	private async Task<ModelGenerateResponse> SendRequestToOllama(string prompt)
	{
		var options = _options.Value;
		var request = new ModelGenerateRequest(
				Model: options.ModelName,
				Prompt: prompt,
				Stream: false
		);

		var endpoint = $"{options.Endpoint.TrimEnd('/')}/api/generate";

		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(options.TimeoutSeconds));

		try
		{
			var response = await _httpClient.PostAsJsonAsync(endpoint, request, cts.Token);
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStreamAsync(cts.Token);
			return await JsonSerializer.DeserializeAsync<ModelGenerateResponse>(
					content,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
					cts.Token)
					?? throw new InvalidOperationException("Model service response was null");
		}
		catch (HttpRequestException ex) when (ex.StatusCode != null)
		{
			_logger.LogError(ex, "Model service returned status code {StatusCode}", ex.StatusCode);
			throw;
		}
	}

	private static string ExtractResponseText(ModelGenerateResponse response)
	{
		if (string.IsNullOrWhiteSpace(response.Response))
		{
			return "No response generated";
		}

		return response.Response.Trim();
	}
}
