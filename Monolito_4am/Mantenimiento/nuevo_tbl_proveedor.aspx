<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="nuevo_tbl_proveedor.aspx.cs" Inherits="Monolito_4am.Mantenimiento.nuevo_tbl_proveedor1" %>
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
            transition: all 0.3s;
        }
        .custom-input:focus {
            border-color: #38bdf8;
            box-shadow: 0 0 10px rgba(56,189,248,0.3);
        }
        .btnGuardar {
            width: 100%;
            padding: 14px;
            border: none;
            border-radius: 12px;
            background: linear-gradient(135deg,#22c55e,#16a34a);
            color: white;
            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
            transition: .3s;
        }
        .btnGuardar:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(34,197,94,0.35);
        }
        .error-text {
            color: #f87171;
            font-size: 13px;
            margin-top: 5px;
            display: none;
        }
    </style>

    <div class="form-card">
        <div class="title">Nuevo Proveedor</div>

        <div class="form-group">
            <label class="label">Nombre del Proveedor *</label>
            <asp:TextBox ID="txt_nombre" runat="server" CssClass="custom-input" MaxLength="100" placeholder="Ej: Distribuidora ABC"></asp:TextBox>
            <span id="error_nombre" class="error-text"></span>
        </div>

        <div class="form-group">
            <label class="label">RUC *</label>
            <asp:TextBox ID="txt_ruc" runat="server" CssClass="custom-input" MaxLength="13" placeholder="Ej: 1798765432001"></asp:TextBox>
            <span id="error_ruc" class="error-text"></span>
        </div>

        <div class="form-group">
            <label class="label">Teléfono *</label>
            <asp:TextBox ID="txt_telefono" runat="server" CssClass="custom-input" MaxLength="15" placeholder="Ej: 0987654321"></asp:TextBox>
            <span id="error_telefono" class="error-text"></span>
        </div>

        <div class="form-group">
            <label class="label">Correo Electrónico *</label>
            <asp:TextBox ID="txt_correo" runat="server" CssClass="custom-input" TextMode="Email" MaxLength="100" placeholder="proveedor@ejemplo.com"></asp:TextBox>
            <span id="error_correo" class="error-text"></span>
        </div>

        <asp:Button ID="btnGuardar" runat="server" Text="Guardar Proveedor" CssClass="btnGuardar" OnClick="btnGuardar_Click" OnClientClick="return validarFormulario();" />
    </div>

    <script type="text/javascript">
// @ts-nocheck
        function validarFormulario() {
            let valido = true;

            // Limpiar errores previos
            document.querySelectorAll('.error-text').forEach(el => el.style.display = 'none');

            // Validar Nombre
            const nombre = document.getElementById('<%= txt_nombre.ClientID %>').value.trim();
            if (nombre === "" || nombre.length < 3) {
                document.getElementById('error_nombre').innerText = "El nombre debe tener al menos 3 caracteres.";
                document.getElementById('error_nombre').style.display = 'block';
                valido = false;
            }

            // Validar RUC (13 dígitos)
            const ruc = document.getElementById('<%= txt_ruc.ClientID %>').value.trim();
            if (ruc === "" || ruc.length !== 13 || !/^\d+$/.test(ruc)) {
                document.getElementById('error_ruc').innerText = "El RUC debe tener exactamente 13 dígitos numéricos.";
                document.getElementById('error_ruc').style.display = 'block';
                valido = false;
            }

            // Validar Teléfono (solo números)
            const telefono = document.getElementById('<%= txt_telefono.ClientID %>').value.trim();
            if (telefono === "" || telefono.length < 7 || !/^\d+$/.test(telefono)) {
                document.getElementById('error_telefono').innerText = "Ingrese un teléfono válido (solo números).";
                document.getElementById('error_telefono').style.display = 'block';
                valido = false;
            }

            // Validar Correo
            const correo = document.getElementById('<%= txt_correo.ClientID %>').value.trim();
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (correo === "" || !emailRegex.test(correo)) {
                document.getElementById('error_correo').innerText = "Ingrese un correo electrónico válido.";
                document.getElementById('error_correo').style.display = 'block';
                valido = false;
            }

            if (!valido) {
                Swal.fire({
                    title: 'Campos inválidos',
                    text: 'Por favor corrige los errores antes de continuar.',
                    icon: 'warning',
                    confirmButtonColor: '#38bdf8'
                });
            }

            return valido;
        }

        // Restricciones en tiempo real
        document.addEventListener('DOMContentLoaded', function () {
            // Solo números en Teléfono y RUC
            const txtTelefono = document.getElementById('<%= txt_telefono.ClientID %>');
            const txtRuc = document.getElementById('<%= txt_ruc.ClientID %>');

            if (txtTelefono) {
                txtTelefono.addEventListener('input', function () {
                    this.value = this.value.replace(/[^0-9]/g, '');
                });
            }
            if (txtRuc) {
                txtRuc.addEventListener('input', function () {
                    this.value = this.value.replace(/[^0-9]/g, '');
                });
            }
        });
</script>

</asp:Content>
