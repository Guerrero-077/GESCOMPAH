namespace Entity.DTOs.Implements.Business.Plaza
{
    public class PlazaUpdateDto : PlazaBaseDto
    {
        public int Id { get; set; }
        public bool Active { get; set; } // Indicates if the plaza is active
    }
}