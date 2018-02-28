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
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Guid AddAuthor(AuthorDto authorDto)
        {
            if (!authorDto.IsValid)
                throw new InvalidDataException();

            if (AuthorExists(authorDto))
                throw new DataAlreadyExistsException("Author already exists");

            var id = _unitOfWork.Authors.Create(_mapper.Map<Author>(authorDto));
            _unitOfWork.Complete();
            return id;
        }

        public bool AuthorExists(Guid authorId)
        {
            return _unitOfWork.Authors.Read(authorId) != null;
        }

        public bool AuthorExists(AuthorDto authorDto)
        {
            return _unitOfWork.Authors.Read(a =>
                a.FirstName == authorDto.FirstName &&
                a.LastName == authorDto.LastName &&
                a.DateOfBirth == authorDto.DateOfBirth
            ).Any();
        }

        public void DeleteAuthor(Guid authorId)
        {
            if (!AuthorExists(authorId))
                throw new DataNotFoundException("Author not found");

            _unitOfWork.Authors.Delete(authorId);
            _unitOfWork.Complete();
        }

        public AuthorDto GetAuthor(Guid authorId)
        {
            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException("Author not found");

            return _mapper.Map<Author, AuthorDto>(author);
        }

        public IEnumerable<AuthorDto> GetAuthors()
        {
            var authors = _unitOfWork.Authors.ReadAll();

            if (!authors.Any())
                throw new DataNotFoundException();

            return authors.Select(_mapper.Map<Author, AuthorDto>)
                .OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public void UpdateAuthor(Guid id, AuthorDto authorDto)
        {
            if (!authorDto.IsValid)
                throw new InvalidDataException();

            var authorDb = _unitOfWork.Authors.Read(id);

            if (authorDb == null)
                throw new DataNotFoundException("Author not found");

            authorDb.Modify(
                authorDto.FirstName, authorDto.LastName, authorDto.DateOfBirth, authorDto.Genre);
            _unitOfWork.Complete();
        }
    }
}