<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="editar_tbl_proveedor.aspx.cs" Inherits="Monolito_4am.Mantenimiento.editar_tbl_proveedor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
    .form-card {
        max-width: 700px;
        margin: auto;
        background: rgba(255,255,255,0.04);
        border: 1px solid rgba(255,255,255,0.08);
        border-radius: 24px;
        padding: 35px;
        backdrop-filter: blur(10px);
        box-shadow: 0 10px 30px rgba(0,0,0,0.25);
    }

    .title {
        text-align: center;
        color: #38bdf8;
        font-size: 30px;
        font-weight: bold;
        margin-bottom: 35px;
    }

    .form-group {
        margin-bottom: 22px;
    }

    .label {
        display: block;
        margin-bottom: 8px;
        color: #cbd5e1;
        font-weight: 600;
    }

    .custom-input {
        width: 100%;
        padding: 14px 16px;
        border-radius: 12px;
        border: 1px solid rgba(255,255,255,0.15);
        background: rgba(255,255,255,0.07);
        color: white;
        outline: none;
        font-size: 15px;
    }

    .btnGuardar {
        width: 100%;
        padding: 14px;
        border: none;
        border-radius: 12px;
        background: linear-gradient(135deg,#f59e0b,#d97706);
        color: white;
        font-size: 16px;
        font-weight: bold;
        cursor: pointer;
    }
</style>

<div class="form-card">

    <div class="title">
        Editar Proveedor
    </div>

    <div class="form-group">
        <label class="label">Nombre</label>

        <asp:TextBox ID="txt_nombre"
            runat="server"
            CssClass="custom-input">
        </asp:TextBox>
    </div>

    <div class="form-group">
        <label class="label">RUC</label>

        <asp:TextBox ID="txt_ruc"
            runat="server"
            CssClass="custom-input">
        </asp:TextBox>
    </div>

    <div class="form-group">
        <label class="label">Teléfono</label>

        <asp:TextBox ID="txt_telefono"
            runat="server"
            CssClass="custom-input">
        </asp:TextBox>
    </div>

    <div class="form-group">
        <label class="label">Correo</label>

        <asp:TextBox ID="txt_correo"
            runat="server"
            CssClass="custom-input">
        </asp:TextBox>
    </div>

    <asp:Button ID="btnEditar"
        runat="server"
        Text="Actualizar Proveedor"
        CssClass="btnGuardar"
        OnClick="btnEditar_Click" />

</div>

<script>
// @ts-nocheck

document.addEventListener('DOMContentLoaded', function () {

    const txtTelefono =
        document.getElementById('<%= txt_telefono.ClientID %>');

    const txtRuc =
        document.getElementById('<%= txt_ruc.ClientID %>');

    const txtNombre =
        document.getElementById('<%= txt_nombre.ClientID %>');

    const txtCorreo =
        document.getElementById('<%= txt_correo.ClientID %>');

    if (txtTelefono) {
        txtTelefono.addEventListener('input', function () {
            this.value =
                this.value.replace(/[^0-9]/g, '');
        });
    }

    if (txtRuc) {
        txtRuc.addEventListener('input', function () {
            this.value =
                this.value.replace(/[^0-9]/g, '');
        });
    }

    if (txtNombre) {
        txtNombre.addEventListener('input', function () {

            if (this.value.length > 100) {
                this.value = this.value.substring(0, 100);
            }

        });
    }

    if (txtCorreo) {
        txtCorreo.addEventListener('input', function () {

            if (this.value.length > 100) {
                this.value = this.value.substring(0, 100);
            }

        });
    }
});
</script>
</asp:Content>
