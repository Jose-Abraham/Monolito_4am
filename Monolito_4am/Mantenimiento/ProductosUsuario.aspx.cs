using Capa_Datos;
using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4am.Mantenimiento
{
    public partial class ProductosUsuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarProductos();
            }
        }

        private void CargarProductos()
        {
            var productos = CN_tbl_producto.traerTodosProductos()
                            .Where(p => p.pro_estado == 'A')
                            .ToList();

            rpProductos.DataSource = productos;
            rpProductos.DataBind();
        }

        protected void rpProductos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var producto = (tbl_productos)e.Item.DataItem;

                Repeater rpFotos =
                    (Repeater)e.Item.FindControl("rpImagenesProducto");

                if (producto.tbl_fotos_productos == null ||
                    !producto.tbl_fotos_productos.Any())
                {
                    rpFotos.DataSource = new[]
                    {
                new
                {
                    fpro_ruta_imagen = "~/Sources/Productos/no-image.png"
                }
            };
                }
                else
                {
                    rpFotos.DataSource = producto.tbl_fotos_productos;
                }

                rpFotos.DataBind();
            }
        }
    }
}