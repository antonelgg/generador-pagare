using GeneradorPDF.Domain;
using GeneradorPDF.Infrastructure;

namespace GeneradorPDF.Application;

public class GenerarPagareUseCase
{
    public byte[] Ejecutar(Pagare modelo)
    {
        return PagarePdfService.Generar(modelo);
    }
}
