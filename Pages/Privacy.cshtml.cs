using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace pdf_viewer.Pages;

/// <summary>
/// Privacy model to capture information about privacy policy.
/// </summary>
public class PrivacyModel : PageModel
{
    /// <summary>
    /// Unique identifier for the request.
    /// </summary>
    private readonly ILogger<PrivacyModel> _logger;

    /// <summary>
    /// Indicates whether the RequestId should be displayed.
    /// </summary>
    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the GET request for the privacy page.
    /// </summary>
    public void OnGet()
    {
    }
}

