
<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master"
    AutoEventWireup="true"
    CodeBehind="listar_tbl_productos.aspx.cs"
    Inherits="Monolito_4am.Mantenimiento.listar_tbl_productos" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

<style>

.list-card{
    background: rgba(255,255,255,0.04);
    border:1px solid rgba(255,255,255,0.08);
    border-radius:20px;
    padding:30px;
    backdrop-filter:blur(10px);
    box-shadow:0 10px 30px rgba(0,0,0,0.25);
}

.title{
    text-align:center;
    color:#38bdf8;
    font-size:28px;
    font-weight:bold;
    margin-bottom:30px;
}

.search-container{
    display:flex;
    gap:15px;
    margin-bottom:25px;
    flex-wrap:wrap;
}

.custom-input{
    flex:1;
    min-width:250px;
    padding:12px 16px;
    border-radius:12px;
    border:1px solid rgba(255,255,255,0.15);
    background:rgba(255,255,255,0.07);
    color:white;
    outline:none;
    font-size:15px;
}

.btnBuscar,
.btnAgregar{
    padding:12px 24px;
    border:none;
    border-radius:12px;
    font-weight:bold;
    cursor:pointer;
}

.btnBuscar{
    background:linear-gradient(135deg,#38bdf8,#0ea5e9);
    color:white;
}

.btnAgregar{
    background:linear-gradient(135deg,#22c55e,#16a34a);
    color:white;
}

.grid{
    width:100%;
    border-collapse:collapse;
}

.grid th{
    background:rgba(56,189,248,0.15);
    color:#38bdf8;
    padding:16px;
    text-align:left;
}

.grid td{
    padding:14px 16px;
    color:white;
    background:rgba(255,255,255,0.03);
    border-bottom:1px solid rgba(255,255,255,0.05);
}

.btnEditar{
    background:rgba(34,197,94,0.15);
    color:#22c55e;
    border:1px solid rgba(34,197,94,0.25);
    padding:6px 14px;
    border-radius:8px;
    cursor:pointer;
}

.btnEliminar{
    background:rgba(239,68,68,0.15);
    color:#ef4444;
    border:1px solid rgba(239,68,68,0.25);
    padding:6px 14px;
    border-radius:8px;
    cursor:pointer;
}

.product-img{
    width:70px;
    height:70px;
    object-fit:contain;
    border-radius:10px;
}

.slider{
    width:90px;
    height:90px;
    position:relative;
    overflow:hidden;
    border-radius:12px;
    background:#1e293b;
}

.slide-img{
    width:100%;
    height:100%;
    object-fit:contain;
    position:absolute;
    top:0;
    left:0;
    opacity:0;
    transition:opacity 0.5s ease;
    background:#1e293b;
}

/* SOLO LA ACTIVA SE VE */
.slide-img.active{
    opacity:1;
}

.btnEliminarFisico { 
    background:rgba(249,115,22,0.15); 
    color:#f97316; 
    border:1px solid rgba(249,115,22,0.25); 
    padding:6px 14px; 
    border-radius:8px; 
    cursor:pointer; 
}

.paginacion {
    margin-top: 20px;
}

.paginacion a, .paginacion span {
    padding: 8px 14px;
    margin: 0 4px;
    border-radius: 8px;
    text-decoration: none;
    color: #38bdf8;
    border: 1px solid rgba(56,189,248,0.3);
}

.paginacion a:hover {
    background: rgba(56,189,248,0.15);
}

</style>

<asp:UpdatePanel ID="UpdatePanel1"
    runat="server">

<ContentTemplate>

<div class="list-card">

<div class="title">
    Lista de Productos
</div>

<div style="margin-bottom:20px;">

    <asp:Button
        ID="btnNuevoProducto"
        runat="server"
        Text="Agregar Producto"
        CssClass="btnAgregar"
        OnClick="btnNuevoProducto_Click" />

    <asp:Button ID="btnImportarExcel" runat="server" Text="Importar Excel" 
        CssClass="btnAgregar" Style="background: linear-gradient(135deg, #8b5cf6, #7c3aed);" 
        OnClick="btnImportarExcel_Click" />

</div>

<div class="search-container">

    <asp:TextBox
    ID="txt_nombre"
    runat="server"
    CssClass="custom-input"
    placeholder="Buscar por ID, código, nombre, descripción, proveedor..."
    MaxLength="100">
    </asp:TextBox>

    <asp:Button
        ID="btnBuscar"
        runat="server"
        Text="Buscar"
        CssClass="btnBuscar"
        OnClick="btnBuscar_Click" />

    <asp:Button
        ID="btnLimpiar"
        runat="server"
        Text="Limpiar"
        CssClass="btnBuscar"
        OnClick="btnLimpiar_Click"
        Style="background: rgba(255,255,255,0.1);" />

</div>

<asp:GridView
    ID="gvProductos"
    runat="server"
    AutoGenerateColumns="False"
    CssClass="grid"
    GridLines="None"
    DataKeyNames="pro_id"
    OnRowDataBound="gvProductos_RowDataBound"
    
    AllowPaging="true"
    PageSize="10"
    OnPageIndexChanging="gvProductos_PageIndexChanging"
    
    PagersStyle-HorizontalAlign="Center"
    PagerStyle-CssClass="paginacion">

<Columns>

<asp:BoundField
    HeaderText="ID"
    DataField="pro_id" />

<asp:BoundField
    HeaderText="Código"
    DataField="pro_codigo" />

<asp:BoundField
    HeaderText="Nombre"
    DataField="pro_nombre" />

<asp:BoundField
    HeaderText="Descripción"
    DataField="pro_descripcion" />

<asp:BoundField
    HeaderText="Cantidad"
    DataField="pro_cantidad" />

<asp:BoundField
    HeaderText="Precio"
    DataField="pro_precio" />

<asp:TemplateField HeaderText="Proveedor">
<ItemTemplate>

<%# Eval("tbl_proveedor.prov_nombre") %>

</ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Fotos">
<ItemTemplate>

<div class="slider">

<asp:Repeater
    ID="rpFotos"
    runat="server">

<ItemTemplate>

<img
    src='<%# ResolveUrl(Eval("fpro_ruta_imagen").ToString()) %>'
    class="slide-img" />

</ItemTemplate>

</asp:Repeater>

</div>

</ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Estado">
<ItemTemplate>

<asp:DropDownList
    ID="ddlEstado"
    runat="server"
    AutoPostBack="true"
    OnSelectedIndexChanged="ddlEstado_SelectedIndexChanged">

    <asp:ListItem Value="A">
        Activo
    </asp:ListItem>

    <asp:ListItem Value="I">
        Inactivo
    </asp:ListItem>

</asp:DropDownList>

</ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Acciones">
    <ItemTemplate>
        <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btnEditar" 
            OnClick="btnEditar_Click" CommandArgument='<%# Eval("pro_id") %>' />
        <asp:Button ID="btnEliminar" runat="server" Text="Eliminar Lógico" CssClass="btnEliminar" 
            OnClientClick='<%# "return confirmarEliminar(" + Eval("pro_id") + ");" %>' />
        <asp:Button ID="btnEliminarFisico" runat="server" Text="Eliminar Físico" CssClass="btnEliminarFisico" 
            OnClientClick='<%# "return confirmarEliminarFisico(" + Eval("pro_id") + ");" %>' />
    </ItemTemplate>
</asp:TemplateField>

</Columns>

</asp:GridView>

<script>
// @ts-nocheck

function iniciarSliders() {

    const sliders =
        document.querySelectorAll(".slider");

    sliders.forEach(slider => {

        const imagenes =
            slider.querySelectorAll(".slide-img");

        // LIMPIAR TODAS
        imagenes.forEach(img => {
            img.classList.remove("active");
        });

        // SI NO HAY IMÁGENES
        if (imagenes.length === 0) {
            return;
        }

        // MOSTRAR PRIMERA
        imagenes[0].classList.add("active");

        // SI SOLO HAY 1
        // NO HACER TRANSICIÓN
        if (imagenes.length === 1) {
            return;
        }

        let index = 0;

        // EVITAR DUPLICAR INTERVALOS
        if (slider.dataset.intervalo === "true") {
            return;
        }

        slider.dataset.intervalo = "true";

        setInterval(() => {

            imagenes[index]
                .classList.remove("active");

            index++;

            if (index >= imagenes.length) {
                index = 0;
            }

            imagenes[index]
                .classList.add("active");

        }, 3000);

    });
}

// PRIMERA CARGA
document.addEventListener(
    "DOMContentLoaded",
    iniciarSliders);

// CUANDO UPDATEPANEL RECARGA
if (typeof(Sys) !== "undefined") {

    Sys.WebForms.PageRequestManager
        .getInstance()
        .add_endRequest(function () {

            iniciarSliders();

        });
}

function confirmarEliminar(id) {
    Swal.fire({
        title: '¿Estás seguro?',
        text: "Se eliminará lógicamente este producto.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sí, eliminar lógico'
    }).then((result) => {
        if (result.isConfirmed) {
            __doPostBack('btnEliminar_Click', id.toString());
        }
    });
    return false;
}

function confirmarEliminarFisico(id) {
    Swal.fire({
        title: '¿Eliminar permanentemente?',
        text: "Esta acción no se puede deshacer. Se borrarán todas las imágenes.",
        icon: 'error',
        showCancelButton: true,
        confirmButtonColor: '#f97316',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sí, eliminar FÍSICAMENTE'
    }).then((result) => {
        if (result.isConfirmed) {
            __doPostBack('btnEliminarFisico_Click', id.toString());
        }
    });
    return false;
}
</script>

</div>

</ContentTemplate>
</asp:UpdatePanel>

</asp:Content>

