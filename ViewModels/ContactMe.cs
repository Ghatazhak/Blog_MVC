﻿using System.ComponentModel.DataAnnotations;

namespace Blog_MVC.ViewModels
{
    public class ContactMe
    {

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} and less than {1}"), MinLength(2)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and less than {1}"), MinLength(2)]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and less than {1}"), MinLength(2)]
        public string Subject { get; set; }


        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and less than {1}"), MinLength(5)]
        public string Message { get; set; }
    }
}
