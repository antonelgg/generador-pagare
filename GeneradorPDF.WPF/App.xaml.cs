using System.Windows;
using QuestPDF.Infrastructure;

namespace GeneradorPDF.WPF;

public partial class App : Application
{
    public App()
    {
        // Licencia de QuestPDF (OBLIGATORIA) — igual que en la app MAUI.
        QuestPDF.Settings.License = LicenseType.Community;
    }
}
