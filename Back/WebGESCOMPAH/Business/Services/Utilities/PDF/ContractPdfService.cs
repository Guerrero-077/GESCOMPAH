using Business.Interfaces.PDF;
using Entity.DTOs.Implements.Business.Contract;
using Microsoft.Playwright;
using System.Text;
using System.Text.RegularExpressions;
using Templates.Templates;

namespace Business.Services.Utilities.PDF
{
    /// <summary>
    /// Generador de PDF basado en Playwright con reutilización de Browser.
    /// - Reutiliza IPlaywright e IBrowser (lazy, thread-safe)
    /// - Crea IBrowserContext e IPage por request (thread-safe)
    /// - Optimiza reemplazos y tiempos de espera
    /// </summary>
    public class ContractPdfService : IContractPdfGeneratorService
    {
        // Lazy singletons para evitar condiciones de carrera en arranque
        private static readonly SemaphoreSlim _initLock = new(1, 1);
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;

        // Regex precompilado para el bloque foreach de cláusulas
        // Reemplaza el bucle Razor por una <ul> construida a mano.
        private static readonly Regex _foreachRegex = new(
            @"@foreach\s*\(.*?\)\s*{.*?}",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public async Task<byte[]> GeneratePdfAsync(ContractSelectDto contract)
        {
            // 1) Preparar HTML a partir de plantilla
            var template = ContractTemplate.Html;
            var html = BuildHtml(template, contract);

            // 2) Asegurar que Playwright y Browser están inicializados una sola vez
            await EnsureBrowserAsync();

            // 3) Usar un contexto/página por request (thread-safe)
            //    Contexto limpio = sin fugas de estado/cookies/almacenamiento.
            var context = await _browser!.NewContextAsync(new()
            {
                // Viewport null => tamaño "fit to page" para PDF,
                // no imprescindible, pero ayuda a evitar reflow extraño.
                ViewportSize = null
            });

            try
            {
                var page = await context.NewPageAsync();

                // Emular media "print" para respetar estilos de impresión
                await page.EmulateMediaAsync(new() { Media = Media.Print });

                // 4) Cargar HTML y esperar a estar ocioso en red.
                //    BaseURL opcional si usas rutas relativas a recursos (css/img).
                await page.SetContentAsync(
                    html,
                    new PageSetContentOptions
                    {
                        WaitUntil = WaitUntilState.NetworkIdle,
                        // Ajusta si tu plantilla trae recursos externos
                        Timeout = 10_000
                    });

                // 5) Exportar PDF
                var pdfBytes = await page.PdfAsync(new PagePdfOptions
                {
                    Format = "A4",
                    PrintBackground = true,
                    PreferCSSPageSize = true, // respeta @page css size si lo defines
                    Margin = new()
                    {
                        Top = "40px",
                        Bottom = "40px",
                        Left = "40px",
                        Right = "40px"
                    },
                    // Opcional: DisplayHeaderFooter = true, HeaderTemplate = ..., FooterTemplate = ...
                });

                return pdfBytes;
            }
            finally
            {
                await context.CloseAsync();
            }
        }

        /// <summary>
        /// Inicializa IPlaywright e IBrowser una sola vez. Thread-safe y tolerante a caídas.
        /// </summary>
        private static async Task EnsureBrowserAsync()
        {
            if (_browser is not null) return;

            await _initLock.WaitAsync();
            try
            {
                if (_browser is not null) return;

                _playwright ??= await Playwright.CreateAsync();

                // Flags útiles en contenedores Linux (ajústalos por entorno)
                var launchOptions = new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[]
                    {
                        // Comenta si NO estás en contenedor endurecido
                        "--no-sandbox",
                        "--disable-dev-shm-usage"
                    }
                };

                _browser = await _playwright.Chromium.LaunchAsync(launchOptions);
            }
            catch
            {
                // Si algo falló, forzar reintento en la próxima llamada
                _browser = null;
                _playwright = null;
                throw;
            }
            finally
            {
                _initLock.Release();
            }
        }

        /// <summary>
        /// Construye el HTML final: reemplazos directos + renderizado de cláusulas.
        /// Evita usos innecesarios de Regex salvo para el bloque foreach.
        /// </summary>
        private static string BuildHtml(string template, ContractSelectDto c)
        {
            var sb = new StringBuilder(template);

            // Reemplazos simples (usa valores seguros)
            sb.Replace("@Model.FullName", HtmlEncode(c.FullName));
            sb.Replace("@Model.Document", HtmlEncode(c.Document));
            sb.Replace("@Model.StartDate.ToString(\"dd/MM/yyyy\")", c.StartDate.ToString("dd/MM/yyyy"));
            sb.Replace("@Model.EndDate.ToString(\"dd/MM/yyyy\")", c.EndDate.ToString("dd/MM/yyyy"));

            var p = c.PremisesLeased?.FirstOrDefault();
            if (p != null)
            {
                sb.Replace("@Model.PremisesLeased[0].EstablishmentName", HtmlEncode(p.EstablishmentName));
                sb.Replace("@Model.PremisesLeased[0].Address", HtmlEncode(p.Address));
                sb.Replace("@Model.PremisesLeased[0].AreaM2", p.AreaM2.ToString("0.##"));
                sb.Replace("@Model.PremisesLeased[0].PlazaName", HtmlEncode(p.PlazaName));
            }

            // Renderizar cláusulas
            var clausesHtml = new StringBuilder();
            if (c.Clauses is not null)
            {
                foreach (var clause in c.Clauses)
                {
                    if (!string.IsNullOrWhiteSpace(clause?.Description))
                        clausesHtml.AppendLine($"<li>{HtmlEncode(clause.Description)}</li>");
                }
            }

            var ul = $"<ul>{clausesHtml}</ul>";
            var result = _foreachRegex.Replace(sb.ToString(), ul);

            return result;
        }

        /// <summary>
        /// Muy básico para evitar romper el HTML con datos de usuario.
        /// (Si ya controlas el origen, puedes omitirlo.)
        /// </summary>
        private static string HtmlEncode(string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }
    }
}