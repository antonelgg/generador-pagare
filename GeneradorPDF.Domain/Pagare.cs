namespace GeneradorPDF.Domain;

public class Pagare
{
    public int Numero { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public int Dia { get; set; }
    public string Mes { get; set; } = string.Empty;
    public int Anio { get; set; }
    public decimal Monto { get; set; }
    public string ALaOrdenDe { get; set; } = string.Empty;
    public string CantidadLetra { get; set; } = string.Empty;
    public string NombreDeudor { get; set; } = string.Empty;
    public string NombreAval1 { get; set; } = string.Empty;
    public string NombreAval2 { get; set; } = string.Empty;
    public string NombreAval3 { get; set; } = string.Empty;
    public string NombreAval4 { get; set; } = string.Empty;
    public string NombreAval5 { get; set; } = string.Empty;
    public string NombreAval6 { get; set; } = string.Empty;
    public string NombreAval7 { get; set; } = string.Empty;
    public string SerieDesde { get; set; } = string.Empty;
    public string SerieHasta { get; set; } = string.Empty;
    public string InteresMoratorioPorcentaje { get; set; } = string.Empty;
}
