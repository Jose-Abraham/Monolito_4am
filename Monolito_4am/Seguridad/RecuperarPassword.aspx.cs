using Capa_Datos;
using Capa_Negocio;
using System;
using System.Linq;
using System.Web.UI;
using System.Configuration;
using System.Net.Http;
using System.Collections.Generic;

namespace Monolito_4am.Seguridad
{
    public partial class RecuperarPassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btn_recuperar_Click(object sender, EventArgs e)
        {
            string cedula = txt_cedula_rec.Text.Trim();

            if (string.IsNullOrEmpty(cedula))
            {
                MostrarAlerta("Error", "Por favor ingrese su número de cédula.", "warning");
                return;
            }

            // 1. Verificar si el usuario existe
            tbl_usario usuario = CN_tbl_usario.traerced(cedula);

            if (usuario == null)
            {
                MostrarAlerta("Error", "No se encontró ningún usuario con esa cédula.", "error");
                return;
            }

            if (string.IsNullOrEmpty(usuario.usu_celular))
            {
                MostrarAlerta("Error", "El usuario no tiene un número de celular registrado para la recuperación.", "error");
                return;
            }

            try
            {
                // 2. Generar clave temporal (8 caracteres)
                string claveTemporal = GenerarClaveInterna(8);

                // 3. Encriptar y actualizar en la base de datos
                CN_tbl_usario.ResetearPassword(cedula, claveTemporal);

                // 4. Preparar el envío a WhatsApp
                string celular = usuario.usu_celular.Trim();
                
                // Formatear número para Ecuador
                if (celular.StartsWith("0")) celular = "593" + celular.Substring(1);
                else if (!celular.StartsWith("593")) celular = "593" + celular;

                string mensaje = string.Format("Hola {0}, tu nueva clave temporal para el Sistema Monolito es: *{1}* . Por favor, cámbiala al iniciar sesión.", usuario.usu_nombres, claveTemporal);

                // 5. Enviar mensaje directo vía UltraMsg
                bool enviado = EnviarMensajeUltraMsg(celular, mensaje);

                if (enviado)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "SuccessRedirect",
                        "Swal.fire('¡Éxito!', 'Se ha enviado una nueva clave a tu WhatsApp.', 'success').then(() => { window.location.href = 'Login.aspx'; });", true);
                }
                else
                {
                    MostrarAlerta("Error", "No se pudo enviar el mensaje automático. Contacte al administrador.", "error");
                }
            }
            catch (Exception ex)
            {
                MostrarAlerta("Error", "Ocurrió un problema: " + ex.Message, "error");
            }
        }

        private bool EnviarMensajeUltraMsg(string celular, string mensaje)
        {
            try
            {
                string instance = ConfigurationManager.AppSettings["UltraMsgInstance"];
                string token = ConfigurationManager.AppSettings["UltraMsgToken"];
                string url = string.Format("https://api.ultramsg.com/{0}/messages/chat", instance);

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "token", token },
                        { "to", celular },
                        { "body", mensaje },
                        { "priority", "10" }
                    };

                    var content = new FormUrlEncodedContent(values);
                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();

                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        private string GenerarClaveInterna(int longitud)
        {
            const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
            Random rnd = new Random();
            char[] resultado = new char[longitud];
            for (int i = 0; i < longitud; i++)
            {
                resultado[i] = caracteres[rnd.Next(caracteres.Length)];
            }
            return new string(resultado);
        }

        private void MostrarAlerta(string titulo, string texto, string tipo)
        {
            string script = string.Format("Swal.fire('{0}', '{1}', '{2}');", titulo, texto, tipo);
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
        }
    }
}
