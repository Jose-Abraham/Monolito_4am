using Capa_Datos;
using Capa_Negocio;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4am.Mantenimiento
{
    public partial class Excel_tbl_foto_producto : System.Web.UI.Page
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
                MostrarSweetAlert("Error", "Seleccione un archivo Excel o CSV", "error");
                return;
            }

            try
            {
                DataTable dt = ProcesarArchivoExcel();
                Session["PreviewFotosProducto"] = dt;
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
            DataTable dt = Session["PreviewFotosProducto"] as DataTable;
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
                    int fila = dt.Rows.IndexOf(row) + 2;

                    string proIdStr = row["ProductoID"]?.ToString().Trim() ?? "";
                    string nombreImagen = row["NombreImagen"]?.ToString().Trim() ?? "";
                    string esPrincipalStr = row["EsPrincipal"]?.ToString().Trim().ToLower() ?? "";

                    // ====================== VALIDACIONES ======================

                    // 1. ProductoID
                    if (string.IsNullOrWhiteSpace(proIdStr))
                    {
                        errores += $"• Fila {fila}: ProductoID no puede estar vacío<br>";
                        omitidos++;
                        continue;
                    }

                    if (!int.TryParse(proIdStr, out int pro_id) || pro_id <= 0)
                    {
                        errores += $"• Fila {fila}: ProductoID debe ser un número válido mayor a 0<br>";
                        omitidos++;
                        continue;
                    }

                    // Verificar si existe (activo o en historial)
                    if (!CN_tbl_producto.ExisteProducto(pro_id))
                    {
                        if (CN_tbl_producto.ExisteEnHistorial(pro_id))
                        {
                            CN_tbl_producto.ReactivarProductoDesdeHistorial(pro_id);
                            errores += $"• Fila {fila}: Producto {pro_id} reactivado desde historial.<br>";
                        }
                        else
                        {
                            errores += $"• Fila {fila}: El producto con ID {pro_id} no existe.<br>";
                            omitidos++;
                            continue;
                        }
                    }

                    // 2. NombreImagen
                    if (string.IsNullOrWhiteSpace(nombreImagen))
                    {
                        errores += $"• Fila {fila}: NombreImagen no puede estar vacío<br>";
                        omitidos++;
                        continue;
                    }

                    string extension = Path.GetExtension(nombreImagen).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        errores += $"• Fila {fila}: NombreImagen debe tener formato .jpg, .jpeg o .png<br>";
                        omitidos++;
                        continue;
                    }

                    // 3. EsPrincipal (solo true/false)
                    bool esPrincipal = false;
                    if (!string.IsNullOrWhiteSpace(esPrincipalStr))
                    {
                        if (esPrincipalStr == "true" || esPrincipalStr == "1" || esPrincipalStr == "sí" || esPrincipalStr == "si")
                            esPrincipal = true;
                        else if (esPrincipalStr == "false" || esPrincipalStr == "0" || esPrincipalStr == "no")
                            esPrincipal = false;
                        else
                        {
                            errores += $"• Fila {fila}: EsPrincipal solo puede ser 'true' o 'false'<br>";
                            omitidos++;
                            continue;
                        }
                    }

                    // Si es principal y ya existe una, advertir
                    if (esPrincipal && CN_tbl_fotos_productos.TieneFotoPrincipal(pro_id))
                    {
                        errores += $"• Fila {fila}: El producto {pro_id} ya tiene foto principal. Se guardará como no principal.<br>";
                        esPrincipal = false;
                    }

                    // ====================== PROCESAR IMAGEN ======================
                    string carpetaImport = Server.MapPath("~/Sources/Productos/Import/");
                    string carpetaDestino = Server.MapPath("~/Sources/Productos/");

                    if (!Directory.Exists(carpetaImport)) Directory.CreateDirectory(carpetaImport);
                    if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);

                    string rutaOrigen = Path.Combine(carpetaImport, nombreImagen);

                    if (!File.Exists(rutaOrigen))
                    {
                        errores += $"• Fila {fila}: La imagen '{nombreImagen}' no se encontró en la carpeta Import<br>";
                        omitidos++;
                        continue;
                    }

                    string nuevoNombre = Guid.NewGuid().ToString() + extension;
                    string rutaDestino = Path.Combine(carpetaDestino, nuevoNombre);

                    File.Copy(rutaOrigen, rutaDestino, true);

                    // Guardar en base de datos
                    tbl_fotos_productos foto = new tbl_fotos_productos
                    {
                        pro_id = pro_id,
                        fpro_ruta_imagen = "~/Sources/Productos/" + nuevoNombre,
                        fpro_nombre_archivo = nuevoNombre,
                        fpro_es_principal = esPrincipal,
                        fpro_fecha_subida = DateTime.Now
                    };

                    CN_tbl_fotos_productos.Guardar(foto);
                    insertados++;
                }

                string resumen = $"<b>Importación completada:</b><br>Imágenes insertadas: {insertados} | Omitidas: {omitidos}";
                if (!string.IsNullOrEmpty(errores))
                    resumen += "<br><br><b>Errores encontrados:</b><br>" + errores;

                lblMensaje.Text = resumen;

                if (insertados > 0)
                    MostrarSweetAlert("Éxito", $"Se importaron {insertados} imágenes correctamente", "success");
                else
                    MostrarSweetAlert("Advertencia", "No se importó ninguna imagen", "warning");

                Session["PreviewFotosProducto"] = null;
                gvPreview.Visible = false;
            }
            catch (Exception ex)
            {
                MostrarSweetAlert("Error", ex.Message, "error");
            }
        }

        private DataTable ProcesarArchivoExcel()
        {
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

        private void MostrarSweetAlert(string titulo, string mensaje, string tipo)
        {
            string script = $"Swal.fire({{ title: '{titulo}', html: '{mensaje.Replace("'", "\\'")}', icon: '{tipo}' }});";
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("listar_tbl_fotos_productos.aspx");
        }
    }
}