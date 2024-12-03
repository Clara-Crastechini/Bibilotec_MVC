using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bibliotec.Models;
using Bibliotec.Contexts;

namespace Bibliotec.Controllers;

[Route("[controller]")]
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
        // ToDictionary (chave, valor)
        var livrosReservados = context.LivroReserva.ToDictionary(livro => livro.LivroID, livror => livror.DtReserva);

        ViewBag.Livros = listalivros;
        ViewBag.LivrosComReserva = livrosReservados;
        
        return View();
    }

    [Route("Cadastro")]
    // Metodo que retorna a tela de cadastro

    public IActionResult Cadastro(){
        ViewBag.Admin = HttpContext.Session.GetString("Admin")!;

        ViewBag.Categorias = context.Categoria.ToList(); 

        return View();
    }


    // Metodo para cadastrar um livro
    [Route("Cadastrar")]

    public IActionResult Cadastrar(IFormCollection form){
        Livro novoLivro = new Livro();
        // O que meu usuario escrever no formulario sera atribuido ao novoLivro

        novoLivro.Nome = form["Nome"].ToString();
        novoLivro.Descricao = form["Descricao"].ToString();
        novoLivro.Editora = form["Editora"].ToString();
        novoLivro.Escritor = form["Escritor"].ToString();
        novoLivro.Idioma = form["Idioma"].ToString();

        // imagem 
        context.Livro.Add(novoLivro);
        context.SaveChanges();
        return RedirectToAction("Cadastro");
    }


    // public IActionResult Privacy()
    // {
    //     return View();
    // }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
