using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace pdf_viewer.Pages;

/// <summary>
/// Index model for the main page of the PDF viewer application.
/// </summary>
public class IndexModel : PageModel
{
    /// <summary>
    /// Unique identifier for the request.
    /// </summary>
    private readonly ILogger<IndexModel> _logger;

    /// <summary>
    /// Index model constructor.
    /// </summary>
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the GET request for the index page.
    /// </summary>
    public void OnGet()
    {

    }
}
