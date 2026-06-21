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
    public partial class editar_foto_producto : System.Web.UI.Page
    {
        private int FotoId = 0;
        private List<PreviewImage> ImagenesPreview
        {
            get
            {
                if (Session["PreviewEditarFoto"] == null)
                    Session["PreviewEditarFoto"] = new List<PreviewImage>();
                return (List<PreviewImage>)Session["PreviewEditarFoto"];
            }
            set { Session["PreviewEditarFoto"] = value; }
        }

        public class PreviewImage
        {
            public int Index { get; set; }
            public string PreviewUrl { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out FotoId))
            {
                Response.Redirect("listar_tbl_fotos_productos.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatosFoto();
                ImagenesPreview.Clear();
            }
        }

        private void CargarDatosFoto()
        {
            var foto = CN_tbl_fotos_productos.ObtenerPorId(FotoId); // Necesitamos agregar este método
            if (foto == null)
            {
                Response.Redirect("listar_tbl_fotos_productos.aspx");
                return;
            }

            lblProductoNombre.Text = foto.tbl_productos?.pro_nombre ?? "Producto no encontrado";
            imgActual.ImageUrl = ResolveUrl(foto.fpro_ruta_imagen);
            chkEsPrincipal.Checked = foto.fpro_es_principal == true;
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuFotos.HasFiles) return;

            try
            {
                validarFotos();

                string carpetaTemp = Server.MapPath("~/Sources/Temp/");
                if (!Directory.Exists(carpetaTemp)) Directory.CreateDirectory(carpetaTemp);

                foreach (HttpPostedFile archivo in fuFotos.PostedFiles)
                {
                    string extension = Path.GetExtension(archivo.FileName).ToLower();
                    if (extension != ".png" && extension != ".jpg" && extension != ".jpeg") continue;

                    string nombreTemp = Guid.NewGuid().ToString() + extension;
                    string rutaTemp = Path.Combine(carpetaTemp, nombreTemp);
                    archivo.SaveAs(rutaTemp);

                    ImagenesPreview.Add(new PreviewImage
                    {
                        Index = ImagenesPreview.Count,
                        PreviewUrl = "~/Sources/Temp/" + nombreTemp
                    });
                }

                rpPreview.DataSource = ImagenesPreview;
                rpPreview.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"Swal.fire('Error','{ex.Message}','error')", true);
            }
        }

        protected void rpPreview_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                var item = ImagenesPreview.FirstOrDefault(x => x.Index == index);
                if (item != null)
                {
                    string ruta = Server.MapPath(item.PreviewUrl);
                    if (File.Exists(ruta)) File.Delete(ruta);
                    ImagenesPreview.Remove(item);
                }
                rpPreview.DataSource = ImagenesPreview;
                rpPreview.DataBind();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var fotoActual = CN_tbl_fotos_productos.ObtenerPorId(FotoId);
                if (fotoActual == null)
                    throw new Exception("Foto no encontrada");

                bool esPrincipal = chkEsPrincipal.Checked;
                int pro_id = fotoActual.pro_id;

                // Si el usuario cambió el producto (lo agregaremos después)
                // int nuevoProId = ... (lo veremos más abajo)

                if (esPrincipal)
                {
                    CN_tbl_fotos_productos.MarcarComoPrincipal(0, pro_id);
                }

                if (ImagenesPreview.Count > 0)
                {
                    string carpeta = Server.MapPath("~/Sources/Productos/");
                    if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

                    var preview = ImagenesPreview[0];
                    string origen = Server.MapPath(preview.PreviewUrl);
                    string nuevoNombre = Guid.NewGuid().ToString() + Path.GetExtension(origen);
                    string destino = Path.Combine(carpeta, nuevoNombre);

                    File.Copy(origen, destino, true);

                    // Eliminar archivo físico anterior
                    string rutaVieja = Server.MapPath(fotoActual.fpro_ruta_imagen);
                    if (File.Exists(rutaVieja))
                        File.Delete(rutaVieja);

                    // === ACTUALIZAR el registro existente (NO crear nuevo) ===
                    fotoActual.fpro_ruta_imagen = "~/Sources/Productos/" + nuevoNombre;
                    fotoActual.fpro_nombre_archivo = nuevoNombre;
                    fotoActual.fpro_es_principal = esPrincipal;
                    fotoActual.fpro_fecha_subida = DateTime.Now;

                    // Si más adelante permitimos cambiar de producto:
                    // fotoActual.pro_id = nuevoProId;

                    CN_tbl_fotos_productos.Actualizar(fotoActual);   // ← Nuevo método
                }
                else
                {
                    // Solo cambió "Foto Principal"
                    CN_tbl_fotos_productos.Actualizar(fotoActual);
                }

                LimpiarPreviews();

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "Swal.fire('Éxito','Imagen actualizada correctamente','success').then(() => { window.location='listar_tbl_fotos_productos.aspx'; });", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"Swal.fire('Error','{ex.Message.Replace("'", "\\'")}','error')", true);
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
                string ext = Path.GetExtension(archivo.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                    throw new Exception("Solo se permiten PNG, JPG y JPEG.");

                if (archivo.ContentLength > 2 * 1024 * 1024)
                    throw new Exception($"La imagen {archivo.FileName} supera 2MB.");

                total += archivo.ContentLength;
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("listar_tbl_fotos_productos.aspx");
        }
    }
}