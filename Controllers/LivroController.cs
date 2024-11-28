using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bibliotec.Models;
using Bibliotec.Contexts;

namespace Bibliotec.Controllers;

public class LivroController : Controller
{
    private readonly ILogger<LivroController> _logger;

    public LivroController(ILogger<LivroController> logger)
    {
        _logger = logger;
    }

    Context context = new Context();
    public IActionResult Index()
    {
        //APARECE A MINHA Livro

        ViewBag.Admin = HttpContext.Session.GetString("Admin")!;

        // Criar uma lista de livros
        List<Livro> listalivros = context.Livro.ToList();

        // Verificar se o livro tem reserva ou nao
        var livrosReservados = context.LivroReserva.ToDictionary(livro => livro.LivroID, livror => livror.DtReserva);

        ViewBag.Livros = listalivros;
        ViewBag.LivrosComReserva = livrosReservados;
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
