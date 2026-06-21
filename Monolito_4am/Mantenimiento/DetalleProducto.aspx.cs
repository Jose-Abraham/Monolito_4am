using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4am.Mantenimiento
{
    public partial class DetalleProducto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["id"], out int id))
                {
                    CargarDetalle(id);
                }
                else
                {
                    Response.Redirect("ProductosUsuario.aspx");
                }
            }
        }

        private void CargarDetalle(int id)
        {
            var producto = CN_tbl_producto.traerproductodetalle(id);
            if (producto == null)
            {
                Response.Redirect("ProductosUsuario.aspx");
                return;
            }

            // Información básica
            lblNombre.Text = producto.pro_nombre;
            lblPrecio.Text = producto.pro_precio?.ToString("0.00") ?? "0.00";
            lblDescripcion.Text = producto.pro_descripcion ?? "Sin descripción disponible.";
            lblProveedor.Text = producto.tbl_proveedor?.prov_nombre ?? "Sin proveedor";
            lblCantidad.Text = producto.pro_cantidad.ToString() + " unidades";

            // Imagen principal (primera imagen)
            var primeraFoto = producto.tbl_fotos_productos.FirstOrDefault();
            if (primeraFoto != null)
                imgPrincipal.ImageUrl = ResolveUrl(primeraFoto.fpro_ruta_imagen);
            else
                imgPrincipal.ImageUrl = ResolveUrl("~/Sources/Productos/no-image.png");

            // Todas las imágenes para las miniaturas
            rpTodasImagenes.DataSource = producto.tbl_fotos_productos.ToList();
            rpTodasImagenes.DataBind();
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("ProductosUsuario.aspx");
        }
    }
}