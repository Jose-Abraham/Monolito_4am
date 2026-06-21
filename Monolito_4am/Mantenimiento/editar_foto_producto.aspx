<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="editar_foto_producto.aspx.cs" Inherits="Monolito_4am.Mantenimiento.editar_foto_producto" %>
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
            background: #1e293b;
            color: #f8fafc;
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
            width: 180px;
            height: 180px;
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
        .current-image {
            text-align: center;
            margin-bottom: 20px;
        }
    </style>

    <div class="form-card">
        <div class="form-title">Editar Imagen del Producto</div>
        
        <table class="product-table" style="width:100%; border-spacing:0 15px;">
            <tr>
                <td style="width:150px;">Producto</td>
                <td>
                    <asp:Label ID="lblProductoNombre" runat="server" CssClass="form-control" 
                        style="background:rgba(255,255,255,0.1); padding:12px; border-radius:8px; display:block;"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Imagen Actual</td>
                <td>
                    <div class="current-image">
                        <asp:Image ID="imgActual" runat="server" CssClass="preview-img" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>Nueva Imagen</td>
                <td>
                    <asp:FileUpload ID="fuFotos" runat="server" AllowMultiple="false" CssClass="custom-input" 
                        accept=".png,.jpg,.jpeg" onchange="validarArchivosSeleccionados(this)" />
                    
                    <asp:Button ID="btnPrevisualizar" runat="server" Text="Previsualizar Nueva Imagen" 
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
            <tr>
                <td>Foto Principal</td>
                <td>
                    <asp:CheckBox ID="chkEsPrincipal" runat="server" Text=" Marcar como foto principal" 
                        style="color:white; font-size:16px;" />
                </td>
            </tr>
        </table>

        <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" CssClass="btnGuardar" 
            OnClick="btnGuardar_Click" OnClientClick="return validarFormulario();" />
        
        <asp:Button ID="btnVolver" runat="server" Text="Volver" CssClass="btnGuardar" 
            Style="background:#6b7280; margin-top:10px;" OnClick="btnVolver_Click" />
    </div>

    <script type="text/javascript">
// @ts-nocheck

function validarFormulario() {
    return true; // Ya validamos en servidor
}

function validarArchivosSeleccionados(input) {
    if (!input || !input.files || input.files.length === 0) return;

    var previewCount = document.querySelectorAll('#previewContainer img').length;
    if (previewCount > 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Solo una imagen',
            text: 'Ya tienes una imagen en previsualizacion. Quitala primero.',
        });
        input.value = "";
        return;
    }

    var archivo = input.files[0];
    var formatos = ['image/png', 'image/jpeg', 'image/jpg'];
    var maxSize = 2 * 1024 * 1024;

    if (formatos.indexOf(archivo.type) === -1) {
        Swal.fire('Error', 'Solo PNG, JPG o JPEG', 'error');
        input.value = "";
        return;
    }
    if (archivo.size > maxSize) {
        Swal.fire('Error', 'La imagen no debe superar 2MB', 'error');
        input.value = "";
        return;
    }
}
</script>
</asp:Content>
