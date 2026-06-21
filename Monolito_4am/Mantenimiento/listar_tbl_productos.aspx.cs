
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Datos;
using Capa_Negocio;
using System.IO;

namespace Monolito_4am.Mantenimiento
{
    public partial class listar_tbl_productos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Procesar eliminaciones vía SweetAlert + __doPostBack
            if (IsPostBack && Request.Form["__EVENTTARGET"] != null)
            {
                string target = Request.Form["__EVENTTARGET"];
                string argument = Request.Form["__EVENTARGUMENT"];

                if (target == "btnEliminar_Click" && int.TryParse(argument, out int idLog))
                {
                    ProcesarEliminacionLogica(idLog);
                    return;
                }
                if (target == "btnEliminarFisico_Click" && int.TryParse(argument, out int idFis))
                {
                    ProcesarEliminacionFisica(idFis);
                    return;
                }
            }

            if (!IsPostBack)
            {
                carga_producto();
            }
        }

        // ============================ PAGINACIÓN ============================
        protected void gvProductos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProductos.PageIndex = e.NewPageIndex;
            carga_producto(txt_nombre.Text.Trim());   // Mantiene el filtro de búsqueda si existe
        }

        // ============================ ELIMINACIÓN LÓGICA ============================
        private void ProcesarEliminacionLogica(int id)
        {
            try
            {
                CN_tbl_producto.elimiLog(new tbl_productos { pro_id = id });
                MostrarMensaje("Éxito", "Producto eliminado lógicamente.", "success");
                carga_producto();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", ex.Message, "error");
            }
        }

        // ============================ ELIMINACIÓN FÍSICA ============================
        private void ProcesarEliminacionFisica(int id)
        {
            try
            {
                EliminarFotosFisicas(id);
                CN_tbl_producto.delete(new tbl_productos { pro_id = id });
                MostrarMensaje("Éxito", "Producto y sus imágenes eliminados permanentemente.", "success");
                carga_producto();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", ex.Message, "error");
            }
        }

        // ====================== ELIMINAR FOTOS FÍSICAS ======================
        private void EliminarFotosFisicas(int pro_id)
        {
            using (var dc = new DataClasses1DataContext())
            {
                var fotos = dc.tbl_fotos_productos.Where(f => f.pro_id == pro_id).ToList();
                foreach (var foto in fotos)
                {
                    string rutaFisica = Server.MapPath(foto.fpro_ruta_imagen);
                    if (File.Exists(rutaFisica))
                    {
                        try { File.Delete(rutaFisica); } catch { }
                    }
                }
            }
        }

        // ============================
        // CARGAR PRODUCTOS
        // ============================
        private void carga_producto(string filtro = "")
        {
            try
            {
                var lista =
                    CN_tbl_producto.traerTodosProductos();

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    filtro = filtro.ToLower();

                    lista = lista.Where(x =>

                        x.pro_id.ToString()
                        .Contains(filtro)

                        ||

                        (x.pro_codigo ?? "")
                        .ToLower()
                        .Contains(filtro)

                        ||

                        (x.pro_nombre ?? "")
                        .ToLower()
                        .Contains(filtro)

                        ||

                        (x.pro_descripcion ?? "")
                        .ToLower()
                        .Contains(filtro)

                        ||

                        x.pro_cantidad.ToString()
                        .Contains(filtro)

                        ||

                        x.pro_precio.ToString()
                        .Contains(filtro)

                        ||

                        (
                            x.tbl_proveedor != null
                            ?
                            x.tbl_proveedor.prov_nombre
                            :
                            ""
                        )
                        .ToLower()
                        .Contains(filtro)

                    ).ToList();
                }

                gvProductos.DataSource = lista;
                gvProductos.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje(
                    "Error",
                    ex.Message,
                    "error");
            }
        }

        // ============================
        // OBTENER IMAGEN
        // ============================
        public string ObtenerImagen(int pro_id)
        {
            using (var dc =
                new DataClasses1DataContext())
            {
                var foto = dc.tbl_fotos_productos
                    .FirstOrDefault(x =>
                        x.pro_id == pro_id &&
                        x.fpro_es_principal == true);

                if (foto != null)
                {
                    return ResolveUrl(foto.fpro_ruta_imagen);
                }

                return ResolveUrl(
                    "~/Sources/Productos/no-image.png");
            }
        }

        // ============================
        // ROW DATABOUND
        // ============================
        protected void gvProductos_RowDataBound(
     object sender,
     GridViewRowEventArgs e)
        {
            if (e.Row.RowType ==
                DataControlRowType.DataRow)
            {
                tbl_productos producto =
                    (tbl_productos)e.Row.DataItem;

                if (producto != null)
                {
                    DropDownList ddl =
                        (DropDownList)e.Row
                        .FindControl("ddlEstado");

                    if (ddl != null)
                    {
                        ddl.SelectedValue =
                            producto.pro_estado.ToString();
                    }

                    Repeater rp =
                        (Repeater)e.Row
                        .FindControl("rpFotos");

                    if (rp != null)
                    {
                        var fotos =
                            producto.tbl_fotos_productos
                            .ToList();

                        // SI NO EXISTEN FOTOS
                        if (fotos.Count == 0)
                        {
                            fotos.Add(new tbl_fotos_productos
                            {
                                fpro_ruta_imagen =
                                    "~/Sources/Productos/no-image.png"
                            });
                        }

                        rp.DataSource = fotos;

                        rp.DataBind();
                    }
                }
            }
        }

        // ============================
        // CAMBIAR ESTADO
        // ============================
        protected void ddlEstado_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            try
            {
                DropDownList ddl =
                    (DropDownList)sender;

                GridViewRow row =
                    (GridViewRow)ddl.NamingContainer;

                int id = Convert.ToInt32(
                    gvProductos.DataKeys[
                        row.RowIndex].Value);

                char estado =
                    ddl.SelectedValue[0];

                CN_tbl_producto.CambiarEstado(
                    id,
                    estado);

                carga_producto();
            }
            catch (Exception ex)
            {
                MostrarMensaje(
                    "Error",
                    ex.Message,
                    "error");
            }
        }

        // ============================
        // BUSCAR
        // ============================
        protected void btnBuscar_Click(object sender,EventArgs e)
        {
            if (txt_nombre.Text.Length > 100)
            {
                MostrarMensaje(
                    "Error",
                    "Máximo 100 caracteres.",
                    "error");

                return;
            }

            carga_producto(
                txt_nombre.Text.Trim());
        }

        // ============================
        // EDITAR
        // ============================
        protected void btnEditar_Click(
            object sender,
            EventArgs e)
        {
            int id = Convert.ToInt32(
                ((Button)sender)
                .CommandArgument);

            Response.Redirect(
                "editar_tbl_productos.aspx?id=" + id);
        }

        

        // ============================
        // NUEVO PRODUCTO
        // ============================
        protected void btnNuevoProducto_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "nuevo_tbl_productos.aspx");
        }

        // ============================
        // SWEET ALERT
        // ============================
        private void MostrarMensaje(
            string titulo,
            string texto,
            string tipo)
        {
            string script =
                $"Swal.fire('{titulo}','{texto}','{tipo}')";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "SweetAlert",
                script,
                true);
        }

        // ============================
        // LIMPIAR BUSCADOR
        // ============================
        protected void btnLimpiar_Click(
            object sender,
            EventArgs e)
        {
            txt_nombre.Text = "";

            carga_producto();
        }

        protected void btnImportarExcel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Excel_tbl_productos.aspx");
        }
    }
}

