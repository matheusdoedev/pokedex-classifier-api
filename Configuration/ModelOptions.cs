using System.ComponentModel.DataAnnotations;

public class ModelOptions
{
    public const string SectionName = "Model";

    [Required(ErrorMessage = "Model endpoint is required")]
    [Url(ErrorMessage = "Model endpoint must be a valid URL")]
    public string Endpoint { get; set; } = "http://localhost:11434";

    [Required(ErrorMessage = "Model name is required")]
    [MinLength(1, ErrorMessage = "Model name cannot be empty")]
    public string ModelName { get; set; } = "llama2";

    [Range(1, 300, ErrorMessage = "Timeout must be between 1 and 300 seconds")]
    public int TimeoutSeconds { get; set; } = 30;
}
