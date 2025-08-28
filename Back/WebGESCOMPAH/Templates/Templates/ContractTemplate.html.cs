namespace Templates.Templates
{
    public static class ContractTemplate
    {
        public static readonly string Html = @"
<!DOCTYPE html>
<html lang=""es"">
<head>
  <meta charset=""UTF-8"">
  <style>
    body {
      font-family: Arial, sans-serif;
      font-size: 12pt;
      line-height: 1.5;
      color: #000;
    }

    h1, h2 {
      text-align: center;
      text-transform: uppercase;
    }

    .clause {
      margin-top: 20px;
    }

    .signature-section {
      margin-top: 50px;
    }

    .signature {
      margin-top: 60px;
      text-align: center;
    }
  </style>
</head>
<body>
  <h1>Contrato de Arrendamiento de Local Comercial</h1>

  <p>Entre los suscritos a saber: por una parte <strong>GESCOMPAH S.A.S</strong>, quien en adelante se denominará EL ARRENDADOR, y por otra parte <strong>@Model.FullName</strong>, identificado(a) con cédula de ciudadanía N.° <strong>@Model.Document</strong>, quien en adelante se denominará EL ARRENDATARIO, se ha celebrado el presente contrato de arrendamiento que se regirá por las siguientes cláusulas:</p>

  <div class=""clause"">
    <h2>Primera: Objeto</h2>
    <p>EL ARRENDADOR da en arrendamiento a EL ARRENDATARIO el inmueble denominado <strong>@Model.PremisesLeased[0].EstablishmentName</strong>, ubicado en la dirección <strong>@Model.PremisesLeased[0].Address</strong>, con un área de <strong>@Model.PremisesLeased[0].AreaM2</strong> m2, perteneciente a la plaza <strong>@Model.PremisesLeased[0].PlazaName</strong>.</p>
  </div>

  <div class=""clause"">
    <h2>Segunda: Canon de Arrendamiento</h2>
    <p>El canon mensual de arrendamiento será equivalente a <strong>@Model.Terms.UvtQty</strong> UVTs, según el valor vigente al momento de la facturación, determinado por la DIAN. Este valor puede ser ajustado según los parámetros del sistema: <strong>@(Model.Terms.UseSystemParameters ? ""Sí"" : ""No"")</strong>.</p>
  </div>

  <div class=""clause"">
    <h2>Tercera: Plazo del Contrato</h2>
    <p>El presente contrato tendrá una duración desde el día <strong>@Model.StartDate.ToString(""dd/MM/yyyy"")</strong> hasta el <strong>@Model.EndDate.ToString(""dd/MM/yyyy"")</strong>.</p>
  </div>

  <div class=""clause"">
    <h2>Cláusulas Especiales</h2>
    <ul>
      @foreach (var clausula in Model.Clauses) {
        <li>@clausula.Description</li>
      }
    </ul>
  </div>

  <div class=""signature-section"">
    <div class=""signature"">
      ___________________________<br />
      EL ARRENDADOR<br />
      GESCOMPAH S.A.S
    </div>

    <div class=""signature"">
      ___________________________<br />
      EL ARRENDATARIO<br />
      @Model.FullName<br />
      CC @Model.Document
    </div>
  </div>
</body>
</html>";
    }
}
