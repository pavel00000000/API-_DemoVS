using MySql.Data.MySqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������� � ���������.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������� ����������� � ���� ������.
builder.Services.AddScoped<IDbConnection>(sp =>
    new MySqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� CORS ������ � ������������� �� ���������.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000") // ��������� ������� � ������ ����������� React ����������.
                             .AllowAnyHeader()
                             .AllowAnyMethod());
});

var app = builder.Build();

// ������������� �������� ��������� HTTP ��������.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // �������� ������������ ����������� ������ �� ����� wwwroot.

// ���������� CORS � ��������� ���������.
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
