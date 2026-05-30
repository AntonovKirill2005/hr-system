using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using rut_shop.net.dto;
using rut_shop.net.dto.request;
using rut_shop.net.dto.response;
using rut_shop.net.interfaces;

namespace rut_shop.net.api;

public static class CandidatesEndpoints
{
    /// <summary>
    /// Группа endpoint-ов для обработки кандидатов и откликов.
    /// </summary>
    public static RouteGroupBuilder MapCandidatesEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/candidates").WithTags("Candidates");

        group.MapGet("/", async (ICandidateService candidates, IMapper mapper) =>
            {
                var result = await candidates.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список кандидатов")
            .WithDescription("Возвращает список всех зарегистрированных соискателей в системе.")
            .Produces<IEnumerable<CandidateResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{candidateId:guid}",
                async (Guid candidateId, ICandidateService candidates, IMapper mapper) =>
            {
                var candidate = await candidates.GetByIdAsync(candidateId);
                return candidate is null
                    ? Results.NotFound(new ErrorResponse { Message = "Кандидат не найден." })
                    : Results.Ok(mapper.Map(candidate));
            })
            .WithSummary("Получить кандидата по идентификатору")
            .Produces<CandidateResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateCandidateRequest body, ICandidateService candidates, IMapper mapper) =>
            {
                try
                {
                    var created = await candidates.AddAsync(body);
                    return Results.Created($"/api/candidates/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Создать отклик кандидата")
            .WithDescription("Регистрирует новый отклик на существующую вакансию.")
            .Produces<CandidateResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{candidateId:guid}",
                async (Guid candidateId, UpdateCandidateRequest body, ICandidateService candidates, IMapper mapper) =>
            {
                try
                {
                    var updated = await candidates.UpdateAsync(candidateId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Кандидат не найден." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить данные кандидата")
            .WithDescription("Позволяет изменить персональные данные соискателя или обновить его статус отбора.")
            .Produces<CandidateResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{candidateId:guid}",
                async (Guid candidateId, ICandidateService candidates) =>
            {
                var deleted = await candidates.DeleteAsync(candidateId);
                return deleted
                    ? Results.NoContent()
                    : Results.NotFound(new ErrorResponse { Message = "Кандидат не найден." });
            })
            .WithSummary("Удалить кандидата")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}