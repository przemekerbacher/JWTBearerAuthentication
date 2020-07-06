using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JWTAutentication.Models
{
    public class Owner
    {
        [Key]
        public string Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Surname { get; set; }

        public IList<Car> Cars { get; set; }
    }
}