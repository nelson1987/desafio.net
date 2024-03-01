namespace Vanisher.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData("3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ")]
    //[InlineData("5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ")]
    public void Map_Document_Line_To_Entity(string line)
    {
        var documento = LinhaDocumentoBuider.Create(line);
        Assert.Equal("3", documento.TipoTransacao);
        Assert.Equal("20190301", documento.Data);
        Assert.Equal("0000014200", documento.Valor);
        Assert.Equal("09620676017", documento.CPF);
        Assert.Equal("4753****3153", documento.Cartao);
        Assert.Equal("153453", documento.Hora);
        Assert.Equal("JOÃO MACEDO   ", documento.DonoLoja);
        Assert.Equal("BAR DO JOÃO       ", documento.NomeLoja);
    }
}
public static class LinhaDocumentoBuider
{
    public static LinhaDocumento Create(string line)
    {
        var tipoTransacao = line[0..1];
        var data = line[1..9];
        var valor = line[9..19];
        var cPF = line[19..30];
        var cartao = line[30..42];
        var hora = line[42..48];
        var donoLoja = line[48..62];
        var nomeLoja = line[62..80];
        //GetTransacao(tipoTransacao, valor);
        return new LinhaDocumento()
        {
            TipoTransacao = line[0..1],
            Data = line[1..9],
            Valor = line[9..19],
            CPF = line[19..30],
            Cartao = line[30..42],
            Hora = line[42..48],
            DonoLoja = line[48..62],
            NomeLoja = line[62..80]
        };
    }
    private static void GetTransacao(string tipoTransacao, string valor)
    {
        int[] debitos = new {2,3,9};
        if(debitos.Contains(tipoTransacao)) valor = valor +-1;
        //(int tipo, string descricao) =
        //tipoTransacao = 
    }
}
public class LinhaDocumento
{
    public string TipoTransacao {get; set;}
    public string Data {get; set;}
    public string Valor {get; set;}
    public string CPF {get; set;}
    public string Cartao {get; set;}
    public string Hora {get; set;}
    public string DonoLoja {get; set;}
    public string NomeLoja {get; set;}
}