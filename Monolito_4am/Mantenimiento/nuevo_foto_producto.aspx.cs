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
    public partial class nuevo_foto_producto : System.Web.UI.Page
    {
        private List<PreviewImage> ImagenesPreview
        {
            get
            {
                if (Session["PreviewImagenesFoto"] == null)
                    Session["PreviewImagenesFoto"] = new List<PreviewImage>();
                return (List<PreviewImage>)Session["PreviewImagenesFoto"];
            }
            set { Session["PreviewImagenesFoto"] = value; }
        }

        public class PreviewImage
        {
            public int Index { get; set; }
            public string PreviewUrl { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarProductos();
                ImagenesPreview.Clear();
            }
        }

        private void CargarProductos()
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                ddlProducto.DataSource = dc.tbl_productos
                    .Where(p => p.pro_estado == 'A')
                    .OrderBy(p => p.pro_nombre)
                    .ToList();
                ddlProducto.DataTextField = "pro_nombre";
                ddlProducto.DataValueField = "pro_id";
                ddlProducto.DataBind();
                ddlProducto.Items.Insert(0, new ListItem("-- Seleccione un producto --", "0"));
            }
        }

        protected void ddlProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProducto.SelectedValue != "0" && chkEsPrincipal.Checked)
            {
                int proId = Convert.ToInt32(ddlProducto.SelectedValue);
                bool tienePrincipal = CN_tbl_fotos_productos.TieneFotoPrincipal(proId);

                if (tienePrincipal)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "yaTienePrincipal",
                        "Swal.fire({title: 'Aviso', text: 'Este producto ya tiene una foto principal. Si continúas, la anterior se desmarcará.', icon: 'info'});", true);
                }
            }
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuFotos.HasFiles)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "noimagen",
                    "Swal.fire('Aviso','No hay imágenes para previsualizar','warning');", true);
                return;
            }

            // Validación extra: solo permitir 1 imagen
            if (fuFotos.PostedFiles.Count > 1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "soloUna",
                    "Swal.fire('Error','Solo se permite una imagen','warning');", true);
                return;
            }

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
                if (ddlProducto.SelectedValue == "0") throw new Exception("Seleccione un producto");
                if (ImagenesPreview.Count == 0) throw new Exception("Debe subir al menos una imagen");

                int pro_id = Convert.ToInt32(ddlProducto.SelectedValue);
                bool esPrincipal = chkEsPrincipal.Checked;

                // Si se marca como principal, desmarcar las anteriores
                if (esPrincipal)
                {
                    CN_tbl_fotos_productos.MarcarComoPrincipal(0, pro_id); // 0 = desmarcar todas
                }

                string carpeta = Server.MapPath("~/Sources/Productos/");
                if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

                using (DataClasses1DataContext dc = new DataClasses1DataContext())
                {
                    int contador = 0;
                    foreach (var preview in ImagenesPreview)
                    {
                        string origen = Server.MapPath(preview.PreviewUrl);
                        string nuevoNombre = Guid.NewGuid().ToString() + Path.GetExtension(origen);
                        string destino = Path.Combine(carpeta, nuevoNombre);

                        File.Copy(origen, destino, true);

                        tbl_fotos_productos foto = new tbl_fotos_productos
                        {
                            pro_id = pro_id,
                            fpro_ruta_imagen = "~/Sources/Productos/" + nuevoNombre,
                            fpro_nombre_archivo = nuevoNombre,
                            fpro_es_principal = esPrincipal && contador == 0,
                            fpro_fecha_subida = DateTime.Now
                        };

                        dc.tbl_fotos_productos.InsertOnSubmit(foto);
                        contador++;
                    }
                    dc.SubmitChanges();
                }

                LimpiarPreviews();
                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "Swal.fire('Éxito','Imágenes guardadas correctamente','success').then(() => { window.location='listar_tbl_fotos_productos.aspx'; });", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"Swal.fire('Error','{ex.Message}','error')", true);
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
            if (total > 10 * 1024 * 1024)
                throw new Exception("El total supera 10MB.");
        }


    }
}