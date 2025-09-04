using Microsoft.OpenApi.Models;
using Stamp.Application.Interfaces;
using Stamp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder( args );

// اضافه کردن سرویس‌های پایه
builder.Services.AddControllers( );
builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc( "v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    } );
} );

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>( );

var app = builder.Build( );

// فعال‌سازی Swagger
if( app.Environment.IsDevelopment( ) )
{
    app.UseSwagger( );
    app.UseSwaggerUI( c =>
    {
        c.SwaggerEndpoint( "/swagger/v1/swagger.json", "My API V1" );
    } );
}

app.UseHttpsRedirection( );
app.UseAuthorization( );
app.MapControllers( );

app.Run( );
