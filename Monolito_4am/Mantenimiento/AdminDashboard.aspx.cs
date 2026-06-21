using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4am.Mantenimiento
{
    public partial class AdminDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Manejar el PostBack manual para el cambio de contraseña (desde SweetAlert)
            if (IsPostBack && Request.Form["__EVENTTARGET"] == "cambiar_clave_confirm")
            {
                string data = Request.Form["__EVENTARGUMENT"];
                ConfirmarCambioClave(data);
            }

            if (!IsPostBack)
            {
                CargarUsuarios();
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                var usuarios = CN_tbl_usario.ListarUsuariosDetallado();
                rpt_usuarios.DataSource = usuarios;
                rpt_usuarios.DataBind();
            }
            catch (Exception ex)
            {
                MostrarAlerta("Error", "No se pudieron cargar los usuarios: " + ex.Message, "error");
            }
        }

        protected void btn_buscar_Click(object sender, EventArgs e)
        {
            string filtro = txt_buscar.Text.Trim().ToLower();
            try
            {
                // Obtenemos la lista completa
                var todosLosUsuarios = CN_tbl_usario.ListarUsuariosDetallado();
                
                if (string.IsNullOrEmpty(filtro))
                {
                    rpt_usuarios.DataSource = todosLosUsuarios;
                }
                else
                {
                    // Filtramos por Nombre, Cédula o Nick
                    var listaFiltrada = todosLosUsuarios
                        .Where(u => (u.NombreCompleto != null && u.NombreCompleto.ToLower().Contains(filtro)) || 
                                    (u.usu_cedula != null && u.usu_cedula.Contains(filtro)) || 
                                    (u.usu_nick != null && u.usu_nick.ToLower().Contains(filtro)))
                        .ToList();

                    rpt_usuarios.DataSource = listaFiltrada;
                }
                
                rpt_usuarios.DataBind();
            }
            catch (Exception ex)
            {
                MostrarAlerta("Error", "Error al buscar: " + ex.Message, "error");
            }
        }

        protected void btn_limpiar_Click(object sender, EventArgs e)
        {
            txt_buscar.Text = string.Empty;
            CargarUsuarios();
        }

        protected void btn_desbloquear_Command(object sender, CommandEventArgs e)
        {
            string cedula = e.CommandArgument.ToString();
            try
            {
                CN_tbl_usario.DesbloquearUsuario(cedula);
                MostrarAlerta("¡Desbloqueado!", "El usuario ha sido desbloqueado correctamente.", "success");
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MostrarAlerta("Error", "No se pudo desbloquear: " + ex.Message, "error");
            }
        }

        protected void btn_pass_Command(object sender, CommandEventArgs e)
        {
            string cedula = e.CommandArgument.ToString();
            // Disparamos el modal de SweetAlert desde el cliente
            ScriptManager.RegisterStartupScript(this, GetType(), "ModalPass", 
                string.Format("modalCambiarPass('{0}');", cedula), true);
        }

        private void ConfirmarCambioClave(string data)
        {
            // El formato es "cedula|nuevaClave"
            string[] partes = data.Split('|');
            if (partes.Length == 2)
            {
                string cedula = partes[0];
                string nuevaClave = partes[1];

                try
                {
                    CN_tbl_usario.CambiarPasswordAdmin(cedula, nuevaClave);
                    MostrarAlerta("Éxito", "Contraseña actualizada correctamente.", "success");
                    CargarUsuarios();
                }
                catch (Exception ex)
                {
                    MostrarAlerta("Error", "No se pudo cambiar la clave: " + ex.Message, "error");
                }
            }
        }

        private void MostrarAlerta(string titulo, string texto, string tipo)
        {
            string script = string.Format("Swal.fire('{0}', '{1}', '{2}');", titulo, texto, tipo);
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
        }
    }
}