using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4am.Mantenimiento
{
    public partial class Principal : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Evitar caché para que no puedan regresar con el botón atrás después de cerrar sesión
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
           if (!IsPostBack) 
           { 
                pnlAdmin.Visible = false;
                pnlUsuario.Visible = false;
           }

            if (Session["adm"] != null || Session["usu"] != null)
            {
                bool esAdmin = Session["adm"] != null;
                lbl_nse.Text = esAdmin ? Session["adm"].ToString() : Session["usu"].ToString();
                lbl_rol.Text = esAdmin ? "Administrador" : "Usuario Estándar";
                
                pnlAdmin.Visible = esAdmin;
                pnlUsuario.Visible = !esAdmin;

                // Cargar Foto de Perfil con la lógica de la lista (Probada en las tarjetas)
                string fotoBase64 = "https://cdn-icons-png.flaticon.com/512/3135/3135715.png";

                if (Session["cedula"] != null)
                {
                    string cedActual = Session["cedula"].ToString().Trim();
                    // Usamos la misma lógica que las tarjetas del admin
                    var usuarioActual = CN_tbl_usario.ListarUsuariosDetallado()
                                        .FirstOrDefault(u => u.usu_cedula.Trim() == cedActual);
                    
                    if (usuarioActual != null && usuarioActual.FotoPerfil != null && usuarioActual.FotoPerfil.Length > 0)
                    {
                        fotoBase64 = "data:image/png;base64," + Convert.ToBase64String(usuarioActual.FotoPerfil);
                    }
                }
                
                img_perfil_master.ImageUrl = fotoBase64;
            }
            else
            {
                Response.Redirect("~/Seguridad/Login.aspx");
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/Seguridad/Login.aspx");
        }
    }
}