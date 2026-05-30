using rut_shop.net.dto;
using rut_shop.net.dto.request;
using rut_shop.net.dto.response;
using rut_shop.net.interfaces;

namespace rut_shop.net.api;

public static class VacanciesEndpoints
{
   
    public static RouteGroupBuilder MapVacanciesEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/vacancies").WithTags("Vacancies");

        group.MapGet("/", async (IVacancyService vacancies, IMapper mapper) =>
            {
                var result = await vacancies.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список вакансий")
            .WithDescription("Возвращает список всех доступных вакансий организации.")
            .Produces<IEnumerable<VacancyResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{vacancyId:guid}",
                async (Guid vacancyId, IVacancyService vacancies, IMapper mapper) =>
            {
                var vacancy = await vacancies.GetByIdAsync(vacancyId);
                return vacancy is null
                    ? Results.NotFound(new ErrorResponse { Message = "Вакансия не найдена." })
                    : Results.Ok(mapper.Map(vacancy));
            })
            .WithSummary("Получить вакансию по идентификатору")
            .Produces<VacancyResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateVacancyRequest body, IVacancyService vacancies, IMapper mapper) =>
            {
                try
                {
                    var created = await vacancies.AddAsync(body);
                    return Results.Created($"/api/vacancies/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить вакансию")
            .Produces<VacancyResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{vacancyId:guid}",
                async (Guid vacancyId, UpdateVacancyRequest body, IVacancyService vacancies, IMapper mapper) =>
            {
                try
                {
                    var updated = await vacancies.UpdateAsync(vacancyId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Вакансия не найдена." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить вакансию")
            .Produces<VacancyResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{vacancyId:guid}",
                async (Guid vacancyId, IVacancyService vacancies) =>
            {
                try
                {
                    var deleted = await vacancies.DeleteAsync(vacancyId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Вакансия не найдена." });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить вакансию")
            .WithDescription("Удаляет вакансию, если на неё отсутствуют отклики кандидатов.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}