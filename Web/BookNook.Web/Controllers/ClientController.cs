using System.Security.Claims;
using BookNook.Services.Data.Service;
using BookNook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNook.Controllers;

public class ClientController : Controller
{
    private readonly ClientService _clientService;
    private readonly OrderService _orderService;

    public ClientController(ClientService clientService, OrderService orderService)
    {
        _clientService = clientService;
        _orderService = orderService;
    }

    public IActionResult Register()
    {
        return RedirectToPage("/Account/Register", new { area = "Identity" });
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        var client = await _clientService.GetByIdAsync(currentUserId);
        if (client == null) return NotFound();

        var allOrders = await _orderService.GetAllAsync(null, null, null, null);
        var orders = allOrders
            .Where(o => o.ClientId == currentUserId)
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        var viewModel = new ProfileViewModel
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            DeliveryAddress = client.DeliveryAddress,
            Role = User.IsInRole("Admin") ? "Admin" : "Client",
            Orders = orders
        };

        return View(viewModel);
    }
}
