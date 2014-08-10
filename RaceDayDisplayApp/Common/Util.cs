using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaceDayDisplayApp.Common
{
    public class Util
    {
        public static void SendEmailConfirmation(string to, string username, string confirmationToken)
        {
            dynamic email = new Email("ConfirmationEmail");
            email.To = to;
            email.UserName = username;
            email.ConfirmationToken = confirmationToken;
            email.Send();
        }
    }
}