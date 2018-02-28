using System;
using System.Collections.Generic;

namespace Library.Domain.Dtos
{
    public struct AuthorDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public string Genre { get; set; }

        public ICollection<BookDto> Books { get; set; }

        public bool IsValid
        {
            get
            {
                if (FirstName == null ||
                    LastName == null ||
                    DateOfBirth == null ||
                    Genre == null)
                {
                    return false;
                }
                if (FirstName.Length > 50 ||
                    LastName.Length > 50 ||
                    Genre.Length > 50)
                {
                    return false;
                }
                if (DateOfBirth > DateTimeOffset.Now)
                {
                    return false;
                }
                return true;
            }
        }
    }
}