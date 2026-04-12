public class PostClassifyPokemonImageResponseDto
{
    public string ImageId { get; set; } = string.Empty;
    public string Classification { get; set; } = string.Empty;
    public DateTime ClassifiedAt { get; set; }
}
