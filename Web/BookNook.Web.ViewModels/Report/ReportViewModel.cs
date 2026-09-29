using System.ComponentModel.DataAnnotations;

namespace BookNook.Web.Models
{
    public class ReportViewModel
    {
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public List<GenreStat> GenreStats { get; set; } = new List<GenreStat>();
        public List<TopBookStat> TopBooks { get; set; } = new List<TopBookStat>();
    }

    public class GenreStat
    {
        public string Genre { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopBookStat
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
    }
}