using System.Reflection;
using Business.Services.Utilities.PDF;
using Entity.DTOs.Implements.Business.Clause;
using Entity.DTOs.Implements.Business.Contract;
using Entity.DTOs.Implements.Business.PremisesLeased;
using FluentAssertions;
using Templates.Templates;

namespace Tests.Business.PDF;

public class ContractPdfServiceTests
{
    private static string InvokeBuildHtml(ContractSelectDto dto)
    {
        var mi = typeof(ContractPdfService).GetMethod(
            "BuildHtml",
            BindingFlags.NonPublic | BindingFlags.Static);

        mi.Should().NotBeNull("BuildHtml es un método privado esperado");

        var html = (string)mi!.Invoke(null, new object[] { ContractTemplate.Html, dto })!;
        return html;
    }

    private static string InvokeHtmlEncode(string value)
    {
        var mi = typeof(ContractPdfService).GetMethod(
            "HtmlEncode",
            BindingFlags.NonPublic | BindingFlags.Static);

        mi.Should().NotBeNull("HtmlEncode es un método privado esperado");

        return (string)mi!.Invoke(null, new object[] { value })!;
    }

    [Fact]
    public void BuildHtml_ReplacesPlaceholders_EncodesValues_AndRendersClauses()
    {
        var dto = new ContractSelectDto
        {
            FullName = "Juan & <Ana>",
            Document = "123\"<>&'",
            StartDate = new DateTime(2024, 1, 2),
            EndDate = new DateTime(2025, 2, 3),
            PremisesLeased =
            {
                new PremisesLeasedSelectDto
                {
                    EstablishmentName = "Local <1>",
                    Address = "Calle & 123",
                    AreaM2 = 10, // evitar problemas de cultura con separador decimal
                    PlazaName = "Plaza 'Central'"
                }
            },
            Clauses =
            {
                new ClauseSelectDto { Name = "A", Description = "Debe <cumplir> & pagar" },
                new ClauseSelectDto { Name = "B", Description = "" }, // vacío no debe renderizarse
                new ClauseSelectDto { Name = "C", Description = null! } // null no debe renderizarse
            }
        };

        var html = InvokeBuildHtml(dto);

        html.Should().NotBeNullOrWhiteSpace();

        // Reemplazos y encoding básicos
        html.Should().Contain("Juan &amp; &lt;Ana&gt;");
        html.Should().Contain("123&quot;&lt;&gt;&amp;&#39;");

        // Fechas formateadas dd/MM/yyyy
        html.Should().Contain("02/01/2024");
        html.Should().Contain("03/02/2025");

        // Datos del local (con encoding) y área sin decimales
        html.Should().Contain("Local &lt;1&gt;");
        html.Should().Contain("Calle &amp; 123");
        html.Should().Contain(">10<");
        html.Should().Contain("Plaza &#39;Central&#39;");

        // Cláusulas: sólo las con descripción no vacía, con encoding
        html.Should().Contain("<li>Debe &lt;cumplir&gt; &amp; pagar</li>");
        html.Should().NotContain("<li></li>");
    }

    [Fact]
    public void BuildHtml_UsesDefaultLandlordValues_WhenNullOrEmpty()
    {
        var dto = new ContractSelectDto
        {
            FullName = "Test",
            Document = "1",
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 1, 2),
            LandlordEntityName = null, // usa valor por defecto
            LandlordNIT = "",          // usa valor por defecto
            LandlordRepName = " ",     // usa valor por defecto
            LandlordRepDocument = null, // usa valor por defecto
            LandlordRepTitle = null     // usa valor por defecto
        };

        var html = InvokeBuildHtml(dto);

        html.Should().Contain("MUNICIPIO DE PALERMO (H)");
        html.Should().Contain("891.180.021-9");
        html.Should().Contain("KLEYVER OVIEDO FARFAN");
        html.Should().Contain("7.717.624");
        html.Should().Contain("Alcalde Municipal");
    }

    [Fact]
    public void HtmlEncode_EncodesSpecialCharacters()
    {
        var encoded = InvokeHtmlEncode("<&>\"'");
        encoded.Should().Be("&lt;&amp;&gt;&quot;&#39;");
    }
}

