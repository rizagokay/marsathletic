using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MarsAhletic.WebUI.Models
{
    public class TravelPlanViewModel
    {
        public string TravelId { get; set; }
        public string NameSurname { get; set; }
        public string ExpanseCenterId { get; set; }
        public DateTime Date { get; set; }
        public string TravelRoute { get; set; }
    }

    public class AddUserViewModel
    {
        [Required]
        [MinLength(0, ErrorMessage = "Kullanıcı Adı minimum 6 karakter olmalıdır.")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Adı Soyadı")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "E-Mail Aktivasyonu Gönder")]
        public bool SendConfirmationMail { get; set; }

        public string ADUserId { get; set; }
        public string ADDomain { get; set; }

    }
}