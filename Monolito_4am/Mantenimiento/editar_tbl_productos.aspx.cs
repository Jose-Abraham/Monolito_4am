using Capa_Datos;
using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4am.Mantenimiento
{
    public partial class editar_tbl_productos : System.Web.UI.Page
    {
        int idProducto = 0;

        // ====================== PREVISUALIZACIÓN NUEVAS IMÁGENES ======================
        private List<PreviewImage> ImagenesPreview
        {
            get
            {
                if (Session["PreviewImagenesEditar"] == null)
                    Session["PreviewImagenesEditar"] = new List<PreviewImage>();
                return (List<PreviewImage>)Session["PreviewImagenesEditar"];
            }
            set { Session["PreviewImagenesEditar"] = value; }
        }

        public class PreviewImage
        {
            public int Index { get; set; }
            public string PreviewUrl { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(Request.QueryString["id"], out idProducto))
                {
                    Response.Redirect("listar_tbl_productos.aspx");
                    return;
                }

                if (!IsPostBack)
                {
                    cargarProveedores();
                    cargarDatos();
                    ImagenesPreview.Clear();
                }
            }
            catch
            {
                Response.Redirect("listar_tbl_productos.aspx");
            }
        }

        private void cargarProveedores()
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                ddlProveedor.DataSource = dc.tbl_proveedor.Where(x => x.prov_estado == 'A').ToList();
                ddlProveedor.DataTextField = "prov_nombre";
                ddlProveedor.DataValueField = "prov_id";
                ddlProveedor.DataBind();
                ddlProveedor.Items.Insert(0, new ListItem("-- Seleccione --", "0"));
            }
        }

        private void cargarDatos()
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                var producto = dc.tbl_productos.FirstOrDefault(x => x.pro_id == idProducto);
                if (producto == null) throw new Exception("Producto no encontrado.");

                txt_codigo.Text = producto.pro_codigo;
                txt_nombre.Text = producto.pro_nombre;
                txt_descripcion.Text = producto.pro_descripcion;
                txt_cantidad.Text = producto.pro_cantidad.ToString();

                if (producto.pro_precio != null)
                    txt_precio.Text = Convert.ToDecimal(producto.pro_precio).ToString("0.00").Replace(".", ",");

                if (producto.prov_id != null)
                    ddlProveedor.SelectedValue = producto.prov_id.ToString();

                rpImagenes.DataSource = producto.tbl_fotos_productos.ToList();
                rpImagenes.DataBind();
            }
        }

        // ====================== PREVISUALIZAR ======================
        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDAR SI NO HAY ARCHIVOS
                if (!fuFotos.HasFiles)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "sinimagenes",
                        "Swal.fire('Aviso','No hay imágenes para previsualizar','warning');",
                        true);

                    return;
                }

                validarFotos();

                string carpetaTemp = Server.MapPath("~/Sources/Temp/");

                if (!Directory.Exists(carpetaTemp))
                    Directory.CreateDirectory(carpetaTemp);

                foreach (HttpPostedFile archivo in fuFotos.PostedFiles)
                {
                    string extension =
                        Path.GetExtension(archivo.FileName).ToLower();

                    if (extension != ".png" &&
                        extension != ".jpg" &&
                        extension != ".jpeg")
                        continue;

                    // EVITAR DUPLICADOS
                    bool existe =
                        ImagenesPreview.Any(x =>
                            x.PreviewUrl.Contains(
                                Path.GetFileNameWithoutExtension(archivo.FileName)));

                    if (existe)
                        continue;

                    string nombreTemp =
                        Guid.NewGuid().ToString() + extension;

                    string rutaTemp =
                        Path.Combine(carpetaTemp, nombreTemp);

                    archivo.SaveAs(rutaTemp);

                    ImagenesPreview.Add(new PreviewImage
                    {
                        Index = ImagenesPreview.Count,
                        PreviewUrl = "~/Sources/Temp/" + nombreTemp
                    });
                }

                rpPreview.DataSource = ImagenesPreview;
                rpPreview.DataBind();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "limpiar",
                    "limpiarFileUpload();",
                    true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "limpiar",
                    "limpiarFileUpload();",
                    true);

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{ex.Message}','error')",
                    true);
            }
        }

        // ====================== ELIMINAR DE PREVISUALIZACIÓN ======================
        protected void rpPreview_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                var item = ImagenesPreview.FirstOrDefault(x => x.Index == index);

                if (item != null)
                {
                    string rutaFisica = Server.MapPath(item.PreviewUrl);
                    if (File.Exists(rutaFisica)) File.Delete(rutaFisica);
                    ImagenesPreview.Remove(item);
                }

                rpPreview.DataSource = ImagenesPreview;
                rpPreview.DataBind();
            }
        }

        // ====================== EDITAR ======================
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                string codigo = txt_codigo.Text.Trim();
                string nombre = txt_nombre.Text.Trim();
                string descripcion = txt_descripcion.Text.Trim();
                int cantidad;
                decimal precio;

                if (string.IsNullOrWhiteSpace(codigo)) throw new Exception("Ingrese el código.");
                if (codigo.Length > 50) throw new Exception("Máximo 50 caracteres.");
                if (!System.Text.RegularExpressions.Regex.IsMatch(codigo, @"^pro_\d+$")) throw new Exception("Formato inválido.");
                if (CN_tbl_producto.ExisteCodigo(codigo, idProducto)) throw new Exception("El código ya existe.");

                if (string.IsNullOrWhiteSpace(nombre)) throw new Exception("Ingrese el nombre.");
                if (nombre.Length < 3 || nombre.Length > 100) throw new Exception("Nombre inválido.");
                if (!System.Text.RegularExpressions.Regex.IsMatch(nombre, @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ ]+$")) throw new Exception("Nombre contiene caracteres inválidos.");

                if (string.IsNullOrWhiteSpace(descripcion)) throw new Exception("Ingrese descripción.");
                if (descripcion.Length > 150) throw new Exception("Descripción demasiado larga.");

                if (!int.TryParse(txt_cantidad.Text, out cantidad) || cantidad <= 0 || cantidad > 1000) throw new Exception("Cantidad inválida.");

                string precioTexto = txt_precio.Text.Trim();

                if (string.IsNullOrWhiteSpace(precioTexto))
                    throw new Exception("Ingrese precio.");

                if (precioTexto.Contains("."))
                    throw new Exception("Use coma decimal. Ejemplo: 8,50");

                precioTexto = precioTexto.Replace(",", ".");

                if (!decimal.TryParse(precioTexto, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out precio))
                    throw new Exception("Precio inválido.");

                if (precio <= 0) throw new Exception("El precio debe ser mayor a 0.");
                if (precio > 1000000) throw new Exception("Precio demasiado alto.");

                if (ddlProveedor.SelectedValue == "0") throw new Exception("Seleccione un proveedor.");

                // Eliminar imágenes marcadas (existentes)
                eliminarImagenesMarcadas();

                // Actualizar producto
                tbl_productos pro = new tbl_productos
                {
                    pro_id = idProducto,
                    pro_codigo = codigo,
                    pro_nombre = nombre,
                    pro_descripcion = descripcion,
                    pro_cantidad = cantidad,
                    pro_precio = precio,
                    prov_id = Convert.ToInt32(ddlProveedor.SelectedValue),
                    pro_estado = 'A'
                };

                CN_tbl_producto.modify(pro);

                // Guardar nuevas imágenes si existen
                if (ImagenesPreview.Count > 0)
                    guardarFotosDesdePreview(idProducto);

                LimpiarPreviews();

                ScriptManager.RegisterStartupScript(this, GetType(), "ok",
                    "Swal.fire('Correcto','Producto actualizado','success').then(()=>{window.location='listar_tbl_productos.aspx';});",
                    true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"Swal.fire('Error','{ex.Message}','error')", true);
            }
        }

        private void guardarFotosDesdePreview(int pro_id)
        {
            string carpeta = Server.MapPath("~/Sources/Productos/");
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                int contador = 0;
                foreach (var preview in ImagenesPreview)
                {
                    string rutaOrigen = Server.MapPath(preview.PreviewUrl);
                    string nuevoNombre = Guid.NewGuid().ToString() + Path.GetExtension(rutaOrigen);
                    string rutaDestino = Path.Combine(carpeta, nuevoNombre);

                    File.Copy(rutaOrigen, rutaDestino, true);

                    tbl_fotos_productos foto = new tbl_fotos_productos
                    {
                        pro_id = pro_id,
                        fpro_ruta_imagen = "~/Sources/Productos/" + nuevoNombre,
                        fpro_nombre_archivo = nuevoNombre,
                        fpro_es_principal = false,
                        fpro_fecha_subida = DateTime.Now
                    };

                    dc.tbl_fotos_productos.InsertOnSubmit(foto);
                    contador++;
                }
                dc.SubmitChanges();
            }
        }

        private void LimpiarPreviews()
        {
            foreach (var item in ImagenesPreview)
            {
                string ruta = Server.MapPath(item.PreviewUrl);
                if (File.Exists(ruta)) File.Delete(ruta);
            }
            ImagenesPreview.Clear();
            rpPreview.DataSource = null;
            rpPreview.DataBind();
        }

        private void validarFotos()
        {
            if (!fuFotos.HasFiles) return;

            long total = 0;
            foreach (HttpPostedFile archivo in fuFotos.PostedFiles)
            {
                string extension = Path.GetExtension(archivo.FileName).ToLower();
                if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
                    throw new Exception("Formato inválido. Solo PNG, JPG y JPEG.");

                if (archivo.ContentLength > 2 * 1024 * 1024)
                    throw new Exception($"La imagen '{archivo.FileName}' supera 2MB.");

                total += archivo.ContentLength;
            }

            if (total > 10 * 1024 * 1024)
                throw new Exception("El total de las imágenes supera 10MB.");
        }

        private void eliminarImagenesMarcadas()
        {
            string ids = hfImagenesEliminar.Value.Trim();
            if (string.IsNullOrEmpty(ids)) return;

            var listaIds = ids.Split(',').Select(int.Parse).ToList();

            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                var fotosEliminar = dc.tbl_fotos_productos
                    .Where(f => listaIds.Contains(f.fpro_id) && f.pro_id == idProducto)
                    .ToList();

                foreach (var foto in fotosEliminar)
                {
                    string rutaFisica = Server.MapPath(foto.fpro_ruta_imagen);
                    if (File.Exists(rutaFisica))
                    {
                        try { File.Delete(rutaFisica); } catch { }
                    }

                    dc.tbl_fotos_productos.DeleteOnSubmit(foto);
                }
                dc.SubmitChanges();
            }
        }

        // Para limpiar desde JavaScript
        protected void btnLimpiarServidor_Click(object sender, EventArgs e)
        {
            LimpiarPreviews();
        }
    }
}