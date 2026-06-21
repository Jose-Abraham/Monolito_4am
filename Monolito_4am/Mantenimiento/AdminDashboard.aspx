<%@ Page Title="" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="Monolito_4am.Mantenimiento.AdminDashboard" %>
<asp:Content
    ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
    <style>
        .swal-pass-wrapper {
            position: relative;
            display: flex;
            align-items: center;
            width: 100%;
            margin-top: 10px;
        }
        .swal-pass-input {
            width: 100%;
            padding: 12px 45px 12px 15px !important;
            border-radius: 10px !important;
            background: rgba(0,0,0,0.2) !important;
            border: 1px solid rgba(255,255,255,0.1) !important;
            color: white !important;
        }
        .swal-toggle-eye {
            position: absolute;
            right: 15px;
            cursor: pointer;
            color: #94a3b8;
            z-index: 10;
            display: flex;
            align-items: center;
        }
    </style>

    <div class="dashboard-header" style="margin-bottom: 30px;">
        <h1 style="font-weight: 700; color: white;">Gestión de Usuarios</h1>
        <p style="color: var(--text-muted);">Administra la seguridad y accesos de los usuarios del sistema.</p>
    </div>

    <!-- Buscador -->
    <div class="search-section" style="margin-bottom: 40px; display: flex; gap: 15px;">
        <asp:TextBox ID="txt_buscar" runat="server" CssClass="search-input" placeholder="Buscar por nombre, nick o cédula..." 
            style="flex: 1; padding: 15px 20px; border-radius: 12px; border: 1px solid rgba(255,255,255,0.1); background: rgba(255,255,255,0.05); color: white;"></asp:TextBox>
        
        <asp:Button ID="btn_buscar" runat="server" Text="Buscar" OnClick="btn_buscar_Click"
            style="padding: 0 30px; border-radius: 12px; border: none; background: var(--primary); color: white; font-weight: 600; cursor: pointer;" />
        
        <asp:Button ID="btn_limpiar" runat="server" Text="Limpiar" OnClick="btn_limpiar_Click"
            style="padding: 0 30px; border-radius: 12px; border: 1px solid rgba(255,255,255,0.2); background: transparent; color: white; font-weight: 600; cursor: pointer; transition: 0.3s;" />
    </div>

    <!-- Lista de Usuarios -->
    <div class="users-grid" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 25px;">
        <asp:Repeater ID="rpt_usuarios" runat="server">
            <ItemTemplate>
                <div class="user-card" style="background: rgba(255,255,255,0.03); border: 1px solid rgba(255,255,255,0.08); border-radius: 20px; padding: 25px; transition: 0.3s; position: relative; overflow: hidden;">
                    
                    <!-- Badge de Estado -->
                    <div style='position: absolute; top: 15px; right: 15px; padding: 5px 12px; border-radius: 20px; font-size: 0.75rem; font-weight: 700; background: <%# (bool)Eval("usu_bloqueado") ? "#ef4444" : "#22c55e" %>; color: white;'>
                        <%# (bool)Eval("usu_bloqueado") ? "BLOQUEADO" : "ACTIVO" %>
                    </div>

                    <div style="display: flex; align-items: center; gap: 20px; margin-bottom: 20px;">
                        <img src='<%# Eval("FotoPerfil") != null ? "data:image/png;base64," + Convert.ToBase64String((byte[])Eval("FotoPerfil")) : "https://cdn-icons-png.flaticon.com/512/3135/3135715.png" %>' 
                             style="width: 70px; height: 70px; border-radius: 50%; object-fit: cover; border: 2px solid var(--primary);" />
                        <div>
                            <h3 style="color: white; font-size: 1.1rem; margin-bottom: 5px;"><%# Eval("NombreCompleto") %></h3>
                            <p style="color: var(--text-muted); font-size: 0.9rem;">@<%# Eval("usu_nick") %></p>
                            <small style="color: var(--primary);"><%# Eval("usu_cedula") %></small>
                        </div>
                    </div>

                    <div style="display: flex; flex-direction: column; gap: 10px;">
                        <!-- Botón Desbloquear (Solo si está bloqueado) -->
                        <asp:LinkButton ID="btn_desbloquear" runat="server" 
                            Visible='<%# (bool)Eval("usu_bloqueado") %>'
                            CommandArgument='<%# Eval("usu_cedula") %>'
                            OnCommand="btn_desbloquear_Command"
                            style="text-align: center; padding: 12px; border-radius: 10px; background: #22c55e; color: white; text-decoration: none; font-weight: 600; font-size: 0.9rem;">
                            <i class="fas fa-unlock"></i> Desbloquear Usuario
                        </asp:LinkButton>

                        <!-- Botón Cambiar Password -->
                        <asp:LinkButton ID="btn_pass" runat="server" 
                            CommandArgument='<%# Eval("usu_cedula") %>'
                            OnCommand="btn_pass_Command"
                            style="text-align: center; padding: 12px; border-radius: 10px; background: rgba(255,255,255,0.05); color: white; text-decoration: none; font-weight: 600; font-size: 0.9rem; border: 1px solid rgba(255,255,255,0.1);">
                            <i class="fas fa-key"></i> Resetear Password
                        </asp:LinkButton>
                    </div>

                    <div style="margin-top: 15px; text-align: center;">
                        <small style="color: var(--text-muted);">Intentos fallidos: <span style="color: white;"><%# Eval("usu_intentos") %></span></small>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <!-- Scripts de SweetAlert para el Dashboard -->
    <script type="text/javascript">
        // @ts-nocheck
        /* global Swal, __doPostBack */

        function modalCambiarPass(cedula) {
            Swal.fire({
                title: 'Cambiar Contraseña',
                html: `
                    <p style="color: #94a3b8; margin-bottom: 15px;">Ingrese la nueva clave para ${cedula}</p>
                    <div class="swal-pass-wrapper">
                        <input type="password" id="swal-input-pass" class="swal-pass-input" placeholder="Mínimo 4 caracteres..." maxlength="50">
                        <span class="swal-toggle-eye" onclick="toggleSwalPass()">
                            <svg id="swal-eye-open" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                            <svg id="swal-eye-closed" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="display:none;"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path><line x1="1" y1="1" x2="23" y2="23"></line></svg>
                        </span>
                    </div>
                `,
                showCancelButton: true,
                confirmButtonText: 'Actualizar',
                preConfirm: () => {
                    const pass = document.getElementById('swal-input-pass').value;
                    if (!pass || pass.length < 4) {
                        Swal.showValidationMessage('La clave debe tener al menos 4 caracteres');
                        return false;
                    }
                    return pass;
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    __doPostBack('cambiar_clave_confirm', cedula + '|' + result.value);
                }
            });
        }

        function toggleSwalPass() {
            const input = document.getElementById('swal-input-pass');
            const open = document.getElementById('swal-eye-open');
            const closed = document.getElementById('swal-eye-closed');
            
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
    </script>

</asp:Content>
