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
using DotNetEnv;

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
    // Configura el puerto HTTP & HTTPS
    if (int.TryParse(Environment.GetEnvironmentVariable("LISTEN_PORT"), out int port))
    {
        options.ListenLocalhost(port, listenOptions =>
        {
            listenOptions.UseHttps(httpsOptions =>
            {
                //httpsOptions.ServerCertificate = new X509Certificate2("certificado.pfx", "password");
            });
        });
    }
    else
    {
        options.ListenLocalhost(5000, listenOptions =>
        {
            listenOptions.UseHttps(httpsOptions =>
            {
                //httpsOptions.ServerCertificate = new X509Certificate2("certificado.pfx", "password");
            });
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
        c.RoutePrefix = string.Empty; // Acceder a Swagger en la raíz (`/`)
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
