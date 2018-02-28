using Library.Controllers.MVC;
using Library.ViewModels.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Library.ViewModels
{
    public class AuthorEditViewModel
    {
        private DateTimeOffset _dateOfBirth = new DateTime(1900,01,01);

        public Guid Id { get; set; }

        [Display(Name = "First Name")]
        [MinLength(1, ErrorMessage = "The \"First Name\" field must be at least 1 character long.")]
        [MaxLength(50, ErrorMessage = "The \"First Name\" field must be no more than 50 characters long.")]
        [Required(ErrorMessage = "The \"First Name\" field is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MinLength(1, ErrorMessage = "The \"Last Name\" field must be at least 1 character long.")]
        [MaxLength(50, ErrorMessage = "The \"Last Name\" field must be no more than 50 characters long.")]
        [Required(ErrorMessage = "The \"Last Name\" field is required.")]
        public string LastName { get; set; }

        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "The \"Date of Birth\" field is required.")]
        [ValidBirthDate]
        [DataType(DataType.Date)]
        public DateTimeOffset DateOfBirth
        {
            get
            {
                return (_dateOfBirth == DateTime.MinValue) ? DateTime.Now : _dateOfBirth;
            }
            set
            {
                _dateOfBirth = value;
            }
        }

        [MinLength(1, ErrorMessage = "The \"Genre\" field must be at least 1 character long.")]
        [MaxLength(50, ErrorMessage = "The \"Genre\" field must be no more than 50 characters long.")]
        [Required(ErrorMessage = "The \"Genre\" field is required.")]
        public string Genre { get; set; }

        public string Heading { get; set; }

        public string Action
        {
            get
            {
                Expression<Func<AuthorsController, IActionResult>>
                    update = (c => c.Update(this));

                Expression<Func<AuthorsController, IActionResult>>
                    create = (c => c.Create(this));

                var action = (Id != Guid.Empty) ? update : create;
                var actionName = (action.Body as MethodCallExpression).Method.Name;
                return actionName;
            }
        }
    }
}