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
    /* Configuración de página y estilo general conforme a prácticas comunes (Colombia) */
    @page { size: Letter; margin: 25mm; }
    body { font-family: Arial, sans-serif; font-size: 12pt; line-height: 1.5; color: #000; }
    h1, h2 { text-align: center; text-transform: uppercase; }
    p { text-align: justify; text-justify: inter-word; hyphens: auto; }
    .section { margin-top: 20px; }
    .info { margin: 12px 0; }
    .grid { width: 100%; border-collapse: collapse; margin-top: 8px; }
    .grid td { padding: 6px 8px; vertical-align: top; }
    .grid .label { color: #555; width: 25%; }
    .signature-section { margin-top: 50px; display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }
    .signature { margin-top: 60px; text-align: center; }
    ul { padding-left: 20px; }
    li { text-align: justify; hyphens: auto; }
  </style>
</head>
<body>
  <h1>Contrato de Arrendamiento de Local Comercial</h1>

  <p class=""info"">Entre los suscritos a saber: por una parte el <strong>@Model.LandlordEntityName</strong>, identificado con Nit. No. <strong>@Model.LandlordNIT</strong>, representado legalmente por <strong>@Model.LandlordRepName</strong>, identificado con cédula de ciudadanía No. <strong>@Model.LandlordRepDocument</strong>, en calidad de <strong>@Model.LandlordRepTitle</strong>, quien en adelante se denominará EL ARRENDADOR; y por otra parte <strong>@Model.FullName</strong>, identificado(a) con cédula de ciudadanía No. <strong>@Model.Document</strong>, en adelante EL ARRENDATARIO; se celebra el presente contrato de arrendamiento que se regirá por las siguientes estipulaciones:</p>

  <table class=""grid"">
    <tr>
      <td class=""label""><strong>Arrendador</strong></td>
      <td>
        @Model.LandlordEntityName — NIT @Model.LandlordNIT<br />
        Rep. legal: @Model.LandlordRepName — CC @Model.LandlordRepDocument — @Model.LandlordRepTitle
      </td>
    </tr>
    <tr>
      <td class=""label""><strong>Arrendatario</strong></td>
      <td>@Model.FullName — CC @Model.Document</td>
    </tr>
  </table>

  <div class=""section"">
    <h2>Primera: Objeto</h2>
    <p>EL ARRENDADOR da en arrendamiento a EL ARRENDATARIO el inmueble/local denominado <strong>@Model.PremisesLeased[0].EstablishmentName</strong>, ubicado en <strong>@Model.PremisesLeased[0].Address</strong>, con un área aproximada de <strong>@Model.PremisesLeased[0].AreaM2</strong> m², perteneciente a la plaza <strong>@Model.PremisesLeased[0].PlazaName</strong>. El local se destinará exclusivamente a las actividades comerciales permitidas por el reglamento del centro comercial y la normatividad vigente.</p>
  </div>

  <div class=""section"">
    <h2>Segunda: Canon de Arrendamiento y Pagos</h2>
    <p>El canon mensual de arrendamiento será el informado por el sistema para el contrato, valor expresado en pesos colombianos y, cuando aplique, más el impuesto al valor agregado (IVA). El pago se efectuará por mensualidades vencidas en los medios de pago autorizados por EL ARRENDADOR. La mora causará intereses a la tasa máxima legal permitida.</p>
  </div>

  <div class=""section"">
    <h2>Tercera: Plazo</h2>
    <p>El contrato tendrá vigencia desde el <strong>@Model.StartDate.ToString(""dd/MM/yyyy"")</strong> hasta el <strong>@Model.EndDate.ToString(""dd/MM/yyyy"")</strong>. Podrá renovarse por períodos iguales salvo preaviso en los términos pactados entre las partes.</p>
  </div>

  <div class=""section"">
    <h2>Cuarta: Entrega e Inventario</h2>
    <p>La entrega del inmueble se realizará mediante acta que deje constancia del estado del local, dotaciones y elementos recibidos. Al finalizar, EL ARRENDATARIO restituirá el inmueble en condiciones similares, salvo deterioro natural por el uso normal.</p>
  </div>

  <div class=""section"">
    <h2>Quinta: Uso y Reglamento</h2>
    <p>EL ARRENDATARIO se obliga a dar al inmueble el uso convenido y a cumplir el reglamento interno del centro comercial, horarios, manuales de imagen y demás políticas comunicadas por EL ARRENDADOR.</p>
  </div>

  <div class=""section"">
    <h2>Sexta: Mantenimiento y Servicios</h2>
    <p>Serán a cargo de EL ARRENDATARIO los gastos ordinarios de mantenimiento locativo y el pago de los servicios públicos, administración y demás expensas inherentes a la operación del local. Las reparaciones estructurales o de fuerza mayor serán asumidas por EL ARRENDADOR, salvo culpa del arrendatario.</p>
  </div>

  <div class=""section"">
    <h2>Séptima: Incrementos</h2>
    <p>El canon podrá ser ajustado anualmente con base en el índice de precios al consumidor (IPC) u otro índice que las partes acuerden por escrito, aplicable a partir del aniversario de inicio del contrato.</p>
  </div>

  <div class=""section"">
    <h2>Octava: Garantías</h2>
    <p>Cuando aplique, EL ARRENDATARIO mantendrá vigente la garantía acordada (depósito, póliza o codeudor) para asegurar el cumplimiento de sus obligaciones.</p>
  </div>

  <div class=""section"">
    <h2>Novena: Cesión y Subarriendo</h2>
    <p>EL ARRENDATARIO no podrá ceder el contrato ni subarrendar total o parcialmente el inmueble sin autorización previa y escrita de EL ARRENDADOR.</p>
  </div>

  <div class=""section"">
    <h2>Décima: Terminación</h2>
    <p>Son causales de terminación, entre otras, el incumplimiento de obligaciones esenciales, el uso indebido del inmueble, la falta de pago reiterada y la vulneración del reglamento. La parte cumplida podrá declarar la terminación y exigir la restitución del local y las indemnizaciones a que haya lugar.</p>
  </div>

  <div class=""section"">
    <h2>Cláusulas Especiales</h2>
    <ul>
      {{CLAUSES}}
    </ul>
  </div>

  <div class=""section"">
    <h2>Notificaciones</h2>
    <p>Las comunicaciones se entenderán válidamente realizadas en las direcciones y correos informados por las partes. Cualquier cambio deberá ser notificado por escrito con no menos de cinco (5) días hábiles de antelación.</p>
  </div>

  <div class=""section"">
    <h2>Tratamiento de Datos</h2>
    <p>EL ARRENDATARIO autoriza el tratamiento de sus datos personales conforme a la Ley 1581 de 2012 y a la política de tratamiento de datos de EL ARRENDADOR.</p>
  </div>

  <div class=""section"">
    <h2>Jurisdicción</h2>
    <p>Para todos los efectos del presente contrato, las partes fijan como domicilio contractual el del lugar de ubicación del inmueble. Las controversias se procurarán resolver por conciliación; en su defecto, ante la jurisdicción competente.</p>
  </div>

  <div class=""signature-section"">
    <div class=""signature"">
      ___________________________<br />
      EL ARRENDADOR<br />
      @Model.LandlordEntityName<br />
      Rep. Legal: @Model.LandlordRepName<br />
      @Model.LandlordRepTitle<br />
      CC @Model.LandlordRepDocument
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
