using Library.Domain.Dtos;
using Library.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Library.Controllers.MVC
{
    public class BooksController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new BookEditViewModel()
            {
                Heading = "Add a Book"
            };

            return View("BookForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("BookForm", viewModel);
            }

            var bookDto = new BookDto()
            {
                Title = viewModel.Title,
                Description = viewModel.Description
            };

            //AddBookForAuthor(bookDto);

            return RedirectToAction("Index", "Books");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(BookEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("BookForm", viewModel);
            }

            //UpdateBookForAuthor(viewModel.Id, authorDto);

            return RedirectToAction("Index", "Books");
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            //DeleteBook(id);

            return RedirectToAction("Index", "Authors");
        }
    }
}