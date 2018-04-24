using System;
using System.Net.Mail;
using System.Globalization;
using System.Text.RegularExpressions;


namespace MyCommon
{
    public class MyEmail
    {
        public static void SendMessage(MailMessage mailMessage)
        {
            mailMessage.Priority = System.Net.Mail.MailPriority.Normal;
            //Text/HTML
            mailMessage.IsBodyHtml = false;

            //Create an instance of the SmtpClient class for sending the email
            var smtpClient = new System.Net.Mail.SmtpClient();

            //Use a Try/Catch block to trap sending errors
            //Especially useful when looping through multiple sends
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (System.Net.Mail.SmtpException smtpExc)
            {
                //Log error information on which email failed.
                string errorMsg = smtpExc.Message;
            }
            catch (Exception ex)
            {
                //Log general errors
                string errorMsg = ex.Message;
            }

        }

    }



    public class RegexUtilities
    {
        private bool invalid = false;

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper);
            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                                 @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                 @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                                 RegexOptions.IgnoreCase);
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}
 

