<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="listar_tbl_proveedor.aspx.cs" Inherits="Monolito_4am.Mantenimiento.listar_tbl_proveedor" %>

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

        .btnEditar { background: rgba(34,197,94,0.15); color: #22c55e; border: 1px solid rgba(34,197,94,0.25); padding: 6px 14px; border-radius: 8px; cursor: pointer; }
        .btnEliminar { background: rgba(239,68,68,0.15); color: #ef4444; border: 1px solid rgba(239,68,68,0.25); padding: 6px 14px; border-radius: 8px; cursor: pointer; }
        .btnEliminarFisico { background: rgba(249,115,22,0.15); color: #f97316; border: 1px solid rgba(249,115,22,0.25); padding: 6px 14px; border-radius: 8px; cursor: pointer; }

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
                <div class="title">Lista de Proveedores</div>
                
                <div style="margin-bottom: 20px;">
                    <asp:Button ID="btnNuevoProveedor" runat="server" Text="Agregar Proveedor" CssClass="btnAgregar" OnClick="btnNuevoProveedor_Click" />

                    <asp:Button ID="btnImportarExcel" runat="server" Text="Importar Excel / CSV" 
                        CssClass="btnAgregar" Style="background: linear-gradient(135deg, #8b5cf6, #7c3aed);" 
                        OnClick="btnImportarExcel_Click" />
                </div>

                <div class="search-container">
                    <asp:TextBox ID="txt_nombre" runat="server" CssClass="custom-input" 
                        placeholder="Buscar por nombre..." MaxLength="100"></asp:TextBox>
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btnBuscar" OnClick="btnBuscar_Click" />
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btnBuscar" 
                        OnClick="btnLimpiar_Click" style="background: rgba(255,255,255,0.1);" />
                </div>

                <asp:GridView ID="gvProveedores" runat="server" AutoGenerateColumns="False" CssClass="grid" 
                    GridLines="None" DataKeyNames="prov_id" OnRowDataBound="gvProveedores_RowDataBound"
                     AllowPaging="true"
                        PageSize="10"
                        OnPageIndexChanging="gvProveedores_PageIndexChanging"

                        PagerStyle-CssClass="paginacion"
                        PagersStyle-HorizontalAlign="Center">
                    <Columns>
                        <asp:BoundField HeaderText="ID" DataField="prov_id" />
                        <asp:BoundField HeaderText="Nombre" DataField="prov_nombre" />
                        <asp:BoundField HeaderText="RUC" DataField="prov_ruc" />
                        <asp:BoundField HeaderText="Teléfono" DataField="prov_telefono" />
                        <asp:BoundField HeaderText="Correo" DataField="prov_correo" />
                        
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlEstado" runat="server" AutoPostBack="true" 
                                    OnSelectedIndexChanged="ddlEstado_SelectedIndexChanged"
                                    CssClass="form-control" style="background:#1e2937; color:white; border:none; padding:6px 10px; border-radius:6px;">
                                    <asp:ListItem Value="A" Text="Activo"></asp:ListItem>
                                    <asp:ListItem Value="I" Text="Inactivo"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btnEditar" 
                                    OnClick="btnEditar_Click" CommandArgument='<%# Eval("prov_id") %>' />
                                
                                <asp:Button ID="btnEliminar" runat="server" Text="Eliminar Lógico" CssClass="btnEliminar" 
                                    OnClientClick='<%# "return confirmarEliminar(" + Eval("prov_id") + ");" %>' />
                                
                                <asp:Button ID="btnEliminarFisico" runat="server" Text="Eliminar Físico" CssClass="btnEliminarFisico" 
                                    OnClientClick='<%# "return confirmarEliminarFisico(" + Eval("prov_id") + ");" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
// @ts-nocheck
function confirmarEliminar(id) {
    Swal.fire({
        title: '¿Estás seguro?',
        text: "Se eliminará lógicamente este proveedor.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sí, eliminar'
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
        text: "!Si algún prodcuto esta relacionado con este proveedor, ese campo quedará null!",
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

</asp:Content>
