using API.Models;
using API.Repositories;
using API.Services;

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

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

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
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllers();

            app.Run();
        }
    }
}
