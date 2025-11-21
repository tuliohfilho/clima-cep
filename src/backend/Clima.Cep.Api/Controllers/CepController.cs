using Clima.Cep.Application.Commands.PersistZipCode;
using Clima.Cep.Application.Dtos;
using Clima.Cep.Application.Interfaces.Queries;
using Clima.Cep.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clima.Cep.Api.Controllers;

/// <summary>
/// Controller para gerenciar operações de CEP.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CepController(
    IMediator mediator,
    IZipCodeService zipCodeService,
    IZipCodeServiceQuery zipCodeServiceQuery,
    ILogger<CepController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ILogger<CepController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IZipCodeService _zipCodeService = zipCodeService ?? throw new ArgumentNullException(nameof(zipCodeService));
    private readonly IZipCodeServiceQuery _zipCodeServiceQuery = zipCodeServiceQuery ?? throw new ArgumentNullException(nameof(zipCodeServiceQuery));

    /// <summary>
    /// Consulta um CEP sem persistir
    /// </summary>
    /// <param name="zipCode">CEP a consultar (com ou sem hífen: 01001-000 ou 01001000).</param>
    /// <returns>Dados do endereço ou erro.</returns>
    /// <response code="200">CEP consultado com sucesso.</response>
    /// <response code="400">CEP inválido.</response>
    /// <response code="404">CEP não encontrado em nenhum provedor.</response>
    /// <response code="504">Timeout ao consultar provedores externos.</response>
    [HttpGet("{zipCode}", Name = "GetCep")]
    [ProducesResponseType(typeof(ZipCodeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status504GatewayTimeout)]
    public async Task<ActionResult<ZipCodeResponseDto>> GetCep(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            _logger.LogWarning("GET /cep: Invalid CEP - empty or null");
            return BadRequest(CreateProblemDetails(
                "https://errors.sua-api.com/invalid-cep",
                "CEP inválido",
                "CEP não pode estar vazio.",
                400));
        }

        try
        {
            _logger.LogInformation("GET /cep/{zipCode}: Consulting CEP", zipCode);
            var domain = new Domain.ValueObjects.ZipCode(zipCode);

            var result = await _zipCodeServiceQuery.GetByZipCodeAsync(domain) ?? 
                await _zipCodeService.GetZipCodeAsync(domain);

            if (result == null)
            {
                _logger.LogWarning("GET /cep/{zipCode}: CEP not found", zipCode);
                return NotFound(CreateProblemDetails(
                    "https://errors.sua-api.com/cep-not-found",
                    "CEP não encontrado",
                    $"CEP {zipCode} não foi encontrado em nenhum provedor.",
                    404));
            }

            _logger.LogInformation("GET /cep/{zipCode}: CEP found successfully", zipCode);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("GET /cep/{zipCode}: Invalid CEP format - {Error}", zipCode, ex.Message);
            return BadRequest(CreateProblemDetails(
                "https://errors.sua-api.com/invalid-cep",
                "CEP inválido",
                "CEP deve conter 8 dígitos.",
                400));
        }
        catch (HttpRequestException ex) when (ex.InnerException?.GetType().Name.Contains("TimeoutException") ?? false)
        {
            _logger.LogError(ex, "GET /cep/{zipCode}: Timeout", zipCode);
            return StatusCode(504, CreateProblemDetails(
                "https://errors.sua-api.com/external-timeout",
                "Timeout de provedor externo",
                "A consulta aos provedores externos excedeu o tempo limite.",
                504));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET /cep/{zipCode}: Unexpected error", zipCode);
            return StatusCode(500, CreateProblemDetails(
                "https://errors.sua-api.com/internal-error",
                "Erro interno",
                "Ocorreu um erro inesperado ao processar a requisição.",
                500));
        }
    }

    /// <summary>
    /// Consulta e persiste um novo CEP.
    /// </summary>
    /// <param name="request">Request com o CEP a ser consultado e persistido.</param>
    /// <returns>CEP persistido com ID e data de criação.</returns>
    /// <response code="201">CEP consultado e persistido com sucesso.</response>
    /// <response code="400">CEP inválido ou já persistido.</response>
    /// <response code="404">CEP não encontrado em nenhum provedor.</response>
    /// <response code="504">Timeout ao consultar provedores externos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ZipCodeLookupResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status504GatewayTimeout)]
    public async Task<ActionResult<ZipCodeLookupResponseDto>> CreateZipCodeLookup([FromBody] CreateZipCodeLookupRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.ZipCode))
        {
            _logger.LogWarning("POST /cep: Invalid request - empty CEP");
            return BadRequest(CreateProblemDetails(
                "https://errors.sua-api.com/invalid-cep",
                "CEP inválido",
                "CEP não pode estar vazio.",
                400));
        }

        try
        {
            _logger.LogInformation("POST /cep: Consulting and persisting CEP: {ZipCode}", request.ZipCode);
            var sendRequest = new PersistZipCodeCommand(request);
            var result = await _mediator.Send(sendRequest);

            _logger.LogInformation("POST /cep: CEP persisted successfully with ID: {Id}", result.Id);
            return CreatedAtRoute("GetCep", new { zipCode = result.ZipCode }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("POST /cep: Invalid CEP format - {Error}", ex.Message);
            return BadRequest(CreateProblemDetails(
                "https://errors.sua-api.com/invalid-cep",
                "CEP inválido",
                "CEP deve conter 8 dígitos.",
                400));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("POST /cep: {Error}", ex.Message);

            if (ex.Message.Contains("already been persisted"))
            {
                return BadRequest(CreateProblemDetails(
                    "https://errors.sua-api.com/duplicate-cep",
                    "CEP duplicado",
                    ex.Message,
                    400));
            }

            if (ex.Message.Contains("not found"))
            {
                return NotFound(CreateProblemDetails(
                    "https://errors.sua-api.com/cep-not-found",
                    "CEP não encontrado",
                    ex.Message,
                    404));
            }

            return StatusCode(500, CreateProblemDetails(
                "https://errors.sua-api.com/internal-error",
                "Erro ao processar",
                ex.Message,
                500));
        }
        catch (HttpRequestException ex) when (ex.InnerException?.GetType().Name.Contains("TimeoutException") ?? false)
        {
            _logger.LogError(ex, "POST /cep: Timeout");
            return StatusCode(504, CreateProblemDetails(
                "https://errors.sua-api.com/external-timeout",
                "Timeout de provedor externo",
                "A consulta aos provedores externos excedeu o tempo limite.",
                504));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST /cep: Unexpected error");
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
