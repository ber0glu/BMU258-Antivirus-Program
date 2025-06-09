using AntivirusProgram.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Controller ve Swagger servisleri ekleniyor
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// CORS politikas� tan�mlan�yor - t�m origin, method ve header izinli
builder.Services.AddCors(opt => opt.AddPolicy("MyAllowSpecificOrigins", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
}));

// Di�er servis konfig�rasyonlar�
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureVirusTotalClient();

var app = builder.Build();

// Hata yakalama middleware�i
app.ConfigureExceptionHandler();

// CORS middleware�i Authorization�dan �nce �a�r�l�yor
app.UseCors("MyAllowSpecificOrigins");

app.UseSwagger();
app.UseSwaggerUI();

// HTTPS y�nlendirme
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
