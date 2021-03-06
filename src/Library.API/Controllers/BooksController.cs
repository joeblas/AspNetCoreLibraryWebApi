﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthorFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

            var booksForAuthor = Mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepo);

            return Ok(booksForAuthor);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            var bookForAuthor = Mapper.Map<BookDto>(bookForAuthorFromRepo);
            return Ok(bookForAuthor);

        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookForCreationDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = Mapper.Map<Book>(book);

            _libraryRepository.AddBookForAuthor(authorId, bookEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Creating a book for author {authorId} failed on saved");
            }

            var bookToReturn = Mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBookForAuthor", new {authorId = authorId, id = bookToReturn.Id}, bookToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRep = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRep == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteBook(bookForAuthorFromRep);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting book {id} from {authorId} failed on save.");

            }

            return NoContent();

        }

        //[HttpPut("{id}")]
        //public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        //{
        //    if (book == null)
        //    {
        //        return BadRequest();
        //    }

        //    if (!_libraryRepository.AuthorExists(authorId))
        //    {
        //        return NotFound();
        //    }

        //    var bookForAuthorFromRep = _libraryRepository.GetBookForAuthor(authorId, id);
        //    if (bookForAuthorFromRep == null)
        //    {
        //        return NotFound();
        //    }

        //    //map

        //    //apply update

        //    //map back to entity
        //    Mapper.Map(book, bookForAuthorFromRep);

        //    _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRep);

        //    if (!_libraryRepository.Save())
        //    {
        //        throw new Exception("This book failed to update");
        //    }

        //}

    }
}
