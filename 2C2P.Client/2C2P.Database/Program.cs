using Microsoft.AspNetCore.Mvc;
using _2C2P.Database.Services;
using _2C2P.Database.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITransactionServices, TransactionServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();


app.MapPost("/query/transactions", async (
    [FromBody] TransactionRequest trans, ITransactionServices transactionServices) =>
{

    var transactions = await transactionServices.QueryTransactions(trans.Currency, trans.StartDate, trans.EndDate, trans.Status);

    return transactions;

}).WithTags(new string[] { "Query Transactions" })
.WithName("Transactions");

app.MapPost("/query/import-transactions", async (
    [FromBody] ImportTransactions trans, ITransactionServices transactionServices) =>
{

    var transactions = await transactionServices.ImportTransactions(trans.fileName, trans.fileFormat, trans.data);

    return Results.Json(transactions);

}).WithTags(new string[] { "Query Transactions" })
.WithName("ImportTransactions");

app.MapGet("/error", () => "An error occurred.");
Console.WriteLine("Running Now");

app.Run();
