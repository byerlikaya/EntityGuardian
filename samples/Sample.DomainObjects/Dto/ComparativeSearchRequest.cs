namespace Sample.DomainObjects.Dto
{
    public class ComparativeSearchRequest
    {
        public int? AuthorAge { get; set; }


        public int? PublishedStartYear { get; set; }
        public int? PublishedEndYear { get; set; }
        public decimal? Price { get; set; }

        public decimal? StartPrice { get; set; }
        public decimal? EndPrice { get; set; }
        public DateTime? BookCreatedDate { get; set; }

        public DateTime? StartCreatedDate { get; set; }

        public DateTime? EndCreatedDate { get; set; }
    }
}
