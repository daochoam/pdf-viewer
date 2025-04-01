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
        [AcceptVerbs("Post")]
        [HttpPost("Load")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Load")]
        public async Task<IActionResult> Load([FromBody] Dictionary<string, string> request)
        {
          PdfRenderer pdfviewer = new PdfRenderer(_cache);
          MemoryStream stream = new MemoryStream();
          object jsonResult;
          string cacheKey = Guid.NewGuid().ToString();

          if (request != null && request.ContainsKey("document") && !string.IsNullOrEmpty(request["document"]))
          {
              string documentPath = request["document"] ?? string.Empty;
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
                      // Check if it is a valid remote URL
                      if (!string.IsNullOrEmpty(documentPath) && Uri.TryCreate(documentPath, UriKind.Absolute, out Uri? uriResult) &&
                          (uriResult?.Scheme == Uri.UriSchemeHttp || uriResult?.Scheme == Uri.UriSchemeHttps))
                      {
                          try
                          {
                              // Download the file from the remote URL
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

              // Ensure the stream is not empty before storing it in the cache
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

          // Ensure the stream is not null before calling the Load method
          if (stream.Length == 0)
          {
                return BadRequest("Error: The file does not contain any data.");
          }
          jsonResult = pdfviewer.Load(stream, request);
          return Ok(jsonResult);
        }

        /// <summary>
        /// Unloads the PDF document and clears the cache.
        /// </summary>
        [AcceptVerbs("Post")]
        [HttpPost("Unload")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Unload")]
        public IActionResult Unload([FromBody] Dictionary<string, string> request)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            pdfviewer.ClearCache(request);
            return this.Content("Document cache is cleared");
        }

        /// <summary>
        /// Downloads the PDF document as a Base64 string.
        /// </summary>
        /// <param name="request">Dictionary with the key "document" (Base64 or file name).</param>
        /// <returns>Base64 string of the PDF document.</returns>
        /// <remarks>
        /// This method is used to download the PDF document as a Base64 string.
        /// The document can be either a file name or a Base64 string.
        /// </remarks>
        [AcceptVerbs("Post")]
        [HttpPost("Download")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Download")]
        public IActionResult Download([FromBody] Dictionary<string, string> request)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string documentBase = pdfviewer.GetDocumentAsBase64(request);
            return Content(documentBase);
        }
        
        /// <summary>
        /// Prints the PDF document and returns the print images.
        /// </summary>
        /// <param name="request">Dictionary with the key "document" (Base64 or file name).</param>
        /// <returns>Base64 string of the print images.</returns>
        /// <remarks>
        /// This method is used to print the PDF document.
        /// The document can be either a file name or a Base64 string.
        /// </remarks>
        [AcceptVerbs("Post")]
        [HttpPost("PrintImages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/PrintImages")]
        public IActionResult PrintImages([FromBody] Dictionary<string, string> request)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object pageImage = pdfviewer.GetPrintImage(request);
            return Content(JsonConvert.SerializeObject(pageImage));
        }

        /// <summary>
        /// Retrieves the bookmarks of a PDF document.
        /// </summary>
        [AcceptVerbs("Post")]
        [HttpPost("Bookmarks")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Bookmarks")]
        public IActionResult Bookmarks([FromBody] Dictionary<string, string> request)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            var jsonResult = pdfviewer.GetBookmarks(request);
            return Ok(jsonResult);
        }

        /// <summary>
        /// Renders the pages of a PDF.
        /// </summary>
        [AcceptVerbs("Post")]
        [HttpPost("RenderPdfPages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderPdfPages")]
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
        /// Renders the text of a PDF.
        /// </summary>
        /// <remarks>
        /// This method is used to render the text of a PDF document.
        /// The document can be either a file name or a Base64 string.
        /// </remarks>
        [AcceptVerbs("Post")]
        [HttpPost("RenderPdfTexts")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderPdfTexts")]
        public IActionResult RenderPdfTexts([FromBody] Dictionary<string, string> request)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetDocumentText(request);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        /// <summary>
        /// Retrieves thumbnails of the PDF pages.
        /// </summary>
        [AcceptVerbs("Post")]
        [HttpPost("RenderThumbnailImages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderThumbnailImages")]
        public IActionResult RenderThumbnailImages([FromBody] Dictionary<string, string> request)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object result = pdfviewer.GetThumbnailImages(request);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves annotation comments from a PDF.
        /// </summary>
        [AcceptVerbs("Post")]
        [HttpPost("RenderAnnotationComments")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderAnnotationComments")]
        public IActionResult RenderAnnotationComments([FromBody] Dictionary<string, string> request)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetAnnotationComments(request);
            return Ok(jsonResult);
        }

        /// <summary>
        /// Exports annotations from a PDF.
        /// </summary>
        [AcceptVerbs("Post")]
        [HttpPost("ExportAnnotations")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ExportAnnotations")]
        public IActionResult ExportAnnotations([FromBody] Dictionary<string, string> request)
        {
            return Ok("ExportAnnotations method not implemented in PdfRenderer.");
        }

        /// <summary>
        /// Imports annotations from a file.
        /// </summary>
        [AcceptVerbs("Post")]
        [HttpPost("ImportAnnotations")]
        [Route("[controller]/ImportAnnotations")]
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
                    return NotFound("File not found");
                }
            }
            return BadRequest("Error: No valid file was provided.");
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
        /// Checks the status of the API.
        /// </summary>
        [AcceptVerbs("Get")]
        [HttpGet("version")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/GetStatus")]
        public IActionResult GetVersion()
        {

            var pdfNetCoreVersion = typeof(Syncfusion.Pdf.PdfDocument).Assembly.GetName().Version?.ToString();
            var syncfusionVersion = typeof(Syncfusion.EJ2.PdfViewer.PdfRenderer).Assembly.GetName().Version?.ToString();
            return Ok($"PDF Viewer API is running Syncfusion version {pdfNetCoreVersion}.");
        }
    }
}
