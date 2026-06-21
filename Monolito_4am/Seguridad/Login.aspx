<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Monolito_4am.Seguridad.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Acceso al Sistema | Monolito</title>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/html5-qrcode@2.3.8/html5-qrcode.min.js"></script>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700&display=swap" rel="stylesheet">
    
    <style>
        :root {
            --primary: #38bdf8;
            --primary-hover: #0ea5e9;
            --bg-dark: #0f172a;
            --card-bg: rgba(30, 41, 59, 0.7);
            --text-main: #f8fafc;
            --text-muted: #94a3b8;
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
            align-items: center; 
            color: var(--text-main);
            overflow-x: hidden;
            position: relative;
        }

        body::after {
            content: "";
            position: fixed;
            top: 0; left: 0; width: 100%; height: 100%;
            background: rgba(15, 23, 42, 0.7);
            z-index: -1;
        }

        body::before {
            content: "";
            position: absolute;
            width: 350px;
            height: 350px;
            background: var(--primary);
            filter: blur(160px);
            opacity: 0.12;
            top: -100px;
            left: -100px;
            z-index: -1;
        }

        .contenedor-login { 
            width: 100%;
            max-width: 440px; 
            padding: 50px 40px; 
            border-radius: 28px; 
            background: var(--card-bg); 
            backdrop-filter: blur(24px); 
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.6); 
            border: 1px solid rgba(255, 255, 255, 0.08); 
            animation: fadeIn 0.8s cubic-bezier(0.16, 1, 0.3, 1);
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: scale(0.95) translateY(10px); }
            to { opacity: 1; transform: scale(1) translateY(0); }
        }

        .titulo { 
            text-align: center; 
            font-size: 34px; 
            font-weight: 800; 
            margin-bottom: 8px; 
            letter-spacing: -1.5px;
            background: linear-gradient(to bottom, #fff, #94a3b8);
            -webkit-background-clip: text;
            background-clip: text;
            -webkit-text-fill-color: transparent;
        }

        .subtitulo { 
            text-align: center; 
            color: var(--text-muted); 
            margin-bottom: 40px; 
            font-size: 15px; 
            font-weight: 400;
        }

        .grupo { 
            margin-bottom: 24px; 
        }

        .grupo label { 
            display: block; 
            margin-bottom: 10px; 
            font-weight: 500; 
            font-size: 14px;
            color: var(--text-muted);
            padding-left: 4px;
        }

        .textbox { 
            width: 100%; 
            padding: 15px 20px; 
            border: 1px solid rgba(255, 255, 255, 0.1); 
            outline: none; 
            border-radius: 14px; 
            font-size: 15px; 
            background: rgba(15, 23, 42, 0.5); 
            color: white;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }

        .textbox:focus { 
            border-color: var(--primary);
            box-shadow: 0 0 0 4px rgba(56, 189, 248, 0.1);
            background: rgba(15, 23, 42, 0.8);
            transform: translateY(-1px);
        }

        .fila-opciones {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
            font-size: 14px;
            padding: 0 4px;
        }

        .fila-opciones div {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .fila-opciones a {
            color: var(--primary);
            text-decoration: none;
            font-weight: 600;
            transition: all 0.3s;
        }

        .fila-opciones a:hover {
            color: #fff;
            text-shadow: 0 0 15px var(--primary);
        }

        .boton-login { 
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
            margin-bottom: 12px;
        }

        .boton-login:hover { 
            background: #fff;
            transform: scale(1.02);
            box-shadow: 0 20px 25px -5px rgba(56, 189, 248, 0.3);
        }

        .boton-login:active {
            transform: scale(0.98);
        }

        #reader { 
            width: 100%; 
            max-width: 400px; 
            margin: 15px auto; 
            border: 1px solid rgba(255, 255, 255, 0.1); 
            border-radius: 20px; 
            overflow: hidden;
            background: #000;
        }

        .seccion-qr { 
            display: none; 
            margin-top: 20px; 
            background: rgba(0,0,0,0.3);
            padding: 25px;
            border-radius: 24px;
            border: 1px solid rgba(255,255,255,0.05);
        }

        .registro {
            text-align: center;
            margin-top: 40px;
            padding-top: 30px;
            border-top: 1px solid rgba(255, 255, 255, 0.05);
            font-size: 14px;
            color: var(--text-muted);
        }

        .registro a {
            color: var(--primary);
            text-decoration: none;
            font-weight: 700;
            margin-left: 5px;
        }

        input[type="checkbox"] {
            accent-color: var(--primary);
            width: 18px;
            height: 18px;
            cursor: pointer;
        }

        .password-wrapper {
            position: relative;
            display: flex;
            align-items: center;
        }

        .toggle-password {
            position: absolute;
            right: 15px;
            cursor: pointer;
            color: var(--text-muted);
            user-select: none;
            transition: color 0.3s;
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10;
        }

        .toggle-password:hover {
            color: var(--primary);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

        <div class="contenedor-login">
            <div class="titulo">Monolito</div>
            <div class="subtitulo">Gestión de Seguridad Avanzada</div>

            <!-- Credenciales -->
            <div class="grupo">
                <asp:Label ID="lbl_usuario" runat="server" Text="Identificación"></asp:Label>
                <asp:TextBox ID="txt_ced" runat="server" CssClass="textbox" placeholder="Ingrese su cédula" MaxLength="10" autocomplete="off"></asp:TextBox>
            </div>
            <div class="grupo">
                <asp:Label ID="lbl_password" runat="server" Text="Contraseña"></asp:Label>
                <div class="password-wrapper">
                    <asp:TextBox ID="txt_pass" runat="server" CssClass="textbox" TextMode="Password" placeholder="••••••••" autocomplete="off" MaxLength="50"></asp:TextBox>
                    <span id="btnTogglePass" class="toggle-password" style="display: flex;">
                        <svg id="eyeOpen" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                        <svg id="eyeClosed" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="display:none;"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path><line x1="1" y1="1" x2="23" y2="23"></line></svg>
                    </span>
                </div>
            </div>

            <!-- OTP Manual -->
            <div class="grupo">
                <asp:Label ID="lbl_otp" runat="server" Text="Código de Seguridad OTP" Visible="false" style="color: var(--primary); font-size: 13px; margin-bottom: 10px; display: block; font-weight: 600; text-align: center;"></asp:Label>
                <asp:TextBox ID="txt_otp" runat="server" CssClass="textbox" Visible="false" placeholder="000000" style="text-align: center; letter-spacing: 8px; font-size: 24px; font-weight: 800; border-color: var(--primary);"></asp:TextBox>
            </div>

            <div id="pnl_opciones" runat="server" class="fila-opciones">
                <div>
                    <asp:CheckBox ID="chkRecordarme" runat="server" />
                    <label>Mantener sesión</label>
                </div>
                <a href="RecuperarPassword.aspx">¿Olvidó su clave?</a>
            </div>

            <div id="pnl_volver" runat="server" visible="false" style="text-align: center; margin-bottom: 25px;">
                <a href="Login.aspx" style="color: var(--primary); text-decoration: none; font-weight: 600; font-size: 14px;">
                    <i class="fas fa-arrow-left"></i> Volver al Login
                </a>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Entrar al Sistema" CssClass="boton-login" OnClick="btnLogin_Click" />
            
            <asp:Button ID="btn_otp" runat="server" CssClass="boton-login" Text="Verificar Identidad" OnClick="btn_otp_Click" Visible="false" 
                        style="background: #10b981; color: white;" />
            
            <div id="seccionQR" class="seccion-qr">
                <h4 style="text-align:center; margin-bottom:20px; color: var(--primary); font-weight: 700;">Acceso Biométrico</h4>
                <div id="reader"></div>
            </div>

            <asp:Button ID="btnEscanearQR" runat="server" Text="Acceder con QR" 
                        CssClass="boton-login" OnClientClick="iniciarEscaneo(); return false;" Visible="false" 
                        style="background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); color: white; margin-top: 10px;" />

            <asp:Label ID="lblMensaje" runat="server" style="display: block; text-align: center; color: #f87171; font-size: 13px; margin-top: 15px; font-weight: 500;"></asp:Label>

            <div class="registro">
                ¿No tienes cuenta? <a href="Registrar.aspx">Regístrate ahora</a>
            </div>

            <div style="text-align: center; margin-top: 20px; padding-top: 20px; border-top: 1px solid rgba(255, 255, 255, 0.05);">
                <a href="../Home.aspx" style="color: var(--text-muted); text-decoration: none; font-size: 13px; font-weight: 500; transition: color 0.3s;">
                    <i class="fas fa-home" style="margin-right: 5px;"></i> Volver al Home
                </a>
            </div>
        </div>
    </form>

           <script>
// @ts-nocheck
// Bloquear el botón atrás del navegador por seguridad
window.history.pushState(null, "", window.location.href);
window.onpopstate = function () {
    window.history.pushState(null, "", window.location.href);
};

document.addEventListener('DOMContentLoaded', function () {
    const txtCed = document.getElementById('<%= txt_ced.ClientID %>');
    const txtPass = document.getElementById('<%= txt_pass.ClientID %>');
    const btnToggle = document.getElementById('btnTogglePass');
    const eyeOpen = document.getElementById('eyeOpen');
    const eyeClosed = document.getElementById('eyeClosed');

    // Restricción para Cédula: Solo 10 números
    if (txtCed) {
        txtCed.addEventListener('input', function (e) {
            this.value = this.value.replace(/[^0-9]/g, '').slice(0, 10);
        });

        txtCed.addEventListener('keypress', function (e) {
            if (e.which < 48 || e.which > 57) {
                e.preventDefault();
            }
        });
    }

    // Mostrar/Ocultar Contraseña
    if (btnToggle && txtPass) {
        btnToggle.addEventListener('click', function () {
            const isPassword = txtPass.type === 'password';
            txtPass.type = isPassword ? 'text' : 'password';
            
            eyeOpen.style.display = isPassword ? 'none' : 'block';
            eyeClosed.style.display = isPassword ? 'block' : 'none';
        });
    }



    // Restricción para OTP: Solo 6 números
    const txtOtp = document.getElementById('<%= txt_otp.ClientID %>');
    if (txtOtp) {
        txtOtp.addEventListener('input', function (e) {
            this.value = this.value.replace(/[^0-9]/g, '').slice(0, 6);
        });
        txtOtp.addEventListener('keypress', function (e) {
            if (e.which < 48 || e.which > 57) e.preventDefault();
        });
    }
});

let html5QrCode = null;

function iniciarEscaneo() {
    const seccionQR = document.getElementById("seccionQR");
    seccionQR.style.display = "block";

    // Si ya hay una cámara abierta, la detenemos primero
    if (html5QrCode !== null) {
        html5QrCode.stop().catch(() => { });
    }

    // Pequeño delay para asegurar que se detuvo
    setTimeout(() => {
        iniciarCamara();
    }, 300);
}

function iniciarCamara() {
    html5QrCode = new Html5Qrcode("reader");

    html5QrCode.start(
        { facingMode: "environment" },
        {
            fps: 12,
            qrbox: { width: 280, height: 280 }
        },
        function (decodedText) {
            // Éxito al leer QR
            console.log("QR Escaneado:", decodedText);
            detenerCamara();
            document.getElementById("seccionQR").style.display = "none";

            // Usamos FETCH en lugar de PageMethods para evitar problemas de rutas amigables
            fetch('Login.aspx/ProcesarQR', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                body: JSON.stringify({ qrData: decodedText })
            })
            .then(response => response.json())
            .then(data => {
                let result = data.d; // ASP.NET envuelve la respuesta en una propiedad "d"
                if (result.success) {
                    Swal.fire({
                        title: '¡Éxito!',
                        text: result.message,
                        icon: 'success',
                        timer: 1800
                    }).then(() => {
                        window.location.href = result.redirect;
                    });
                } else {
                    Swal.fire('Error', 'Servidor: ' + result.message, 'error');
                }
            })
            .catch(error => {
                console.error('Error Fetch:', error);
                Swal.fire('Error', 'No se pudo conectar con el servidor', 'error');
            });
        },
        function (errorMessage) {
            // Solo errores de escaneo (no molestar)
        }
    ).catch(function (err) {
        console.error(err);
        Swal.fire('Error de Cámara', 'No se pudo acceder a la cámara. Verifica permisos.', 'error');
    });
}

function detenerCamara() {
    if (html5QrCode) {
        html5QrCode.stop().catch(() => { });
        html5QrCode = null;
    }
}
</script>
</body>
</html>