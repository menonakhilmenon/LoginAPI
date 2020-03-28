using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginWebSite.Models
{
    public class ConfirmViewModel
    {
        [Required]
        [Display(Name ="Activation Key")]
        public string activationKey { get; set; }
        public bool activationFail { get; set; } = false;
    }
}
