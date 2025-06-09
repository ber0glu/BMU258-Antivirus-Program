using AntivirusProgram.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Controller ve Swagger servisleri ekleniyor
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// CORS politikasý tanýmlanýyor - tüm origin, method ve header izinli
builder.Services.AddCors(opt => opt.AddPolicy("MyAllowSpecificOrigins", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
}));

// Diðer servis konfigürasyonlarý
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureVirusTotalClient();

var app = builder.Build();

// Hata yakalama middleware’i
app.ConfigureExceptionHandler();

// CORS middleware’i Authorization’dan önce çaðrýlýyor
app.UseCors("MyAllowSpecificOrigins");

app.UseSwagger();
app.UseSwaggerUI();

// HTTPS yönlendirme
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
