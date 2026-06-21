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
    public partial class listar_tbl_proveedor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Procesar eliminación lógica
            if (IsPostBack && Request.Form["__EVENTTARGET"] != null)
            {
                string target = Request.Form["__EVENTTARGET"];
                string argument = Request.Form["__EVENTARGUMENT"];

                if (target == "btnEliminar_Click" &&
                    int.TryParse(argument, out int idLog))
                {
                    ProcesarEliminacionLogica(idLog);
                    return;
                }

                if (target == "btnEliminarFisico_Click" &&
                    int.TryParse(argument, out int idFis))
                {
                    ProcesarEliminacionFisica(idFis);
                    return;
                }
            }

            // Cargar GridView
            if (!IsPostBack)
            {
                CargarProveedores();
            }
        }

        // ===========================
        // PAGINACIÓN
        // ===========================
        protected void gvProveedores_PageIndexChanging(
            object sender,
            GridViewPageEventArgs e)
        {
            gvProveedores.PageIndex = e.NewPageIndex;

            CargarProveedores(
                txt_nombre.Text.Trim()
            );
        }

        // ===========================
        // ELIMINACIÓN LÓGICA
        // ===========================
        private void ProcesarEliminacionLogica(int id)
        {
            try
            {
                using (var dc = new DataClasses1DataContext())
                {
                    var prov = dc.tbl_proveedor
                        .FirstOrDefault(x => x.prov_id == id);

                    if (prov == null)
                    {
                        MostrarMensaje(
                            "Error",
                            "Proveedor no encontrado.",
                            "error"
                        );
                        return;
                    }

                    prov.prov_estado = 'I';

                    dc.SubmitChanges();
                }

                // RECARGAR PÁGINA
                Response.Redirect(Request.RawUrl);
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

        // ===========================
        // ELIMINACIÓN FÍSICA
        // ===========================
        private void ProcesarEliminacionFisica(int id)
        {
            try
            {
                CN_tbl_proveedor.elimiFis(
                    new tbl_proveedor
                    {
                        prov_id = id
                    }
                );

                // RECARGAR PÁGINA
                Response.Redirect(Request.RawUrl);
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

        // ===========================
        // CARGAR GRID
        // ===========================
        private void CargarProveedores(string filtro = "")
        {
            try
            {
                // VALIDAR GRIDVIEW
                if (gvProveedores == null)
                {
                    return;
                }

                var lista = CN_tbl_proveedor.traerTodosProveedores();

                if (lista == null)
                {
                    lista = new List<tbl_proveedor>();
                }

                // FILTRO
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    lista = lista.Where(x =>
                        (x.prov_nombre ?? "")
                        .ToLower()
                        .Contains(filtro.ToLower())
                    ).ToList();
                }

                gvProveedores.DataSource = lista;
                gvProveedores.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje(
                    "ERROR",
                    ex.ToString(),
                    "error"
                );
            }
        }

        // ===========================
        // ROW DATABOUND
        // ===========================
        protected void gvProveedores_RowDataBound(
            object sender,
            GridViewRowEventArgs e
        )
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                tbl_proveedor proveedor =
                    (tbl_proveedor)e.Row.DataItem;

                if (proveedor != null)
                {
                    DropDownList ddl =
                        (DropDownList)e.Row.FindControl("ddlEstado");

                    if (ddl != null)
                    {
                        ddl.SelectedValue =
                            proveedor.prov_estado.ToString();
                    }
                }
            }
        }

        // ===========================
        // CAMBIAR ESTADO
        // ===========================
        protected void ddlEstado_SelectedIndexChanged(
            object sender,
            EventArgs e
        )
        {
            try
            {
                DropDownList ddl = (DropDownList)sender;

                GridViewRow row =
                    (GridViewRow)ddl.NamingContainer;

                int id = Convert.ToInt32(
                    gvProveedores.DataKeys[row.RowIndex].Value
                );

                char nuevoEstado =
                    ddl.SelectedValue[0];

                CN_tbl_proveedor.CambiarEstado(
                    id,
                    nuevoEstado
                );

                CargarProveedores();
            }
            catch
            {
                MostrarMensaje(
                    "Error",
                    "No se pudo cambiar el estado.",
                    "error"
                );
            }
        }

        // ===========================
        // BUSCAR
        // ===========================
        protected void btnBuscar_Click(
            object sender,
            EventArgs e
        )
                {
                    gvProveedores.PageIndex = 0;

                    CargarProveedores(
                        txt_nombre.Text.Trim()
                    );
                }

        // ===========================
        // LIMPIAR
        // ===========================
        protected void btnLimpiar_Click(
            object sender,
            EventArgs e
        )
                {
                    txt_nombre.Text = "";

                    gvProveedores.PageIndex = 0;

                    CargarProveedores();
                }

        // ===========================
        // EDITAR
        // ===========================
        protected void btnEditar_Click(
            object sender,
            EventArgs e
        )
        {
            int id = Convert.ToInt32(
                ((Button)sender).CommandArgument
            );

            Response.Redirect(
                "editar_tbl_proveedor.aspx?id=" + id
            );
        }

        // ===========================
        // NUEVO
        // ===========================
        protected void btnNuevoProveedor_Click(
            object sender,
            EventArgs e
        )
        {
            Response.Redirect(
                "nuevo_tbl_proveedor.aspx"
            );
        }

        // ===========================
        // MENSAJES
        // ===========================
        private void MostrarMensaje(
            string titulo,
            string texto,
            string tipo
        )
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

        protected void btnImportarExcel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Excel_tbl_proveedor.aspx");
        }
    }
}