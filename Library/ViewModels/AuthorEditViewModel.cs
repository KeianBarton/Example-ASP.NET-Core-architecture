using System;

namespace Library.ViewModels
{
    public class AuthorEditViewModel
    {
        public string Heading { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public string Genre { get; set; }
    }
}