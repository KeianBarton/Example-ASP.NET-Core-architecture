namespace Library.Domain.Dtos
{
    public struct BookDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsValid
        {
            get
            {
                if (Title == null ||
                    Description == null)
                {
                    return false;
                }
                if (Title.Length > 100 ||
                    Description.Length > 500)
                {
                    return false;
                }
                return true;
            }
        }
    }
}