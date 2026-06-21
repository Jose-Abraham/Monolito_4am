using System;
using System.Net;
using System.Net.Mail;

namespace Capa_Negocio
{
    public class Mail
    {
        private readonly string from = "joseabraham30032004@gmail.com";
        private readonly string pass = "wfiwmfytfiwsivto";

        // Método actualizado con Subject
        public bool envia_correo(string to, string subject, string body)
        {
            try
            {
                using (MailMessage m = new MailMessage())
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    m.From = new MailAddress(from);
                    m.To.Add(new MailAddress(to));
                    m.Subject = subject;
                    m.Body = body;
                    m.IsBodyHtml = true;

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(from, pass);
                    smtp.EnableSsl = true;

                    smtp.Send(m);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar correo: " + ex.Message);
                return false;
            }
        }

        // Método antiguo (por compatibilidad)
        public bool envia_correo(string to, string msj)
        {
            return envia_correo(to, "Mensaje del Sistema", msj);
        }
    }
}