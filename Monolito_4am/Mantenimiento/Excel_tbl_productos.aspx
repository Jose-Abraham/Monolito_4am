<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="Excel_tbl_productos.aspx.cs" Inherits="Monolito_4am.Mantenimiento.Excel_tbl_productos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .import-card { max-width: 1200px; margin: auto; background: rgba(255,255,255,0.04); border-radius: 20px; padding: 30px; }
        .title { text-align: center; color: #38bdf8; font-size: 28px; margin-bottom: 25px; }
        .custom-input { width: 100%; padding: 12px; border-radius: 12px; background: rgba(255,255,255,0.08); color: white; border: none; }
        .btn { padding: 12px 24px; border: none; border-radius: 12px; font-weight: bold; cursor: pointer; }
        .btn-primary { background: #38bdf8; color: white; }
        .btn-success { background: #22c55e; color: white; }
        .grid { width: 100%; border-collapse: collapse; margin-top: 20px; }
        .grid th { background: rgba(56,189,248,0.2); padding: 12px; text-align: left; }
        .grid td { padding: 12px; border-bottom: 1px solid rgba(255,255,255,0.1); }
    </style>

    <div class="import-card">
        <div class="title">Importar Productos desde Excel / CSV</div>

        <asp:FileUpload ID="fuExcel" runat="server" CssClass="custom-input" 
            accept=".xlsx,.xls,.csv" onchange="validarArchivo(this)" />
        <br /><br />

        <asp:Button ID="btnPrevisualizar" runat="server" Text="Previsualizar Datos" 
            CssClass="btn btn-primary" OnClick="btnPrevisualizar_Click" />

        <asp:Button ID="btnImportar" runat="server" Text="Importar a Base de Datos" 
            CssClass="btn btn-success" OnClick="btnImportar_Click" Style="margin-left:10px;" />

        <asp:Button ID="btnVolver" runat="server" Text="Volver" 
            CssClass="btn" Style="background:#64748b; margin-left:10px;" OnClick="btnVolver_Click" />

        <hr />
        <asp:Label ID="lblMensaje" runat="server" ForeColor="Yellow" />

        <asp:GridView ID="gvPreview" runat="server" CssClass="grid" AutoGenerateColumns="True" GridLines="None"></asp:GridView>
    </div>

    <script type="text/javascript">
// @ts-nocheck
function validarArchivo(input) {
    if (!input || !input.files || input.files.length === 0) return;
    var archivo = input.files[0];
    var extension = archivo.name.toLowerCase().substring(archivo.name.lastIndexOf('.'));
    var permitidos = ['.xlsx', '.xls', '.csv'];
    var maxSize = 50 * 1024 * 1024;

    if (!permitidos.includes(extension)) {
        Swal.fire({ icon: 'error', title: 'Formato Incorrecto', text: 'Solo se permiten .xlsx, .xls o .csv' });
        input.value = '';
        return;
    }
    if (archivo.size > maxSize) {
        Swal.fire({ icon: 'error', title: 'Archivo muy grande', text: 'Máximo 50 MB' });
        input.value = '';
        return;
    }
}
</script>
</asp:Content>
