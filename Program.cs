using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.IO.Compression;
using Syncfusion.Licensing;
using System.Security.Cryptography.X509Certificates;

// Cargar variables desde .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Registrar la licencia de Syncfusion
var licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
if (!string.IsNullOrEmpty(licenseKey))
{
    SyncfusionLicenseProvider.RegisterLicense(licenseKey);
}

// Configuración de servicios
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
});

builder.Services.AddMemoryCache();
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PDF Viewer API",
        Version = "v1",
        Description = "API para manejar la visualización de documentos PDF."
    });

    var xmlPath = Path.Combine(AppContext.BaseDirectory, "api-docs.xml");
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configuración de Kestrel para archivos grandes
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50 MB

    // Verificar el entorno
    var environment = (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development").ToLower();
    var PORT = int.TryParse(Environment.GetEnvironmentVariable("LISTEN_PORT"), out var port) ? port : 5000;
    var certificatePath = Environment.GetEnvironmentVariable("CERTIFICATE_PATH");
    if (string.IsNullOrEmpty(certificatePath))
    {
        certificatePath = Directory.GetFiles(AppContext.BaseDirectory, "*.pfx").FirstOrDefault();
        if (certificatePath != null)
        {
            Environment.SetEnvironmentVariable("CERTIFICATE_PATH", certificatePath);
        }
    }

    if (environment == "development" && !string.IsNullOrEmpty(certificatePath))
    {
        options.ListenLocalhost(PORT, listenOptions =>
        {
            listenOptions.UseHttps(httpsOptions =>
            {
                httpsOptions.ServerCertificate = new X509Certificate2(certificatePath,Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD"));
            });
        });
    }
    else if (environment == "production" && !string.IsNullOrEmpty(certificatePath))
    {
        options.ListenLocalhost(443, listenOptions =>
        {
            listenOptions.UseHttps(httpsOptions =>
            {
                httpsOptions.ServerCertificate = new X509Certificate2(certificatePath,Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD"));
            });
        });
    }
    else
    {
        options.ListenLocalhost(5000, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    }
});

var app = builder.Build();

// Configuración de middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PDF Viewer API v1");
        c.RoutePrefix = string.Empty;
    });

    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection(); 
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseResponseCompression();
app.UseRouting();
app.UseCors("MyPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
