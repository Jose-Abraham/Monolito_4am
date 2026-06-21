<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecuperarPassword.aspx.cs" Inherits="Monolito_4am.Seguridad.RecuperarPassword" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Recuperar Contraseña | Monolito</title>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
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

        * { margin: 0; padding: 0; box-sizing: border-box; font-family: 'Inter', sans-serif; }

        body { 
            min-height: 100vh; 
            background: #0f172a url('../Images/fondo_premium.png') no-repeat center center fixed; 
            background-size: cover;
            display: flex; 
            justify-content: center; 
            align-items: center; 
            color: var(--text-main);
            position: relative;
        }

        body::after {
            content: "";
            position: fixed;
            top: 0; left: 0; width: 100%; height: 100%;
            background: rgba(15, 23, 42, 0.75);
            z-index: -1;
        }

        .contenedor-recuperar { 
            width: 100%;
            max-width: 440px; 
            padding: 50px 40px; 
            border-radius: 28px; 
            background: var(--card-bg); 
            backdrop-filter: blur(24px); 
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.6); 
            border: 1px solid rgba(255, 255, 255, 0.08); 
            animation: fadeIn 0.8s ease-out;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .titulo { 
            text-align: center; 
            font-size: 28px; 
            font-weight: 800; 
            margin-bottom: 8px; 
            letter-spacing: -1px;
            background: linear-gradient(to bottom, #fff, #94a3b8);
            -webkit-background-clip: text;
            background-clip: text;
            -webkit-text-fill-color: transparent;
        }

        .subtitulo { 
            text-align: center; 
            color: var(--text-muted); 
            margin-bottom: 35px; 
            font-size: 14px; 
        }

        .grupo { margin-bottom: 24px; }
        .grupo label { display: block; margin-bottom: 10px; font-weight: 500; font-size: 14px; color: var(--text-muted); }

        .textbox { 
            width: 100%; 
            padding: 15px 20px; 
            border: 1px solid rgba(255, 255, 255, 0.1); 
            outline: none; 
            border-radius: 14px; 
            font-size: 15px; 
            background: rgba(15, 23, 42, 0.5); 
            color: white;
            transition: all 0.3s ease;
        }

        .textbox:focus { 
            border-color: var(--primary);
            box-shadow: 0 0 0 4px rgba(56, 189, 248, 0.1);
        }

        .boton-recuperar { 
            width: 100%; 
            padding: 16px; 
            border: none; 
            border-radius: 14px; 
            background: var(--primary); 
            color: #0f172a; 
            font-size: 16px; 
            font-weight: 700; 
            cursor: pointer; 
            transition: all 0.4s;
            margin-top: 10px;
        }

        .boton-recuperar:hover { 
            background: #fff;
            transform: translateY(-2px);
            box-shadow: 0 10px 20px rgba(56, 189, 248, 0.3);
        }

        .volver {
            text-align: center;
            margin-top: 30px;
            font-size: 14px;
        }

        .volver a {
            color: var(--primary);
            text-decoration: none;
            font-weight: 600;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="contenedor-recuperar">
            <div class="titulo">Recuperación</div>
            <div class="subtitulo">Le enviaremos una clave temporal por WhatsApp</div>

            <div class="grupo">
                <label>Número de Cédula</label>
                <asp:TextBox ID="txt_cedula_rec" runat="server" CssClass="textbox" placeholder="Ingrese su identificación" maxlength="10"></asp:TextBox>
            </div>

            <asp:Button ID="btn_recuperar" runat="server" Text="Generar y Enviar Clave" CssClass="boton-recuperar" OnClick="btn_recuperar_Click" />

            <div class="volver">
                ¿Recordó su clave? <a href="Login.aspx">Volver al Login</a>
            </div>
        </div>
    </form>
</body>
</html>
