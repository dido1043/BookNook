using Microsoft.AspNetCore.Mvc;

namespace BookNook.Controllers;

public class ClientController : Controller
{

    public IActionResult Register()
    {
        return RedirectToPage("/Account/Register", new { area = "Identity" });
    }
}