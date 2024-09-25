using FluentValidation;

using PaymentGateway.Api.Interfaces.Repositories;
using PaymentGateway.Api.Interfaces.Services;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.ServiceClients;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Validators;

using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddScoped<IPaymentsService, PaymentsService>()
    .AddScoped<ICurrencyService, CurrencyService>()
    .AddScoped<IValidator<PostPaymentRequest>, PostPaymentRequestValidator>()
    .AddSingleton<IPaymentsRepository, InMemoryPaymentsRepository>()
    .AddRefitClient<IAcquiringBankServiceClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:8080"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
