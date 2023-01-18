using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Alan.Identity;
using BillingService.Database;
using BillingService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// отключаем CORS
builder.Services.AddCors(options =>
{
    // разрешить все
    options.AddPolicy("AllowAllPolicy",
        builder =>
        {
            builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowAnyOrigin();
            //.AllowCredentials();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
         {
               new OpenApiSecurityScheme
                 {
                     Reference = new OpenApiReference
                     {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                     }
                 },
                 new string[] {}
         }
     });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters =
                Jwt.GetValidationParameters(builder.Configuration["Access:Issuer"],
                                            builder.Configuration["Access:Audience"],
                                            bool.Parse(builder.Configuration["Access:ValidateLifetime"]),
                                            builder.Configuration["Access:Key"]));



// -------- настраиваем строку подключения
string connectionString = builder.Configuration.GetConnectionString("ConnectionString");

// ------- подключаем сервисы
builder.Services.AddSingleton<IConnectionString>(new ConnectionStringService(connectionString));

builder.Services.AddDbContext<AppDbContext>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWTAuthDemo v1"));
}

app.UseCors("AllowAllPolicy");
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();