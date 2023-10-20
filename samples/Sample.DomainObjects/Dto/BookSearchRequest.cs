namespace Sample.DomainObjects.Dto
{
    public class BookSearchRequest
    {
        public int Start { get; set; }

        public int Max { get; set; }

        public BookSearchDto SearchData { get; set; }
    }
}
