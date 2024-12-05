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




    [Route("Editar/{id}")]
    public IActionResult Editar(int id ){
        ViewBag.Admin = HttpContext.Session.GetString("Admin")!;

         ViewBag.CategoriasDoSistema = context.Categoria.ToList(); 

        // Buscar o id
        Livro   livroAtualizado = context.Livro.FirstOrDefault(livro => livro.LivroID == id)!;

        // Buscar as categorias que o   livroAtualizado possui
        var categoriasDolivroAtualizado = context.LivroCategoria.Where(identificadorLivro => identificadorLivro.LivroID == id)
        .Select(livro => livro.Categoria).ToList();


        // Quero pegar informacoes do meu livro e mandar para minha View
        ViewBag.Livro = livroAtualizado;
        ViewBag.Categoria = categoriasDolivroAtualizado;
        return View();
    }


    // Metodo que a atualiza as informacoes do livro

    [Route("Atualizar/{id}")]

    public IActionResult Atualizar(IFormCollection form, int id, IFormFile imagem){
        // Buscar um livro especifico pelo id
        Livro livroAtualizado = context.Livro.FirstOrDefault(livro => livro.LivroID == id)!;

        livroAtualizado.Nome = form["Nome"];
        livroAtualizado.Escritor = form["Escritor"];
        livroAtualizado.Idioma = form["Idioma"];
        livroAtualizado.Editora = form["Editora"];
        livroAtualizado.Descricao = form["Descricao"];

        // Upload de imagem
        if(imagem != null && imagem.Length > 0){
            // Definir o caminho da minha imagem do livro Atual, que eu quero alterar:
            var caminhoImagem = Path.Combine("wwwroot/images/Livros", imagem.FileName);

            // Verificar se o usuario colocou uma imagem para atualizar o livro
            if(!string.IsNullOrEmpty(livroAtualizado.Imagem)){
            // caso exista, ela irá ser apagada
            var caminhoImagemAntiga = Path.Combine("wwwroot/images/Livros", livroAtualizado.Imagem);
            // Ver se existe uma imagem no caminho antigo
            if(System.IO.File.Exists(caminhoImagemAntiga)){
                System.IO.File.Delete(caminhoImagemAntiga);
            }
            }

            using(var stream = new FileStream(caminhoImagem, FileMode.Create)){
                imagem.CopyTo(stream);
            }

            // Subir essa mudanca para o meu banco de dados
            livroAtualizado.Imagem = imagem.FileName;
        }

        // Primeiro pegamos as categorias selecionadas do usuario
        var categoriasSelecionadas = form["Categoria"].ToList();

        // Segundo, pegaremos as categorias atuais do Livro
        var categoriasAtuais = context.LivroCategoria.Where(livro => livro.LivroID == id).ToList();

        // Terceiro, removeremos as categorias antigas
        foreach(var categoria in categoriasAtuais){
            if(!categoriasSelecionadas.Contains(categoria.CategoriaID.ToString())){
                // Nos removeremos a categoria do nosso context
                context.LivroCategoria.Remove(categoria);
            }
        }
        
        // Quarto, adicionaremos as novas categorias
        foreach(var categoria in categoriasSelecionadas){
            if(!categoriasAtuais.Any(c => c.CategoriaID.ToString() == categoria)){
                context.LivroCategoria.Add(new LivroCategoria{
                    CategoriaID = int.Parse(categoria),
                    LivroID = id
                });
                
            }
        }

            context.SaveChanges();

            return LocalRedirect("/Livro");
    }



    [Route("Excluir/{id}")]

    public IActionResult Excluir(int id){
        // Buscar qual o livro do id que precisamos excluir
        Livro livroEncontrado = context.Livro.First(livro => livro.LivroID == id);


        // Buscar as categorias desse livro
        var categoriasDoLivro = context.LivroCategoria.Where(livro => livro.LivroID == id).ToList();


        // Precisa excluir primeiro o registro da tabela intermediaria
        foreach(var categoria in categoriasDoLivro){
            context.LivroCategoria.Remove(categoria);
        }

        context.Livro.Remove(livroEncontrado);

        context.SaveChanges();

        return LocalRedirect("/Livro");
    }
}



