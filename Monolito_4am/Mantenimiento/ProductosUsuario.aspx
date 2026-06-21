<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="ProductosUsuario.aspx.cs" Inherits="Monolito_4am.Mantenimiento.ProductosUsuario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .productos-section {
            padding: 30px 15px;
        }

        .section-title {
            text-align: center;
            color: #38bdf8;
            font-size: 2rem;
            margin-bottom: 35px;
            text-shadow: 0 0 15px rgba(56,189,248,0.3);
        }

        /* CENTRAR FILAS */
        .row {
            justify-content: center;
        }

        /* TARJETA */
        .product-card {
            background: rgba(30, 41, 59, 0.97);
            border-radius: 16px;
            overflow: hidden;
            border: 1px solid rgba(56,189,248,0.15);
            transition: all .3s ease;
            height: 100%;
            display: flex;
            flex-direction: column;
            box-shadow: 0 8px 20px rgba(0,0,0,.25);
        }

            .product-card:hover {
                transform: translateY(-5px);
                box-shadow: 0 12px 30px rgba(56,189,248,.25);
                border-color: rgba(56,189,248,.4);
            }

        /* CARRUSEL */
        .product-carousel {
            position: relative;
            width: 100%;
            height: 170px;
            overflow: hidden;
            background: #0f172a;
        }

        .product-images {
            display: flex;
            height: 100%;
            transition: transform .5s ease;
        }

        .product-img {
            min-width: 100%;
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        /* BOTONES DEL CARRUSEL */
        .carousel-btn {
            position: absolute;
            top: 50%;
            transform: translateY(-50%);
            width: 32px;
            height: 32px;
            border: none;
            border-radius: 50%;
            background: rgba(0,0,0,.55);
            color: white;
            cursor: pointer;
            z-index: 10;
            font-size: 18px;
            transition: .3s;
        }

            .carousel-btn:hover {
                background: rgba(0,0,0,.85);
            }

            .carousel-btn.prev {
                left: 8px;
            }

            .carousel-btn.next {
                right: 8px;
            }

        /* INFORMACIÓN */
        .product-info {
            padding: 12px;
            text-align: center;
            display: flex;
            flex-direction: column;
            justify-content: center;
        }

        /* NOMBRE */
        .product-name {
            font-size: 0.95rem;
            font-weight: 700;
            color: white;
            min-height: 40px;
            margin-bottom: 8px;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        /* PRECIO */
        .product-price {
            color: #38bdf8;
            font-size: 1.2rem;
            font-weight: 800;
            margin-bottom: 10px;
        }

        /* BOTÓN */
        .btn-comprar {
            background: linear-gradient(135deg,#22c55e,#16a34a);
            color: white;
            border: none;
            border-radius: 40px;
            padding: 8px;
            font-size: 0.85rem;
            font-weight: 700;
            width: 100%;
            transition: .3s;
        }

            .btn-comprar:hover {
                transform: scale(1.03);
            }

        /* ESPACIADO ENTRE TARJETAS */
        .col-12.col-sm-6.col-md-4.col-lg-3 {
            padding: 10px;
        }

        /* TABLET */
        @media (max-width: 991px) {
            .product-carousel {
                height: 160px;
            }
        }

        /* MÓVIL */
        @media (max-width: 768px) {

            .section-title {
                font-size: 1.7rem;
            }

            .product-carousel {
                height: 150px;
            }

            .product-name {
                font-size: 0.9rem;
            }

            .product-price {
                font-size: 1.1rem;
            }

            .btn-comprar {
                font-size: 0.8rem;
            }
        }
    </style>

    <div class="productos-section">
        <h2 class="section-title">Nuestros Productos</h2>

        <div class="row">
            <asp:Repeater ID="rpProductos" runat="server" OnItemDataBound="rpProductos_ItemDataBound">
                <ItemTemplate>
                    <div class="col-12 col-sm-6 col-md-4 col-lg-3 mb-4">
                        <div class="product-card">
                            <!-- Carrusel por producto -->
                            <div class="product-carousel">
                                <div class="product-images" id='carousel_<%# Eval("pro_id") %>'>

                                    <asp:Repeater ID="rpImagenesProducto" runat="server">
                                        <ItemTemplate>

                                            <img src='<%# ResolveUrl(Eval("fpro_ruta_imagen").ToString()) %>'
                                                class="product-img"
                                                alt="Producto"
                                                onerror="this.src='<%= ResolveUrl("~/Sources/Productos/no-image.png") %>';" />

                                        </ItemTemplate>
                                    </asp:Repeater>

                                </div>

                                <button type="button"
                                    class="carousel-btn prev"
                                    onclick="moveProductCarousel(<%# Eval("pro_id") %>, -1); return false;">
                                    ‹
                                </button>

                                <button type="button"
                                    class="carousel-btn next"
                                    onclick="moveProductCarousel(<%# Eval("pro_id") %>, 1); return false;">
                                    ›
                                </button>
                            </div>

                            <div class="product-info">
                                <div class="product-name"><%# Eval("pro_nombre") %></div>
                                <div class="product-price">$ <%# Eval("pro_precio", "{0:0.00}") %></div>
                                <button type="button"
                                    class="btn-comprar"
                                    onclick="verProducto(<%# Eval("pro_id") %>); return false;">
                                    Ver Detalles
                                </button>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <script>
// @ts-nocheck
function moveProductCarousel(proId, direction) {
    const track = document.getElementById('carousel_' + proId);
    if (!track) return;

    const images = track.getElementsByTagName('img');
    if (images.length <= 1) return;

    let current = track.dataset.current ? parseInt(track.dataset.current) : 0;
    current = (current + direction + images.length) % images.length;

    track.style.transform = `translateX(-${current * 100}%)`;
    track.dataset.current = current;
}

// Auto-slide para cada carrusel
setInterval(() => {
    document.querySelectorAll('.product-images').forEach(track => {
        const images = track.getElementsByTagName('img');
        if (images.length > 1) {
            let current = track.dataset.current ? parseInt(track.dataset.current) : 0;
            current = (current + 1) % images.length;
            track.style.transform = `translateX(-${current * 100}%)`;
            track.dataset.current = current;
        }
    });
}, 5000);

function verProducto(id) {
    window.location.href = 'DetalleProducto.aspx?id=' + id;
}
</script>
</asp:Content>
