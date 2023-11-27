using AwesomeEvents.Mappers;
using AwesomeEvents.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("EventsCs");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<EventsDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddAutoMapper(typeof(EventProfile).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1", 
        new OpenApiInfo
        {
            Title = "AwesomeEvents",
            Version = "v1",
            Contact = new OpenApiContact
            {
                Name = "João Paulo Hotequil",
                Email = "hotequil@protonmail.com",
                Url = new Uri("https://hotequil.tech")
            }
        }
    );

    var xmlFile = "AwesomeEvents.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    options.IncludeXmlComments(xmlPath);
});

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
