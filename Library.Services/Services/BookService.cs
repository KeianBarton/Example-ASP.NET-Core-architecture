using AutoMapper;
using Library.Domain.Dtos;
using Library.Domain.Entities;
using Library.EntityFramework.DatabaseContext;
using Library.EntityFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Guid AddBookForAuthor(Guid authorId, BookDto bookDto)
        {
            if (!bookDto.IsValid)
                throw new InvalidDataException();

            var author = _unitOfWork.Authors.Read(authorId);
            if (author == null)
                throw new DataNotFoundException("Author not found");

            if (author.Books.Any(b => b.Title == bookDto.Title))
                throw new DataAlreadyExistsException("Book already exists");

            var book = _mapper.Map<Book>(bookDto);
            author.Books.Add(book);
            _unitOfWork.Complete();
            return book.Id;
        }

        public bool BookExists(Guid bookId)
        {
            return _unitOfWork.Books.Read(bookId) != null;
        }

        public bool BookExists(BookDto bookDto)
        {
            return _unitOfWork.Books.Read(b =>
                b.Title == bookDto.Title &&
                b.Description == bookDto.Description
            ).Any();
        }

        public void DeleteBook(Guid bookId)
        {
            if (!BookExists(bookId))
                throw new DataNotFoundException("Book not found");

            _unitOfWork.Books.Delete(bookId);
            _unitOfWork.Complete();
        }

        public BookDto GetBookForAuthor(Guid authorId, Guid bookId)
        {
            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException("Author not found");

            var book = author.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
                throw new DataNotFoundException("Book not found");

            return _mapper.Map<Book, BookDto>(book);
        }

        public IEnumerable<BookDto> GetBooksForAuthor(Guid authorId)
        {
            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException("Author not found");

            if (!author.Books.Any())
                throw new DataNotFoundException("No books found");

            return author.Books.Select(_mapper.Map<Book, BookDto>);
        }

        public void UpdateBookForAuthor(Guid authorId, Guid bookId, BookDto bookDto)
        {
            if (!bookDto.IsValid)
                throw new InvalidDataException();

            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException("Author not found");

            var book = _mapper.Map<Book>(bookDto);
            var bookDb = author.Books.SingleOrDefault(b => b.Id == bookId);
            if (bookDb == null)
                throw new DataNotFoundException("Book not found");

            bookDb.Modify(bookDto.Title, bookDto.Description);
            _unitOfWork.Complete();
        }
    }
}