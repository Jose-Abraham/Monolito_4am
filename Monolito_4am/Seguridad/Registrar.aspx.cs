using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Datos;
using Capa_Negocio;
using SimpleCrypto;

namespace Monolito_4am.Seguridad
{
    public partial class Registrar : System.Web.UI.Page
    {
        private tbl_usario objusuario = new tbl_usario();
        
        // Declaraciones manuales para corregir errores de Designer
        protected global::System.Web.UI.WebControls.Repeater rptFotos;
        protected global::System.Web.UI.WebControls.Label lblSinFotos;
        protected global::System.Web.UI.WebControls.Button btn_subir_foto;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["FotosTemporales"] = null; // Limpiar al entrar
                cargar_perfil();
            }
        }

        private List<byte[]> FotosTemporales
        {
            get
            {
                if (Session["FotosTemporales"] == null)
                    Session["FotosTemporales"] = new List<byte[]>();
                return (List<byte[]>)Session["FotosTemporales"];
            }
            set { Session["FotosTemporales"] = value; }
        }

        protected void btn_subir_foto_Click(object sender, EventArgs e)
        {
            if (fuFoto.HasFiles)
            {
                long totalSize = FotosTemporales.Sum(f => f.Length);
                foreach (HttpPostedFile postFile in fuFoto.PostedFiles)
                {
                    string extension = Path.GetExtension(postFile.FileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        if (postFile.ContentLength > 5 * 1024 * 1024)
                        {
                            MostrarMensaje("Error", $"La imagen {postFile.FileName} supera los 5MB.", "error");
                            return;
                        }
                        totalSize += postFile.ContentLength;
                        if (totalSize > 10 * 1024 * 1024)
                        {
                            MostrarMensaje("Error", "El total de imágenes supera los 10MB.", "error");
                            return;
                        }

                        using (BinaryReader br = new BinaryReader(postFile.InputStream))
                        {
                            FotosTemporales.Add(br.ReadBytes(postFile.ContentLength));
                        }
                    }
                    else
                    {
                        MostrarMensaje("Error", "Formato de imagen no permitido.", "error");
                        return;
                    }
                }
                ActualizarGaleria();
            }
        }

        private void ActualizarGaleria()
        {
            rptFotos.DataSource = FotosTemporales;
            rptFotos.DataBind();
            lblSinFotos.Visible = (FotosTemporales.Count == 0);
        }

        private void cargar_perfil()
        {
            List<tbl_tipo_usuario> objtu = new List<tbl_tipo_usuario>();
            objtu = CN_tbl_tipo_usuario.ListarTipoUsuario();
            objtu.Insert(0, new tbl_tipo_usuario { tusu_id = 0, tusu_nombre = "--Seleccione-- " });

            ddl_perfil.DataSource = objtu;
            ddl_perfil.DataTextField = "tusu_nombre";
            ddl_perfil.DataValueField = "tusu_id";
            ddl_perfil.DataBind();

        }

        protected void lnk_login_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }

        private void MostrarMensaje(string titulo, string texto, string tipo)
        {
            string script = $"Swal.fire('{titulo}', '{texto}', '{tipo}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
        }

        private bool ValidarDatos()
        {
            // Validar Cédula
            string cedula = txt_cedula.Text.Trim();
            if (string.IsNullOrEmpty(cedula) || cedula.Length != 10 || !cedula.All(char.IsDigit))
            {
                MostrarMensaje("Error", "Cédula inválida. Debe tener 10 dígitos.", "error");
                return false;
            }
            // Unicidad de Cédula
            if (CN_tbl_usario.ExisteDato("cedula", cedula))
            {
                MostrarMensaje("Error", "Esta cédula ya se encuentra registrada.", "error");
                return false;
            }

            // No permitir más de 7 veces el mismo número
            var grupos = cedula.GroupBy(c => c).Select(g => g.Count());
            if (grupos.Any(count => count > 7))
            {
                MostrarMensaje("Error", "La cédula no puede tener un dígito repetido más de 7 veces.", "error");
                return false;
            }

            // Validar Nombres
            string nombres = txt_nombres.Text.Trim();
            if (string.IsNullOrEmpty(nombres) || nombres.Length > 100 || nombres.Any(char.IsDigit))
            {
                MostrarMensaje("Error", "Nombres inválidos. No debe contener números y máximo 100 caracteres.", "error");
                return false;
            }

            // Validar Apellidos
            string apellidos = txt_apellidos.Text.Trim();
            if (string.IsNullOrEmpty(apellidos) || apellidos.Length > 100 || apellidos.Any(char.IsDigit))
            {
                MostrarMensaje("Error", "Apellidos inválidos. No debe contener números y máximo 100 caracteres.", "error");
                return false;
            }

            // Validar Correo
            string correo = txt_correo.Text.Trim();
            if (string.IsNullOrEmpty(correo) || correo.Length > 150 || !correo.Contains("@"))
            {
                MostrarMensaje("Error", "Correo inválido o demasiado largo.", "error");
                return false;
            }

            // Validar Dirección
            if (string.IsNullOrEmpty(txt_direccion.Text.Trim()) || txt_direccion.Text.Length > 100)
            {
                MostrarMensaje("Error", "Dirección obligatoria y máximo 100 caracteres.", "error");
                return false;
            }

            // Validar Celular
            string celular = txt_celular.Text.Trim();
            if (string.IsNullOrEmpty(celular) || celular.Length != 10 || !celular.All(char.IsDigit))
            {
                MostrarMensaje("Error", "Celular inválido. Debe tener 10 dígitos numéricos.", "error");
                return false;
            }
            if (CN_tbl_usario.ExisteDato("celular", celular))
            {
                MostrarMensaje("Error", "Este número de celular ya se encuentra registrado.", "error");
                return false;
            }

            // Validar Fecha de Nacimiento
            if (string.IsNullOrEmpty(txt_fecha_cumpleanos.Text))
            {
                MostrarMensaje("Error", "Fecha de nacimiento obligatoria.", "error");
                return false;
            }
            DateTime fechaNac;
            if (!DateTime.TryParse(txt_fecha_cumpleanos.Text, out fechaNac))
            {
                MostrarMensaje("Error", "Fecha de nacimiento inválida.", "error");
                return false;
            }
            int edad = DateTime.Today.Year - fechaNac.Year;
            if (fechaNac > DateTime.Today.AddYears(-edad)) edad--;
            if (edad < 18 || edad > 100 || fechaNac.Year < 1926 || fechaNac > DateTime.Today)
            {
                MostrarMensaje("Error", "Edad no permitida (debe ser entre 18 y 100 años y no fecha futura).", "error");
                return false;
            }

            // Validar Nick y Perfil
            string nick = txt_nick.Text.Trim();
            if (string.IsNullOrEmpty(nick) || nick.Length > 50)
            {
                MostrarMensaje("Error", "Nick inválido.", "error");
                return false;
            }
            if (CN_tbl_usario.ExisteDato("nick", nick))
            {
                MostrarMensaje("Error", "Este nick ya se encuentra registrado.", "error");
                return false;
            }

            if (ddl_perfil.SelectedValue == "0")
            {
                MostrarMensaje("Error", "Debe seleccionar un perfil.", "error");
                return false;
            }

            return true;
        }

        protected void btn_registrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarDatos()) return;

                objusuario = new tbl_usario();
                DataClasses1DataContext dc = new DataClasses1DataContext();
                
                // Asegurar 2 nombres y 2 apellidos
                string[] nomParts = txt_nombres.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string nombresFinal = nomParts.Length == 1 ? $"{nomParts[0]} {nomParts[0]}" : string.Join(" ", nomParts);
                
                string[] apeParts = txt_apellidos.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string apellidosFinal = apeParts.Length == 1 ? $"{apeParts[0]} {apeParts[0]}" : string.Join(" ", apeParts);

                objusuario.usu_cedula = txt_cedula.Text;
                objusuario.usu_nombres = nombresFinal;
                objusuario.usu_apellidos = apellidosFinal;
                objusuario.usu_correo = txt_correo.Text;
                objusuario.usu_direccion = txt_direccion.Text;
                objusuario.usu_celular = txt_celular.Text;
                objusuario.usu_contraseña = dc.encriptacon(txt_contrasena.Text);
                objusuario.usu_fecha_cumple = Convert.ToDateTime(txt_fecha_cumpleanos.Text);
                objusuario.usu_nick = txt_nick.Text;
                objusuario.tusu_id = Convert.ToInt32(ddl_perfil.SelectedValue);
                
                CN_tbl_usario.registrarUsuario(objusuario, FotosTemporales);

                MostrarMensaje("Éxito", "Usuario registrado correctamente", "success");

                //Limpiar campos
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", ex.Message, "error");
            }
        }

        private void LimpiarCampos()
        {
            txt_cedula.Text = "";
            txt_nombres.Text = "";
            txt_apellidos.Text = "";
            txt_correo.Text = "";
            txt_direccion.Text = "";
            txt_celular.Text = "";
            txt_contrasena.Text = "";
            txt_fecha_cumpleanos.Text = "";
            txt_nick.Text = "";
            ddl_perfil.SelectedIndex = 0;
            lblErrorFoto.Text = "";
            FotosTemporales = null;
            ActualizarGaleria();
        }

        protected void txt_apellidos_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_nombres.Text) || string.IsNullOrEmpty(txt_apellidos.Text) || txt_cedula.Text.Length < 1) return;

            string[] nom = txt_nombres.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] ape = txt_apellidos.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Recomendar duplicar si falta uno (esto ya se avisa en JS pero por si acaso lo manejamos para el correo/nick)
            string n1 = nom[0];
            string n2 = nom.Length > 1 ? nom[1] : nom[0];
            string a1 = ape[0];
            string a2 = ape.Length > 1 ? ape[1] : ape[0];

            txt_correo.Text = n1.ToLower() + "." + a1.ToLower() + "@cordillera.edu.ec";
            txt_contrasena.Text = RandomPassword.Generate(8, PasswordGroup.Numeric, PasswordGroup.Lowercase, PasswordGroup.Uppercase, PasswordGroup.Special);
            
            Random rnd = new Random();
            char[] ced = txt_cedula.Text.ToCharArray();
            txt_nick.Text = n1.Substring(0, 1).ToUpper() + n2.Substring(0, 1).ToLower() + a1.Substring(0, 1).ToUpper() + a2.Substring(0, 1).ToLower() + rnd.Next(100, 1000).ToString() + RandomPassword.Generate(1, PasswordGroup.Special) + (ced[rnd.Next(ced.Length)]) + (ced[rnd.Next(ced.Length)]);
        }
    }
}