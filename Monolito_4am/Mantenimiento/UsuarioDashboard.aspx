<%@ Page Title="Inicio - Mi Perfil" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="UsuarioDashboard.aspx.cs" Inherits="Monolito_4am.Mantenimiento.UsuarioDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .profile-card {
            max-width: 600px;
            margin: 0 auto;
            background: rgba(255, 255, 255, 0.05);
            border-radius: 20px;
            padding: 40px;
            border: 1px solid rgba(255, 255, 255, 0.1);
        }

        .input-group { margin-bottom: 25px; }
        .input-group label { display: block; margin-bottom: 10px; color: var(--text-muted); font-size: 0.9rem; }
        
        .form-control {
            width: 100%;
            padding: 15px;
            border-radius: 12px;
            border: 1px solid rgba(255, 255, 255, 0.1);
            background: rgba(0, 0, 0, 0.2);
            color: white;
            font-size: 1rem;
            transition: 0.3s;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--primary);
            background: rgba(0, 0, 0, 0.3);
            box-shadow: 0 0 15px rgba(56, 189, 248, 0.1);
        }

        .btn-update {
            width: 100%;
            padding: 15px;
            border-radius: 12px;
            border: none;
            background: linear-gradient(135deg, var(--primary), var(--primary-hover));
            color: white;
            font-weight: 700;
            font-size: 1rem;
            cursor: pointer;
            transition: 0.3s;
            margin-top: 10px;
        }

        .btn-update:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 20px rgba(56, 189, 248, 0.3);
        }

        .stats-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-top: 40px;
        }

        .stat-card {
            background: rgba(255, 255, 255, 0.03);
            padding: 20px;
            border-radius: 15px;
            text-align: center;
            border: 1px solid rgba(255, 255, 255, 0.05);
        }

        .stat-val { font-size: 1.5rem; font-weight: 700; color: var(--accent); }

        /* RESPONSIVE */
        @media (max-width: 600px) {
            .profile-card { padding: 25px 20px; }
            .stats-grid { grid-template-columns: 1fr; }
        }

        /* Estilos para el Ojo de Contraseña */
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

    <div class="profile-card">
        <h2 style="text-align: center; margin-bottom: 30px;">Seguridad de la Cuenta</h2>
        
        <div class="input-group">
            <label>Cédula (No editable)</label>
            <asp:TextBox ID="txt_cedula" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
        </div>

        <div class="input-group">
            <label>Nueva Contraseña</label>
            <div class="password-wrapper">
                <asp:TextBox ID="txt_new_pass" runat="server" CssClass="form-control" TextMode="Password" placeholder="Escriba su nueva clave..." MaxLength="50"></asp:TextBox>
                <span class="toggle-password" onclick="togglePass('MainContent_txt_new_pass', this)">
                    <svg class="eye-open" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                    <svg class="eye-closed" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="display:none;"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path><line x1="1" y1="1" x2="23" y2="23"></line></svg>
                </span>
            </div>
        </div>

        <div class="input-group">
            <label>Confirmar Contraseña</label>
            <div class="password-wrapper">
                <asp:TextBox ID="txt_confirm_pass" runat="server" CssClass="form-control" TextMode="Password" placeholder="Repita la clave..." MaxLength="50"></asp:TextBox>
                <span class="toggle-password" onclick="togglePass('MainContent_txt_confirm_pass', this)">
                    <svg class="eye-open" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                    <svg class="eye-closed" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="display:none;"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path><line x1="1" y1="1" x2="23" y2="23"></line></svg>
                </span>
            </div>
        </div>

        <asp:Button ID="btn_cambiar_pass" runat="server" Text="Actualizar Contraseña" OnClick="btn_cambiar_pass_Click" CssClass="btn-update" />

        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-label">Mi Récord</div>
                <div class="stat-val"><asp:Literal ID="lit_record" runat="server">0</asp:Literal> Pts</div>
            </div>
            <div class="stat-card">
                <div class="stat-label">Estado</div>
                <div class="stat-val" style="color: #4ade80;">Activo</div>
            </div>
        </div>
    </div>

     <script type="text/javascript">
// @ts-nocheck
/* global Swal, __doPostBack */
        function togglePass(id, btn) {
            const input = document.getElementById(id);
            const open = btn.querySelector('.eye-open');
            const closed = btn.querySelector('.eye-closed');

            if (input.type === 'password') {
                input.type = 'text';
                open.style.display = 'none';
                closed.style.display = 'block';
            } else {
                input.type = 'password';
                open.style.display = 'block';
                closed.style.display = 'none';
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            const inputs = [
                document.getElementById('MainContent_txt_new_pass'),
                document.getElementById('MainContent_txt_confirm_pass')
            ];

            inputs.forEach(input => {
                if (input) {
                    input.addEventListener('input', function () {
                        if (this.value.length > 50) {
                            this.value = this.value.slice(0, 50);
                        }
                    });
                }
            });
        });
    </script>
</asp:Content>
