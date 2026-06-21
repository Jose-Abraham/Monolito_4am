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
    public partial class Excel_tbl_productos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) gvPreview.Visible = false;
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuExcel.HasFile) { MostrarSweetAlert("Error", "Seleccione un archivo", "error"); return; }

            try
            {
                DataTable dt = ProcesarArchivoExcel();
                Session["PreviewProductos"] = dt;
                gvPreview.DataSource = dt;
                gvPreview.DataBind();
                gvPreview.Visible = true;
                lblMensaje.Text = $"Se cargaron {dt.Rows.Count} productos.";
            }
            catch (Exception ex)
            {
                MostrarSweetAlert("Error", ex.Message, "error");
            }
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            DataTable dt = Session["PreviewProductos"] as DataTable;
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
                    int fila = dt.Rows.IndexOf(row) + 2; // +2 por encabezado

                    string codigo = row["Codigo"]?.ToString().Trim() ?? "";
                    string nombre = row["Nombre"]?.ToString().Trim() ?? "";
                    string descripcion = row["Descripcion"]?.ToString().Trim() ?? "";
                    string provIdStr = row["ProveedorID"]?.ToString().Trim() ?? "";
                    string cantidadStr = row["Cantidad"]?.ToString().Trim() ?? "";
                    string precioStr = row["Precio"]?.ToString().Trim() ?? "";
                    string imagen = row["Imagen"]?.ToString().Trim() ?? "";

                    // ==================== VALIDACIONES ====================

                    if (string.IsNullOrWhiteSpace(codigo))
                    {
                        errores += $"• Fila {fila}: Código no puede estar vacío<br>";
                        omitidos++;
                        continue;
                    }
                    if (!Regex.IsMatch(codigo, @"^pro_\d+$"))
                    {
                        errores += $"• Fila {fila}: Código debe tener formato pro_# (ej: pro_123)<br>";
                        omitidos++;
                        continue;
                    }
                    if (CN_tbl_producto.ExisteCodigo(codigo))
                    {
                        errores += $"• Fila {fila}: El código '{codigo}' ya está registrado<br>";
                        omitidos++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(nombre))
                    {
                        errores += $"• Fila {fila}: Nombre no puede estar vacío<br>";
                        omitidos++;
                        continue;
                    }
                    if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                    {
                        errores += $"• Fila {fila}: Nombre solo puede contener letras<br>";
                        omitidos++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(descripcion))
                    {
                        errores += $"• Fila {fila}: Descripción no puede estar vacía<br>";
                        omitidos++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(provIdStr) || !int.TryParse(provIdStr, out int prov_id) || prov_id <= 0)
                    {
                        errores += $"• Fila {fila}: ProveedorID debe ser un número válido mayor a 0<br>";
                        omitidos++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(cantidadStr) || !int.TryParse(cantidadStr, out int cantidad) || cantidad <= 0 || cantidad > 1000)
                    {
                        errores += $"• Fila {fila}: Cantidad debe ser un número entero entre 1 y 1000<br>";
                        omitidos++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(precioStr))
                    {
                        errores += $"• Fila {fila}: Precio no puede estar vacío<br>";
                        omitidos++;
                        continue;
                    }

                    // Permitir punto o coma como decimal
                    precioStr = precioStr.Replace(".", ",");

                    if (!decimal.TryParse(precioStr, out decimal precio))
                    {
                        errores += $"• Fila {fila}: Precio inválido<br>";
                        omitidos++;
                        continue;
                    }

                    if (precio <= 0)
                    {
                        errores += $"• Fila {fila}: Precio debe ser mayor a 0<br>";
                        omitidos++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(imagen))
                    {
                        errores += $"• Fila {fila}: Imagen no puede estar vacía<br>";
                        omitidos++;
                        continue;
                    }

                    // ====================== GUARDAR ======================
                    tbl_productos nuevo = new tbl_productos
                    {
                        pro_codigo = codigo,
                        pro_nombre = nombre,
                        pro_descripcion = descripcion,
                        prov_id = prov_id,
                        pro_cantidad = cantidad,
                        pro_precio = precio,
                        pro_estado = 'A'
                    };

                    int pro_id = CN_tbl_producto.save(nuevo);

                    // Guardar imagen (si existe)
                    if (!string.IsNullOrEmpty(imagen))
                    {
                        string rutaOrigen = Server.MapPath("~/Sources/Productos/Import/" + imagen);
                        if (File.Exists(rutaOrigen))
                        {
                            string nuevoNombre = Guid.NewGuid().ToString() + Path.GetExtension(imagen);
                            string rutaDestino = Server.MapPath("~/Sources/Productos/" + nuevoNombre);

                            File.Copy(rutaOrigen, rutaDestino, true);

                            tbl_fotos_productos foto = new tbl_fotos_productos
                            {
                                pro_id = pro_id,
                                fpro_ruta_imagen = "~/Sources/Productos/" + nuevoNombre,
                                fpro_nombre_archivo = nuevoNombre,
                                fpro_es_principal = true,
                                fpro_fecha_subida = DateTime.Now
                            };
                            CN_tbl_fotos_productos.Guardar(foto);
                        }
                    }

                    insertados++;
                }

                string resumen = $"<b>Importación completada:</b><br>Nuevos: {insertados} | Omitidos: {omitidos}";
                if (!string.IsNullOrEmpty(errores))
                    resumen += "<br><br><b>Errores encontrados:</b><br>" + errores;

                lblMensaje.Text = resumen;
                MostrarSweetAlert("Éxito", $"Se importaron {insertados} productos correctamente", "success");

                Session["PreviewProductos"] = null;
                gvPreview.Visible = false;
            }
            catch (Exception ex)
            {
                MostrarSweetAlert("Error", ex.Message, "error");
            }
        }

        private DataTable ProcesarArchivoExcel()
        {
            // (Mismo código que usamos en proveedores, adaptado)
            string extension = Path.GetExtension(fuExcel.FileName).ToLower();
            string rutaTemp = Server.MapPath("~/Sources/Temp/" + Guid.NewGuid() + extension);
            fuExcel.SaveAs(rutaTemp);

            DataTable dt = new DataTable();

            if (extension == ".csv")
            {
                var lines = File.ReadAllLines(rutaTemp);
                if (lines.Length > 0)
                {
                    var headers = lines[0].Split(',');
                    foreach (var h in headers) dt.Columns.Add(h.Trim());
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var values = lines[i].Split(',');
                        if (values.Length == dt.Columns.Count)
                            dt.Rows.Add(values.Select(v => v.Trim()).ToArray());
                    }
                }
            }
            else // Excel
            {
                string conn = extension == ".xls"
                    ? $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={rutaTemp};Extended Properties='Excel 8.0;HDR=YES'"
                    : $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={rutaTemp};Extended Properties='Excel 12.0 Xml;HDR=YES'";

                using (OleDbConnection con = new OleDbConnection(conn))
                {
                    con.Open();
                    OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", con);
                    da.Fill(dt);
                }
            }

            File.Delete(rutaTemp);
            return dt;
        }

        private void MostrarSweetAlert(string titulo, string msg, string tipo)
        {
            string script = $"Swal.fire('{titulo}', '{msg.Replace("'", "\\'")}', '{tipo}');";
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("listar_tbl_productos.aspx");
        }
    }
}