using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Negocio;

namespace Monolito_4am.Mantenimiento
{
    public partial class UsuarioDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usu"] == null && Session["adm"] == null)
            {
                Response.Redirect("~/Seguridad/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatos();
            }
        }

        private void CargarDatos()
        {
            if (Session["cedula"] != null)
            {
                string cedula = Session["cedula"].ToString();
                txt_cedula.Text = cedula;
                
                var user = CN_tbl_usario.traerced(cedula);
                if (user != null)
                {
                    // Mostrar el record real del usuario desde la BD
                    int record = CN_tbl_usario.ObtenerRecord(cedula);
                    lit_record.Text = record.ToString();
                }
            }
        }

        protected void btn_cambiar_pass_Click(object sender, EventArgs e)
        {
            string pass = txt_new_pass.Text.Trim();
            string confirm = txt_confirm_pass.Text.Trim();

            if (string.IsNullOrEmpty(pass) || pass.Length < 4)
            {
                MostrarAlerta("Validación", "La contraseña debe tener al menos 4 caracteres.", "warning");
                return;
            }

            if (pass != confirm)
            {
                MostrarAlerta("Error", "Las contraseñas no coinciden.", "error");
                return;
            }

            try
            {
                string cedula = Session["cedula"].ToString();
                CN_tbl_usario.CambiarPasswordAdmin(cedula, pass); // Reutilizamos el método de cambio de clave
                
                txt_new_pass.Text = "";
                txt_confirm_pass.Text = "";
                MostrarAlerta("¡Éxito!", "Tu contraseña ha sido actualizada correctamente.", "success");
            }
            catch (Exception ex)
            {
                MostrarAlerta("Error", "No se pudo cambiar la clave: " + ex.Message, "error");
            }
        }

        private void MostrarAlerta(string titulo, string mensaje, string tipo)
        {
            string script = $"Swal.fire('{titulo}', '{mensaje}', '{tipo}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }
    }
}