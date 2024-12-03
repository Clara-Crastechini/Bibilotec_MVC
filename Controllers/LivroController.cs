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
        // Trabalhar com imagens:
        if(form.Files.Count > 0){
            // Primeiro passo:
                // Armazenaremos o arquivo enviado pelo usuario
                var arquivo = form.Files[0];
                
            // Segundo passo: 
                // Criar variave do caminho da minha pasta para colocar as fotos dos livros
                var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Livros");

                // Validaremos se a pasta que será armazenada as imagens, existe. Caso nao exista, criaremos uma nova pasta
                if(!Directory.Exists(pasta)){
                    // Criar pasta
                    Directory.CreateDirectory(pasta);
                }
            // Terceiro passo: 
                var caminho = Path.Combine(pasta, arquivo.FileName);
                using (var stream = new FileStream(caminho, FileMode.Create)){
                    //Copiou o arquivo para o meu diretorio
                    arquivo.CopyTo(stream);
                }

                novoLivro.Imagem = arquivo.FileName;
        } else{
            novoLivro.Imagem= "padrão.png";
        }



        // imagem 
        context.Livro.Add(novoLivro);
        context.SaveChanges();

        // Segunda parte é adicionar dentro de LivroCategorias a categoria que pertence ao novoLivro

        List<LivroCategoria> listaLivroCategorias = new List<LivroCategoria>();
        // Lista as categorias

        // Array que possui as categorias selecionadas pelo usuario 
        string[] categoriasSelecionadas = form["Categoria"].ToString().Split(',');

        foreach(string categoria in categoriasSelecionadas){
            // categoria possui a informacao do id da categoria ATUAL selecionada
            LivroCategoria livroCategoria = new LivroCategoria();
            livroCategoria.CategoriaID = int.Parse(categoria);
            livroCategoria.LivroID = novoLivro.LivroID;
            listaLivroCategorias.Add(livroCategoria);

        }

        // peguei a colecao da listaLivroCategorias e coloquei na tabela LivroCategoria
        context.LivroCategoria.AddRange(listaLivroCategorias);
        context.SaveChanges();

        return LocalRedirect("/Livro/Cadastro");
    }
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

