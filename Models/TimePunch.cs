namespace IDSApi.Models;

public partial class TimePunch
{
    public int TimePunchId { get; set; }

    public int ChauffeurId { get; set; }

    public DateTime? PunchIn { get; set; }

    public string? PunchInlocation { get; set; }

    public DateTime? PunchOut { get; set; }

    public string? PunchOutlocation { get; set; }

    public decimal? Duration { get; set; }
}
