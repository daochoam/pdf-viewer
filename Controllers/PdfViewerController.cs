using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Syncfusion.EJ2.PdfViewer;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.PdfToImageConverter;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Models;

namespace ej2_pdfviewer_service.Controllers
{

    /// <summary>
    /// Controlador para manejar la visualización de documentos PDF.
    /// </summary>
    [Route("api/pdf-viewer")]
    [ApiController]
    public class PdfViewerController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;


        /// <summary>
        /// Constructor del controlador PDF Viewer.
        /// </summary>
        /// <param name="hostingEnvironment">Interfaz del entorno de alojamiento web.</param>
        /// <param name="cache">Instancia de caché en memoria.</param>
        public PdfViewerController(IWebHostEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        /// <summary>
        /// Loads a PDF document into the viewer.
        /// </summary>
        /// <param name="request">Dictionary with the key "document" (Base64 or file name).</param>
        /// <returns>JSON with the details of the loaded document.</returns>
        [HttpPost("load")]
        public async Task<IActionResult> Load([FromBody] Dictionary<string, string> request)
        {
          PdfRenderer pdfviewer = new PdfRenderer(_cache);
          MemoryStream stream = new MemoryStream();
          object jsonResult;
          string cacheKey = Guid.NewGuid().ToString();

          if (request != null && request.ContainsKey("document") && !string.IsNullOrEmpty(request["document"]))
          {
              string? documentPath = request["document"];
              if (request.ContainsKey("isFileName") && bool.TryParse(request["isFileName"], out bool isFileName) && isFileName)
              {
                  string path = GetDocumentPath(documentPath);
                  if (!string.IsNullOrEmpty(path))
                  {
                      byte[] bytes = System.IO.File.ReadAllBytes(path);
                      stream = new MemoryStream(bytes);
                  }
                  else
                  {
                      // Verifica si es una URL remota válida
                      if (Uri.TryCreate(documentPath, UriKind.Absolute, out Uri uriResult) &&
                          (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                      {
                          try
                          {
                              // Descarga el archivo desde la URL remota
                              HttpClient client = new HttpClient();
                              byte[] pdfDoc = await client.GetByteArrayAsync(documentPath);
                              stream = new MemoryStream(pdfDoc);
                          }
                          catch (Exception ex)
                          {
                                return BadRequest("Error downloading the document from the URL: " + ex.Message);
                          }
                      }
                      else
                      {
                            return BadRequest("Error: The document path is not valid.");
                      }
                  }
              }
              else
              {
                  try
                  {
                      byte[] bytes = Convert.FromBase64String(documentPath);
                      stream = new MemoryStream(bytes);
                  }
                  catch (FormatException ex)
                  {
                        return BadRequest("Error: The document is not a valid Base64 string. " + ex.Message);
                  }
              }

              // Asegúrate de que el stream no esté vacío antes de almacenarlo en la caché
              if (stream.Length > 0)
              {
                  _cache.Set(cacheKey, stream.ToArray(), TimeSpan.FromMinutes(10));
              }
              else
              {
                    return BadRequest("Error: The document does not contain any data.");
              }
          }
          else
          {
                return BadRequest("Error: No valid document was received.");
          }

          request["document"] = cacheKey;

          // Asegúrate de que el stream no sea nulo antes de llamar al método Load
          if (stream.Length == 0)
          {
                return BadRequest("Error: The file does not contain any data.");
          }
          jsonResult = pdfviewer.Load(stream, request);
          return Ok(jsonResult);
        }

        /// <summary>
        /// Obtiene los marcadores de un documento PDF.
        /// </summary>
        [HttpPost("bookmarks")]
        public IActionResult Bookmarks([FromBody] Dictionary<string, string> request)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            var jsonResult = pdfviewer.GetBookmarks(request);
            return Ok(jsonResult);
        }

        /// <summary>
        /// Renderiza las páginas de un PDF.
        /// </summary>
        [HttpPost("render-pages")]
        public IActionResult RenderPdfPages([FromBody] Dictionary<string, string> request)
        {
          try
          {
              if (!request.ContainsKey("zoomFactor"))
              {
                  request["zoomFactor"] = "1";
              }
              PdfRenderer pdfviewer = new PdfRenderer(_cache);
              object jsonResult = pdfviewer.GetPage(request);
              return Ok(jsonResult);
          }
          catch (Exception ex)
          {
                return BadRequest($"Error processing the PDF: {ex.Message}\n{ex.StackTrace}");
          }
        }

        /// <summary>
        /// Obtiene miniaturas de las páginas del PDF.
        /// </summary>
        [HttpPost("thumbnails")]
        public IActionResult RenderThumbnailImages([FromBody] Dictionary<string, string> request)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object result = pdfviewer.GetThumbnailImages(request);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene comentarios de anotaciones en un PDF.
        /// </summary>
        [HttpPost("annotations")]
        public IActionResult RenderAnnotationComments([FromBody] Dictionary<string, string> request)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetAnnotationComments(request);
            return Ok(jsonResult);
        }

        /// <summary>
        /// Exporta anotaciones de un PDF.
        /// </summary>
        [HttpPost("export-annotations")]
        [Route("[controller]/ExportAnnotations")]
        public IActionResult ExportAnnotations([FromBody] Dictionary<string, string> request)
        {
            return Ok("ExportAnnotations method not implemented in PdfRenderer.");
        }

        /// <summary>
        /// Importa anotaciones de un archivo.
        /// </summary>
        [HttpPost("import-annotations")]
        public IActionResult ImportAnnotations([FromBody] Dictionary<string, string> request)
        {
            if (request != null && request.ContainsKey("fileName"))
            {
                string documentPath = GetDocumentPath(request["fileName"]);
                if (!string.IsNullOrEmpty(documentPath))
                {
                    return Ok(System.IO.File.ReadAllText(documentPath));
                }
                else
                {
                    return NotFound("Archivo no encontrado");
                }
            }
            return BadRequest("Error: No se proporcionó un archivo válido.");
        }

        private string GetDocumentPath(string document)
        {
            string documentPath = string.Empty;
            if (!System.IO.File.Exists(document))
            {
                var path = _hostingEnvironment.ContentRootPath;
                if (System.IO.File.Exists(path + "/Data/" + document))
                    documentPath = path + "/Data/" + document;
            }
            else
            {
                documentPath = document;
            }
            return documentPath;
        }

        /// <summary>
        /// Verifica el estado de la API.
        /// </summary>
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok("PDF Viewer API is running");
        }

    }
}
