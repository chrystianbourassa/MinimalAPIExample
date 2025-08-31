using System.ComponentModel.DataAnnotations;

namespace IDSApi.Models
{
    public partial class Chauffeur
    {
        [Required]
        [Key]
        public int ChauffeurId { get; set; }

        public string? Nom { get; set; }

        public string? NomCache { get; set; }

        public string? NomObibox { get; set; }

        public int? CourtierId { get; set; }

        public string? Telephone { get; set; }

        public string? Adresse { get; set; }

        public string? NumeroPermis { get; set; }

        public string? Nas { get; set; }

        public string? Email { get; set; }

        public bool? CalculerTaxes { get; set; }

        public string? NoTvq { get; set; }

        public string? NoTps { get; set; }

        public bool? IsObibox { get; set; }

        public decimal? MontantObibox { get; set; }

        public bool? Actif { get; set; }

        public bool? Online { get; set; }

        public string? PhotoUrl { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }


    }
}
