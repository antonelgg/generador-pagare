using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GeneradorPDF.Domain;
using GeneradorPDF.Infrastructure;
using QuestPDF.Fluent;

namespace GeneradorPDF.WPF;

public class PagareViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private string _numero = "";
    public string Numero { get => _numero; set => Set(ref _numero, value); }

    private string _lugar = "";
    public string Lugar { get => _lugar; set => Set(ref _lugar, value); }

    private string _dia = "";
    public string Dia { get => _dia; set => Set(ref _dia, value); }

    private string _mes = "";
    public string Mes { get => _mes; set => Set(ref _mes, value); }

    private string _anio = "";
    public string Anio { get => _anio; set => Set(ref _anio, value); }

    private string _monto = "";
    public string Monto { get => _monto; set => Set(ref _monto, value); }

    private string _aLaOrdenDe = "";
    public string ALaOrdenDe { get => _aLaOrdenDe; set => Set(ref _aLaOrdenDe, value); }

    private string _cantidadLetra = "";
    public string CantidadLetra { get => _cantidadLetra; set => Set(ref _cantidadLetra, value); }

    private string _nombreDeudor = "";
    public string NombreDeudor { get => _nombreDeudor; set => Set(ref _nombreDeudor, value); }

    private string _nombreAval1 = "";
    public string NombreAval1 { get => _nombreAval1; set => Set(ref _nombreAval1, value); }

    private string _nombreAval2 = "";
    public string NombreAval2 { get => _nombreAval2; set => Set(ref _nombreAval2, value); }

    private string _nombreAval3 = "";
    public string NombreAval3 { get => _nombreAval3; set => Set(ref _nombreAval3, value); }

    private string _nombreAval4 = "";
    public string NombreAval4 { get => _nombreAval4; set => Set(ref _nombreAval4, value); }

    private string _nombreAval5 = "";
    public string NombreAval5 { get => _nombreAval5; set => Set(ref _nombreAval5, value); }

    private string _nombreAval6 = "";
    public string NombreAval6 { get => _nombreAval6; set => Set(ref _nombreAval6, value); }

    private string _nombreAval7 = "";
    public string NombreAval7 { get => _nombreAval7; set => Set(ref _nombreAval7, value); }

    private string _serieDesde = "";
    public string SerieDesde { get => _serieDesde; set => Set(ref _serieDesde, value); }

    private string _serieHasta = "";
    public string SerieHasta { get => _serieHasta; set => Set(ref _serieHasta, value); }

    private string _interesMoratorioPorcentaje = "";
    public string InteresMoratorioPorcentaje { get => _interesMoratorioPorcentaje; set => Set(ref _interesMoratorioPorcentaje, value); }

    private bool _isGenerando;
    public bool IsGenerando
    {
        get => _isGenerando;
        set { Set(ref _isGenerando, value); ((Command)GenerarPDFCommand).RaiseCanExecuteChanged(); }
    }

    public ICommand GenerarPDFCommand { get; }

    public PagareViewModel()
    {
        GenerarPDFCommand = new Command(GenerarPDF, () => !IsGenerando);
    }

    private static bool TryParsePositiveInt(string input, out int value)
        => int.TryParse(input, out value) && value > 0;

    /// <summary>
    /// Parseo determinista e insensible a cultura: un punto SIEMPRE es decimal,
    /// una coma/según el contexto es de miles. Evita que "15000.50" se lea como 1500050 (H1).
    /// </summary>
    private static bool TryParseMonto(string input, out decimal value)
    {
        value = 0m;
        if (string.IsNullOrWhiteSpace(input)) return false;

        string s = input.Replace(" ", "");
        int countDot = s.Count(c => c == '.');
        int countComma = s.Count(c => c == ',');
        int lastSep = Math.Max(s.LastIndexOf('.'), s.LastIndexOf(','));

        if (lastSep < 0)
            return decimal.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value > 0;

        char decChar;
        if (countDot > 0 && countComma > 0)
        {
            // Con los dos tipos, el más a la derecha es el decimal.
            decChar = s[lastSep];
        }
        else if (countDot + countComma == 1)
        {
            int dsp = 0;
            for (int i = lastSep + 1; i < s.Length; i++)
                if (char.IsDigit(s[i])) dsp++;
            decChar = dsp == 3 ? '\0' : s[lastSep];
        }
        else
        {
            // Varios separadores del mismo tipo => todos de miles.
            decChar = '\0';
        }

        int lastDecIdx = decChar == '\0' ? -1 : s.LastIndexOf(decChar);
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (char.IsDigit(c)) sb.Append(c);
            else if (c == '.' || c == ',') { if (i == lastDecIdx) sb.Append('.'); }
            else return false;
        }

        return decimal.TryParse(sb.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out value) && value > 0;
    }

    private bool TryBuildPagare(out Pagare? pagare, out string error)
    {
        pagare = null;

        if (!TryParsePositiveInt(Numero, out int numero))
        { error = "El campo NÚMERO debe ser un entero válido."; return false; }

        if (!TryParsePositiveInt(Dia, out int dia) || dia > 31)
        { error = "El campo DÍA debe estar entre 1 y 31."; return false; }

        if (!TryParsePositiveInt(Anio, out int anio) || anio < 1900 || anio > 9999)
        { error = "El campo AÑO debe ser un año válido de 4 dígitos."; return false; }

        if (!TryParseMonto(Monto, out decimal monto) || monto <= 0)
        { error = "El campo MONTO debe ser una cantidad válida mayor que cero."; return false; }

        pagare = new Pagare
        {
            Numero = numero,
            Lugar = Lugar,
            Dia = dia,
            Mes = Mes,
            Anio = anio,
            Monto = monto,
            ALaOrdenDe = ALaOrdenDe,
            CantidadLetra = CantidadLetra,
            SerieDesde = SerieDesde,
            SerieHasta = SerieHasta,
            InteresMoratorioPorcentaje = InteresMoratorioPorcentaje,
            NombreDeudor = NombreDeudor,
            NombreAval1 = NombreAval1,
            NombreAval2 = NombreAval2,
            NombreAval3 = NombreAval3,
            NombreAval4 = NombreAval4,
            NombreAval5 = NombreAval5,
            NombreAval6 = NombreAval6,
            NombreAval7 = NombreAval7
        };

        error = string.Empty;
        return true;
    }

    private async void GenerarPDF()
    {
        if (IsGenerando) return;

        try
        {
            if (!TryBuildPagare(out Pagare? pagare, out string error) || pagare is null)
            {
                System.Windows.MessageBox.Show(error, "ERROR", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            IsGenerando = true;

            string ruta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"Pagare_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            await Task.Run(() =>
            {
                byte[] pdf = new PagareDocument(pagare).GeneratePdf();
                File.WriteAllBytes(ruta, pdf);
            });

            System.Windows.MessageBox.Show($"Archivo creado en:\n{ruta}", "PDF GENERADO",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            // Abrir el PDF con el visor predeterminado.
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(ruta) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "ERROR", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsGenerando = false;
        }
    }

    private class Command : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        public Command(Action execute, Func<bool> canExecute) { _execute = execute; _canExecute = canExecute; }
        public bool CanExecute(object? parameter) => _canExecute();
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
