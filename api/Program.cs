using API.Models;
using API.Repositories;
using API.Services;
using NoopFormatter = API.Models.NoopFormatter;
using Python.Runtime;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<PythonService>();
            builder.Services.AddScoped<FilesService>();
            builder.Services.AddScoped<FileRepository>();
            var pythonConfig = builder.Configuration.GetSection(nameof(PythonConfig));
            builder.Services.Configure<PythonConfig>(pythonConfig);

            var app = builder.Build();

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
