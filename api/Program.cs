using API.Models;
using API.Repositories;
using API.Services;
using Python.Runtime;
using System.Diagnostics;
using NoopFormatter = API.Models.NoopFormatter;

namespace API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--worker")
            {
                return RunAsWorker(args);
            }
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

            return 0;
        }
        private static int RunAsWorker(string[] args)
        {
            if (args.Length != 4)
            {
                Console.Error.WriteLine("Usage: --worker <PythonHome> <UserCode> <FileName>");
                return 1;
            }

            string pythonHome = args[1];
            string userCode = args[2];
            string fileName = args[3];

            RuntimeData.FormatterType = typeof(NoopFormatter);
            Runtime.PythonDLL = Path.Combine(pythonHome, "DLLs",  "python39.dll");
            PythonEngine.PythonHome = pythonHome;
            PythonEngine.PythonPath = string.Join(
                Path.PathSeparator.ToString(),
                $"{pythonHome}\\site-packages",
                $"{pythonHome}\\DLLs",
                pythonHome
            );

            PythonEngine.Initialize();
            var threadState = PythonEngine.BeginAllowThreads();


            try
            {
                using (Py.GIL())
                {
                    PythonEngine.RunSimpleString(@"
import builtins
import clr
from System import Console

def write_to_cs(*args, sep=' ', end='\n', **kwargs):
    text = sep.join(str(a) for a in args)
    Console.Write(text + end)

builtins.print = write_to_cs
");

                    dynamic rgkGLTFConvertLib = Py.Import("RGKGLTF");

                    try
                    {
                        rgkGLTFConvertLib.UserCodeToGLTF(userCode, fileName);
                    }
                    catch (PythonException Ex)
                    {
                        Console.Error.WriteLine($"PythonException: {Ex.Message}\n{Ex.StackTrace}");
                        return 2;
                    }
                    finally
                    {
                        rgkGLTFConvertLib.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"WorkerException: {ex}");
                return 3;
            }
            finally
            {
                PythonEngine.EndAllowThreads(threadState);
                PythonEngine.Shutdown();
            }

            return 0;
        }
    }
}
