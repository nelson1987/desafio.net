namespace Vanisher.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData("3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ")]
    [InlineData("5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ")]
    public void Map_Document_Line_To_Entity(string line)
    {
        var linhaDocumento = LinhaDocumentoBuider.Create(line);
        Assert.Equal("3", linhaDocumento.TipoTransacao);
    }
    /*public static class ArquivoLeituraBuilder
{
    public static ArquivoLeitura Create(string line, string file)
    {
        return new ArquivoLeitura()
        {
            FileName = file,
            TipoCreditante = int.Parse(line[7..9]),
            CreditanteId = int.Parse(line[9..14]),
            TipoDebitante = int.Parse(line[14..16]),
            DebitanteId = int.Parse(line[16..21]),
            Valor = decimal.Parse($"{line[21..31]},{line[31..33]}")
        };
    }
}
3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       
5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ
    */
}
public static class LinhaDocumentoBuider{
    public static LinhaDocumento Create(string line)
    {
        return new LinhaDocumento()
        {
            TipoTransacao = line[0..1]
        };
    }
}
public class LinhaDocumento
{
    public string TipoTransacao{get; set;}
    public string Data{get; set;}
    public string Valor{get; set;}
    public string CPF{get; set;}
    public string Cartao{get; set;}
    public string Hora{get; set;}
    public string Hora{get; set;}
    public string DonoLoja{get; set;}
    public string NomeLoja{get; set;}
}