using System.ComponentModel.DataAnnotations;

namespace JWTAutentication.Models
{
    public class Car
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Color { get; set; }

        public Owner Owner { get; set; }
    }
}