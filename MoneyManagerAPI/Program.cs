using System.Text;
using DAL;
using DAL.Repositories;
using Domain.Entities;
using Domain.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyManagerAPI.Helpers;
using MoneyManagerAPI.Middlewares;
using Serilog;
using Services.Interfaces;
using Services.Jobs;
using Services.Mapping;
using Services.RepositoryInterfaces;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("MoneyManagerDBConnectionString")
    ?? builder.Configuration.GetConnectionString("MoneyManagerDBConnectionString")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=MoneyManagerDB;Trusted_Connection=True;";
var hangfireConnectionString = Environment.GetEnvironmentVariable("HangfireDBConnectionString")
    ?? builder.Configuration.GetConnectionString("HangfireDBConnectionString")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=HangfireDB;Trusted_Connection=True;";


builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options
        .UseSqlServer(connectionString)
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

builder.Services.AddScoped<AnalyticJob>();

builder.Services.AddAutoMapper(typeof(AuthorizationMapperProfile), typeof(CategoryMapperProfile), typeof(TransactionMapperProfile), typeof(AnalyticMapperProfile));

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(hangfireConnectionString);
});

builder.Services.AddHangfireServer();
builder.Services.AddHttpClient();

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
    .WriteTo.Console()
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

RecurringJob.AddOrUpdate<AnalyticJob>(
    "WeeklyAnalyticsJob",
    job => job.Execute(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, CancellationToken.None),
    Cron.Weekly(DayOfWeek.Monday, 9),
    new RecurringJobOptions()
);

app.MapControllers();

await app.InitializeDbForIdentity();

app.Run();
