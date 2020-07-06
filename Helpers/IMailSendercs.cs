using JWTAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthentication.Helpers
{
    public interface IMailSender
    {
        void Send(MailModel model);
    }
}
