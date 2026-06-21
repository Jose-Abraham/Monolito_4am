<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="nuevo_foto_producto.aspx.cs" Inherits="Monolito_4am.Mantenimiento.nuevo_foto_producto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   
   <style>
        .form-card {
            max-width: 900px;
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
        .custom-input, .custom-dropdown {
            width: 100%;
            padding: 12px;
            border-radius: 12px;
            border: none;
            background: rgba(255,255,255,0.08);
            color: white;
        }

        .custom-dropdown option {
            background: #ffffff;
            color: #000000;
        }
        .btnGuardar {
            width: 100%;
            padding: 14px;
            background: #0ea5e9;
            border: none;
            border-radius: 12px;
            color: white;
            font-weight: bold;
            margin-top: 20px;
        }
        .preview-img {
            width: 120px;
            height: 120px;
            object-fit: cover;
            border-radius: 10px;
            border: 2px solid #38bdf8;
        }
        .btnEliminarPreview {
            position: absolute;
            top: -10px;
            right: -10px;
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
        <div class="form-title">Nueva Imagen para Producto</div>
        
        <table class="product-table" style="width:100%; border-spacing:0 15px;">
            <tr>
                <td style="width:150px;">Producto</td>
                <td>
                    <asp:DropDownList ID="ddlProducto" runat="server" CssClass="custom-dropdown" 
                        AutoPostBack="true" OnSelectedIndexChanged="ddlProducto_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Foto Principal</td>
                <td>
                    <asp:CheckBox ID="chkEsPrincipal" runat="server" Text=" Marcar como foto principal" 
                        style="color:white; font-size:16px;" />
                </td>
            </tr>
            <tr>
                <td>Imágenes</td>
                <td>
                    <asp:FileUpload ID="fuFotos" runat="server" AllowMultiple="false" CssClass="custom-input" 
                        accept=".png,.jpg,.jpeg" onchange="validarArchivosSeleccionados(this)" />
                    
                    <asp:Button ID="btnPrevisualizar" runat="server" Text="Previsualizar Foto" 
                        CssClass="btnGuardar" Style="margin-top:10px; background:#64748b; width:auto; padding:10px 25px;" 
                        OnClick="btnPrevisualizar_Click" />
                    
                    <div style="margin-top:15px; display:flex; flex-wrap:wrap; gap:12px;" id="previewContainer">
                        <asp:Repeater ID="rpPreview" runat="server" OnItemCommand="rpPreview_ItemCommand">
                            <ItemTemplate>
                                <div style="position:relative; display:inline-block;">
                                    <img src='<%# ResolveUrl(Eval("PreviewUrl").ToString()) %>' class="preview-img" />
                                    <asp:Button ID="btnEliminar" runat="server" Text="X" CssClass="btnEliminarPreview"
                                        CommandName="Eliminar" CommandArgument='<%# Eval("Index") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </td>
            </tr>
        </table>

        <asp:Button ID="btnGuardar" runat="server" Text="Guardar Foto" CssClass="btnGuardar" 
            OnClick="btnGuardar_Click" OnClientClick="return validarFormulario();" />
    </div>

   <script type="text/javascript">
// @ts-nocheck

function validarFormulario() {
    var ddl = document.getElementById('<%= ddlProducto.ClientID %>');
        if (!ddl || ddl.value === "0") {
            Swal.fire('Error', 'Debe seleccionar un producto', 'error');
            return false;
        }
        return true;
    }

    function validarArchivosSeleccionados(input) {
        if (!input || !input.files || input.files.length === 0) return;

        // VERIFICAR SI YA HAY IMAGEN PREVISUALIZADA
        var previewCount = document.querySelectorAll('#previewContainer img').length;
        if (previewCount > 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Solo una imagen permitida',
                text: 'Ya has examinado una foto. Quítala con la "X" para subir otra.',
                confirmButtonText: 'Entendido'
            });
            input.value = ""; // Limpiar selección
            return;
        }

        // Solo permitir 1 archivo
        if (input.files.length > 1) {
            Swal.fire({
                icon: 'warning',
                title: 'Solo una imagen',
                text: 'Ya has examinado una foto. Eliminala para subir la que realmente necesitas.',
                confirmButtonText: 'Entendido'
            });
            input.value = "";
            return;
        }

        var archivo = input.files[0];
        var formatosPermitidos = ['image/png', 'image/jpeg', 'image/jpg'];
        var maxPorImagen = 2 * 1024 * 1024;

        if (formatosPermitidos.indexOf(archivo.type) === -1) {
            Swal.fire('Error', 'Solo se permiten PNG, JPG o JPEG', 'error');
            input.value = "";
            return;
        }

        if (archivo.size > maxPorImagen) {
            Swal.fire('Error', 'La imagen supera 2MB', 'error');
            input.value = "";
            return;
        }
    }
   </script>
</asp:Content>
