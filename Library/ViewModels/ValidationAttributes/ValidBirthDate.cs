using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Library.ViewModels.ValidationAttributes
{
    public class ValidBirthDate : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var isValidFormat = DateTimeOffset.TryParseExact(
                Convert.ToString(value),
                "yyyy-MM-dd",
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out var dateTimeOffset);
            if (!isValidFormat)
                return false;

            var isValidDate = dateTimeOffset < DateTime.Now;
            return isValidDate;
        }
    }
}
