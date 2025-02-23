using Microsoft.AspNetCore.Mvc;
using Vanisher.Core;
using static Vanisher.Core.Context;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var lines = new[]
{
    "3201903010000014200096206760174753****3153153453JO�O MACEDO   BAR DO JO�O       ",
    "5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO � - MATRIZ",
    "3201903010000012200845152540736777****1313172712MARCOS PEREIRAMERCADO DA AVENIDA",
    "2201903010000011200096206760173648****0099234234JO�O MACEDO   BAR DO JO�O       ",
    "1201903010000015200096206760171234****7890233000JO�O MACEDO   BAR DO JO�O       ",
    "2201903010000010700845152540738723****9987123333MARCOS PEREIRAMERCADO DA AVENIDA",
    "2201903010000050200845152540738473****1231231233MARCOS PEREIRAMERCADO DA AVENIDA",
    "3201903010000060200232702980566777****1313172712JOS� COSTA    MERCEARIA 3 IRM�OS",
    "1201903010000020000556418150631234****3324090002MARIA JOSEFINALOJA DO � - MATRIZ",
    "5201903010000080200845152540733123****7687145607MARCOS PEREIRAMERCADO DA AVENIDA",
    "2201903010000010200232702980568473****1231231233JOS� COSTA    MERCEARIA 3 IRM�OS",
    "3201903010000610200232702980566777****1313172712JOS� COSTA    MERCEARIA 3 IRM�OS",
    "4201903010000015232556418150631234****6678100000MARIA JOSEFINALOJA DO � - FILIAL",
    "8201903010000010203845152540732344****1222123222MARCOS PEREIRAMERCADO DA AVENIDA",
    "3201903010000010300232702980566777****1313172712JOS� COSTA    MERCEARIA 3 IRM�OS",
    "9201903010000010200556418150636228****9090000000MARIA JOSEFINALOJA DO � - MATRIZ",
    "4201906010000050617845152540731234****2231100000MARCOS PEREIRAMERCADO DA AVENIDA",
    "2201903010000010900232702980568723****9987123333JOS� COSTA    MERCEARIA 3 IRM�OS",
    "8201903010000000200845152540732344****1222123222MARCOS PEREIRAMERCADO DA AVENIDA",
    "2201903010000000500232702980567677****8778141808JOS� COSTA    MERCEARIA 3 IRM�OS",
    "3201903010000019200845152540736777****1313172712MARCOS PEREIRAMERCADO DA AVENIDA"
};
app.MapGet("/{filename}", async (string filename,
    [FromServices] IFileHandler handler) =>
{
    handler.Listar($"\\CNABs\\{filename}");
})
.WithName("GetWeatherForecast")
.WithOpenApi();
app.MapPost("/", async (IFormFile arquivo,
    [FromServices] IFileHandler handler,
    [FromServices] IWebHostEnvironment _environment) =>
{
    if (arquivo.Length > 0)
    {
        try
        {
            var path = $"./CNABs";
            var fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            using (FileStream filestream = File.Create($"{fullPath}/{arquivo.FileName}"))
            {
                await arquivo.CopyToAsync(filestream);
                filestream.Flush();
            }
            handler.Handle($"\\CNABs\\{arquivo.FileName}");
            return "\\CNABs\\" + arquivo.FileName;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    else
    {
        return "Ocorreu uma falha no envio do arquivo...";
    }

    //ListaPorLoja lisatgem = new ListaPorLoja(lines.Select(x => LinhaDocumentoBuider.Create(x)).ToList());
    //return lisatgem.Listagem();
})
.WithName("PostWeatherForecast");

app.Run();
