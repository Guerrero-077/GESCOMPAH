using Business.Interfaces.PDF;
using Entity.DTOs.Implements.Business.Contract;
using Microsoft.Playwright;
using System.Text;
using System.Text.RegularExpressions;
using Templates.Templates;

namespace Business.Services.Utilities.PDF
{
    public class ContractPdfService : IContractPdfGeneratorService
    {
        public async Task<byte[]> GeneratePdfAsync(ContractSelectDto contract)
        {
            // 1. Preparar HTML (usando plantilla y reemplazos)
            string template = ContractTemplate.Html;
            string html = ReplacePlaceholders(template, contract);

            // 2. Inicializar Playwright y abrir navegador
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            // 3. Cargar el HTML directamente (sin necesidad de host)
            await page.SetContentAsync(html);

            // 4. Generar PDF (tamaño A4, márgenes, etc.)
            var pdfBytes = await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                Margin = new() { Top = "40px", Bottom = "40px", Left = "40px", Right = "40px" },
                PrintBackground = true
            });

            return pdfBytes;
        }

        private string ReplacePlaceholders(string template, ContractSelectDto c)
        {
            template = template.Replace("@Model.FullName", c.FullName);
            template = template.Replace("@Model.Document", c.Document);
            template = template.Replace("@Model.StartDate.ToString(\"dd/MM/yyyy\")", c.StartDate.ToString("dd/MM/yyyy"));
            template = template.Replace("@Model.EndDate.ToString(\"dd/MM/yyyy\")", c.EndDate.ToString("dd/MM/yyyy"));
            //template = template.Replace("@Model.Terms.UvtQty", c.Terms.UvtQty.ToString("0.##"));
            //template = template.Replace("@(Model.Terms.UseSystemParameters ? \"Sí\" : \"No\")", c.Terms.UseSystemParameters ? "Sí" : "No");

            var p = c.PremisesLeased.FirstOrDefault();
            if (p != null)
            {
                template = template.Replace("@Model.PremisesLeased[0].EstablishmentName", p.EstablishmentName);
                template = template.Replace("@Model.PremisesLeased[0].Address", p.Address);
                template = template.Replace("@Model.PremisesLeased[0].AreaM2", p.AreaM2.ToString("0.##"));
                template = template.Replace("@Model.PremisesLeased[0].PlazaName", p.PlazaName);
            }

            var clausulasHtml = new StringBuilder();
            foreach (var clause in c.Clauses)
                clausulasHtml.AppendLine($"<li>{clause.Description}</li>");

            return Regex.Replace(template, "@foreach\\s*\\(.*?\\)\\s*{.*?}", $"<ul>{clausulasHtml}</ul>", RegexOptions.Singleline);
        }
    }
}
