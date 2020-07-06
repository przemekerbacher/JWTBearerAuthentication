using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthentication.Models
{
    public class SMTP
    {
        public string Host { get; set; }
        public string PortUnencrypted { get; set; }
        public int PortSSL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
