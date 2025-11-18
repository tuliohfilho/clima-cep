using Clima.Cep.Application.Dtos;
using Clima.Cep.Application.Interfaces.Queries;
using Clima.Cep.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Clima.Cep.Api.Controllers;

/// <summary>
/// Controller para gerenciar operações de clima (US03).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WeatherController(
    IWeatherService weatherService, 
    ILogger<WeatherController> logger,
    IZipCodeServiceQuery zipCodeServiceQuery) : ControllerBase
{
    private readonly ILogger<WeatherController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IWeatherService _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
    private readonly IZipCodeServiceQuery _zipCodeServiceQuery = zipCodeServiceQuery ?? throw new ArgumentNullException(nameof(zipCodeServiceQuery));

    /// <summary>
    /// Consulta clima atual e previsão para todos os CEPs persistidos (US03).
    /// </summary>
    /// <param name="days">Número de dias de previsão (1-7, padrão 3).</param>
    /// <returns>Lista de clima para cada CEP persistido.</returns>
    /// <response code="200">Clima consultado com sucesso.</response>
    /// <response code="400">Parâmetro 'days' inválido.</response>
    /// <response code="404">Nenhum CEP persistido.</response>
    /// <response code="504">Timeout ao consultar API de clima.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WeatherResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status504GatewayTimeout)]
    public async Task<ActionResult<IEnumerable<WeatherResponseDto>>> GetWeather([FromQuery] int days = 3)
    {
        try
        {
            if (days < 1 || days > 7)
            {
                _logger.LogWarning("GET /weather: Invalid days parameter: {Days}", days);
                return BadRequest(CreateProblemDetails(
                    "https://errors.sua-api.com/invalid-days",
                    "Parâmetro inválido",
                    "Parâmetro 'days' deve estar entre 1 e 7.",
                    400));
            }

            _logger.LogInformation("GET /weather: Fetching weather for {Days} days", days);

            var zipCodeResponseDtos = await _zipCodeServiceQuery.GetAllOrderedByCreationDateAsync();
            if (!zipCodeResponseDtos.Any())
            {
                _logger.LogWarning("GET /weather: No saved CEPs found");
                throw new InvalidOperationException("No saved CEPs found.");
            }

            var result = await _weatherService.GetWeatherBySavedZipCodesAsync(zipCodeResponseDtos, days);

            _logger.LogInformation("GET /weather: Successfully retrieved weather for {Count} locations", result.Count());
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("GET /weather: {Error}", ex.Message);

            if (ex.Message.Contains("No saved CEPs"))
            {
                return NotFound(CreateProblemDetails(
                    "https://errors.sua-api.com/no-saved-cep",
                    "Nenhum CEP persistido",
                    "Nenhum CEP foi persistido. Crie um CEP usando POST /api/cep primeiro.",
                    404));
            }

            return StatusCode(500, CreateProblemDetails(
                "https://errors.sua-api.com/internal-error",
                "Erro ao processar",
                ex.Message,
                500));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("GET /weather: Invalid argument - {Error}", ex.Message);
            return BadRequest(CreateProblemDetails(
                "https://errors.sua-api.com/invalid-argument",
                "Argumento inválido",
                ex.Message,
                400));
        }
        catch (HttpRequestException ex) when (ex.InnerException?.GetType().Name.Contains("TimeoutException") ?? false)
        {
            _logger.LogError(ex, "GET /weather: Timeout");
            return StatusCode(504, CreateProblemDetails(
                "https://errors.sua-api.com/external-timeout",
                "Timeout de provedor externo",
                "A consulta aos provedores de clima excedeu o tempo limite.",
                504));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET /weather: Unexpected error");
            return StatusCode(500, CreateProblemDetails(
                "https://errors.sua-api.com/internal-error",
                "Erro interno",
                "Ocorreu um erro inesperado ao processar a requisição.",
                500));
        }
    }

    private ProblemDetails CreateProblemDetails(string type, string title, string detail, int status)
    {
        return new ProblemDetails
        {
            Type = type,
            Title = title,
            Detail = detail,
            Status = status,
            Instance = HttpContext.Request.Path,
            Extensions = new Dictionary<string, object?>
            {
                { "traceId", HttpContext.TraceIdentifier }
            }
        };
    }
}
