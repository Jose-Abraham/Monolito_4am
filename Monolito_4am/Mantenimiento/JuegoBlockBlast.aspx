<%@ Page Title="Block Blast - Juego" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master"
    AutoEventWireup="true" CodeBehind="JuegoBlockBlast.aspx.cs" Inherits="Monolito_4am.Mantenimiento.JuegoBlockBlast" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <style>
            .game-container {
                display: flex;
                flex-direction: column;
                align-items: center;
                gap: 20px;
                padding: 10px;
                background: #5d4037;
                border-radius: 30px;
                box-shadow: 0 10px 40px rgba(0, 0, 0, 0.5);
                border: 5px solid #4e342e;
                max-width: 550px;
                margin: 0 auto;
            }

            .game-header {
                display: flex;
                justify-content: space-between;
                width: 100%;
                padding: 10px 20px;
            }

            .score-box {
                text-align: center;
                color: #fff;
            }

            .score-box div {
                font-size: 2.2rem;
                font-weight: 900;
                text-shadow: 2px 2px #000;
            }

            #grid {
                display: grid;
                grid-template-columns: repeat(8, 52px);
                grid-template-rows: repeat(8, 52px);
                gap: 4px;
                background: #3e2723;
                padding: 10px;
                border-radius: 10px;
                border: 4px solid #2d1d1a;
            }

            .cell {
                width: 52px;
                height: 52px;
                background: #4e342e;
                border-radius: 4px;
                box-shadow: inset 2px 2px 5px rgba(0, 0, 0, 0.2);
            }

            /* COLORES DE BLOQUES PREMIUM */
            .color-1 {
                background: #4fc3f7;
                box-shadow: inset -4px -4px #0288d1, inset 4px 4px #b3e5fc;
            }

            /* Azul */
            .color-2 {
                background: #81c784;
                box-shadow: inset -4px -4px #388e3c, inset 4px 4px #c8e6c9;
            }

            /* Verde */
            .color-3 {
                background: #fff176;
                box-shadow: inset -4px -4px #fbc02d, inset 4px 4px #fff9c4;
            }

            /* Amarillo */
            .color-4 {
                background: #ff8a65;
                box-shadow: inset -4px -4px #d84315, inset 4px 4px #ffccbc;
            }

            /* Naranja */
            .color-5 {
                background: #ba68c8;
                box-shadow: inset -4px -4px #7b1fa2, inset 4px 4px #e1bee7;
            }

            /* Púrpura */

            .pieces-container {
                display: flex;
                gap: 20px;
                margin-top: 25px;
                min-height: 140px;
                justify-content: center;
                width: 100%;
                background: rgba(0, 0, 0, 0.1);
                padding: 15px;
                border-radius: 20px;
            }

            .piece {
                cursor: grab;
                display: grid;
                gap: 2px;
                transition: 0.1s;
                transform-origin: top left;
            }

            .piece:active {
                cursor: grabbing;
                transform: scale(1.1);
            }

            .block {
                width: 30px;
                height: 30px;
                border-radius: 4px;
            }

            .game-title {
                font-size: 3.5rem;
                font-weight: 900;
                text-align: center;
                margin-bottom: 5px;
                letter-spacing: -2px;
                text-transform: uppercase;
                background: linear-gradient(to bottom, #4fc3f7 0%, #ba68c8 50%, #ff8a65 100%);
                -webkit-background-clip: text;
                -webkit-text-fill-color: transparent;
                filter: drop-shadow(4px 4px 0px #2d1d1a);
                font-family: 'Arial Black', sans-serif;
            }
        </style>

        <div class="game-container">
            <h1 class="game-title">BLOCK BLAST</h1>
            <div class="game-header">
                <div class="score-box">
                    <div id="current-score">0</div>
                    <small>PUNTOS</small>
                </div>
                <div style="color: #ffd54f; text-align: center;">
                    <i class="fas fa-crown" style="font-size: 1.5rem;"></i><br />
                    <span id="high-score" style="font-size: 1.5rem; font-weight: 800;">0</span>
                </div>
            </div>

            <div id="grid"></div>
            <div class="pieces-container" id="pieces"></div>

            <button type="button" onclick="resetServerGame()" class="btnLogout"
                style="border: none; color: #fff; background: #3e2723; padding: 10px 30px; margin: 15px 0;">REINICIAR</button>
        </div>

        <script type="text/javascript">
            // @ts-nocheck
            let currentPiecesData = [];

            function renderGame(state) {
                if (!state) return;
                if (state.isGameOver) {
                    Swal.fire({
                        title: '¡GAME OVER!',
                        text: `Puntuación final: ${state.score}. ¡Ya no hay más movimientos posibles!`,
                        icon: 'warning',
                        confirmButtonText: 'Reintentar',
                        background: '#3e2723',
                        color: '#fff',
                        confirmButtonColor: '#ffd54f'
                    }).then(() => resetServerGame());
                }

                // Render Tablero
                const gridEl = document.getElementById('grid');
                gridEl.innerHTML = '';
                state.grid.forEach((row, r) => {
                    row.forEach((val, c) => {
                        const cell = document.createElement('div');
                        cell.className = val > 0 ? `cell color-${val}` : 'cell';
                        cell.dataset.r = r;
                        cell.dataset.c = c;
                        gridEl.appendChild(cell);
                    });
                });

                // Render Piezas
                const piecesEl = document.getElementById('pieces');
                piecesEl.innerHTML = '';
                currentPiecesData = state.pieces;
                state.pieces.forEach((p, index) => {
                    const pieceDiv = document.createElement('div');
                    pieceDiv.className = 'piece';
                    pieceDiv.draggable = true;
                    pieceDiv.style.gridTemplateColumns = `repeat(${p.shape[0].length}, 30px)`;

                    p.shape.forEach(row => {
                        row.forEach(val => {
                            const b = document.createElement('div');
                            b.className = val === 1 ? `block color-${p.color}` : 'block';
                            if (val === 0) b.style.visibility = 'hidden';
                            pieceDiv.appendChild(b);
                        });
                    });

                    pieceDiv.addEventListener('dragstart', (e) => {
                        e.dataTransfer.setData('pieceIndex', index);
                    });
                    piecesEl.appendChild(pieceDiv);
                });

                document.getElementById('current-score').innerText = state.score;
                document.getElementById('high-score').innerText = state.highScore;
            }

            function loadGameState() { PageMethods.GetGameState(renderGame); }
            function resetServerGame() { PageMethods.ResetGame(renderGame); }

            document.getElementById('grid').addEventListener('dragover', (e) => e.preventDefault());
            document.getElementById('grid').addEventListener('drop', (e) => {
                e.preventDefault();
                const idx = parseInt(e.dataTransfer.getData('pieceIndex'));
                const target = e.target.closest('.cell');
                if (!target || isNaN(idx)) return;

                // Para que la pieza encaje mejor, el punto donde sueltas es el r,c inicial
                const r = parseInt(target.dataset.r);
                const c = parseInt(target.dataset.c);

                PageMethods.PlacePiece(idx, r, c, (res) => {
                    if (res.success) renderGame(res);
                    else console.log(res.message);
                });
            });

            window.onload = loadGameState;
        </script>
    </asp:Content>