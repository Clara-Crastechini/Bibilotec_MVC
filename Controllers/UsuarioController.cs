using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bibliotec.Models;
using Bibliotec.Contexts;

namespace Bibliotec.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger)
    {
        _logger = logger;
    }

    // Criando um obj da classe:
    Context context = new Context();

    // O metodo retornando a view Usuario/Index.cshtml
    public IActionResult Index()
    {
        //Pegar as informações da session que são necessarias para que aparece os detalhes do meu usuario
        int Id = int.Parse(HttpContext.Session.GetString("UsuarioID")!);
        ViewBag.Admin = HttpContext.Session.GetString("Admin")!;

        // id = 1
        Usuario usuarioencontrado = context.Usuario.FirstOrDefault(usuario => usuario.UsuarioID == Id)!;

        // Se nao for encontrado ninguem
        if (usuarioencontrado == null){
            return NotFound();
        }

        // Procurar o curso que meu usuario esta cadastrado

        // Tabela Usuario -> FK CursoID
        // Tabela Curso -> PK CursoID

        Curso cursoencontrado = context.Curso.FirstOrDefault(curso => curso.CursoID == usuarioencontrado.CursoID)!;


        // Verificar se o usuario possui ou nao o curso

        if(cursoencontrado == null){
            // Preciso que vc mande essa mensagem pra view
           ViewBag.Curso = "O usuario não possui curso cadastrado";
        }else{
            // Preciso que mande o nome do curso pra View;
            ViewBag.Curso = cursoencontrado.Nome;
        }


        ViewBag.Nome = usuarioencontrado.Nome;
        ViewBag.Email = usuarioencontrado.Email;
        ViewBag.Contato = usuarioencontrado.Contato;
        ViewBag.DtNascimento = usuarioencontrado.DtNascimento.ToString("dd/MM/yyyy");

        


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
