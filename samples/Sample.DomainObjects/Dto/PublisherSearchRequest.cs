namespace Sample.DomainObjects.Dto
{
    public class PublisherSearchRequest
    {

        public int? PublisherId { get; set; }


        public string Name { get; set; }


        public string BookName { get; set; }


        public string AuthorName { get; set; }


        public int? BookPublishedYear { get; set; }


        public int? AuthorAge { get; set; }


        public string AuthorCountry { get; set; }
    }
}
