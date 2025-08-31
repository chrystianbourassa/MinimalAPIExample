using System.ComponentModel.DataAnnotations;

namespace IDSApi.Models
{
    public partial class Client
    {
        [Required][Key] public int ClientId { get; set; }


        public string? ClientCode { get; set; }

        public string? Compagnie { get; set; }

        public string? CompagnieCache { get; set; }

        public string? Contact { get; set; }

        public string? Adresse { get; set; }

        public string? Telephone { get; set; }

        public string? Email { get; set; }

        public string? Email2 { get; set; }

        public string? Email3 { get; set; }

        public string? UserId { get; set; }

        public int? CalculationMethodClientId { get; set; }

        public bool? Glaciere { get; set; }

        public decimal? GlaciereMontantClient { get; set; }

        public decimal? GlaciereMontantChauffeur { get; set; }

        public int? CourtierId { get; set; }

        public decimal? MontantCourtier { get; set; }

        public decimal? PrixFixeWeekend { get; set; }

        public bool? Actif { get; set; }

        public int? EchangeClientId { get; set; }

        public int? ExchangelivraisonGratuite { get; set; }

        public decimal? EchangeCredit { get; set; }

        public bool? Echange { get; set; }

        public bool? EtatDesComptes { get; set; }



    }
}