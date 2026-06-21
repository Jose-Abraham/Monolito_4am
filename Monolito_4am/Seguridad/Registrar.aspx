<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registrar.aspx.cs" Inherits="Monolito_4am.Seguridad.Registrar" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Crear Cuenta | Sistema Monolito</title>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">

    <style>
        :root {
            --primary: #38bdf8;
            --primary-hover: #0ea5e9;
            --bg-dark: #0f172a;
            --card-bg: rgba(30, 41, 59, 0.6);
            --text-main: #f8fafc;
            --text-muted: #94a3b8;
            --accent: #10b981;
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Inter', sans-serif;
        }

        body {
            min-height: 100vh;
            background: #0f172a url('../Images/fondo_premium.png') no-repeat center center fixed;
            background-size: cover;
            display: flex;
            justify-content: center;
            padding: 40px 20px;
            color: var(--text-main);
            position: relative;
        }

        body::before {
            content: "";
            position: fixed;
            top: 0; left: 0; width: 100%; height: 100%;
            background: rgba(15, 23, 42, 0.75);
            z-index: -1;
        }

        /* Efectos de luz de fondo */
        body::after {
            content: "";
            position: fixed;
            width: 400px;
            height: 400px;
            background: var(--primary);
            filter: blur(180px);
            opacity: 0.08;
            bottom: -100px;
            right: -100px;
            z-index: -1;
        }

        .contenedor {
            width: 100%;
            max-width: 600px;
            padding: 45px;
            border-radius: 32px;
            background: var(--card-bg);
            backdrop-filter: blur(25px);
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
            border: 1px solid rgba(255, 255, 255, 0.08);
            animation: slideUp 0.8s cubic-bezier(0.16, 1, 0.3, 1);
        }

        @keyframes slideUp {
            from { opacity: 0; transform: translateY(30px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .titulo {
            text-align: center;
            font-size: 32px;
            font-weight: 800;
            margin-bottom: 35px;
            letter-spacing: -1px;
            background: linear-gradient(to right, #fff, var(--text-muted));
            -webkit-background-clip: text;
            background-clip: text;
            -webkit-text-fill-color: transparent;
        }

        .grupo {
            margin-bottom: 22px;
        }

        .grupo label {
            display: block;
            margin-bottom: 10px;
            font-weight: 500;
            font-size: 14px;
            color: var(--text-muted);
            padding-left: 4px;
        }

        .textbox, .dropdown {
            width: 100%;
            padding: 14px 18px;
            border: 1px solid rgba(255, 255, 255, 0.1);
            border-radius: 14px;
            outline: none;
            font-size: 15px;
            background: rgba(15, 23, 42, 0.4);
            color: white;
            transition: all 0.3s ease;
        }

        .textbox:focus, .dropdown:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 4px rgba(56, 189, 248, 0.1);
            background: rgba(15, 23, 42, 0.7);
        }

        .dropdown option {
            background: #1e293b;
            color: white;
        }

        .boton {
            width: 100%;
            padding: 16px;
            border: none;
            border-radius: 14px;
            background: var(--primary);
            color: #0f172a;
            font-size: 16px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
            margin-top: 15px;
        }

        .boton:hover {
            background: #fff;
            transform: translateY(-2px) scale(1.01);
            box-shadow: 0 15px 30px -5px rgba(56, 189, 248, 0.4);
        }

        .login-link {
            text-align: center;
            margin-top: 30px;
            font-size: 14px;
            color: var(--text-muted);
        }

        .login-link a {
            color: var(--primary);
            text-decoration: none;
            font-weight: 700;
            margin-left: 5px;
        }

        /* Estilos para el ojo de la contraseña */
        .input-contenedor {
            position: relative;
            width: 100%;
        }

        .ojo-icon {
            position: absolute;
            right: 18px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            color: var(--primary);
            z-index: 10;
            font-size: 18px;
            transition: opacity 0.3s;
        }
        
        .ojo-icon:hover {
            opacity: 0.8;
        }

        /* Galería de fotos */
        .galeria-contenedor {
            display: flex; 
            flex-wrap: wrap; 
            gap: 12px; 
            background: rgba(15, 23, 42, 0.5); 
            padding: 15px; 
            border-radius: 18px;
            border: 1px dashed rgba(255,255,255,0.1);
            margin-top: 10px;
        }

        .foto-item {
            position: relative;
            border-radius: 12px;
            overflow: hidden;
            border: 2px solid rgba(255,255,255,0.1);
            transition: transform 0.3s;
        }

        .foto-item:hover {
            transform: scale(1.1) rotate(2deg);
            z-index: 5;
            border-color: var(--primary);
        }

        input[type="file"]::file-selector-button {
            background: rgba(255,255,255,0.1);
            color: white;
            border: none;
            padding: 8px 15px;
            border-radius: 8px;
            cursor: pointer;
            margin-right: 15px;
            font-size: 13px;
        }

        /* MEDIA QUERIES PARA RESPONSIVE */
        @media (max-width: 600px) {
            .contenedor { padding: 30px 20px; border-radius: 20px; }
            .titulo { font-size: 24px; }
            .grid-2col { grid-template-columns: 1fr !important; gap: 0 !important; }
        }
    </style>
</head>

<body>

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="contenedor">

            <div class="titulo">Formulario de Registro</div>

            <div class="grupo">
                <label>Documento de Identidad</label>
                <asp:TextBox ID="txt_cedula" runat="server" CssClass="textbox" maxlength="10" onkeypress="return soloNumeros(event)" placeholder="Ej: 1722..." />
            </div>

            <div class="grid-2col" style="display: grid; grid-template-columns: 1fr 1fr; gap: 20px;">
                <div class="grupo">
                    <label>Nombres</label>
                    <asp:TextBox ID="txt_nombres" runat="server" CssClass="textbox" maxlength="100" onkeypress="return soloLetras(event)" onblur="validarDosPalabras(this, 'Nombres')" placeholder="Sus nombres" />
                </div>

                <div class="grupo">
                    <label>Apellidos</label>
                    <asp:TextBox ID="txt_apellidos" runat="server" CssClass="textbox" maxlength="100" onkeypress="return soloLetras(event)" onblur="validarDosPalabras(this, 'Apellidos')" AutoPostBack="true" OnTextChanged="txt_apellidos_TextChanged" placeholder="Sus apellidos" />
                </div>
            </div>

            <div class="grupo">
                <label>Correo Electrónico</label>
                <asp:TextBox ID="txt_correo" runat="server" CssClass="textbox" maxlength="150" placeholder="usuario@correo.com" />
            </div>

            <div class="grupo">
                <label>Dirección Domiciliaria</label>
                <asp:TextBox ID="txt_direccion" runat="server" CssClass="textbox" maxlength="100" placeholder="Calle, Número, Ciudad" />
            </div>

            <div class="grupo">
                <label>Número Celular</label>
                <asp:TextBox ID="txt_celular" runat="server" CssClass="textbox" maxlength="10" onkeypress="return soloNumeros(event)" placeholder="09..." />
            </div>

            <div class="grupo">
                <label>Contraseña de Acceso</label>
                <div class="input-contenedor">
                    <asp:TextBox ID="txt_contrasena" runat="server" CssClass="textbox" maxlength="50" placeholder="Mínimo 8 caracteres" />
                    <i class="fas fa-eye-slash ojo-icon" id="togglePassword" onclick="togglePasswordVisibility()"></i>
                </div>
            </div>

            <div class="grid-2col" style="display: grid; grid-template-columns: 1fr 1fr; gap: 20px;">
                <div class="grupo">
                    <label>Fecha de Nacimiento</label>
                    <asp:TextBox ID="txt_fecha_cumpleanos" runat="server" CssClass="textbox" TextMode="Date" onchange="validarFecha(this)" />
                </div>

                <div class="grupo">
                    <label>Nombre de Usuario (Nick)</label>
                    <asp:TextBox ID="txt_nick" runat="server" CssClass="textbox" maxlength="50" placeholder="Su_Alias" />
                </div>
            </div>

            <div class="grupo">
                <label>Tipo de Perfil</label>
                <asp:DropDownList ID="ddl_perfil" runat="server" CssClass="dropdown"></asp:DropDownList>
            </div>

            <div class="grupo">
                <label>Fotografías de Perfil</label>
                <asp:FileUpload ID="fuFoto" runat="server" CssClass="textbox" 
                    Accept="image/jpeg,image/png,image/jpg" AllowMultiple="true" onchange="validarImagenes(this)" 
                    style="padding: 10px;" />
                
                <asp:Button ID="btn_subir_foto" runat="server" Text="＋ Añadir Imágenes" 
                    CssClass="boton" OnClick="btn_subir_foto_Click" 
                    style="background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); color: white; margin-top: 10px; font-size: 13px; padding: 10px;" />
                
                <asp:Label ID="lblErrorFoto" runat="server" style="color: #fbbf24; font-size: 12px; margin-top: 8px; display: block;" />
            </div>

            <div class="galeria-contenedor">
                <asp:Repeater ID="rptFotos" runat="server">
                    <ItemTemplate>
                        <div class="foto-item">
                            <img src='<%# "data:image/png;base64," + Convert.ToBase64String((byte[])Container.DataItem) %>' 
                                 style="width: 70px; height: 70px; object-fit: cover;" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblSinFotos" runat="server" Text="No hay imágenes seleccionadas" 
                    Visible="true" style="font-size: 12px; color: var(--text-muted); width: 100%; text-align: center; padding: 10px;" />
            </div>

            <asp:Button ID="btn_registrar" runat="server" Text="Finalizar Registro" CssClass="boton" OnClick="btn_registrar_Click" />

            <div class="login-link">
                ¿Ya posee una cuenta? <asp:LinkButton ID="lnk_login" runat="server" OnClick="lnk_login_Click">Inicie Sesión aquí</asp:LinkButton>
            </div>

            <div style="text-align: center; margin-top: 20px; padding-top: 20px; border-top: 1px solid rgba(255, 255, 255, 0.05);">
                <a href="../Home.aspx" style="color: var(--text-muted); text-decoration: none; font-size: 13px; font-weight: 500; transition: color 0.3s;">
                    <i class="fas fa-home" style="margin-right: 5px;"></i> Volver al Home
                </a>
            </div>

        </div>
    </form>

    <script type="text/javascript">
        //@ts-nocheck
        function soloNumeros(e) {
            var key = e.keyCode || e.which;
            var teclado = String.fromCharCode(key);
            var numeros = "0123456789";
            if (numeros.indexOf(teclado) == -1 && key != 8 && key != 46) {
                return false;
            }
        }

        function soloLetras(e) {
            var key = e.keyCode || e.which;
            var teclado = String.fromCharCode(key).toLowerCase();
            var letras = " abcdefghijklmnñopqrstuvwxyz";
            if (letras.indexOf(teclado) == -1 && key != 8 && key != 46) {
                return false;
            }
        }

        function validarDosPalabras(input, tipo) {
            let valor = input.value.trim();
            if (valor !== "") {
                let palabras = valor.split(/\s+/);
                if (palabras.length < 2) {
                    window.Swal.fire({
                        title: 'Atención',
                        text: `Es obligatorio ingresar al menos dos ${tipo}. Si solo tienes uno, por favor escríbelo dos veces.`,
                        icon: 'warning',
                        confirmButtonText: 'Entendido'
                    });
                }
            }
        }

        function validarFecha(input) {
            const fechaSeleccionada = new Date(input.value);
            const hoy = new Date();
            let edad = hoy.getFullYear() - fechaSeleccionada.getFullYear();
            const mes = hoy.getMonth() - fechaSeleccionada.getMonth();

            if (mes < 0 || (mes === 0 && hoy.getDate() < fechaSeleccionada.getDate())) {
                edad--;
            }

            if (fechaSeleccionada > hoy) {
                window.Swal.fire('Error', 'No puedes seleccionar una fecha futura', 'error');
                input.value = "";
            } else if (edad < 18) {
                window.Swal.fire('Atención', 'Debes ser mayor de 18 años para registrarte', 'warning');
                input.value = "";
            } else if (edad > 100 || fechaSeleccionada.getFullYear() < 1926) {
                window.Swal.fire('Atención', 'La fecha no puede ser anterior a 1926 o superar los 100 años de edad', 'warning');
                input.value = "";
            }
        }

        function validarImagenes(input) {
            const files = input.files;
            const maxSizePerFile = 5 * 1024 * 1024; // 5MB
            const maxTotalSize = 10 * 1024 * 1024; // 10MB
            let totalSize = 0;
            const allowedExtensions = /(\.jpg|\.jpeg|\.png)$/i;

            for (let i = 0; i < files.length; i++) {
                if (!allowedExtensions.exec(files[i].name)) {
                    window.Swal.fire('Error', 'Solo se permiten imágenes JPG, JPEG o PNG', 'error');
                    input.value = '';
                    return;
                }
                if (files[i].size > maxSizePerFile) {
                    window.Swal.fire('Error', `La imagen ${files[i].name} es demasiado grande. Máximo 5MB por foto.`, 'error');
                    input.value = '';
                    return;
                }
                totalSize += files[i].size;
            }

            if (totalSize > maxTotalSize) {
                window.Swal.fire('Error', 'El tamaño total de las imágenes no puede superar los 10MB', 'error');
                input.value = '';
            }
        }

        function togglePasswordVisibility() {
            const txtPass = document.getElementById('<%= txt_contrasena.ClientID %>');
            const icon = document.getElementById('togglePassword');
            
            if (txtPass.type === "password") {
                txtPass.type = "text";
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye');
            } else {
                txtPass.type = "password";
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash');
            }
        }

        // Inicializar como password al cargar
        window.addEventListener('DOMContentLoaded', (event) => {
            const txtPass = document.getElementById('<%= txt_contrasena.ClientID %>');
            if (txtPass) txtPass.type = "password";
        });
    </script>
</body>
</html>
