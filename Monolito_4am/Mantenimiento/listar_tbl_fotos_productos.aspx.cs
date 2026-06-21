using Capa_Datos;
using Capa_Negocio;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4am.Mantenimiento
{
    public partial class listar_tbl_fotos_productos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack && Request.Form["__EVENTTARGET"] != null)
            {
                string target = Request.Form["__EVENTTARGET"];
                string argument = Request.Form["__EVENTARGUMENT"];

                if (target == "btnEliminarFisico_Click" && int.TryParse(argument, out int id))
                {
                    ProcesarEliminacionFisica(id);
                    return;
                }
            }

            if (!IsPostBack)
            {
                CargarImagenes();
            }
        }

        private void CargarImagenes(string filtro = "")
        {
            try
            {
                var lista =
                    CN_tbl_fotos_productos.ListarTodas();

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    filtro = filtro.ToLower();

                    lista = lista.Where(f =>

                        f.fpro_id.ToString()
                        .Contains(filtro)

                        ||

                        (
                            f.tbl_productos != null
                            ?
                            f.tbl_productos.pro_nombre
                            :
                            ""
                        )
                        .ToLower()
                        .Contains(filtro)

                        ||

                        (
                            f.fpro_nombre_archivo ?? ""
                        )
                        .ToLower()
                        .Contains(filtro)

                    ).ToList();
                }

                gvImagenes.DataSource = lista;
                gvImagenes.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje(
                    "Error",
                    ex.Message,
                    "error");
            }
        }

        protected void gvImagenes_PageIndexChanging(
        object sender,
        GridViewPageEventArgs e)
        {
            gvImagenes.PageIndex = e.NewPageIndex;

            CargarImagenes(
                txtBuscar.Text.Trim());
        }

        private void ProcesarEliminacionFisica(int id)
        {
            try
            {
                using (var dc = new DataClasses1DataContext())
                {
                    var foto = dc.tbl_fotos_productos.FirstOrDefault(f => f.fpro_id == id);
                    if (foto != null)
                    {
                        string rutaFisica = Server.MapPath(foto.fpro_ruta_imagen);
                        if (System.IO.File.Exists(rutaFisica))
                            System.IO.File.Delete(rutaFisica);

                        dc.tbl_fotos_productos.DeleteOnSubmit(foto);
                        dc.SubmitChanges();
                    }
                }
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", ex.Message, "error");
            }
        }

        protected void btnBuscar_Click(
        object sender,
        EventArgs e)
        {
            if (txtBuscar.Text.Length > 100)
            {
                MostrarMensaje(
                    "Error",
                    "Máximo 100 caracteres.",
                    "error");

                return;
            }

            gvImagenes.PageIndex = 0;

            CargarImagenes(
                txtBuscar.Text.Trim());
        }
        protected void btnLimpiar_Click(
         object sender,
         EventArgs e)
        {
            txtBuscar.Text = "";

            gvImagenes.PageIndex = 0;

            CargarImagenes();
        }

        protected void btnNuevaImagen_Click(object sender, EventArgs e)
        {
            Response.Redirect("nuevo_foto_producto.aspx");
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(((Button)sender).CommandArgument);
            Response.Redirect("editar_foto_producto.aspx?id=" + id);
        }

        private void MostrarMensaje(string titulo, string texto, string tipo)
        {
            string script = $"Swal.fire('{titulo}','{texto}','{tipo}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
        }

        protected void btnImportarExcel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Excel_tbl_foto_producto.aspx");
        }
    }
}