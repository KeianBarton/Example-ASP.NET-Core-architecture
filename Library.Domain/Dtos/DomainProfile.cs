using AutoMapper;
using Library.Domain.Entities;

namespace Library.Domain.Dtos
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<AuthorDto, Author>();
            CreateMap<BookDto, Book>();
        }
    }
}