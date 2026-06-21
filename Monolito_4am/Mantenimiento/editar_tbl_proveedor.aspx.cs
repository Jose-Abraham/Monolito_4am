using Capa_Datos;
using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4am.Mantenimiento
{
    public partial class editar_tbl_proveedor : System.Web.UI.Page
    {
        int idProveedor = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out idProveedor))
            {
                Response.Redirect("listar_tbl_proveedor.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarProveedor();
            }
        }

        private void CargarProveedor()
        {
            try
            {
                var proveedor =
                    CN_tbl_proveedor.traerproveedorxid(idProveedor);

                if (proveedor == null)
                {
                    MostrarMensaje(
                        "Error",
                        "Proveedor no encontrado.",
                        "error"
                    );

                    return;
                }

                txt_nombre.Text = proveedor.prov_nombre;
                txt_ruc.Text = proveedor.prov_ruc;
                txt_telefono.Text = proveedor.prov_telefono;
                txt_correo.Text = proveedor.prov_correo;
            }
            catch (Exception ex)
            {
                MostrarMensaje(
                    "Error",
                    ex.Message,
                    "error"
                );
            }
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarDatos())
                    return;

                tbl_proveedor proveedor =
                    new tbl_proveedor();

                proveedor.prov_id = idProveedor;
                proveedor.prov_nombre = txt_nombre.Text.Trim();
                proveedor.prov_ruc = txt_ruc.Text.Trim();
                proveedor.prov_telefono = txt_telefono.Text.Trim();
                proveedor.prov_correo = txt_correo.Text.Trim();

                CN_tbl_proveedor.modify(proveedor);

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "SweetAlert",
                    "Swal.fire({title:'Éxito',text:'Proveedor actualizado correctamente.',icon:'success'}).then(()=>{window.location='listar_tbl_proveedor.aspx';});",
                    true
                );
            }
            catch (Exception ex)
            {
                MostrarMensaje(
                    "Error",
                    ex.Message,
                    "error"
                );
            }
        }

        private bool ValidarDatos()
        {
            string nombre = txt_nombre.Text.Trim();
            string ruc = txt_ruc.Text.Trim();
            string telefono = txt_telefono.Text.Trim();
            string correo = txt_correo.Text.Trim();

            if (nombre.Length < 3)
            {
                MostrarMensaje(
                    "Validación",
                    "El nombre debe tener mínimo 3 caracteres.",
                    "warning"
                );

                return false;
            }

            if (ruc.Length != 13 || !ruc.All(char.IsDigit))
            {
                MostrarMensaje(
                    "Validación",
                    "El RUC debe tener 13 números.",
                    "warning"
                );

                return false;
            }

            if (telefono.Length < 7 || !telefono.All(char.IsDigit))
            {
                MostrarMensaje(
                    "Validación",
                    "Teléfono inválido.",
                    "warning"
                );

                return false;
            }

            if (!correo.Contains("@"))
            {
                MostrarMensaje(
                    "Validación",
                    "Correo inválido.",
                    "warning"
                );

                return false;
            }

            // VALIDACIONES DE DUPLICADOS

            if (CN_tbl_proveedor.ExisteNombreEditar(
                nombre,
                idProveedor))
            {
                MostrarMensaje(
                    "Validación",
                    "Ya existe un proveedor con ese nombre.",
                    "warning"
                );

                return false;
            }

            if (CN_tbl_proveedor.ExisteRUCEditar(
                ruc,
                idProveedor))
            {
                MostrarMensaje(
                    "Validación",
                    "Ya existe un proveedor con ese RUC.",
                    "warning"
                );

                return false;
            }

            if (CN_tbl_proveedor.ExisteTelefonoEditar(
                telefono,
                idProveedor))
            {
                MostrarMensaje(
                    "Validación",
                    "Ya existe un proveedor con ese teléfono.",
                    "warning"
                );

                return false;
            }

            if (CN_tbl_proveedor.ExisteCorreoEditar(
                correo,
                idProveedor))
            {
                MostrarMensaje(
                    "Validación",
                    "Ya existe un proveedor con ese correo.",
                    "warning"
                );

                return false;
            }

            return true;
        }

        private void MostrarMensaje(
            string titulo,
            string texto,
            string tipo)
        {
            string script =
                $"Swal.fire('{titulo}','{texto}','{tipo}');";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "SweetAlert",
                script,
                true
            );
        }
    }
}