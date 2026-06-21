using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Datos;
using Capa_Negocio;

namespace Monolito_4am.Mantenimiento
{
    public partial class nuevo_tbl_proveedor1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LimpiarCampos();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarDatos())
                    return;

                tbl_proveedor proveedor = new tbl_proveedor
                {
                    prov_nombre = txt_nombre.Text.Trim(),
                    prov_ruc = txt_ruc.Text.Trim(),
                    prov_telefono = txt_telefono.Text.Trim(),
                    prov_correo = txt_correo.Text.Trim(),
                    prov_estado = 'A'
                };

                CN_tbl_proveedor.save(proveedor);

                ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                    "Swal.fire({title: '¡Éxito!', text: 'Proveedor registrado correctamente.', icon: 'success', timer: 1800}).then(() => { window.location.href = 'listar_tbl_proveedor.aspx'; });",
                    true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                    $"Swal.fire('Error', 'No se pudo guardar: {ex.Message}', 'error');", true);
            }
        }

        private bool ValidarDatos()
        {
            string nombre = txt_nombre.Text.Trim();
            string ruc = txt_ruc.Text.Trim();
            string telefono = txt_telefono.Text.Trim();
            string correo = txt_correo.Text.Trim();

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 3)
            {
                MostrarError("El nombre del proveedor debe tener al menos 3 caracteres.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != 13 || !ruc.All(char.IsDigit))
            {
                MostrarError("El RUC debe tener exactamente 13 dígitos numéricos.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(telefono) || !telefono.All(char.IsDigit) || telefono.Length < 7)
            {
                MostrarError("El teléfono debe contener solo números y mínimo 7 dígitos.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
            {
                MostrarError("Ingrese un correo electrónico válido.");
                return false;
            }

            // Validaciones de Unicidad
            if (CN_tbl_proveedor.ExisteNombre(nombre))
            {
                MostrarError("Ya existe un proveedor con este **nombre**.");
                return false;
            }

            if (CN_tbl_proveedor.ExisteRUC(ruc))
            {
                MostrarError("Ya existe un proveedor con este **RUC**.");
                return false;
            }

            if (CN_tbl_proveedor.ExisteTelefono(telefono))
            {
                MostrarError("Ya existe un proveedor con este **teléfono**.");
                return false;
            }

            if (CN_tbl_proveedor.ExisteCorreo(correo))
            {
                MostrarError("Ya existe un proveedor con este **correo electrónico**.");
                return false;
            }

            return true;
        }

        private void MostrarError(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                $"Swal.fire({{title: 'Validación', text: '{mensaje}', icon: 'warning', confirmButtonColor: '#f59e0b'}});", true);
        }

        private void LimpiarCampos()
        {
            txt_nombre.Text = "";
            txt_ruc.Text = "";
            txt_telefono.Text = "";
            txt_correo.Text = "";
        }
    }
}