using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using Capa_Negocio;

namespace Monolito_4am.Mantenimiento
{
    [Serializable]
    public class BlockBlastState
    {
        public int[,] Grid = new int[8, 8]; // 0: vacío, 1-5: colores
        public List<PieceData> Pieces = new List<PieceData>();
        public int Score = 0;
        public int HighScore = 0;
        public bool IsGameOver = false;
    }

    [Serializable]
    public class PieceData
    {
        public int[,] Shape;
        public int Color;
    }

    public partial class JuegoBlockBlast : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["cedula"] == null) Response.Redirect("~/Seguridad/Login.aspx");
            if (!IsPostBack && Session["GameState"] == null) InitNewGame();
        }

        private void InitNewGame()
        {
            string cedula = Session["cedula"].ToString();
            var state = new BlockBlastState();
            state.HighScore = CN_tbl_usario.ObtenerRecord(cedula);
            state.Pieces = GenerateNewPieces();
            Session["GameState"] = state;
        }

        [WebMethod(EnableSession = true)]
        public static object GetGameState()
        {
            var state = (BlockBlastState)HttpContext.Current.Session["GameState"];
            if (state == null) return null;
            return FormatState(state);
        }

        [WebMethod(EnableSession = true)]
        public static object PlacePiece(int pieceIndex, int row, int col)
        {
            var state = (BlockBlastState)HttpContext.Current.Session["GameState"];
            if (state == null || state.IsGameOver || pieceIndex >= state.Pieces.Count) 
                return new { success = false };

            var piece = state.Pieces[pieceIndex];
            
            if (!CanPlace(state.Grid, piece.Shape, row, col))
                return new { success = false, message = "No cabe" };

            // Colocar con color
            for (int r = 0; r < piece.Shape.GetLength(0); r++) {
                for (int c = 0; c < piece.Shape.GetLength(1); c++) {
                    if (piece.Shape[r, c] == 1) state.Grid[row + r, col + c] = piece.Color;
                }
            }

            state.Score += 10;
            state.Pieces.RemoveAt(pieceIndex);

            // Limpiar líneas
            state.Score += ClearLines(state.Grid) * 100;

            // Nuevas piezas
            if (state.Pieces.Count == 0) state.Pieces = GenerateNewPieces();

            // Verificar Game Over REAL
            if (!AnyMovePossible(state))
            {
                state.IsGameOver = true;
            }

            // Récord
            if (state.Score > state.HighScore)
            {
                state.HighScore = state.Score;
                CN_tbl_usario.GuardarRecord(HttpContext.Current.Session["cedula"].ToString(), state.HighScore);
            }

            HttpContext.Current.Session["GameState"] = state;
            return FormatState(state);
        }

        [WebMethod(EnableSession = true)]
        public static object ResetGame()
        {
            var state = (BlockBlastState)HttpContext.Current.Session["GameState"];
            int hi = state != null ? state.HighScore : 0;
            var newState = new BlockBlastState { HighScore = hi, Pieces = GenerateNewPieces() };
            HttpContext.Current.Session["GameState"] = newState;
            return FormatState(newState);
        }

        private static bool AnyMovePossible(BlockBlastState state)
        {
            foreach (var piece in state.Pieces)
            {
                for (int r = 0; r < 8; r++) {
                    for (int c = 0; c < 8; c++) {
                        if (CanPlace(state.Grid, piece.Shape, r, c)) return true;
                    }
                }
            }
            return false;
        }

        private static bool CanPlace(int[,] grid, int[,] shape, int row, int col)
        {
            if (row < 0 || col < 0) return false;
            for (int r = 0; r < shape.GetLength(0); r++) {
                for (int c = 0; c < shape.GetLength(1); c++) {
                    if (shape[r, c] == 1) {
                        int nr = row + r;
                        int nc = col + c;
                        if (nr >= 8 || nc >= 8 || grid[nr, nc] != 0) return false;
                    }
                }
            }
            return true;
        }

        private static int ClearLines(int[,] grid)
        {
            var rToClear = new List<int>();
            var cToClear = new List<int>();
            for (int r = 0; r < 8; r++) {
                bool full = true;
                for (int c = 0; c < 8; c++) if (grid[r, c] == 0) full = false;
                if (full) rToClear.Add(r);
            }
            for (int c = 0; c < 8; c++) {
                bool full = true;
                for (int r = 0; r < 8; r++) if (grid[r, c] == 0) full = false;
                if (full) cToClear.Add(c);
            }
            foreach (int r in rToClear) for (int c = 0; c < 8; c++) grid[r, c] = 0;
            foreach (int c in cToClear) for (int r = 0; r < 8; r++) grid[r, c] = 0;
            return rToClear.Count + cToClear.Count;
        }

        private static List<PieceData> GenerateNewPieces()
        {
            var rnd = new Random();
            var list = new List<PieceData>();
            int[][,] shapes = {
                new int[,] {{1,1},{1,1}}, new int[,] {{1,1,1,1}}, new int[,] {{1},{1},{1},{1}},
                new int[,] {{1,1,1}}, new int[,] {{1,1,1},{0,1,0}}, new int[,] {{1,0},{1,0},{1,1}},
                new int[,] {{1,1,0},{0,1,1}}, new int[,] {{1}}
            };
            for (int i = 0; i < 3; i++) {
                list.Add(new PieceData { Shape = shapes[rnd.Next(shapes.Length)], Color = rnd.Next(1, 6) });
            }
            return list;
        }

        private static object FormatState(BlockBlastState s)
        {
            return new {
                success = true,
                grid = MatrixToList(s.Grid),
                pieces = s.Pieces.Select(p => new { shape = MatrixToList(p.Shape), color = p.Color }).ToList(),
                score = s.Score,
                highScore = s.HighScore,
                isGameOver = s.IsGameOver
            };
        }

        private static List<List<int>> MatrixToList(int[,] m)
        {
            var l = new List<List<int>>();
            for (int r = 0; r < m.GetLength(0); r++) {
                var row = new List<int>();
                for (int c = 0; c < m.GetLength(1); c++) row.Add(m[r, c]);
                l.Add(row);
            }
            return l;
        }
    }
}
