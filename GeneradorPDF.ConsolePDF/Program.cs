using GeneradorPDF.Domain;
using GeneradorPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

// La licencia de QuestPDF es OBLIGATORIA antes de generar.
QuestPDF.Settings.License = LicenseType.Community;

Console.WriteLine("Generando PDF de ejemplo...");

var pagare = new Pagare
{
    Numero = 1,
    Lugar = "Cuernavaca",
    Dia = 10,
    Mes = "Febrero",
    Anio = 2025,
    Monto = 15000.50m,
    ALaOrdenDe = "[NOMBRE DEL ACREEDOR]",
    CantidadLetra = "Quince mil pesos 50/100 M.N.",
    NombreDeudor = "[NOMBRE DEL DEUDOR]\n[DIRECCIÓN]\n[CIUDAD, ESTADO]\nRFC: [RFC]\nTEL: [TELÉFONO]",
    NombreAval1 = "[NOMBRE DEL AVAL 1]\n[DIRECCIÓN]\n[CIUDAD, ESTADO]\nRFC: [RFC]\nTEL: [TELÉFONO]",
    NombreAval2 = "[NOMBRE DEL AVAL 2]\n[DIRECCIÓN]\n[CIUDAD, ESTADO]\nRFC: [RFC]\nTEL: [TELÉFONO]",
    SerieDesde = "0001",
    SerieHasta = "0050",
    InteresMoratorioPorcentaje = "5"
};

string output = Path.Combine(Path.GetTempPath(), "Pagare.pdf");

new PagareDocument(pagare).GeneratePdf(output);

Console.WriteLine("Listo! PDF generado en: " + output);
