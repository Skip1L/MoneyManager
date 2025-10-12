using DAL;
using DAL.Configs;
using DAL.Extensions;
using DAL.Repositories;
using Domain.Entities;
using Domain.Helpers;
using DTOs.Exchanges;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyManagerAPI.Configs;
using MoneyManagerAPI.Extensions;
using MoneyManagerAPI.Helpers;
using MoneyManagerAPI.Middlewares;
using Serilog;
using Services.Interfaces;
using Services.Jobs;
using Services.Jobs.Constants;
using Services.Mapping;
using Services.Protos;
using Services.RepositoryInterfaces;
using Services.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables("MONEYMANAGER_");

builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<NotificationServiceConfig>(builder.Configuration.GetSection("NotificationService"));
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("Auth"));
builder.Services.Configure<AdminConfig>(builder.Configuration.GetSection("Admin"));

var databaseConfig = builder.Configuration.GetSection("Database").Get<DatabaseConfig>();
var notificationConfig = builder.Configuration.GetSection("NotificationService").Get<NotificationServiceConfig>();
var authConfig = builder.Configuration.GetSection("Auth").Get<AuthConfig>();
var adminConfig = builder.Configuration.GetSection("Admin").Get<AdminConfig>();

AuthOptionsHelper.Initialize(builder.Configuration);

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options
        .UseSqlServer(databaseConfig.MoneyManagerDB)
        .UseLazyLoadingProxies();
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAnalyticService, AnalyticService>();

builder.Services.AddScoped<GrpcWeeklyAnalyticJob>();
builder.Services.AddScoped<HttpWeeklyAnalyticJob>();
builder.Services.AddScoped<KafkaWeeklyAnalyticJob>();
builder.Services.AddScoped<RabbitMqWeeklyAnalyticJob>();

builder.Services.AddAutoMapper(typeof(AuthorizationMapperProfile),
                               typeof(CategoryMapperProfile),
                               typeof(TransactionMapperProfile),
                               typeof(AnalyticMapperProfile),
                               typeof(GrpcMappingProfile));

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(databaseConfig.HangfireDB);
});

builder.Services.AddHangfireServer();

await builder.EnsureTopicExistsAsync(notificationConfig.KafkaUri, "analytic-email-created");

builder.Services.AddMassTransit(opt =>
{
    opt.SetKebabCaseEndpointNameFormatter();

    opt.UsingRabbitMq((context, config) =>
    {
        config.Host(notificationConfig.RabbitMQUri, "/", conf =>
        {
            conf.Username(notificationConfig.RabbitMQUsername);
            conf.Password(notificationConfig.RabbitMQPassword);
        });
        config.ConfigureEndpoints(context);
    });

    opt.AddRider(rider =>
    {
        rider.AddProducer<string, AnalyticEmailCreated>("analytic-email-created");

        rider.UsingKafka((ctx, k) =>
        {
            k.Host(notificationConfig.KafkaUri);
        });
    });
});

builder.Services.AddGrpcClient<GrpcEmail.GrpcEmailClient>(client =>
{
    client.Address = new Uri(notificationConfig.GrpcUri);
}).AddCallCredentials((context, metadata) =>
    {
        if (!string.IsNullOrEmpty(authConfig.NotificationApiKey))
        {
            metadata.Add("x-api-key", authConfig.NotificationApiKey);
        }

        return Task.CompletedTask;
    });

builder.Services.AddHttpClient(HttpClientNames.NotificationServiceName, client =>
{
    client.BaseAddress = new Uri(notificationConfig.HttpUri);
    client.DefaultRequestHeaders.Add("x-api-key", authConfig.NotificationApiKey);
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<AuthHeaderOperationHeader>();

    c.SwaggerDoc("v1", new OpenApiInfo());

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token",
    });
});

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.Debug()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
    builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("Content-Disposition")
        .AllowCredentials()
        .SetIsOriginAllowed(isOriginAllowed: _ => true);
}));

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
}).AddEntityFrameworkStores<ApplicationContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = AuthOptionsHelper.GetIssuer(),
        ValidAudience = AuthOptionsHelper.GetAudience(),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptionsHelper.GetSecretKey())),
        RequireExpirationTime = true
    };
});

var app = builder.Build();

await app.Services.EnsureDatabasesAndMigrationsAppliedAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseMiddleware<ExceptionsMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.MapHealthChecks("/health");

app.Services.AddRecurringJobs();

await app.InitializeDbForIdentity(adminConfig);

app.Run();
