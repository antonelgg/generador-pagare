using System.Globalization;
using GeneradorPDF.Domain;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeneradorPDF.Infrastructure;

public class PagareDocument : IDocument
{
    private readonly Pagare _p;

    public PagareDocument(Pagare pagare)
    {
        _p = pagare;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    /// <summary>Tamaño de letra fijo para los datos del deudor y los avales.</summary>
    private const int FontCuerpo = 11;

    private int AutoShrink(string text, int max = 10, int min = 7)
    {
        if (string.IsNullOrWhiteSpace(text))
            return max;

        text = NormalizeNewLines(text);
        int length = text.Length;
        int lines = text.Split('\n').Length;

        // Decreciente y monótona: a más texto, letra más chica (nunca al revés).
        int size = max;
        if (length > 80 || lines > 2) size = max - 1;
        if (length > 120 || lines > 3) size = max - 2;
        if (length > 180 || lines > 4) size = max - 3;
        if (length > 250 || lines > 5) size = max - 4;
        if (length > 350 || lines > 6) size = max - 5;

        return Math.Max(size, min);
    }

    private static string NormalizeNewLines(string text)
    {
        return (text ?? string.Empty)
            .Replace("\r\n", "\n")
            .Replace("\r", "\n");
    }

    private static string Campo(string valor, int largo = 8)
    {
        return string.IsNullOrWhiteSpace(valor) ? new string('_', largo) : valor;
    }

    private string MontoFormateado()
    {
        return _p.Monto.ToString("C2", CultureInfo.GetCultureInfo("es-MX"));
    }

    private static IContainer Box(IContainer container)
    {
        return container.Border(1).Padding(5);
    }

    private static IContainer InnerField(IContainer container)
    {
        return container.PaddingTop(2).PaddingBottom(2);
    }

    private IReadOnlyList<(string Title, string Content)> ObtenerAvales()
    {
        return
        [
            ("AVAL 1", _p.NombreAval1),
            ("AVAL 2", _p.NombreAval2),
            ("AVAL 3", _p.NombreAval3),
            ("AVAL 4", _p.NombreAval4),
            ("AVAL 5", _p.NombreAval5),
            ("AVAL 6", _p.NombreAval6),
            ("AVAL 7", _p.NombreAval7)
        ];
    }

    private void PrintMultiline(ColumnDescriptor column, string contenido)
    {
        string normalizado = NormalizeNewLines(contenido);
        string[] lineas = normalizado.Split('\n', StringSplitOptions.None);

        foreach (string linea in lineas)
        {
            column.Item()
                .ShowEntire()
                .Text(linea)
                .FontSize(FontCuerpo)
                .LineHeight(1.2f);
        }
    }

    private void ComposeHeader(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.8f);
                columns.RelativeColumn(1.5f);
                columns.RelativeColumn(0.95f);
                columns.RelativeColumn(0.95f);
                columns.RelativeColumn(0.95f);
                columns.RelativeColumn(1.2f);
            });

            table.Cell().Element(Box).Column(column =>
            {
                column.Item().Text("PAGARÉ").Bold().FontSize(11);
                column.Item().Text($"No: {Campo(_p.Numero.ToString(), 2)}").Bold().FontSize(10);
            });

            table.Cell().Element(Box).Column(column =>
            {
                column.Item().Text("LUGAR DE\nEXPEDICIÓN").Bold().FontSize(8).LineHeight(1.0f);
                column.Item().Text(Campo(_p.Lugar, 6)).FontSize(AutoShrink(_p.Lugar, 9, 7));
            });

            table.Cell().Element(Box).Column(column =>
            {
                column.Item().Text("DIA").Bold().FontSize(8);
                column.Item().AlignCenter().Text(Campo(_p.Dia.ToString(), 2)).FontSize(10);
            });

            table.Cell().Element(Box).Column(column =>
            {
                column.Item().Text("MES").Bold().FontSize(8);
                column.Item().AlignCenter().Text(Campo(_p.Mes, 2)).FontSize(AutoShrink(_p.Mes, 10, 7));
            });

            table.Cell().Element(Box).Column(column =>
            {
                column.Item().Text("AÑO").Bold().FontSize(8);
                column.Item().AlignCenter().Text(Campo(_p.Anio.ToString(), 4)).FontSize(10);
            });

            table.Cell().Element(Box).Column(column =>
            {
                column.Item().Text("BUENO POR").Bold().FontSize(8);
                column.Item().AlignCenter().Text(MontoFormateado()).Bold().FontSize(10);
            });
        });
    }

    private void ComposeLineField(IContainer container, string label, string value, int fallbackLength)
    {
        container.Column(column =>
        {
            column.Item().Text($"{label} {Campo(value, fallbackLength)}").FontSize(AutoShrink(value, 10, 7));
            column.Item().PaddingTop(3).Height(1).Background(Colors.Black);
        });
    }

    private void ComposePartyBox(IContainer container, string title, string content, float minHeight)
    {
        container.Border(1).Padding(8).Column(column =>
        {
            column.Item().AlignCenter().Text(title).Bold().FontSize(9);
            column.Item().PaddingTop(8).MinHeight(minHeight).Column(textColumn =>
            {
                PrintMultiline(textColumn, content);
            });
            column.Item().PaddingTop(8).AlignCenter().Text("FIRMA").Bold().FontSize(9);
        });
    }

    private void ComposeAvalRow(
        IContainer container,
        (string Title, string Content) leftAval,
        (string Title, string Content)? rightAval)
    {
        container.Border(1).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Cell().BorderRight(1).Padding(8).Column(column =>
            {
                column.Item().AlignCenter().Text(leftAval.Title).Bold().FontSize(9);
                column.Item().PaddingTop(8).MinHeight(95).Column(textColumn =>
                {
                    PrintMultiline(textColumn, leftAval.Content);
                });
                column.Item().PaddingTop(8).AlignCenter().Text("FIRMA").Bold().FontSize(9);
            });

            table.Cell().Padding(8).Column(column =>
            {
                if (rightAval is { } aval)
                {
                    column.Item().AlignCenter().Text(aval.Title).Bold().FontSize(9);
                    column.Item().PaddingTop(8).MinHeight(95).Column(textColumn =>
                    {
                        PrintMultiline(textColumn, aval.Content);
                    });
                    column.Item().PaddingTop(8).AlignCenter().Text("FIRMA").Bold().FontSize(9);
                }
                else
                {
                    column.Item().MinHeight(120);
                }
            });
        });
    }

    private void ComposeAvales(IContainer container)
    {
        // Solo renderizamos los avales con contenido: los vacíos no deben
        // imprimir cajas gigantes ni empujar el documento a una 2ª página.
        var avales = ObtenerAvales()
            .Where(a => !string.IsNullOrWhiteSpace(a.Content))
            .ToList();

        if (avales.Count == 0)
            return;

        container.Column(column =>
        {
            column.Spacing(12);

            for (int i = 0; i < avales.Count; i += 2)
            {
                var leftAval = avales[i];
                (string Title, string Content)? rightAval = i + 1 < avales.Count
                    ? avales[i + 1]
                    : null;

                column.Item()
                    .ShowEntire()
                    .Element(x => ComposeAvalRow(x, leftAval, rightAval));
            }
        });
    }

    public void Compose(IDocumentContainer container)
    {
        string textoLegal =
            $"VALOR RECIBIDO A MI (NUESTRA) ENTERA SATISFACCIÓN, ESTE PAGARÉ FORMA PARTE DE UNA SERIE NUMERADA DEL {Campo(_p.SerieDesde, 4)} AL {Campo(_p.SerieHasta, 4)} Y TODOS ESTÁN SUJETOS A LA CONDICIÓN DE QUE DE NO PAGARSE CUALQUIERA DE ELLOS A SU VENCIMIENTO, SERÁN EXIGIBLES TODOS LOS QUE LE SIGUEN EN NÚMERO, ADEMÁS DE LOS YA VENCIDOS DE ACUERDO AL ART. 79 DE LA LEY GENERAL DE TÍTULOS Y OPERACIONES DE CRÉDITO, CAUSARÁ INTERESES MORATORIOS DEL {Campo(_p.InteresMoratorioPorcentaje, 2)}% POR CADA MES O FRACCIÓN, PAGADERO JUNTAMENTE CON EL PRINCIPAL, DICHOS INTERESES SE CAUSARÁN SOBRE EL CAPITAL INSOLUTO, CONFORME A LO DISPUESTO POR EL ART. 152, INCISOS I, II, III Y IV, DE LA LEY DE TÍTULOS Y OPERACIONES DE CRÉDITO.";

        container.Page(page =>
        {
            page.Size(PageSizes.Letter);
            page.PageColor(Colors.White);
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Black));

            page.Content().Padding(4).Column(column =>
            {
                column.Spacing(8);

                column.Item().Element(ComposeHeader);

                column.Item()
                    .PaddingTop(2)
                    .Text("DEBO (EMOS) Y PAGARÉ (MOS) INCONDICIONALMENTE, SIN PRETEXTO ESTE PAGARÉ EN EL LUGAR Y FECHAS CITADAS DONDE ELIJA EL TENEDOR EL DÍA DE SU VENCIMIENTO.")
                    .FontSize(8.5f);

                column.Item().Element(x => ComposeLineField(x, "A la orden de:", _p.ALaOrdenDe, 14));
                column.Item().Element(x => ComposeLineField(x, "La cantidad de:", _p.CantidadLetra, 18));

                column.Item()
                    .PaddingTop(10)
                    .PaddingLeft(10)
                    .PaddingRight(10)
                    .Text(textoLegal)
                    .FontSize(8)
                    .LineHeight(1.15f);

                column.Item()
                    .PaddingTop(12)
                    .Element(x => ComposePartyBox(x, "NOMBRE Y DATOS DEL DEUDOR", _p.NombreDeudor, 100));

                column.Item()
                    .PaddingTop(12)
                    .Element(ComposeAvales);
            });
        });
    }
}
