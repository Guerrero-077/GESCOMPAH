using Business.Interfaces.PDF;

namespace WebGESCOMPAH.Extensions.Infrastructure
{
    /// <summary>
    /// Inicialización temprana del generador de PDF para evitar latencia en la primera petición.
    /// </summary>
    /// <remarks>
    /// Qué hace: crea un scope, resuelve el servicio de generación de PDF y ejecuta una rutina
    /// de warmup (que descarga/arranca el navegador si es necesario).
    /// 
    /// Por qué: la primera renderización con Playwright/Chromium suele ser lenta; calentar el
    /// stack mejora el TTFB de la primera solicitud real.
    /// 
    /// Para qué: experiencia más fluida tras el despliegue o reinicio.
    /// </remarks>
    public static class PdfWarmupExtensions
    {
        /// <summary>
        /// Ejecuta la rutina de warmup de PDF de forma segura (no revienta la app si falla).
        /// </summary>
        public static async Task UsePdfWarmupAsync(this WebApplication app)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var pdfGen = scope.ServiceProvider.GetRequiredService<IContractPdfGeneratorService>();
                await pdfGen.WarmupAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Warmup PDF] Aviso: {ex.Message}");
            }
        }
    }
}

