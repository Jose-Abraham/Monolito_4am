<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Monolito_4am.Home" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Bienvenido | Monolito</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    
    <style>
        :root {
            --primary: #38bdf8;
            --primary-hover: #0ea5e9;
            --bg-dark: #0f172a;
            --card-bg: rgba(30, 41, 59, 0.5);
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
            background: #0f172a url('Images/fondo_premium.png') no-repeat center center fixed;
            background-size: cover;
            display: flex;
            justify-content: center;
            align-items: center;
            color: var(--text-main);
            overflow: hidden;
            position: relative;
        }

        /* Overlay oscuro */
        body::after {
            content: "";
            position: fixed;
            top: 0; left: 0; width: 100%; height: 100%;
            background: radial-gradient(circle at center, rgba(15, 23, 42, 0.4) 0%, rgba(15, 23, 42, 0.9) 100%);
            z-index: -1;
        }

        /* Efectos de luces flotantes */
        .light-blob {
            position: absolute;
            width: 500px;
            height: 500px;
            background: var(--primary);
            filter: blur(180px);
            opacity: 0.15;
            border-radius: 50%;
            z-index: -1;
            animation: float 20s infinite alternate;
        }

        @keyframes float {
            0% { transform: translate(-20%, -20%) scale(1); }
            100% { transform: translate(20%, 20%) scale(1.1); }
        }

        .container {
            text-align: center;
            max-width: 900px;
            padding: 20px;
            z-index: 10;
            animation: fadeIn 1.2s ease-out;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(30px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .logo-main {
            font-size: 84px;
            font-weight: 800;
            letter-spacing: -4px;
            background: linear-gradient(to bottom, #fff 30%, #94a3b8 100%);
            -webkit-background-clip: text;
            background-clip: text;
            -webkit-text-fill-color: transparent;
            margin-bottom: 10px;
            text-transform: uppercase;
        }

        .tagline {
            font-size: 20px;
            color: var(--text-muted);
            margin-bottom: 50px;
            font-weight: 300;
            letter-spacing: 2px;
            text-transform: uppercase;
        }

        .grid-buttons {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 30px;
            width: 100%;
            max-width: 700px;
            margin: 0 auto;
        }

        .btn-card {
            background: var(--card-bg);
            backdrop-filter: blur(16px);
            border: 1px solid rgba(255, 255, 255, 0.1);
            padding: 40px;
            border-radius: 32px;
            text-decoration: none;
            color: white;
            transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 20px;
        }

        .btn-card:hover {
            transform: translateY(-15px) scale(1.02);
            background: rgba(255, 255, 255, 0.08);
            border-color: var(--primary);
            box-shadow: 0 30px 60px -12px rgba(0, 0, 0, 0.5), 0 0 20px rgba(56, 189, 248, 0.2);
        }

        .btn-card i {
            font-size: 48px;
            color: var(--primary);
            transition: transform 0.4s;
        }

        .btn-card:hover i {
            transform: scale(1.2) rotate(-5deg);
            color: #fff;
        }

        .btn-title {
            font-size: 22px;
            font-weight: 700;
        }

        .btn-desc {
            font-size: 14px;
            color: var(--text-muted);
            line-height: 1.5;
        }

        /* Footer sutil */
        .footer-info {
            position: absolute;
            bottom: 40px;
            width: 100%;
            text-align: center;
            color: var(--text-muted);
            font-size: 13px;
            opacity: 0.6;
        }

        @media (max-width: 600px) {
            .grid-buttons { grid-template-columns: 1fr; }
            .logo-main { font-size: 56px; }
        }
    </style>
</head>
<body>
    <div class="light-blob" style="top: -10%; left: -10%;"></div>
    <div class="light-blob" style="bottom: -10%; right: -10%; background: #818cf8;"></div>

    <form id="form1" runat="server">
        <div class="container">
            <h1 class="logo-main">Monolito</h1>
            <p class="tagline">Monolito desplegado por Jenkins</p>

            <div class="grid-buttons">
                <!-- Opción Login -->
                <a href="Seguridad/Login.aspx" class="btn-card">
                    <i class="fas fa-sign-in-alt"></i>
                    <div class="btn-title">Iniciar Sesión</div>
                    <p class="btn-desc">Accede a tu panel personal y gestiona tu seguridad avanzada.</p>
                </a>

                <!-- Opción Registro -->
                <a href="Seguridad/Registrar.aspx" class="btn-card">
                    <i class="fas fa-user-plus"></i>
                    <div class="btn-title">Registrarse</div>
                    <p class="btn-desc">Crea una nueva cuenta y únete a nuestra plataforma segura.</p>
                </a>
            </div>
        </div>

        <div class="footer-info">
            &copy; 2026 Monolito Security Systems. Todos los derechos reservados.
        </div>
    </form>
</body>
</html>
