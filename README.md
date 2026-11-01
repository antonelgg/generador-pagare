# Generador de Pagarés (.NET)

Aplicación en **C# / .NET 8** que genera **pagarés en PDF**, construida con **arquitectura limpia (Clean Architecture)** en tres capas. Incluye una interfaz de escritorio (**WPF**) y una utilidad de **línea de comandos**.

> ⚠️ Repositorio **demo / versión despersonalizada**: los datos de ejemplo que aparecen en el código son ficticios. No contiene información de clientes ni de terceros.

## Arquitectura

```
 ┌──────────────────────────────────────────────────────────┐
 │  UI                                                       │
 │  GeneradorPDF.WPF          (escritorio, MVVM)             │
 │  GeneradorPDF.ConsolePDF   (línea de comandos)            │
 └───────────────────────────┬───────────────────────────────┘
                             │
 ┌───────────────────────────▼───────────────────────────────┐
 │  GeneradorPDF.Application                                 │
 │  Casos de uso (GenerarPagareUseCase)                      │
 └───────────────────────────┬───────────────────────────────┘
                             │
 ┌───────────────────────────▼───────────────────────────────┐
 │  GeneradorPDF.Domain                                      │
 │  Entidad Pagare (núcleo, sin dependencias externas)        │
 └───────────────────────────────────────────────────────────┘

 GeneradorPDF.Infrastructure →  implementación técnica:
 PagareDocument + PagarePdfService (PDF con QuestPDF)
```

**Flujo:** la UI recolecta los datos → `Application` orquesta el caso de uso → `Domain` define el modelo → `Infrastructure` materializa el PDF.

## Estructura del repositorio

| Proyecto | Tipo | Descripción |
|---|---|---|
| `GeneradorPDF.Domain` | Class library | Entidad `Pagare`: datos, avales, serie, interés moratorio |
| `GeneradorPDF.Application` | Class library | Casos de uso (puerta entre UI e infraestructura) |
| `GeneradorPDF.Infrastructure` | Class library | Generación del PDF con **QuestPDF** |
| `GeneradorPDF.WPF` | App WPF (`net8.0-windows`) | Formulario de captura con patrón MVVM |
| `GeneradorPDF.ConsolePDF` | App consola | Genera un PDF de ejemplo desde la terminal |

## Características

- Generación de pagarés en PDF carta con QuestPDF (comunidad).
- Captura de deudor y hasta **7 avales**, con dirección, RFC y teléfono.
- **Cantidad con letra**, monto en formato moneda MXN (`$15,000.50`).
- Serie (`desde` / `hasta`) y **interés moratorio**.
- Tipografía que se **auto-ajusta** según el largo de los datos.
- Apps listas para publicarse como **archivo único autocontenido** (no requiere .NET instalado en el equipo destino).

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Cómo ejecutar

```bash
# Línea de comandos: genera un PDF de ejemplo en %TEMP%\Pagare.pdf
dotnet run --project GeneradorPDF.ConsolePDF

# Aplicación de escritorio (WPF)
dotnet run --project GeneradorPDF.WPF
```

## Publicar (archivo único, sin instalar .NET)

```bash
dotnet publish GeneradorPDF.WPF          -c Release -r win-x64
dotnet publish GeneradorPDF.ConsolePDF   -c Release -r win-x64
```

El resultado queda en `bin/Release/net8.0-windows/win-x64/publish/` (WPF) y `bin/Release/net8.0/win-x64/publish/` (consola).

## Ejemplo

Se incluye un pagaré generado con datos ficticios en [`docs/ejemplo_pagare.pdf`](docs/ejemplo_pagare.pdf).

## Licencia

Distribuido bajo [MIT](LICENSE).
