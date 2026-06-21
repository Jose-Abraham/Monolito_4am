using Capa_Datos;
using Capa_Negocio;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4am.Mantenimiento
{
    public partial class Excel_tbl_proveedor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvPreview.Visible = false;
            }
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuExcel.HasFile)
            {
                MostrarSweetAlert("Error", "Seleccione un archivo", "error");
                return;
            }

            try
            {
                DataTable dt = ProcesarArchivoExcel();

                if (dt == null || dt.Columns.Count == 0)
                {
                    MostrarSweetAlert("Error", "No se pudo leer el archivo", "error");
                    return;
                }

                if (!dt.Columns.Contains("Nombre"))
                {
                    MostrarSweetAlert("Error", "El archivo debe tener la columna 'Nombre'", "error");
                    return;
                }

                Session["PreviewProveedores"] = dt;
                gvPreview.DataSource = dt;
                gvPreview.DataBind();
                gvPreview.Visible = true;

                lblMensaje.Text = $"Se cargaron {dt.Rows.Count} registros para previsualización.";
            }
            catch (Exception ex)
            {
                MostrarSweetAlert("Error", ex.Message, "error");
            }
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            DataTable dt = Session["PreviewProveedores"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                MostrarSweetAlert("Advertencia", "Primero previsualice los datos", "warning");
                return;
            }

            int insertados = 0, omitidos = 0;
            string errores = "";

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    string nombre = row["Nombre"]?.ToString().Trim() ?? "";
                    string ruc = row["RUC"]?.ToString().Trim() ?? "";
                    string telefono = row["Telefono"]?.ToString().Trim() ?? "";
                    string correo = row["Correo"]?.ToString().Trim() ?? "";

                    // ====================== VALIDACIONES ======================

                    // Nombre: Obligatorio y solo letras
                    if (string.IsNullOrWhiteSpace(nombre))
                    {
                        errores += $"• Fila {row.Table.Rows.IndexOf(row) + 2}: Nombre vacío<br>";
                        omitidos++;
                        continue;
                    }
                    if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                    {
                        errores += $"• Fila {row.Table.Rows.IndexOf(row) + 2}: Nombre solo puede contener letras<br>";
                        omitidos++;
                        continue;
                    }

                    // RUC: Obligatorio y exactamente 13 dígitos
                    if (string.IsNullOrWhiteSpace(ruc))
                    {
                        errores += $"• Fila {row.Table.Rows.IndexOf(row) + 2}: RUC es obligatorio<br>";
                        omitidos++;
                        continue;
                    }
                    if (!Regex.IsMatch(ruc, @"^\d{13}$"))
                    {
                        errores += $"• Fila {row.Table.Rows.IndexOf(row) + 2}: RUC debe tener exactamente 13 dígitos<br>";
                        omitidos++;
                        continue;
                    }

                    // Teléfono: Obligatorio y exactamente 10 dígitos
                    if (string.IsNullOrWhiteSpace(telefono))
                    {
                        errores += $"• Fila {row.Table.Rows.IndexOf(row) + 2}: Teléfono es obligatorio<br>";
                        omitidos++;
                        continue;
                    }
                    if (!Regex.IsMatch(telefono, @"^\d{10}$"))
                    {
                        errores += $"• Fila {row.Table.Rows.IndexOf(row) + 2}: Teléfono debe tener exactamente 10 dígitos<br>";
                        omitidos++;
                        continue;
                    }

                    // Correo: Obligatorio
                    if (string.IsNullOrWhiteSpace(correo))
                    {
                        errores += $"• Fila {row.Table.Rows.IndexOf(row) + 2}: Correo es obligatorio<br>";
                        omitidos++;
                        continue;
                    }

                    // Validaciones de unicidad en BD
                    if (CN_tbl_proveedor.ExisteNombre(nombre))
                    {
                        errores += $"• Fila {row.Table.Rows.IndexOf(row) + 2}: El nombre '{nombre}' ya existe<br>";
                        omitidos++;
                        continue;
                    }

                    if (CN_tbl_proveedor.ExisteRUC(ruc))
                    {
                        errores += $"• Fila {row.Table.Rows.IndexOf(row) + 2}: El RUC '{ruc}' ya está registrado<br>";
                        omitidos++;
                        continue;
                    }

                    // Si pasó todas las validaciones → Guardar
                    tbl_proveedor nuevo = new tbl_proveedor
                    {
                        prov_nombre = nombre,
                        prov_ruc = ruc,
                        prov_telefono = telefono,
                        prov_correo = correo,
                        prov_estado = 'A'
                    };

                    CN_tbl_proveedor.GuardarConSecuencia(nuevo);
                    insertados++;
                }

                // Mensaje final
                string resumen = $"Importación completada → <b>Nuevos: {insertados}</b> | Omitidos: {omitidos}";

                if (!string.IsNullOrEmpty(errores))
                {
                    resumen += "<br><br><b>Errores encontrados:</b><br>" + errores;
                }

                lblMensaje.Text = resumen;
                Session["PreviewProveedores"] = null;
                gvPreview.Visible = false;

                MostrarSweetAlert("Éxito", $"Se importaron {insertados} proveedores correctamente", "success");
            }
            catch (Exception ex)
            {
                MostrarSweetAlert("Error", ex.Message, "error");
            }
        }

        private DataTable ProcesarArchivoExcel()
        {
            string extension = Path.GetExtension(fuExcel.FileName).ToLower();
            string rutaTemp = Server.MapPath("~/Sources/Temp/" + Guid.NewGuid().ToString() + extension);

            if (!Directory.Exists(Server.MapPath("~/Sources/Temp/")))
                Directory.CreateDirectory(Server.MapPath("~/Sources/Temp/"));

            fuExcel.SaveAs(rutaTemp);

            DataTable dt = new DataTable();

            if (extension == ".csv")
            {
                string[] lineas = File.ReadAllLines(rutaTemp);
                if (lineas.Length > 0)
                {
                    string[] headers = lineas[0].Split(',');
                    foreach (string h in headers)
                        dt.Columns.Add(h.Trim());

                    for (int i = 1; i < lineas.Length; i++)
                    {
                        string[] valores = lineas[i].Split(',');
                        if (valores.Length == dt.Columns.Count)
                            dt.Rows.Add(valores.Select(v => v.Trim()).ToArray());
                    }
                }
            }
            else // Excel
            {
                string connString = extension == ".xls"
                    ? $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={rutaTemp};Extended Properties='Excel 8.0;HDR=YES'"
                    : $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={rutaTemp};Extended Properties='Excel 12.0 Xml;HDR=YES'";

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", conn);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            File.Delete(rutaTemp);
            return dt;
        }

        private void MostrarSweetAlert(string titulo, string mensaje, string tipo)
        {
            string script = $"Swal.fire({{ title: '{titulo}', html: '{mensaje.Replace("'", "\\'")}', icon: '{tipo}' }});";
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("listar_tbl_proveedor.aspx");
        }

        protected void btnLimpiarPreview_Click(object sender, EventArgs e)
        {
            Session["PreviewProveedores"] = null;

            gvPreview.DataSource = null;
            gvPreview.DataBind();

            gvPreview.Visible = false;

            lblMensaje.Text = "";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                Guid.NewGuid().ToString(),
                "limpiarArchivo();",
                true
            );

            MostrarSweetAlert(
                "Limpio",
                "La previsualización fue eliminada correctamente",
                "success"
            );
        }
    }
}