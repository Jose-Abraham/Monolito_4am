<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="listar_tbl_fotos_productos.aspx.cs" Inherits="Monolito_4am.Mantenimiento.listar_tbl_fotos_productos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .list-card {
            background: rgba(255,255,255,0.04);
            border: 1px solid rgba(255,255,255,0.08);
            border-radius: 20px;
            padding: 30px;
            backdrop-filter: blur(10px);
            box-shadow: 0 10px 30px rgba(0,0,0,0.25);
        }
        .title { text-align: center; color: #38bdf8; font-size: 28px; font-weight: bold; margin-bottom: 30px; }
        
        .search-container { display: flex; gap: 15px; margin-bottom: 25px; flex-wrap: wrap; }
        .custom-input { 
            flex: 1; min-width: 250px; padding: 12px 16px; border-radius: 12px; 
            border: 1px solid rgba(255,255,255,0.15); background: rgba(255,255,255,0.07); 
            color: white; outline: none; font-size: 15px; 
        }
        .btnBuscar, .btnAgregar { 
            padding: 12px 24px; border: none; border-radius: 12px; 
            font-weight: bold; cursor: pointer; transition: .3s; 
        }
        .btnAgregar { background: linear-gradient(135deg,#22c55e,#16a34a); color: white; }
        .btnBuscar { background: linear-gradient(135deg,#38bdf8,#0ea5e9); color: white; }

        .grid { width: 100%; border-collapse: collapse; }
        .grid th { background: rgba(56,189,248,0.15); color: #38bdf8; padding: 16px; text-align: left; }
        .grid td { padding: 14px 16px; color: white; background: rgba(255,255,255,0.03); border-bottom: 1px solid rgba(255,255,255,0.05); }

        .img-preview { width: 80px; height: 80px; object-fit: cover; border-radius: 8px; border: 2px solid #38bdf8; }
        .btnEditar { background: rgba(34,197,94,0.15); color: #22c55e; border: 1px solid rgba(34,197,94,0.25); padding: 6px 14px; border-radius: 8px; cursor: pointer; }
        .btnEliminar { background: rgba(239,68,68,0.15); color: #ef4444; border: 1px solid rgba(239,68,68,0.25); padding: 6px 14px; border-radius: 8px; cursor: pointer; }
        .paginacion {
            margin-top: 20px;
        }

        .paginacion a,
        .paginacion span {
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

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="list-card">
                <div class="title">Gestión de Imágenes de Productos</div>
                
                <div style="margin-bottom: 20px;">
                    <asp:Button ID="btnNuevaImagen" runat="server" Text="Agregar Nueva Imagen" CssClass="btnAgregar" OnClick="btnNuevaImagen_Click" />
                    <asp:Button ID="btnImportarExcel" runat="server" Text="Importar Excel" 
    CssClass="btnAgregar" Style="background: linear-gradient(135deg, #8b5cf6, #7c3aed);" 
    OnClick="btnImportarExcel_Click" />
                </div>

                <div class="search-container">
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="custom-input" 
                        placeholder="Buscar por ID, producto o nombre de archivo..." />
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btnBuscar" OnClick="btnBuscar_Click" />
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btnBuscar" OnClick="btnLimpiar_Click" style="background: rgba(255,255,255,0.1);" />
                </div>

                <asp:GridView ID="gvImagenes" runat="server" AutoGenerateColumns="False" CssClass="grid" 
                    GridLines="None" DataKeyNames="fpro_id"
                    AllowPaging="true"
                        PageSize="10"
                        OnPageIndexChanging="gvImagenes_PageIndexChanging"

                        PagerStyle-CssClass="paginacion"
                        PagersStyle-HorizontalAlign="Center">
                    <Columns>
                        <asp:BoundField HeaderText="ID" DataField="fpro_id" />
                        <asp:BoundField HeaderText="Producto" DataField="tbl_productos.pro_nombre" />
                        <asp:TemplateField HeaderText="Imagen">
                            <ItemTemplate>
                                <img src='<%# ResolveUrl(Eval("fpro_ruta_imagen").ToString()) %>' class="img-preview" alt="Imagen" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Nombre Archivo" DataField="fpro_nombre_archivo" />
                        <asp:BoundField HeaderText="Fecha Subida" DataField="fpro_fecha_subida" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        <asp:TemplateField HeaderText="Principal">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# (bool?)Eval("fpro_es_principal") == true ? "✅ Sí" : "No" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btnEditar" 
                                    OnClick="btnEditar_Click" CommandArgument='<%# Eval("fpro_id") %>' />
                                
                             <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" CssClass="btnEliminar"
                                UseSubmitBehavior="false"
                                OnClientClick='<%# "return confirmarEliminarImagen(" + Eval("fpro_id") + ");" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

   

<script>
// @ts-nocheck

var Swal;

function confirmarEliminarImagen(id) {

    Swal.fire({
        title: '¿Estás seguro?',
        text: 'Se eliminará esta imagen permanentemente.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then(function (result) {

        if (result.isConfirmed) {

            var form = document.forms[0];

            var target = document.getElementById('__EVENTTARGET');
            if (!target) {
                target = document.createElement('input');
                target.type = 'hidden';
                target.name = '__EVENTTARGET';
                target.id = '__EVENTTARGET';
                form.appendChild(target);
            }

            var argument = document.getElementById('__EVENTARGUMENT');
            if (!argument) {
                argument = document.createElement('input');
                argument.type = 'hidden';
                argument.name = '__EVENTARGUMENT';
                argument.id = '__EVENTARGUMENT';
                form.appendChild(argument);
            }

            target.setAttribute('value', 'btnEliminarFisico_Click');
            argument.setAttribute('value', id.toString());

            form.submit();
        }
    });

    return false;
}
</script>
</asp:Content>
