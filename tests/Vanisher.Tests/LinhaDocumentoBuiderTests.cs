using Vanisher.Core;
namespace Vanisher.Tests;

public class LinhaDocumentoBuiderTests
{
    [Theory]
    [InlineData("3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ")]
    //[InlineData(" ")]
    public void Map_Document_Line_To_Entity(string? line)
    {
        var documento = LinhaDocumentoBuider.Create(line);
        Assert.Equal(TipoTransacao.Financiamento, documento.TipoTransacao);
        Assert.Equal("20190301", documento.Data);
        Assert.Equal(-142.00M, documento.Valor);
        Assert.Equal("09620676017", documento.CPF);
        Assert.Equal("4753****3153", documento.Cartao);
        Assert.Equal("153453", documento.Hora);
        Assert.Equal("JOÃO MACEDO   ", documento.DonoLoja);
        Assert.Equal("BAR DO JOÃO       ", documento.NomeLoja);
    }
}

public class ListaPorLojaTests
{
    [Theory]
    [InlineData("3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ")]
    public void Map_Document_Line_To_Entity(string line)
    {
        var documento = LinhaDocumentoBuider.Create(line);
        ListaPorLoja listar = new ListaPorLoja(new List<LinhaDocumento>() { documento });
        var listagem = listar.Listar();
        Assert.Equal(1, listagem.Count);
        var primeiroElemento = listagem[0];
        Assert.Equal(1, primeiroElemento.documentos.Count);
        Assert.Equal("BAR DO JOÃO       ", primeiroElemento.loja.nome);
        Assert.Equal(-142.00M, primeiroElemento.loja.total);
    }
}