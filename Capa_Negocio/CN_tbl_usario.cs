using System;
using System.Collections.Generic;
using System.Linq;
using Capa_Datos;
using System.Data.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Capa_Negocio
{
    // Clase de transporte para el Dashboard
    public class UserDTO
    {
        public int usu_id { get; set; }
        public string usu_cedula { get; set; }
        public string NombreCompleto { get; set; }
        public string usu_nick { get; set; }
        public bool usu_bloqueado { get; set; }
        public int? usu_intentos { get; set; }
        public byte[] FotoPerfil { get; set; }
    }

    public class CN_tbl_usario
    {
        // ==================== MÉTODOS BÁSICOS ====================
        public static List<tbl_usario> ListarUsuario()
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_usario.Where(u => u.usu_estado == 'A' || u.usu_estado == 'T').ToList();
            }
        }

        public static bool autentixced(string cedula)
        {
            using (var dc = new DataClasses1DataContext())
            {
                string c = cedula.Trim();
                return dc.tbl_usario.Any(u => u.usu_cedula.Trim() == c && (u.usu_estado == 'A' || u.usu_estado == 'T'));
            }
        }

        public static bool autentixcc(string cedula, string password)
        {
            using (var dc = new DataClasses1DataContext())
            {
                string c = cedula.Trim();
                return dc.tbl_usario.Any(u => u.usu_cedula.Trim() == c && dc.desencriptacon(u.usu_contraseña) == password && (u.usu_estado == 'A' || u.usu_estado == 'T'));
            }
        }

        public static tbl_usario traerced(string cedula)
        {
            if (string.IsNullOrEmpty(cedula)) return null;
            using (var dc = new DataClasses1DataContext())
            {
                string c = cedula.Trim();
                return dc.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == c && (u.usu_estado == 'A' || u.usu_estado == 'T'));
            }
        }

        public static void modificarusu(tbl_usario usuario)
        {
            using (var dc = new DataClasses1DataContext())
            {
                dc.tbl_usario.Attach(usuario);
                dc.Refresh(RefreshMode.KeepCurrentValues, usuario);
                dc.SubmitChanges();
            }
        }

        public static void registrarUsuario(tbl_usario usuario, List<byte[]> listaFotos)
        {
            using (var dc = new DataClasses1DataContext())
            {
                usuario.usu_fecha_creacion = DateTime.Now;
                usuario.usu_estado = 'A';
                usuario.usu_intentos = 0;
                usuario.usu_bloqueado = false;
                dc.tbl_usario.InsertOnSubmit(usuario);
                dc.SubmitChanges();

                if (listaFotos != null && listaFotos.Count > 0)
                {
                    bool esPrimero = true;
                    foreach (byte[] fotoBytes in listaFotos)
                    {
                        tbl_usuario_foto foto = new tbl_usuario_foto
                        {
                            usu_id = usuario.usu_id,
                            foto = fotoBytes,
                            nombre_archivo = esPrimero ? "perfil_principal" : "foto_adicional",
                            es_principal = esPrimero,
                            fecha_subida = DateTime.Now
                        };
                        dc.tbl_usuario_foto.InsertOnSubmit(foto);
                        esPrimero = false;
                    }
                    dc.SubmitChanges();
                }
            }
        }

        public static bool ExisteDato(string campo, string valor)
        {
            using (var dc = new DataClasses1DataContext())
            {
                valor = valor.Trim().ToLower();
                switch (campo.ToLower())
                {
                    case "cedula": return dc.tbl_usario.Any(u => u.usu_cedula.Trim() == valor);
                    case "nombres": return dc.tbl_usario.Any(u => u.usu_nombres.Trim().ToLower() == valor);
                    case "apellidos": return dc.tbl_usario.Any(u => u.usu_apellidos.Trim().ToLower() == valor);
                    case "celular": return dc.tbl_usario.Any(u => u.usu_celular.Trim() == valor);
                    case "nick": return dc.tbl_usario.Any(u => u.usu_nick.Trim().ToLower() == valor);
                    default: return false;
                }
            }
        }

        // ==================== OTP Y SEGURIDAD ====================
        public static void GuardarOTPEncriptado(string cedula, string otpPlano)
        {
            using (var dc_temp = new DataClasses1DataContext())
            {
                var usuario = dc_temp.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (usuario != null)
                {
                    usuario.usu_codigo_OTP = dc_temp.encriptacon(otpPlano);
                    usuario.usu_fecha_ultimo_intento = DateTime.Now;
                    dc_temp.SubmitChanges();
                }
            }
        }

        public static bool VerificarOTP(string cedula, string otpIngresado)
        {
            using (var dc_temp = new DataClasses1DataContext())
            {
                var usuario = dc_temp.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (usuario?.usu_codigo_OTP == null) return false;
                string otpDesencriptado = dc_temp.desencriptacon(usuario.usu_codigo_OTP);
                return otpDesencriptado?.Trim() == otpIngresado?.Trim();
            }
        }

        public static void LimpiarOTP(string cedula)
        {
            using (var dc_temp = new DataClasses1DataContext())
            {
                var usuario = dc_temp.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (usuario != null)
                {
                    usuario.usu_codigo_OTP = null;
                    dc_temp.SubmitChanges();
                }
            }
        }

        public static bool VerificarOTPQR(string cedula, byte[] otpEnviadoBytes)
        {
            using (var dc_temp = new DataClasses1DataContext())
            {
                var usuario = dc_temp.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (usuario?.usu_codigo_OTP == null) return false;
                string plainQR = dc_temp.desencriptacon(new Binary(otpEnviadoBytes));
                string plainBD = dc_temp.desencriptacon(usuario.usu_codigo_OTP);
                return !string.IsNullOrEmpty(plainQR) && plainQR.Trim() == plainBD?.Trim();
            }
        }

        public static byte[] GenerarQRConOTP(string cedula)
        {
            try
            {
                using (var dc_temp = new DataClasses1DataContext())
                {
                    var usuario = dc_temp.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                    if (usuario?.usu_codigo_OTP == null) return null;
                    byte[] otpBytes = usuario.usu_codigo_OTP.ToArray();
                    string otpBase64 = Convert.ToBase64String(otpBytes);
                    string contenidoQR = string.Format("OTP:{0}|CED:{1}", otpBase64, cedula.Trim());

                    using (var qrGenerator = new QRCoder.QRCodeGenerator())
                    {
                        var qrCodeData = qrGenerator.CreateQrCode(contenidoQR, QRCoder.QRCodeGenerator.ECCLevel.Q);
                        var qrCode = new QRCoder.QRCode(qrCodeData);
                        using (var qrBitmap = qrCode.GetGraphic(20))
                        using (var ms = new MemoryStream())
                        {
                            qrBitmap.Save(ms, ImageFormat.Png);
                            return ms.ToArray();
                        }
                    }
                }
            }
            catch { return null; }
        }

        public static void ResetearPassword(string cedula, string nuevaClavePlana)
        {
            using (var dc = new DataClasses1DataContext())
            {
                var user = dc.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (user != null)
                {
                    user.usu_contraseña = dc.encriptacon(nuevaClavePlana);
                    user.usu_intentos = 0;
                    user.usu_bloqueado = false;
                    dc.SubmitChanges();
                }
            }
        }

        public static int RegistrarIntentoFallido(string cedula)
        {
            using (var dc = new DataClasses1DataContext())
            {
                var user = dc.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (user != null)
                {
                    user.usu_intentos = (user.usu_intentos ?? 0) + 1;
                    user.usu_fecha_ultimo_intento = DateTime.Now;
                    if (user.usu_intentos >= 3) user.usu_bloqueado = true;
                    dc.SubmitChanges();
                    return user.usu_intentos.Value;
                }
                return 0;
            }
        }

        // ==================== MÉTODOS DE ADMINISTRACIÓN ====================
        public static byte[] ObtenerFotoPerfil(string cedula)
        {
            if (string.IsNullOrEmpty(cedula)) return null;
            using (var dc = new DataClasses1DataContext())
            {
                string c = cedula.Trim();
                var user = dc.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == c);
                if (user == null) return null;

                var foto = dc.tbl_usuario_foto
                             .Where(f => f.usu_id == user.usu_id)
                             .OrderByDescending(f => f.es_principal)
                             .ThenBy(f => f.fecha_subida)
                             .Select(f => f.foto)
                             .FirstOrDefault();

                return foto != null ? foto.ToArray() : null;
            }
        }

        public static List<UserDTO> ListarUsuariosDetallado()
        {
            using (var dc = new DataClasses1DataContext())
            {
                var consulta = from u in dc.tbl_usario
                              join f in dc.tbl_usuario_foto on u.usu_id equals f.usu_id into fotos
                              from fotoPrincipal in fotos.Where(x => x.es_principal == true).DefaultIfEmpty()
                              where u.usu_estado == 'A' || u.usu_estado == 'T'
                              select new UserDTO
                              {
                                  usu_id = u.usu_id,
                                  usu_cedula = u.usu_cedula,
                                  NombreCompleto = u.usu_nombres + " " + u.usu_apellidos,
                                  usu_nick = u.usu_nick,
                                  usu_bloqueado = u.usu_bloqueado,
                                  usu_intentos = u.usu_intentos,
                                  FotoPerfil = fotoPrincipal != null ? fotoPrincipal.foto.ToArray() : null
                              };
                return consulta.ToList();
            }
        }

        public static void DesbloquearUsuario(string cedula)
        {
            using (var dc = new DataClasses1DataContext())
            {
                var user = dc.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (user != null)
                {
                    user.usu_bloqueado = false;
                    user.usu_intentos = 0;
                    dc.SubmitChanges();
                }
            }
        }

        public static void CambiarPasswordAdmin(string cedula, string nuevaClave)
        {
            using (var dc = new DataClasses1DataContext())
            {
                var user = dc.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (user != null)
                {
                    user.usu_contraseña = dc.encriptacon(nuevaClave);
                    dc.SubmitChanges();
                }
            }
        }

        public static void GuardarRecord(string cedula, int record)
        {
            using (var dc = new DataClasses1DataContext())
            {
                var user = dc.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (user != null)
                {
                    try {
                        dc.ExecuteCommand("UPDATE tbl_usario SET usu_record = {0} WHERE usu_id = {1}", record, user.usu_id);
                    } catch { }
                }
            }
        }

        public static int ObtenerRecord(string cedula)
        {
            using (var dc = new DataClasses1DataContext())
            {
                var user = dc.tbl_usario.FirstOrDefault(u => u.usu_cedula.Trim() == cedula.Trim());
                if (user != null)
                {
                    try {
                        var res = dc.ExecuteQuery<int>("SELECT ISNULL(usu_record, 0) FROM tbl_usario WHERE usu_id = {0}", user.usu_id).FirstOrDefault();
                        return res;
                    } catch { return 0; }
                }
                return 0;
            }
        }
    }
}