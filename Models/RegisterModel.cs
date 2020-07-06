﻿using System.ComponentModel.DataAnnotations;

namespace JWTAutentication.Models
{
    public class RegisterModel
    {
        [MinLength(3)]
        [Required]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}