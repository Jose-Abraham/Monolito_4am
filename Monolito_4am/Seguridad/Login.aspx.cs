using Capa_Datos;
using Capa_Negocio;
using SimpleCrypto;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Data.Linq;

namespace Monolito_4am.Seguridad
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                new DataClasses1DataContext().sp_ReiniciarIntentosDiarios();

                // Lógica de "Mantener Sesión" - Cargar cédula guardada
                if (Request.Cookies["Recordarme"] != null)
                {
                    txt_ced.Text = Request.Cookies["Recordarme"].Value;
                    chkRecordarme.Checked = true;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_ced.Text.Trim()) || string.IsNullOrEmpty(txt_pass.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                    "Swal.fire('Error', 'Ingrese cédula y contraseña', 'warning');", true);
                return;
            }

            tbl_usario usuario = CN_tbl_usario.traerced(txt_ced.Text.Trim());

            if (usuario == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                    "Swal.fire('Error', 'Usuario no encontrado', 'error');", true);
                return;
            }

            if (usuario.usu_bloqueado == true || (usuario.usu_intentos ?? 0) >= 3)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                    "Swal.fire('Bloqueado', 'Usuario bloqueado por múltiples intentos.<br>Contacte al administrador.', 'error');", true);
                return;
            }

            bool credencialesCorrectas = CN_tbl_usario.autentixcc(txt_ced.Text.Trim(), txt_pass.Text);

            if (credencialesCorrectas)
            {
                string otpPlano = RandomPassword.Generate(6, PasswordGroup.Numeric);

                // Guardar OTP encriptado en BD
                CN_tbl_usario.GuardarOTPEncriptado(txt_ced.Text.Trim(), otpPlano);

                // Generar QR con el mismo OTP encriptado que se guardó en la BD (Máxima seguridad)
                byte[] qrBytes = CN_tbl_usario.GenerarQRConOTP(txt_ced.Text.Trim());

                // Mostrar panel OTP
                MostrarPanelOTP(true);

                string cuerpoHtml = $@"
            <h3>Código de Verificación OTP</h3>
            <p>Escanea el siguiente código QR con tu cámara:</p>
            <p style='text-align:center;'>
                <img src='cid:qrCode' alt='Código QR OTP' style='width:280px; border:1px solid #ccc; padding:5px;'/>
            </p>
            <p><strong>O ingresa manualmente:</strong> {otpPlano}</p>
            <p>Este código expira en 5 minutos.</p>
            <small>Si no solicitaste este acceso, ignora este correo.</small>";

                // Enviar correo con QR embebido
                bool enviado = EnviarCorreoConQR(usuario.usu_correo, "Código OTP - Sistema Monolito", cuerpoHtml, qrBytes);

                // Lógica de "Mantener Sesión" - Guardar o Eliminar Cookie
                if (chkRecordarme.Checked)
                {
                    HttpCookie cookie = new HttpCookie("Recordarme", txt_ced.Text.Trim());
                    cookie.Expires = DateTime.Now.AddDays(30); // Persistir por 30 días
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    if (Request.Cookies["Recordarme"] != null)
                    {
                        HttpCookie cookie = new HttpCookie("Recordarme");
                        cookie.Expires = DateTime.Now.AddDays(-1); // Eliminar
                        Response.Cookies.Add(cookie);
                    }
                }

                if (enviado)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                        $"Swal.fire('Éxito', 'Código QR enviado a {usuario.usu_correo}', 'success');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                        "Swal.fire('Advertencia', 'No se pudo enviar el correo con QR.', 'warning');", true);
                }
            }
            else
            {
                int intentos = CN_tbl_usario.RegistrarIntentoFallido(txt_ced.Text.Trim());

                ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                    string.Format("Swal.fire('Error', 'Credenciales incorrectas.<br>Intento {0}/3', 'warning');", intentos), true);
            }
        }

        private void MostrarPanelOTP(bool mostrar)
        {
            lbl_usuario.Visible = !mostrar;
            txt_ced.Visible = !mostrar;
            lbl_password.Visible = !mostrar;
            txt_pass.Visible = !mostrar;
            btnLogin.Visible = !mostrar;

            // Panel de opciones (Mantener sesión y Olvidó su clave)
            pnl_opciones.Visible = !mostrar;
            pnl_volver.Visible = mostrar;

            lbl_otp.Visible = mostrar;
            txt_otp.Visible = mostrar;
            btn_otp.Visible = mostrar;

            btnEscanearQR.Visible = mostrar;

            // Ocultar el ojo de contraseña si estamos en modo OTP
            string script = mostrar ? "document.getElementById('btnTogglePass').style.display = 'none';" : "document.getElementById('btnTogglePass').style.display = 'flex';";
            ScriptManager.RegisterStartupScript(this, GetType(), "ToggleEye", script, true);
        }

        protected void btn_otp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_otp.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                    "Swal.fire('Error', 'Ingrese el código OTP', 'warning');", true);
                return;
            }

            bool otpCorrecto = CN_tbl_usario.VerificarOTP(txt_ced.Text.Trim(), txt_otp.Text.Trim());

            if (otpCorrecto)
            {
                CN_tbl_usario.LimpiarOTP(txt_ced.Text.Trim());

                tbl_usario usuario = CN_tbl_usario.traerced(txt_ced.Text.Trim());

                if (usuario.tusu_id == 1)
                {
                    Session["adm"] = usuario.usu_nombres + " " + usuario.usu_apellidos;
                    Session["cedula"] = usuario.usu_cedula.Trim();
                    Response.Redirect("~/Mantenimiento/AdminDashboard.aspx");
                }
                else
                {
                    Session["usu"] = usuario.usu_nombres + " " + usuario.usu_apellidos;
                    Session["cedula"] = usuario.usu_cedula.Trim();
                    Response.Redirect("~/Mantenimiento/UsuarioDashboard.aspx");
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                    "Swal.fire('Error', 'Código OTP incorrecto', 'error');", true);
            }
        }

        private bool EnviarCorreoConQR(string emailDestino, string subject, string bodyHtml, byte[] qrBytes)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    mail.From = new MailAddress("joseabraham30032004@gmail.com");
                    mail.To.Add(emailDestino);
                    mail.Subject = subject;
                    mail.Body = bodyHtml;
                    mail.IsBodyHtml = true;

                    // Adjuntar el QR como imagen embebida
                    if (qrBytes != null && qrBytes.Length > 0)
                    {
                        var qrAttachment = new Attachment(new MemoryStream(qrBytes), "qr_otp.png", "image/png");
                        qrAttachment.ContentId = "qrCode";
                        mail.Attachments.Add(qrAttachment);
                    }

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("joseabraham30032004@gmail.com", "wfiwmfytfiwsivto");
                    smtp.EnableSsl = true;

                    smtp.Send(mail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo con QR: " + ex.Message);
                return false;
            }
        }

        [WebMethod(EnableSession = true)]
        public static object ProcesarQR(string qrData)
        {
            try
            {
                if (string.IsNullOrEmpty(qrData))
                    return new { success = false, message = "E0: Datos de QR vacíos" };

                string[] partes = qrData.Split('|');
                if (partes.Length < 2)
                    return new { success = false, message = "E1: Formato de QR inválido (falta el separador |)" };

                // Parsing más robusto para extraer los valores, manejando posibles variaciones
                string rawOTP = partes[0].Trim();
                string rawCED = partes[1].Trim();

                string otpEncriptadoBase64 = rawOTP.StartsWith("OTP:", StringComparison.OrdinalIgnoreCase) 
                    ? rawOTP.Substring(4).Trim() 
                    : rawOTP;

                string cedula = rawCED.StartsWith("CED:", StringComparison.OrdinalIgnoreCase) 
                    ? rawCED.Substring(4).Trim() 
                    : rawCED;

                // 1. Convertir el Base64 que viene del QR a bytes
                byte[] otpBytesFromQR = Convert.FromBase64String(otpEncriptadoBase64);

                // 2. Verificar usando la comparación binaria con desencriptación (vence al Salt)
                bool valido = CN_tbl_usario.VerificarOTPQR(cedula, otpBytesFromQR);

                if (valido)
                {
                    CN_tbl_usario.LimpiarOTP(cedula);
                    
                    // IMPORTANTE: traerced ahora usa un DataContext nuevo internamente
                    var usuario = CN_tbl_usario.traerced(cedula);

                    if (usuario != null)
                    {
                        if (usuario.tusu_id == 1)
                            HttpContext.Current.Session["adm"] = usuario.usu_nombres + " " + usuario.usu_apellidos;
                        else
                            HttpContext.Current.Session["usu"] = usuario.usu_nombres + " " + usuario.usu_apellidos;

                        HttpContext.Current.Session["cedula"] = usuario.usu_cedula.Trim();

                        string virtualPath = usuario.tusu_id == 1 
                            ? "~/Mantenimiento/AdminDashboard.aspx" 
                            : "~/Mantenimiento/UsuarioDashboard.aspx";
                        
                        string redirect = VirtualPathUtility.ToAbsolute(virtualPath);
                        return new { success = true, message = "¡Acceso Correcto!", redirect = redirect };
                    }
                    else
                    {
                        return new { success = false, message = "Sesión iniciada pero no se pudo cargar el perfil." };
                    }
                }
                else
                {
                    // Diagnóstico avanzado
                    var userCheck = CN_tbl_usario.traerced(cedula);
                    if (userCheck == null)
                        return new { success = false, message = $"E2: Usuario no encontrado en BD ({cedula})" };
                    
                    if (userCheck.usu_codigo_OTP == null)
                        return new { success = false, message = "E3: El usuario no tiene un código OTP activo" };

                    return new { success = false, message = $"E4: Código incorrecto. (QR: '{otpEncriptadoBase64}')" };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error ProcesarQR: " + ex.Message);
                return new { success = false, message = "Error técnico: " + ex.Message };
            }
        }
    }
}