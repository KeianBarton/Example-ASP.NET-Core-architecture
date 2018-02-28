using Library.Domain.Dtos;
using Library.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Library.Controllers.MVC
{
    public class AuthorsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new AuthorEditViewModel()
            {
                Heading = "Add an Author"
            };

            return View("AuthorForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuthorEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("AuthorForm", viewModel);
            }

            var authorDto = new AuthorDto()
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DateOfBirth = viewModel.DateOfBirth,
                Genre = viewModel.Genre
            };

            //AddAuthor(authorDto);

            return RedirectToAction("Index", "Authors");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(AuthorEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("AuthorForm", viewModel);
            }

            //UpdateAuthor(viewModel.Id, authorDto);

            return RedirectToAction("Index", "Authors");
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            //DeleteAuthor(id);

            return RedirectToAction("Index", "Authors");
        }
    }
}