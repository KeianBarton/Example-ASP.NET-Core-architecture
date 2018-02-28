using Library.Controllers.MVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Library.ViewModels
{
    public class BookEditViewModel
    {
        public Guid BookId { get; set; }

        [MinLength(1, ErrorMessage = "The \"Title\" field must be at least 1 character long.")]
        [MaxLength(100, ErrorMessage = "The \"Title\" field must be no more than 100 characters long.")]
        [Required(ErrorMessage = "The \"Title\" field is required.")]
        public string Title { get; set; }

        [MinLength(1, ErrorMessage = "The \"Last Name\" field must be at least 1 character long.")]
        [MaxLength(50, ErrorMessage = "The \"Last Name\" field must be no more than 50 characters long.")]
        [Required(ErrorMessage = "The \"Last Name\" field is required.")]
        public string Description { get; set; }

        [Display(Name = "Author")]
        [Required(ErrorMessage = "The \"Author\" field is required.")]
        public Guid AuthorId { get; set; }

        public IEnumerable<SelectListItem> Authors { get; set; }

        public string Heading { get; set; }

        public string Action
        {
            get
            {
                Expression<Func<BooksController, IActionResult>>
                    update = (c => c.Update(this));

                Expression<Func<BooksController, IActionResult>>
                    create = (c => c.Create(this));

                var action = (BookId != Guid.Empty) ? update : create;
                var actionName = (action.Body as MethodCallExpression).Method.Name;
                return actionName;
            }
        }
    }
}