using Entity.Domain.Models.Implements.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Business
{
    public class ClauseSeeder : IEntityTypeConfiguration<Clause>
    {
        public void Configure(EntityTypeBuilder<Clause> builder)
        {
            var seedDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Clause
                {
                    Id = 1,
                    Name = "CLÁUSULA PRIMERA. - OBJETO",
                    Description = @"En virtud del presente contrato, el ARRENDADOR concede al ARRENDATARIO el uso y goce del inmueble de las siguientes características: a) Locales 5A, 6A Y 7A, ubicados en el costado norte de la Plaza de Mercado del municipio de Palermo (H).",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 2,
                    Name = "CLÁUSULA SEGUNDA: - VIGENCIA",
                    Description = @"El término de duración del presente contrato será de SEIS (6) MESES, contados a partir de la fecha de la firma del presente contrato.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 3,
                    Name = "CLÁUSULA TERCERA.- VALOR DEL CANON DE ARRENDAMIENTO Y FORMA DE PAGO",
                    Description = @"EL ARRENDATARIO se obliga a pagar mensualmente al ARRENDADOR el Valor/Canon, de 0,8  U.V.T. por cada uno de los Locales dados en arriendo, por un valor/Canon total de 2,4 U.V.T., que al momento de la suscripción del presente contrato (año 2025) es de, CIENTO DIECINUEVE MIL QUINIENTOS DIECISIETE PESOS ($119.517) M/CTE, más I.V.A., según lo estipulado en el artículo 362 del Acuerdo No. 037 (Estatuto Tributario Municipal) del 04 de diciembre del 2012,  pagaderos de manera anticipada dentro de los cinco (5) primeros días de cada mes previa expedición del recibo de pago en las oficinas de la Secretaría de Hacienda-Tesorería del Municipio de Palermo (H). Parágrafo: El valor se ajustará periódicamente en los términos señalados por el Estatuto Tributario Municipal y el Honorable Concejo Municipal.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 4,
                    Name = "CLÁUSULA CUARTA. - DESTINACIÓN",
                    Description = @"EL ARRENDATARIO se compromete a utilizar los Locales Nº 5A, 6A Y 7A ubicados en el costado norte de la Plaza de Mercado del municipio de Palermo (H), para establecer un centro de acopio y gestión de residuos reciclables, operado por el establecimiento de comercio supermercado centro sur Palermo, EL ARRENDATARIO no podrá dar un uso distinto al aquí establecido. El incumplimiento de esta obligación, dará derecho al ARRENDADOR para la terminación unilateral del presente contrato y exigir la entrega del bien inmueble.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 5,
                    Name = "CLÁUSULA QUINTA. – REPARACIONES",
                    Description = @"Los daños que se ocasionen a los bienes entregados en arrendamiento durante la vigencia del presente contrato, serán reparados y cubiertos sus costos de reparación en su totalidad por el ARRENDATARIO.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 6,
                    Name = "CLÁUSULA SEXTA. - RECIBO Y ESTADO",
                    Description = @"El ARRENDATARIO declara que ha recibido el inmueble en buen uso y estado de funcionamiento para desarrollar la destinación contratada a su vez se obliga a cuidarlo, conservarlo y mantenerlo y, que en el mismo estado lo restituirá a EL ARRENDADOR. Los daños al inmueble derivados del uso, mal trato y/o descuido por parte del ARRENDATARIO durante su tenencia, serán de su cargo y EL ARRENDADOR estará facultado para hacerlos por su cuenta y posteriormente reclamar su valor al ARRENDATARIO.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 7,
                    Name = "CLÁUSULA SEPTIMA. – MEJORAS",
                    Description = @"El ARRENDATARIO tendrá a su cargo las reparaciones locativas a que se refiere la Ley y no podrán realizarse otras sin el consentimiento previo expreso del ARRENDADOR.  Parágrafo: Las mejoras realizadas al bien serán del ARRENDADOR y no habrá lugar al reconocimiento del precio, costo o indemnización alguna por parte del Arrendatario por las mejoras hechas. Las mejoras no podrán retirarse salvo que el ARRENDADOR lo exija por escrito, a lo que el Arrendatario accederá inmediatamente a su costa, dejando el Inmueble en el mismo buen estado en que lo recibió del ARRENDADOR, salvo el deterioro natural por el uso legítimo.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 8,
                    Name = "CLÁUSULA OCTAVA. – SERVICIOS PÚBLICOS",
                    Description = @"EL ARRENDATARIO pagará oportuna y totalmente los servicios públicos de agua y energía y los demás que se encontraren instalados en los locales comerciales, desde la fecha en que inicie el arrendamiento hasta la terminación del mismo. El incumplimiento del Arrendatario en el pago oportuno de los servicios públicos antes mencionados se tendrá como incumplimiento del Contrato. Parágrafo 1: EL ARRENDATARIO declara que ha recibido en perfecto estado de funcionamiento y de conservación las instalaciones para uso de los servicios públicos del bien, que se abstendrá de modificarlas sin permiso previo y escrito del ARRENDADOR y que responderá por daños y/o violaciones de los reglamentos de las correspondientes empresas de servicios públicos, según contrato de condiciones uniformes. Parágrafo 2: EL ARRENDATARIO reconoce que el ARRENDADOR en ningún caso y bajo ninguna circunstancia es responsable por la interrupción o deficiencia en la prestación de cualquiera de los servicios públicos del bien. En caso de la prestación deficiente o suspensión de cualquiera de los servicios públicos del bien, el Arrendatario reclamará de manera directa a las empresas prestadoras del servicio y no al ARRENDADOR.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 9,
                    Name = "CLÁUSULA NOVENA. – OBLIGACIONES DE LAS PARTES",
                    Description = @"A) DEL ARRENDADOR: 1. EL ARRENDADOR, Hará entrega material del bien inmueble al ARRENDATARIO, de los locales 5A , 6A Y 7A ubicados en el costado norte de la Plaza de Mercado del municipio de Palermo (H), en buen estado de servicio, seguridad, sanidad y demás de usos conexos convenidos en el presente contrato mediante inventario, el cual hará el ARRENDADOR. B) DEL ARRENDATARIO: 1. Pagar al ARRENDADOR en el lugar convenido en la cláusula segunda del presente contrato, mensualmente el canon de arrendamiento. 2. Efectuar del pago mensual de los Servicios Públicos, si este goza de ellos 3. Gozar del inmueble según los términos del contrato. 4. Velar por la conservación del bien inmueble y las cosas recibidas en arrendamiento. 5. Restituir/Entregar el bien inmueble a la terminación del contrato de arrendamiento, en el estado en que fue entregado, poniéndolo a disposición del ARRENDADOR, éste debe encontrarse paz y salvo por todo concepto entiéndase canon de arrendamiento y servicios públicos. 6. Permitir en cualquier tiempo las visitas del Arrendador o de sus representantes, para constatar el estado y la conservación del inmueble u otras circunstancias que sean de su interés, siempre y cuando dichas visitas no afecten la continuidad regular del servicio a cargo del Arrendatario. 7. Analizar y responder los requerimientos que formule razonablemente el Arrendador. Parágrafo: La responsabilidad del Arrendatario subsistirá aún después de restituido el inmueble, mientras el Municipio no haya entregado el paz y salvo correspondiente por escrito al ARRENDATARIO.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 10,
                    Name = "CLÁUSULA DÉCIMA. – TERMINACIÓN DEL CONTRATO",
                    Description = @"Son causales de terminación del contrato de arrendamiento las estipuladas en artículo 17 de la Ley 80 de 1993, entre otras: 1. El no pago del canon de arrendamiento y/o servicios públicos por parte del arrendatario. 2. La enajenación, el subarriendo y/o La Cesión del inmueble y/o contrato de arrendamiento y el cambio de destinación del bien sin previa autorización expresa y escrita por parte del ARRENDADOR. 3. Las mejoras, cambios, ampliaciones, modificaciones y demás que se le realicen al bien sin previa autorización expresa y escrita por parte del ARRENDADOR. 4. Comportamientos por parte del arrendatario que afecten la tranquilidad y sana convivencia de los demás arrendatarios y ciudadanos. 5. La suspensión de la prestación de los servicios públicos del bien inmueble por mora en el pago de las facturas y/o acción del ARRENDATARIO. 6. Por la expiración del tiempo pactado. 7. Por incumplimiento de alguna de las obligaciones pactadas en el presente contrato. parágrafo: Las partes de común acuerdo en cualquier tiempo podrán dar por terminado el presente contrato de arrendamiento, aclarando que por cualquiera que sea la causa y/o motivo de la terminación del presente contrato de arrendamiento, éste debe ser entregado encontrándose a paz y salvo por todo concepto entiéndase canon de arrendamiento y servicios públicos y en el mismo estado en que fue recibido por parte del ARRENDATARIO.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 11,
                    Name = "CLÁUSULA DÉCIMA PRIMERA: RESTITUCIÓN",
                    Description = @"Vencido el periodo inicial o la última prórroga del Contrato o declarada la terminación del mismo por cualquier causa, EL ARRENDATARIO (i) restituirá el bien al ARRENDADOR en las mismas buenas condiciones en que lo recibió  del ARRENDADOR, salvo el deterioro natural causado por el uso legítimo, (ii) entregará al  ARRENDADOR los ejemplares originales de las facturas de cobro por concepto de servicios públicos del Inmueble correspondientes a los últimos tres (3) meses, debidamente canceladas por el Arrendatario, con una antelación de dos (2) días hábiles a la fecha fijada para la restitución material del bien inmueble al ARRENDADOR. parágrafo: La responsabilidad del Arrendatario subsistirá aún después de restituido el Inmueble, mientras el ARRENDADOR no haya entregado el paz y salvo correspondiente por escrito al Arrendatario.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 12,
                    Name = "CLÁUSULA DÉCIMA TERCERA. – RENUNCIA",
                    Description = @"EL ARRENDATARIO declara que (i) no ha tenido ni tiene posesión del bien, y (ii) que renuncia en beneficio del ARRENDADOR, a todo requerimiento para constituirlo en mora en el cumplimiento de las obligaciones a su cargo derivadas de este Contrato.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 13,
                    Name = "CLÁUSULA DÉCIMA TERCERA. – INCUMPLIMIENTO",
                    Description = @"El incumplimiento del ARRENDATARIO a cualquiera de  sus obligaciones legales o contractuales faculta al ARRENDADOR para ejercer las siguientes acciones, simultáneamente o en el orden que él elija: a) Declarar terminado el presente contrato y ordenar la restitución inmediatamente del bien, judicial y/o extrajudicialmente; b) Exigir y perseguir a través de cualquier medio, judicial o extrajudicialmente, al  ARRENDATARIO el monto y/o valor de los perjuicios resultantes del incumplimiento, así como de la multa por incumplimiento pactada en este Contrato.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 14,
                    Name = "CLÁUSULA DÉCIMA CUARTA. – ABANDONO",
                    Description = @"EL ARRENDATARIO autoriza de manera expresa e irrevocable al ARRENDADOR para ingresar al bien en aras de recuperar su tenencia y ordenar la restitución del bien, con el solo requisito de la presencia de dos (2) testigos, en procura de evitar el deterioro o desmantelamiento del bien, en el evento que por cualquier causa o circunstancia el bien permanezca abandonado o deshabitado por el término de dos (2) meses o más.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 15,
                    Name = "CLÁUSULA DÉCIMA QUINTA. – MORA",
                    Description = @"Cuando el ARRENDATARIO incumpliera en el pago del canon de arrendamiento establecido en la cláusula tercera del presente contrato, EL ARRENDADOR, podrá dar por terminado de manera unilateral el contrato y exigir la restitución del bien inmueble.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 16,
                    Name = "CLÁUSULA DÉCIMA SEXTA.- SUPERVISIÓN",
                    Description = @"La vigilancia, seguimiento, verificación y cumplimiento del presente contrato de arrendamiento, serán ejercidos por el alcalde del Municipio de Palermo y/o el funcionario que designe para la Supervisión, quien podrá impartir al ARRENDATARIO las instrucciones e indicaciones necesarias para la cabal ejecución del objeto contratado y desarrollará las demás actividades previstas en este contrato y en el contrato de supervisión o en el acto de designación, según sea el caso.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 17,
                    Name = "CLÁUSULA DÉCIMA SÉPTIMA. - PREAVISO",
                    Description = @"Tanto EL ARRENDADOR como EL ARRENDATARIO podrán dar por terminado el presente contrato de arrendamiento mediante preaviso por escrito dado a la otra parte con mínimo Un (01) meses de antelación del vencimiento del término pactado en el presente contrato.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 18,
                    Name = "CLÁUSULA DÉCIMA OCTAVA. – PRÓRROGA",
                    Description = @"El presente contrato no se prorrogará de forma automática, sin embargo, las partes previo aviso por escrito podrán manifestar la voluntad continuar con el presente, con mínimo Un (01) mes de antelación a la terminación del contrato. parágrafo: Para concederse la prórroga del contrato, será requisito encontrarse paz y salvo por todo concepto.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 19,
                    Name = "CLÁUSULA DÉCIMA NOVENA. – CLÁUSULA PENAL",
                    Description = @"En caso de incumplimiento de las obligaciones derivadas del presente contrato a cargo del ARRENDATARIO establecidas en cualquiera de las cláusulas establecidas, éste deberá pagar al ARRENDADOR a título de cláusula penal el valor equivalente al treinta por ciento (30%) del valor total del contrato, sin menoscabo del pago del canon de arrendamiento y de los perjuicios que pudieren ocasionarse como consecuencia del incumplimiento contractual.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 20,
                    Name = "CLÁUSULA VIGÉSIMA. – RÉGIMEN LEGAL",
                    Description = @"El presente contrato se rige por lo dispuesto en las leyes del derecho civil y comercial respecto al contrato de arrendamiento y lo estipulado en la Ley 80 de 1993 y sus Decretos reglamentarios, la Ley 9 de 1989, Ley 388 de 1997, Decreto 1504 de 1998 y demás normatividad vigente o la que modificase las anteriores.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 21,
                    Name = "CLÁUSULA VIGÉSIMA PRIMERA. - VALIDEZ",
                    Description = @"El presente Contrato anula todo convenio anterior relativo al arrendamiento del mismo inmueble y solamente podrá ser modificado por acuerdo suscrito por las partes o por EL ARRENDADOR.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 22,
                    Name = "CLÁUSULA VIGÉSIMA SEGUNDA. - CONTROL PARA EL LAVADO DE ACTIVOS Y FINANCIACIÓN DEL TERRORISMO",
                    Description = @"EL ARRENDATARIO acepta, entiende y conoce, de manera voluntaria e inequívoca, que EL ARRENDADOR  en cumplimiento de su obligación legal de prevenir y controlar el lavado de activos y la financiación del terrorismo, por considerarlo una causal objetiva, podrá terminar unilateralmente el presente contrato en cualquier momento y sin previo aviso, cuando el ARRENDATARIO, llegare a ser: (i) vinculado por parte de las autoridades nacionales e internacionales a cualquier tipo de investigación por delitos de narcotráfico, terrorismo, secuestro, lavado de activos, financiación del terrorismo y administración de recursos relacionados con actividades terroristas u otros delitos relacionados con el lavado de activos y financiación del terrorismo; (ii) incluido en listas para el control de lavado de activos y financiación del terrorismo administradas por cualquier autoridad nacional o extranjera, tales como la lista de la Oficina de Control de Activos en el Exterior – OFAC emitida por la Oficina del Tesoro de los Estados Unidos de Norte América, la lista de la Organización de las Naciones Unidas y otras listas públicas relacionadas con el tema del lavado de activos y financiación del terrorismo; (iii) condenado por parte de las autoridades nacionales o internacionales en cualquier tipo de proceso judicial relacionado con la comisión de los anteriores delitos; o iv) llegare a ser señalado públicamente por cualquier como investigados por delitos de narcotráfico, terrorismo, corrupción, secuestro, lavado de activos, financiación del terrorismo y administración de recursos relacionados con actividades terroristas y/o cualquier delito colateral o subyacente a estos. parágrafo. De llegarse a presentar alguna de las situaciones anteriormente mencionadas frente a algún beneficiario, usuario, u otra persona natural o jurídica que tenga inherencia en el flujo de recursos EL ARRENDATARIO deberá asumir la responsabilidad.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 23,
                    Name = "CLÁUSULA VIGÉSIMA TERCERA. - PROHIBICIONES ESPECIALES",
                    Description = @"EL ARRENDADOR prohíbe expresa y terminantemente al ARRENDATARIO dar al inmueble destinación con los fines contemplados en el literal b) del Parágrafo del Artículo 3º del Decreto 180 del 1988 y del Artículo 34 de la Ley 30 del 1986. En consecuencia, el ARRENDATARIO se obliga a no utilizar el inmueble objeto de este contrato, para ocultar o como depósito de armas o explosivos y dineros de grupos terroristas, o para que en él se elabore o almacene, venda o use drogas, estupefacientes o sustancias alucinógenas, tales como marihuana, hachís, cocaína, morfina, heroína, metacualona y afines. EL ARRENDATARIO se obliga a no guardar, ni permitir que se guarden en el inmueble arrendado sustancias inflamables o explosivas que pongan en peligro la seguridad de él, y en caso de que ocurriere dentro del mismo, enfermedad infectocontagiosa serán de cuenta del ARRENDATARIO los gastos de desinfección que ordenen las autoridades sanitarias. Por lo anterior el bien inmueble de propiedad DEL ARRENDADOR, será protegido al suscribir el presente contrato, de lo preceptuado en la Ley 1708 del 20 de enero de 2014.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 24,
                    Name = "CLÁUSULA VIGÉSIMA CUARTA. - SUBARRIENDO Y CESIÓN",
                    Description = @"El ARRENDATARIO no está facultado para ceder el arriendo ni subarrendar, a menos que medie autorización previa y escrita de EL ARRENDADOR.  En caso de contravención, EL ARRENDADOR podrá dar por terminado el contrato de arrendamiento y exigir la entrega del inmueble.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 25,
                    Name = "CLÁUSULA VIGÉSIMA QUINTA. - INDEPENDENCIA DEL ARRENDADOR",
                    Description = @"El ARRENDADOR es una entidad independiente del ARRENDATARIO y, en consecuencia, el ARRENDADOR no es su representante, agente o mandatario. El ARRENDADOR no tiene la facultad de hacer declaraciones, representaciones o compromisos en nombre del ARRENDATARIO, ni de tomar decisiones o iniciar acciones que generen obligaciones a su cargo y viceversa.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 26,
                    Name = "CLÁUSULA VIGÉSIMA SEXTA. - INDEMNIDAD",
                    Description = @"El ARRENDATARIO se obliga a mantener indemne al ARRENDADOR de cualquier daño o perjuicio originado en reclamaciones de terceros que tenga como causa sus actuaciones hasta por el monto del daño o perjuicio causado. El ARRENDATARIO mantendrá indemne al ARRENDADOR por cualquier obligación de carácter laboral o relacionado que se originen en el incumplimiento de las obligaciones laborales que el ARRENDATARIO asume frente al personal, subordinados o terceros que se vinculen a la ejecución de las obligaciones derivadas del presente contrato.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 27,
                    Name = "CLÁUSULA VIGÉSIMA SEPTIMA. - INCOMPATIBILIDADES",
                    Description = @"EL ARRENDATARIO Con la suscripción del presente contrato, afirma bajo la gravedad de juramento que no se haya incurso en ninguna de las inhabilidades e incompatibilidades y demás prohibiciones para contratar previstas en la constitución política de Colombia la Ley 80 de 1993 Artículo 8,Artículo 5 Ley 1738 de 2014y demás disposiciones legales sobre la materia. En el evento de llegar a sobrevenir alguna causal actuara conforme a lo previsto a la Ley.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 28,
                    Name = "CLÁUSULA VIGÉSIMA OCTAVA. – CONFIDENCIALIDAD",
                    Description = @"EL CONTRATISTA se compromete a guardar estricta confidencialidad y a dar cumplimiento a la normatividad aplicable vigente, respecto de toda la información y datos personales que conozca y se le entregue por cualquier medio durante el plazo de ejecución, y por ende éste no podrá realizar su publicación, divulgación y utilización para fines propios o de terceros no autorizados. Así mismo, respetará los acuerdos de confidencialidad suscritos por EL CONTRATANTE con terceros para la celebración de negocios, preacuerdos o acuerdos por el mismo tiempo por el que EL CONTRATANTE se compromete con los terceros a guardar la debida reserva.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 29,
                    Name = "CLÁUSULA VIGÉSIMA NOVENA. - CASO FORTUITO Y FUERZA MAYOR",
                    Description = @"Las partes quedan exoneradas de responsabilidad por el incumplimiento de cualquiera de sus obligaciones o por la demora en la satisfacción de cualquiera de las prestaciones a su cargo derivadas del presente Contrato, cuando el incumplimiento sea resultado o consecuencia de la ocurrencia de un evento de fuerza mayor y caso fortuito debidamente invocadas y constatadas de acuerdo con la ley y la jurisprudencia colombiana.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 30,
                    Name = "CLÁUSULA TRIGÉSIMA. – MÉRITO EJECUTIVO",
                    Description = @"EL ARRENDATARIO declara de manera expresa que reconoce y acepta que este Contrato presta mérito ejecutivo para exigir del ARRENDATARIO y a favor  del ARRENDADOR el pago de PRIMERO:(i) los cánones de arrendamiento causados y no pagados por EL ARRENDATARIO, SEGUNDO:(ii) las multas y sanciones que se causen por el incumplimiento del ARRENDATARIO de cualquiera de las obligaciones a su cargo en virtud de la ley o de este Contrato, TERCERO:(iii) las sumas causadas y no pagadas por EL ARRENDATARIO por concepto de servicios públicos del Inmueble, cuotas de administración y cualquier otra suma de dinero que por cualquier concepto deba ser pagada por EL ARRENDATARIO; para lo cual bastará la sola afirmación de incumplimiento del Arrendatario hecha por EL ARRENDADOR, afirmación que solo podrá ser desvirtuada por el ARRENDATARIO con la presentación de los respectivos recibos de pago. Parágrafo 1: Las Partes acuerdan que cualquier copia autentica de este Contrato tendrá mismo valor que el original para efectos judiciales y extrajudiciales. Parágrafo 2. El cobro de los conceptos antes referidos se adelantará mediante jurisdicción coactiva del municipio",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 31,
                    Name = "CLÁUSULA TRIGÉSIMA PRIMERA. - SUJECIÓN A LA LEY DE TRANSPARENCIA Y DEL DERECHO A LA INFORMACIÓN PÚBLICA NACIONAL",
                    Description = @"Todas las actuaciones que se deriven del presente documento se harán con sujeción a lo dispuesto en la Ley 1712 de 2014.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 32,
                    Name = "CLÁUSULA TRIGÉSIMA SEGUNDA. - AUTORIZACIÓN TRATAMIENTO DE DATOS",
                    Description = @"EL ARRENDATARIO, en la condición de titular de la información personal autoriza al ARRENDADOR para almacenar en sus bases de información los datos personales y tener acceso a los mismos en cualquier momento, tanto durante la vigencia de la relación contractual como con posterioridad a la misma, esta autorización abarca la posibilidad de recolectar y almacenar dichos datos en las bases de datos y sistemas o software de EL CONTRATANTE. Entiendo que el tratamiento de los datos personales por parte del ARRENDADOR, tiene una finalidad legitima de acuerdo con la ley y la constitución y obedece al manejo interno de los datos en desarrollo de la relación contractual existente entre las partes y que la información personal será manejada con las medidas técnicas, humanas y administrativas necesarias para garantizar la seguridad y reserva de la información. Parágrafo: EL ARRENDADOR ha enterado a EL ARRENDATARIO, del derecho a conocer el uso a sus datos, acceder a ellos, actualizarlos y rectificarlos en cualquier momento. Igualmente, EL ARRENDADOR ha informado sobre el carácter facultativo de la respuesta a las preguntas que versen sobre datos sensibles.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 33,
                    Name = "CLÁUSULA TRIGÉSIMA TERCERA. - NOTIFICACIONES",
                    Description = @"Los avisos, solicitudes, comunicaciones y notificaciones que las partes deban hacer en desarrollo del presente contrato, deben constar por escrito y se entenderán debidamente efectuadas si se envía a cualquiera de los canales de notificación entregados por el ARRENDATARIO.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 34,
                    Name = "CLÁUSULA TRIGÉSIMA CUARTA. – ANEXOS DEL CONTRATO",
                    Description = @"Hacen parte integrante de este contrato los siguientes documentos: 1. Los estudios y documentos previos. 2. La solicitud presentada por el ARRENDATARIO. 3. Acta de entrega e inventario de los inmuebles.",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Clause
                {
                    Id = 35,
                    Name = "CLÁUSULA TRIGÉSIMA QUINTA. – DOMICILIO",
                    Description = @"Para efectos legales, se consagra como Domicilio contractual el Municipio de Palermo (H).",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                }
            );
        }
    }
}

