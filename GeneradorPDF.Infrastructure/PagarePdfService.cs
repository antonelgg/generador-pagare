using GeneradorPDF.Domain;
using QuestPDF.Fluent;

namespace GeneradorPDF.Infrastructure;

public static class PagarePdfService
{
    public static byte[] Generar(Pagare pagare)
    {
        return new PagareDocument(pagare).GeneratePdf();
    }
}
