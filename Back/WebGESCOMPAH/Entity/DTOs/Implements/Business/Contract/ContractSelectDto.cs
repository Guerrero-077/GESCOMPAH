namespace Entity.DTOs.Implements.Business.Contract
{
    public class ContractSelectDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Destination { get; set; } = null!;
        public string PDFPath { get; set; } = null!;
    }
}
