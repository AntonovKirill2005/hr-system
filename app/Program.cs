using Microsoft.EntityFrameworkCore;
using rut_shop.net.database;
using rut_shop.net.dto.request;
using rut_shop.net.interfaces;
using rut_shop.net.services;
using rut_shop.net.exceptions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Строка подключения не найдена.");

builder.Services.AddDbContext<HrDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IVacancyService, VacancyService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HrDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapGet("/", () => Results.Ok(new { 
    system = "RUT HR-API", 
    description = "Система управления вакансиями и откликами кандидатов" 
}));


var api = app.MapGroup("/api/vacancies");

// 1. CREATE
api.MapPost("/", async (CreateVacancyRequest request, IVacancyService vacancyService) =>
{
    var createdVacancy = await vacancyService.AddAsync(request);
    return Results.Created($"/api/vacancies/{createdVacancy.Id}", createdVacancy);
});

// 2. FIND ALL
api.MapGet("/", async (IVacancyService vacancyService) => 
{
    var vacancies = await vacancyService.GetAllAsync();
    return Results.Ok(vacancies);
});

// 3. FIND BY ID
api.MapGet("/{id:guid}", async (Guid id, IVacancyService vacancyService) =>
{
    var vacancy = await vacancyService.GetByIdAsync(id);
    return vacancy is null ? Results.NotFound(new { message = "Вакансия не найдена" }) : Results.Ok(vacancy);
});

// 4. UPDATE
api.MapPut("/{id:guid}", async (Guid id, UpdateVacancyRequest request, IVacancyService vacancyService) =>
{
    var updatedVacancy = await vacancyService.UpdateAsync(id, request);
    return updatedVacancy is null ? Results.NotFound(new { message = "Вакансия не найдена для обновления" }) : Results.Ok(updatedVacancy);
});

// 5. DELETE
api.MapDelete("/{id:guid}", async (Guid id, IVacancyService vacancyService) =>
{
    var isDeleted = await vacancyService.DeleteAsync(id);
    return isDeleted ? Results.NoContent() : Results.NotFound(new { message = "Вакансия не найдена для удаления" });
});



var candidateApi = app.MapGroup("/api/candidates");

// 1. CREATE КАНДИДАТА 
candidateApi.MapPost("/", async (CreateCandidateRequest request, ICandidateService candidateService) =>
{
    var createdCandidate = await candidateService.AddAsync(request);
    return Results.Created($"/api/candidates/{createdCandidate.Id}", createdCandidate);
});

// 2. FIND ALL КАНДИДАТОВ
candidateApi.MapGet("/", async (ICandidateService candidateService) =>
{
    var candidates = await candidateService.GetAllAsync();
    return Results.Ok(candidates);
});

// 3. FIND КАНДИДАТА BY ID
candidateApi.MapGet("/{id:guid}", async (Guid id, ICandidateService candidateService) =>
{
    var candidate = await candidateService.GetByIdAsync(id);
    return candidate is null ? Results.NotFound(new { message = "Кандидат не найден" }) : Results.Ok(candidate);
});

// 4. UPDATE КАНДИДАТА
candidateApi.MapPut("/{id:guid}", async (Guid id, UpdateCandidateRequest request, ICandidateService candidateService) =>
{
    var updatedCandidate = await candidateService.UpdateAsync(id, request);
    return updatedCandidate is null ? Results.NotFound(new { message = "Кандидат не найден для обновления" }) : Results.Ok(updatedCandidate);
});

// 5. DELETE КАНДИДАТА
candidateApi.MapDelete("/{id:guid}", async (Guid id, ICandidateService candidateService) =>
{
    var isDeleted = await candidateService.DeleteAsync(id);
    return isDeleted ? Results.NoContent() : Results.NotFound(new { message = "Кандидат не найден для удаления" });
});



app.MapPost("/api/applications", async (CreateApplicationRequest request, IVacancyService vacancyService) =>
{
    try
    {
        var application = await vacancyService.CreateApplicationAsync(request);
        
        if (application is null) 
            return Results.NotFound(new { message = "Вакансия или кандидат не найдены. Не удалось создать отклик." });

        return Results.Created($"/api/applications/{application.Id}", application);
    }
    catch (BusinessException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// СПИСОК ОТКЛИКОВ
app.MapGet("/api/applications", async (IVacancyService vacancyService) =>
{
    var applications = await vacancyService.GetApplicationsAsync();
    return Results.Ok(applications);
});




app.MapDelete("/api/applications/{id:guid}", async (Guid id, IVacancyService vacancyService) =>
{
    var isDeleted = await vacancyService.DeleteApplicationAsync(id);
    return isDeleted 
        ? Results.NoContent() 
        : Results.NotFound(new { message = "Отклик не найден для удаления" });
});


app.MapPut("/api/applications/{id:guid}", async (Guid id, UpdateApplicationStatusRequest request, IVacancyService vacancyService) =>
{
    var updatedApplication = await vacancyService.UpdateApplicationStatusAsync(id, request);
    return updatedApplication is null 
        ? Results.NotFound(new { message = "Отклик не найден для обновления" }) 
        : Results.Ok(updatedApplication);
});

await app.RunAsync();