namespace IDSApi.Models;

public partial class VacationRequest
{
    public int VacationRequestId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Reason { get; set; } = null!;

    public int ChauffeurId { get; set; }  // Fixed: Changed from SqlDbType to int

    public bool IsApproved { get; set; }
}