using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;
using static Vanisher.Core.Context;

namespace Vanisher.Core;

public record ListagemPorLoja(string nome, decimal total, List<LinhaDocumento> documentos);
public class ListaPorLoja
{
    private readonly List<LinhaDocumento> lista;

    public ListaPorLoja(List<LinhaDocumento> lista)
    {
        this.lista = lista;
    }
    public List<ListagemPorLoja> Listar()
    {
        var lojas = lista
            .Select(x => x.NomeLoja)
            .Distinct();
        List<ListagemPorLoja> listagem = new List<ListagemPorLoja>();
        foreach (string nomeLoja in lojas)
        {
            var lojaDoJoao = lista.Where(x => x.NomeLoja == nomeLoja).ToList();
            decimal total = lojaDoJoao.Sum(x => x.Valor);
            listagem.Add(new ListagemPorLoja(nomeLoja, total, lojaDoJoao));
        }
        return listagem;
    }
}
public static class LinhaDocumentoBuider
{
    public static LinhaDocumento Create(string? line)
    {
        if (string.IsNullOrEmpty(line)) throw new NotImplementedException();
        var tipoTransacao = line[0..1];
        var data = line[1..9];
        var valor = line[9..19];
        var cPF = line[19..30];
        var cartao = line[30..42];
        var hora = line[42..48];
        var donoLoja = line[48..62];
        var nomeLoja = line[62..80];
        var transacao = GetTransacao(tipoTransacao, valor);
        return new LinhaDocumento()
        {
            TipoTransacao = transacao.Tipo,
            Data = data,
            Valor = transacao.Valor,
            CPF = line[19..30],
            Cartao = line[30..42],
            Hora = line[42..48],
            DonoLoja = line[48..62],
            NomeLoja = line[62..80]
        };
    }
    private static Transacao GetTransacao(string tipo, string valor)
    {
        decimal valorTransacao = decimal.Parse(valor) / 100;
        string[] debitos = { "2", "3", "9" };
        if (debitos.Contains(tipo)) valorTransacao = valorTransacao * -1;

        int tipoTransacao = int.Parse(tipo);
        return new Transacao((TipoTransacao)tipoTransacao, valorTransacao);
    }
}
public enum TipoTransacao
{
    Debito = 1,
    Boleto = 2,
    Financiamento = 3,
    Credito = 4,
    RecebimentoEmprestimo = 5,
    Vendas = 6,
    RecebimentoTED = 7,
    RecebimentoDOC = 8,
    Aluguel = 9
}
public record Transacao(TipoTransacao Tipo, decimal Valor);
public class LinhaDocumento
{
    public TipoTransacao TipoTransacao { get; set; }
    public string Data { get; set; }
    public decimal Valor { get; set; }
    public string CPF { get; set; }
    public string Cartao { get; set; }
    public string Hora { get; set; }
    public string DonoLoja { get; set; }
    public string NomeLoja { get; set; }
}

public interface IFileHandler
{
    void Handle(string filename);
    List<ListagemPorLoja> Listar(string filename);
}
public class FileHandler : IFileHandler
{
    private readonly IArquivoRepository arquivoRepository;
    private readonly ILinhaArquivoRepository linhaArquivoRepository;

    public FileHandler(IArquivoRepository arquivoRepository,
        ILinhaArquivoRepository linhaArquivoRepository)
    {
        this.arquivoRepository = arquivoRepository;
        this.linhaArquivoRepository = linhaArquivoRepository;
    }

    public void Handle(string filename)
    {
        string[] linhas = Ler(filename);
        Guid idArquivo = Guid.NewGuid();
        arquivoRepository.Insert(new Arquivo()
        {
            Id = idArquivo,
            Nome = filename,
            UploadEm = DateTime.Now
        });
        foreach (var linha in linhas)
        {
            linhaArquivoRepository.Insert(new ArquivoLinha()
            {
                Id = Guid.NewGuid(),
                ArquivoId = idArquivo,
                Linha = linha
            });
        }
    }
    public List<ListagemPorLoja> Listar(string filename)
    {
        var arquivo = arquivoRepository.GetByName(filename);
        var linhas = arquivo.Linhas.Select(x => x.Linha).ToList();
        ListaPorLoja listagem =
            new ListaPorLoja(linhas
                                .Select(x => LinhaDocumentoBuider.Create(x))
                                .ToList());
        return listagem.Listar();
    }
    public string[] Ler(string file)
    {
        var path = $"./{file}";
        var fullPath = Path.GetFullPath(path);

        return File.ReadAllLines(fullPath);
    }
}
public class Arquivo
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public DateTime UploadEm { get; set; }
    public List<ArquivoLinha> Linhas { get; set; }
}
public interface IArquivoRepository
{
    Arquivo? GetByName(string filename);
    void Insert(Arquivo arquivo);
}
public class ArquivoRepository : IArquivoRepository
{
    private readonly GetConnection connectionGetter;

    public ArquivoRepository(GetConnection connectionGetter)
    {
        this.connectionGetter = connectionGetter;
    }

    public Arquivo? GetByName(string filename)
    {
        return null;
    }

    public void Insert(Arquivo arquivo)
    {
    }
}
public class ArquivoLinha
{
    public Guid Id { get; set; }
    public string Linha { get; set; }
    public Guid ArquivoId { get; set; }
}
public interface ILinhaArquivoRepository
{
    void Insert(ArquivoLinha linha);
}
public class LinhaArquivoRepository : ILinhaArquivoRepository
{
    private readonly GetConnection connectionGetter;

    public LinhaArquivoRepository(GetConnection connectionGetter)
    {
        this.connectionGetter = connectionGetter;
    }

    public void Insert(ArquivoLinha linha)
    {
    }
}
public class Context
{
    public delegate Task<IDbConnection> GetConnection();
}
public static class Dependencies
{
    public static IServiceCollection AddCore(this IServiceCollection services) =>
        services.AddScoped<IFileHandler, FileHandler>()
                .AddScoped<IArquivoRepository, ArquivoRepository>()
                .AddScoped<ILinhaArquivoRepository, LinhaArquivoRepository>()
                .AddScoped<GetConnection>(sp => async () =>
                 {
                     string connectionString = sp.GetService<IConfiguration>()["ConnectionString"];
                     var connection = new SqlConnection(connectionString);
                     await connection.OpenAsync();
                     return connection;
                 });

}