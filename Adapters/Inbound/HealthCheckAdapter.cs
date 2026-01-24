using Microsoft.AspNetCore.Mvc;

[Controller]
[Route("")]
public class HealthCheckAdapter : HealthCheckPort
{
	[HttpGet]
	public string GetHealth()
	{
		return DateTime.UtcNow.ToString();
	}
}