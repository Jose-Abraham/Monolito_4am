<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="editar_tbl_productos.aspx.cs" Inherits="Monolito_4am.Mantenimiento.editar_tbl_productos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .form-card {
            max-width: 800px;
            margin: auto;
            background: rgba(255,255,255,0.04);
            border-radius: 20px;
            padding: 30px;
        }

        .form-title {
            text-align: center;
            color: #38bdf8;
            font-size: 28px;
            margin-bottom: 25px;
        }

        .custom-dropdown {
            width: 100%;
            padding: 12px;
            border-radius: 12px;
            border: none;
            background: rgba(255,255,255,0.12) !important;
            color: white !important;
        }

            .custom-dropdown option {
                background: #1e293b !important;
                color: white !important;
                padding: 10px;
            }

        .product-table {
            width: 100%;
            border-spacing: 0 15px;
        }

        .custom-input {
            width: 100%;
            padding: 12px;
            border-radius: 12px;
            border: none;
            background: rgba(255,255,255,0.08);
            color: white;
        }

        .btnGuardar {
            width: 100%;
            padding: 14px;
            background: #f59e0b;
            border: none;
            border-radius: 12px;
            color: white;
            font-weight: bold;
            margin-top: 20px;
        }

        .preview-img {
            width: 100px;
            height: 100px;
            object-fit: cover;
            border-radius: 10px;
            border: 2px solid #38bdf8;
        }

        .btnEliminarPreview {
            position: absolute;
            top: -8px;
            right: -8px;
            background: #ef4444;
            color: white;
            border: none;
            width: 28px;
            height: 28px;
            border-radius: 50%;
            cursor: pointer;
            font-weight: bold;
        }
    </style>

    <div class="form-card">
        <div class="form-title">Editar Producto</div>
        <table class="product-table">
            <tr>
                <td>Código</td>
                <td>
                    <asp:TextBox ID="txt_codigo" runat="server" MaxLength="50" CssClass="custom-input" /></td>
            </tr>
            <tr>
                <td>Nombre</td>
                <td>
                    <asp:TextBox ID="txt_nombre" runat="server" MaxLength="100" CssClass="custom-input" /></td>
            </tr>
            <tr>
                <td>Descripción</td>
                <td>
                    <asp:TextBox ID="txt_descripcion" runat="server" MaxLength="150" TextMode="MultiLine" CssClass="custom-input" /></td>
            </tr>
            <tr>
                <td>Cantidad</td>
                <td>
                    <asp:TextBox ID="txt_cantidad" runat="server" CssClass="custom-input" onkeypress="return soloNumeros(event)" /></td>
            </tr>
            <tr>
                <td>Precio</td>
                <td>
                    <asp:TextBox ID="txt_precio" runat="server" CssClass="custom-input" onkeypress="return soloDecimal(event)" /></td>
            </tr>
            <tr>
                <td>Proveedor</td>
                <td>
                    <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="custom-dropdown"></asp:DropDownList></td>
            </tr>

            <!-- IMÁGENES ACTUALES -->
            <tr>
                <td>Imágenes actuales</td>
                <td>
                    <asp:Repeater ID="rpImagenes" runat="server">
                        <ItemTemplate>
                            <div style="position: relative; display: inline-block; margin: 5px;">
                                <img src='<%# ResolveUrl(Eval("fpro_ruta_imagen").ToString()) %>' class="preview-img" />
                                <button type="button" onclick="eliminarImagenActual(<%# Eval("fpro_id") %>, this)"
                                    style="position: absolute; top: -8px; right: -8px; background: #ef4444; color: white; border: none; width: 28px; height: 28px; border-radius: 50%; cursor: pointer; font-weight: bold;">
                                    X
                               
                                </button>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:HiddenField ID="hfImagenesEliminar" runat="server" />
                </td>
            </tr>

            <!-- NUEVAS IMÁGENES (Sistema C# como en Nuevo) -->
            <tr>
                <td>Nuevas imágenes</td>
                <td>
                    <asp:FileUpload ID="fuFotos" runat="server" AllowMultiple="true" CssClass="custom-input"
                        accept=".png,.jpg,.jpeg" onchange="validarArchivosSeleccionados(this)" />

                    <asp:Button ID="btnPrevisualizar" runat="server" Text="Previsualizar Imágenes"
                        CssClass="btnGuardar" Style="margin-top: 10px; background: #64748b; width: auto; padding: 10px 20px;"
                        OnClick="btnPrevisualizar_Click" />

                    <div id="previewImagenes" style="margin-top: 15px; display: flex; flex-wrap: wrap; gap: 10px;">
                        <asp:Repeater ID="rpPreview" runat="server" OnItemCommand="rpPreview_ItemCommand">
                            <ItemTemplate>
                                <div style="position: relative; display: inline-block; margin: 5px;">
                                    <img src='<%# ResolveUrl(Eval("PreviewUrl").ToString()) %>' class="preview-img" />
                                    <asp:Button ID="btnEliminar" runat="server" Text="X" CssClass="btnEliminarPreview"
                                        CommandName="Eliminar" CommandArgument='<%# Eval("Index") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div style="margin-top: 15px;">
                        <button type="button" onclick="limpiarTodo()"
                            style="background: #ef4444; border: none; color: white; padding: 10px 15px; border-radius: 10px; cursor: pointer; font-weight: bold;">
                            Limpiar Todo
                       
                        </button>
                    </div>
                </td>
            </tr>
        </table>

        <asp:Button ID="btnEditar" runat="server" Text="Actualizar Producto" CssClass="btnGuardar"
            OnClick="btnEditar_Click" OnClientClick="return validarFormulario();" />

        <asp:Button ID="btnLimpiarServidor" runat="server"
            Style="display: none;"
            OnClick="btnLimpiarServidor_Click" />
    </div>

    <script>
// @ts-nocheck

function mostrarError(mensaje) {

    Swal.fire({
        icon: 'error',
        title: 'Error',
        text: mensaje
    });
}

function soloNumeros(e) {

    let key = e.keyCode || e.which;

    let teclado =
        String.fromCharCode(key);

    let numeros = "0123456789";

    if (numeros.indexOf(teclado) == -1 &&
        key != 8 &&
        key != 46) {

        return false;
    }
}

function soloDecimal(e) {

    let key = e.keyCode || e.which;

    let teclado =
        String.fromCharCode(key);

    let numeros = "0123456789,";

    // BLOQUEAR EL PUNTO
    if (teclado == ".") {

        mostrarError(
            "Los precios deben usar coma decimal. Ejemplo: 8,50");

        return false;
    }

    // SOLO UNA COMA
    if (teclado == ",") {

        if (e.target.value.includes(",")) {
            return false;
        }
    }

    if (numeros.indexOf(teclado) == -1 &&
        key != 8) {

        return false;
    }
}

document.addEventListener(
    "DOMContentLoaded",
    function () {

    const txtCodigo =
        document.getElementById(
            '<%= txt_codigo.ClientID %>');

        const txtNombre =
            document.getElementById(
            '<%= txt_nombre.ClientID %>');

    const txtCantidad =
        document.getElementById(
            '<%= txt_cantidad.ClientID %>');

        const txtPrecio =
            document.getElementById(
            '<%= txt_precio.ClientID %>');

    txtCodigo.addEventListener(
        "input",
        function () {

        this.value =
            this.value.replace(
                /[^a-zA-Z0-9_]/g,
                '');

    });

    txtNombre.addEventListener(
        "input",
        function () {

        this.value =
            this.value.replace(
                /[^a-zA-ZñÑáéíóúÁÉÍÓÚ ]/g,
                '');

    });

    txtCantidad.addEventListener(
        "input",
        function () {

        if (parseInt(this.value) > 1000) {

            mostrarError(
                "El límite máximo por producto es 1000.");

            this.value = "";
        }
    });

        txtPrecio.addEventListener(
            "input",
            function () {

                let valor =
                    this.value.replace(",", ".");

                if (parseFloat(valor) > 1000000) {

                    mostrarError(
                        "El precio no puede superar 1 millón.");

                    this.value = "";
                }
            });
});

function validarFormulario() {

    let codigo =
        document.getElementById(
            '<%= txt_codigo.ClientID %>')
            .value.trim();

    let nombre =
        document.getElementById(
            '<%= txt_nombre.ClientID %>')
            .value.trim();

    let descripcion =
        document.getElementById(
            '<%= txt_descripcion.ClientID %>')
            .value.trim();

    let cantidad =
        document.getElementById(
            '<%= txt_cantidad.ClientID %>')
            .value.trim();

    let precio =
        document.getElementById(
            '<%= txt_precio.ClientID %>')
            .value.trim();

    let proveedor =
        document.getElementById(
            '<%= ddlProveedor.ClientID %>')
            .value;

    let regexCodigo = /^pro_\d+$/;

    if (codigo == "") {

        mostrarError(
            "Ingrese el código.");

        return false;
    }

    if (!regexCodigo.test(codigo)) {

        mostrarError(
            "Formato inválido. Ejemplo: pro_1");

        return false;
    }

    if (codigo.length > 50) {

        mostrarError(
            "Máximo 50 caracteres.");

        return false;
    }

    if (nombre == "") {

        mostrarError(
            "Ingrese el nombre.");

        return false;
    }

    if (nombre.length < 3) {

        mostrarError(
            "Nombre demasiado corto.");

        return false;
    }

    if (nombre.length > 100) {

        mostrarError(
            "Máximo 100 caracteres.");

        return false;
    }

    if (!/^[a-zA-ZñÑáéíóúÁÉÍÓÚ ]+$/
        .test(nombre)) {

        mostrarError(
            "El nombre contiene caracteres inválidos.");

        return false;
    }

    if (descripcion == "") {

        mostrarError(
            "Ingrese descripción.");

        return false;
    }

    if (descripcion.length > 150) {

        mostrarError(
            "Descripción demasiado larga.");

        return false;
    }

    if (cantidad == "") {

        mostrarError(
            "Ingrese cantidad.");

        return false;
    }

    if (!/^\d+$/.test(cantidad)) {

        mostrarError(
            "Solo números enteros positivos.");

        return false;
    }

    if (parseInt(cantidad) <= 0) {

        mostrarError(
            "La cantidad debe ser mayor a 0.");

        return false;
    }

    if (parseInt(cantidad) > 1000) {

        mostrarError(
            "Límite máximo alcanzado.");

        return false;
    }

    if (precio == "") {

        mostrarError(
            "Ingrese precio.");

        return false;
    }

    if (precio.includes(".")) {

        mostrarError(
            "Use coma decimal. Ejemplo: 8,50");

        return false;
    }

    // CONVERTIR COMA A PUNTO PARA VALIDAR
    let precioValidar =
        precio.replace(",", ".");

    if (isNaN(precioValidar)) {

        mostrarError(
            "Precio inválido.");

        return false;
    }

    if (parseFloat(precioValidar) <= 0) {

        mostrarError(
            "El precio debe ser mayor a 0.");

        return false;
    }

    if (parseFloat(precioValidar) > 1000000) {

        mostrarError(
            "Precio demasiado alto.");

        return false;
    }

    if (proveedor == "" ||
        proveedor == "0") {

        mostrarError(
            "Seleccione un proveedor.");

        return false;
    }

    return true;
}

let imagenesSeleccionadas = [];

function validarImagenes(input) {

    const preview =
        document.getElementById(
            "previewImagenes");

    preview.innerHTML = "";

    imagenesSeleccionadas = [];

    const archivos =
        Array.from(input.files);

    const formatosPermitidos = [
        'image/png',
        'image/jpeg',
        'image/jpg'
    ];

    const maxPorImagen =
        2 * 1024 * 1024;

    const maxTotal =
        10 * 1024 * 1024;

    let total = 0;

    for (let i = 0;
        i < archivos.length;
        i++) {

        let archivo =
            archivos[i];

        if (!formatosPermitidos
            .includes(archivo.type)) {

            Swal.fire(
                'Error',
                'Formato inválido. Solo PNG/JPG/JPEG',
                'error');

            input.value = "";
            preview.innerHTML = "";

            return;
        }

        if (archivo.size >
            maxPorImagen) {

            Swal.fire(
                'Error',
                `La imagen ${archivo.name} supera 2MB`,
                'error');

            input.value = "";
            preview.innerHTML = "";

            return;
        }

        total += archivo.size;
    }

    if (total > maxTotal) {

        Swal.fire(
            'Error',
            'Las imágenes superan 10MB',
            'error');

        input.value = "";
        preview.innerHTML = "";

        return;
    }

    imagenesSeleccionadas = archivos;

    renderizarPreview();
}

function renderizarPreview() {

    const preview =
        document.getElementById(
            "previewImagenes");

    preview.innerHTML = "";

    imagenesSeleccionadas.forEach(
        (archivo, index) => {

        let reader =
            new FileReader();

        reader.onload =
            function (e) {

            let contenedor =
                document.createElement("div");

            contenedor.style.position =
                "relative";

            contenedor.innerHTML = `

                <img
                    src="${e.target.result}"
                    style="
                    width:100px;
                    height:100px;
                    object-fit:cover;
                    border-radius:10px;
                    border:2px solid #38bdf8;">

                <button
                    type="button"
                    onclick="eliminarImagen(${index})"
                    style="
                    position:absolute;
                    top:-8px;
                    right:-8px;
                    background:#ef4444;
                    color:white;
                    border:none;
                    width:25px;
                    height:25px;
                    border-radius:50%;
                    cursor:pointer;
                    font-weight:bold;">

                    X

                </button>
            `;

            preview.appendChild(
                contenedor);
        };

        reader.readAsDataURL(
            archivo);
    });

    actualizarInputFiles();
}

function eliminarImagen(index) {

    imagenesSeleccionadas.splice(
        index,
        1);

    renderizarPreview();

    Swal.fire({
        icon: 'success',
        title: 'Imagen eliminada',
        timer: 1000,
        showConfirmButton: false
    });
}

function limpiarImagenes() {

    imagenesSeleccionadas = [];

    document.getElementById(
        '<%= fuFotos.ClientID %>')
        .value = "";

    document.getElementById(
        "previewImagenes")
        .innerHTML = "";

    Swal.fire({
        icon: 'success',
        title: 'Previsualización limpiada',
        timer: 1200,
        showConfirmButton: false
    });
}

function actualizarInputFiles() {

    const dataTransfer =
        new DataTransfer();

    imagenesSeleccionadas.forEach(
        file => {

        dataTransfer.items.add(file);

    });

    document.getElementById(
        '<%= fuFotos.ClientID %>')
        .files =
        dataTransfer.files;
}

let imagenesAEliminar = [];

function eliminarImagenActual(fpro_id, boton) {

    Swal.fire({
        title: '¿Eliminar imagen?',
        text: 'La imagen será eliminada al guardar.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.isConfirmed) {

            imagenesAEliminar.push(fpro_id);

            document.getElementById(
                '<%= hfImagenesEliminar.ClientID %>'
            ).value =
                imagenesAEliminar.join(',');

            // OCULTAR COMPLETAMENTE
            boton.parentElement.remove();

            Swal.fire({
                icon: 'success',
                title: 'Imagen marcada para eliminar',
                timer: 1200,
                showConfirmButton: false
            });
        }
    });
}

function validarArchivosSeleccionados(input) {
    if(input.files.length === 0) {

        Swal.fire({
            icon: 'warning',
            title: 'No seleccionó imágenes'
        });

        return;
    }
    const formatosPermitidos = ['image/png', 'image/jpeg', 'image/jpg'];
    const maxPorImagen = 2 * 1024 * 1024;
    const maxTotal = 10 * 1024 * 1024;
    let total = 0;

    for (let i = 0; i < input.files.length; i++) {
        let archivo = input.files[i];
        if (!formatosPermitidos.includes(archivo.type)) {
            Swal.fire({ icon: 'error', title: 'Formato inválido', text: 'Solo PNG, JPG o JPEG.' });
            input.value = "";
            return;
        }
        if (archivo.size > maxPorImagen) {
            Swal.fire({ icon: 'error', title: 'Archivo grande', text: `La imagen "${archivo.name}" supera 2MB.` });
            input.value = "";
            return;
        }
        total += archivo.size;
    }
    if (total > maxTotal) {
        Swal.fire({ icon: 'error', title: 'Límite excedido', text: 'Total supera 10MB.' });
        input.value = "";
        return;
    }
}

function limpiarTodo() {

    const preview =
        document.getElementById("previewImagenes");

    // VALIDAR SI NO HAY IMÁGENES
    if (preview.innerHTML.trim() === "") {

        Swal.fire({
            icon: 'warning',
            title: 'No hay imágenes que limpiar'
        });

        return;
    }

    imagenesSeleccionadas = [];

    preview.innerHTML = "";

    const fu =
        document.getElementById('<%= fuFotos.ClientID %>');

    fu.value = "";

    // LIMPIAR COMPLETAMENTE INPUT
    fu.disabled = true;

    setTimeout(() => {

        fu.disabled = false;

    }, 100);

    // LIMPIAR SERVIDOR
    document.getElementById(
        '<%= btnLimpiarServidor.ClientID %>'
    ).click();

    Swal.fire({
        icon: 'success',
        title: 'Previsualización limpiada',
        timer: 1200,
        showConfirmButton: false
    });
}

function limpiarFileUpload() {

    const fu =
        document.getElementById('<%= fuFotos.ClientID %>');

    if (fu) {

        fu.value = '';

        fu.disabled = true;

        setTimeout(() => {

            fu.disabled = false;

        }, 100);
    }
}

function ocultarImagenesMarcadas() {

    const hidden =
        document.getElementById(
            '<%= hfImagenesEliminar.ClientID %>'
        );

    if (!hidden || hidden.value.trim() === "")
        return;

    const ids =
        hidden.value.split(',');

    ids.forEach(id => {

        const boton =
            document.querySelector(
                `button[onclick*="eliminarImagenActual(${id}"]`
            );

        if (boton &&
            boton.parentElement) {

            boton.parentElement.remove();
        }
    });
}

// EJECUTAR AL CARGAR
document.addEventListener(
    "DOMContentLoaded",
    function () {

        ocultarImagenesMarcadas();
    });


if (typeof (Sys) !== "undefined") {

    Sys.WebForms.PageRequestManager
        .getInstance()
        .add_endRequest(function () {

            ocultarImagenesMarcadas();
        });
}
    </script>

</asp:Content>
