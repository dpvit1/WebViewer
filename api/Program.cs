using API.Models;
using API.Repositories;
using API.Services;
using Python.Runtime;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<PythonService>();
            builder.Services.AddScoped<FilesService>();
            builder.Services.AddScoped<FileRepository>();
            var pythonConfig = builder.Configuration.GetSection(nameof(PythonConfig));
            builder.Services.Configure<PythonConfig>(pythonConfig);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                builder.Configuration.AddJsonFile("appsettings.Development.json", reloadOnChange: true, optional: true);
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DisplayOperationId();
                c.DisplayRequestDuration();
                c.EnableTryItOutByDefault();
                c.DefaultModelsExpandDepth(10);
            });

            app.UseHttpsRedirection();

            Runtime.PythonDLL = pythonConfig.Get<PythonConfig>()!.PythonPath;
            var tempDir = pythonConfig.Get<PythonConfig>()!.TempFilesDir;
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("Content-Disposition"));
            app.UseAuthorization();
            app.MapControllers();


            app.Run();
        }
    }
}
