using System;
using System.Collections.Generic;

namespace Library.Domain.Entities
{
    public class Author
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public string Genre { get; set; }

        public ICollection<Book> Books { get; set; }
            = new List<Book>();

        public void Modify(string firstName, string lastName,
            DateTimeOffset dateOfBirth, string genre, ICollection<Book> books)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Genre = genre;
            Books = books;
        }
    }
}
