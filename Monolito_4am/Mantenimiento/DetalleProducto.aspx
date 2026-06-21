<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="DetalleProducto.aspx.cs" Inherits="Monolito_4am.Mantenimiento.DetalleProducto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   <style>
        .detail-container { max-width: 1100px; margin: 40px auto; padding: 20px; }
        .product-gallery {
            position: relative;
            background: #1e2937;
            border-radius: 20px;
            overflow: hidden;
            margin-bottom: 30px;
        }
        .main-image {
            width: 100%;
            height: 480px;
            object-fit: contain;
            background: #0f172a;
            padding: 20px;
        }
        .thumbnail-container {
            display: flex;
            gap: 10px;
            padding: 15px;
            background: rgba(0,0,0,0.3);
            overflow-x: auto;
        }
        .thumbnail {
            width: 80px;
            height: 80px;
            object-fit: cover;
            border-radius: 8px;
            cursor: pointer;
            border: 2px solid transparent;
            transition: all 0.3s;
        }
        .thumbnail.active {
            border-color: #38bdf8;
            transform: scale(1.08);
        }

        .product-info h1 {
            font-size: 2.2rem;
            color: white;
            margin-bottom: 15px;
        }
        .product-price {
            font-size: 2rem;
            color: #38bdf8;
            font-weight: 800;
            margin-bottom: 20px;
        }
        .product-description {
            font-size: 1.1rem;
            line-height: 1.7;
            color: #cbd5e1;
            margin-bottom: 30px;
        }

        .back-btn {
            background: rgba(255,255,255,0.1);
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 50px;
            font-weight: bold;
            margin-bottom: 20px;
        }
    </style>

    <div class="detail-container">
        <asp:Button ID="btnVolver" runat="server" Text="← Volver a la Tienda" 
            CssClass="back-btn" OnClick="btnVolver_Click" />

        <div class="row">
            <!-- Galería de Imágenes -->
            <div class="col-lg-7">
                <div class="product-gallery">
                    <asp:Image ID="imgPrincipal" runat="server" CssClass="main-image" />
                    
                    <div class="thumbnail-container">
                        <asp:Repeater ID="rpTodasImagenes" runat="server">
                            <ItemTemplate>
                                <img src='<%# ResolveUrl(Eval("fpro_ruta_imagen").ToString()) %>' 
                                     class="thumbnail" 
                                     onclick="cambiarImagen(this)" 
                                     alt="Imagen del producto" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>

            <!-- Información del Producto -->
            <div class="col-lg-5">
                <div class="product-info">
                    <h1><asp:Label ID="lblNombre" runat="server"></asp:Label></h1>
                    <div class="product-price">$ <asp:Label ID="lblPrecio" runat="server"></asp:Label></div>
                    
                    <div class="product-description">
                        <asp:Label ID="lblDescripcion" runat="server"></asp:Label>
                    </div>

                    <div style="margin-top: 30px;">
                        <strong>Proveedor:</strong> 
                        <asp:Label ID="lblProveedor" runat="server"></asp:Label>
                    </div>
                    <div>
                        <strong>Cantidad disponible:</strong> 
                        <asp:Label ID="lblCantidad" runat="server"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
//@ts-nocheck
        function cambiarImagen(img) {
    document.getElementById('<%= imgPrincipal.ClientID %>').src = img.src;

    // Remover active de todas
    document.querySelectorAll('.thumbnail').forEach(t => t.classList.remove('active'));
    // Activar la seleccionada
    img.classList.add('active');
}
</script>
</asp:Content>
