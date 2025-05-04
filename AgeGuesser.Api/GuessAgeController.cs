using Microsoft.AspNetCore.Mvc;

namespace AgeGuesser.Api;

[Route("api/guess-age")]
[ApiController]
public class GuessAgeController(HttpClient httpClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<string>> GuessAge([FromQuery] string name)
    {
        var result = await httpClient.GetAsync($"https://api.agify.io?name={name}");
        var content = await result.Content.ReadFromJsonAsync<AgeGuessResponse>();
        
        return Ok($"Hello {name}, your age is guessed to be around {content!.Age}.");
    }

    private record AgeGuessResponse(int Age);
}