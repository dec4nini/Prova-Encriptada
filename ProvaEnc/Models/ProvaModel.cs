using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvaEnc.Models
{
    public class ProvaModel
    {
        [Key]
        [Display(Name = "ID:")]
        public int Id { get; set; }
        [Display(Name = "Texto:")]
        [Required(ErrorMessage = "Digite o Texto!")]
        public string Texto { get; set; }
    }
}