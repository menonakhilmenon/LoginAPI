using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginWebSite.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string otp { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(newPassword),ErrorMessage ="Passwords do not match.")]
        public string confirmNewPassword { get; set; }


    }
}
