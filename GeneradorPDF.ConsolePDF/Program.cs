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
    ALaOrdenDe = "Juan Pérez",
    CantidadLetra = "Quince mil pesos 50/100 M.N.",
    NombreDeudor = "Antonio Morales Hernández\nAv. Universidad 123, Col. Centro\nCuernavaca, Morelos\nRFC: MOHA850101AB1\nTEL: 777 123 4567",
    NombreAval1 = "Pedro Gómez López\nBlvd. Vicente Guerrero 45, Col. Jardines\nCuernavaca, Morelos\nRFC: GOPL780215CD2\nTEL: 777 234 5678",
    NombreAval2 = "Luis Ramírez Sánchez\nCalle Niños Héroes 67, Col. Privada\nTemixco, Morelos\nRFC: RASL900310EF3\nTEL: 777 345 6789",
    SerieDesde = "0001",
    SerieHasta = "0050",
    InteresMoratorioPorcentaje = "5"
};

string output = Path.Combine(Path.GetTempPath(), "Pagare.pdf");

new PagareDocument(pagare).GeneratePdf(output);

Console.WriteLine("Listo! PDF generado en: " + output);
