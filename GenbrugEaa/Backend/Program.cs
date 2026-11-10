using Backend.Repositories;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Henter MongoDB-forbindelsen fra appsettings
string connectionString = builder.Configuration["MongoDb:ConnectionString"] ?? throw new InvalidOperationException("MongoDB-forbindelsen mangler");

// Registrerer MongoDB, så repositories kan bruge forbindelsen
builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

// Registrerer alle vores repositories
builder.Services.AddScoped<AdRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserLoginRepository>();
builder.Services.AddScoped<ShoppingListRepository>();
builder.Services.AddScoped<PurchaseRequestRepository>();

// Registrerer controllers og Swagger
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

// Swagger er kun tilgængeligt under udvikling
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();