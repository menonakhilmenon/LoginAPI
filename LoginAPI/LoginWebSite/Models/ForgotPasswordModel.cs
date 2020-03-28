using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginWebSite.Models
{
    public class ForgotPasswordModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-Mail ID")]
        [Required]
        public string Email { get; set; }
    }
}
