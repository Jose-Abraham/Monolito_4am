using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq;

namespace Capa_Negocio
{
    public class CN_tbl_producto
    {
        // ============================
        // LISTAR SOLO ACTIVOS
        // ============================
        public static List<tbl_productos> traerproductos()
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                return dc.tbl_productos
                    .Where(p => p.pro_estado == 'A')
                    .OrderByDescending(p => p.pro_id)
                    .ToList();
            }
        }

        // ============================
        // LISTAR TODOS
        // ============================
        public static List<tbl_productos> traerTodosProductos()
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                DataLoadOptions opciones =
                    new DataLoadOptions();

                opciones.LoadWith<tbl_productos>(
                    x => x.tbl_proveedor);

                opciones.LoadWith<tbl_productos>(
                    x => x.tbl_fotos_productos);

                dc.LoadOptions = opciones;

                return dc.tbl_productos
                    .OrderByDescending(p => p.pro_id)
                    .ToList();
            }
        }

        // ============================
        // TRAER POR ID
        // ============================
        public static tbl_productos traerproductoxid(int id)
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                return dc.tbl_productos
                    .FirstOrDefault(p => p.pro_id == id);
            }
        }

        // ============================
        // BUSCAR POR NOMBRE
        // ============================
        public static List<tbl_productos>
            buscarproductospornombre(string nombre)
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                return dc.tbl_productos
                    .Where(p =>
                        p.pro_nombre.Contains(nombre))
                    .ToList();
            }
        }

        // ============================
        // GUARDAR
        // ============================
        public static int save(tbl_productos producto)
        {
            int idGenerado = 0;

            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                producto.pro_estado = 'A';

                dc.tbl_productos.InsertOnSubmit(producto);

                dc.SubmitChanges();

                idGenerado = producto.pro_id;
            }

            return idGenerado;
        }

        // ============================
        // MODIFICAR
        // ============================
        public static void modify(tbl_productos producto)
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                var pro = dc.tbl_productos
                    .FirstOrDefault(x =>
                        x.pro_id == producto.pro_id);

                if (pro != null)
                {
                    pro.pro_nombre = producto.pro_nombre;
                    pro.pro_codigo = producto.pro_codigo;
                    pro.pro_descripcion = producto.pro_descripcion;
                    pro.pro_cantidad = producto.pro_cantidad;
                    pro.pro_precio = producto.pro_precio;
                    pro.pro_estado = producto.pro_estado;
                    pro.prov_id = producto.prov_id;

                    dc.SubmitChanges();
                }
            }
        }

        // ============================
        // ELIMINAR FÍSICO
        // ============================
        public static void delete(tbl_productos producto)
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                var pro = dc.tbl_productos
                    .FirstOrDefault(x =>
                        x.pro_id == producto.pro_id);

                if (pro != null)
                {
                    dc.tbl_productos.DeleteOnSubmit(pro);

                    dc.SubmitChanges();
                }
            }
        }

        // ============================
        // ELIMINAR LÓGICO
        // ============================
        public static void elimiLog(tbl_productos producto)
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                var pro = dc.tbl_productos
                    .FirstOrDefault(x =>
                        x.pro_id == producto.pro_id);

                if (pro != null)
                {
                    pro.pro_estado = 'I';

                    dc.SubmitChanges();
                }
            }
        }

        // ============================
        // CAMBIAR ESTADO
        // ============================
        public static void CambiarEstado(
            int id,
            char nuevoEstado)
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                var pro = dc.tbl_productos
                    .FirstOrDefault(x => x.pro_id == id);

                if (pro != null)
                {
                    pro.pro_estado = nuevoEstado;

                    dc.SubmitChanges();
                }
            }
        }

        // ============================
        // VALIDACIONES
        // ============================

        public static bool ExisteNombre(
            string nombre,
            int idExcluir = 0)
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                return dc.tbl_productos.Any(p =>
                    p.pro_nombre.Trim().ToLower()
                    == nombre.Trim().ToLower()
                    &&
                    p.pro_id != idExcluir);
            }
        }

        public static bool ExisteCodigo(
            string codigo,
            int idExcluir = 0)
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                return dc.tbl_productos.Any(p =>
                    p.pro_codigo.Trim().ToLower()
                    == codigo.Trim().ToLower()
                    &&
                    p.pro_id != idExcluir);
            }
        }

        public static tbl_productos traerproductodetalle(int id)
        {
            using (DataClasses1DataContext dc =
                new DataClasses1DataContext())
            {
                DataLoadOptions opciones =
                    new DataLoadOptions();

                opciones.LoadWith<tbl_productos>(
                    p => p.tbl_proveedor);

                opciones.LoadWith<tbl_productos>(
                    p => p.tbl_fotos_productos);

                dc.LoadOptions = opciones;

                return dc.tbl_productos
                    .FirstOrDefault(p => p.pro_id == id);
            }
        }

        // ============================
        // VERIFICAR SI EXISTE EL PRODUCTO (para importación de fotos)
        // ============================
        public static bool ExisteProducto(int pro_id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                return dc.tbl_productos
                    .Any(p => p.pro_id == pro_id);
            }
        }

        // ============================
        // VERIFICAR SI EL PRODUCTO ESTÁ ACTIVO
        // ============================
        public static bool ProductoEstaActivo(int pro_id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                return dc.tbl_productos
                    .Any(p => p.pro_id == pro_id && p.pro_estado == 'A');
            }
        }

        // ============================
        // VERIFICAR SI EL PRODUCTO EXISTE EN HISTORIAL
        // ============================
        public static bool ExisteEnHistorial(int pro_id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                return dc.ExecuteQuery<int>(
                    "SELECT COUNT(*) FROM tbl_productos_eliminado WHERE pro_id = {0}",
                    pro_id).FirstOrDefault() > 0;
            }
        }

        // ============================
        // REACTIVAR PRODUCTO DESDE HISTORIAL
        // ============================
        public static void ReactivarProductoDesdeHistorial(int pro_id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                try
                {
                    dc.Connection.Open();
                    dc.ExecuteCommand("SET IDENTITY_INSERT tbl_productos ON");

                    dc.ExecuteCommand(@"
                INSERT INTO tbl_productos 
                (pro_id, pro_nombre, pro_codigo, pro_descripcion, 
                 pro_cantidad, pro_precio, prov_id, pro_estado)
                SELECT 
                    pro_id, pro_nombre, pro_codigo, pro_descripcion, 
                    pro_cantidad, pro_precio, prov_id, 'A'
                FROM tbl_productos_eliminado 
                WHERE pro_id = {0}", pro_id);

                    dc.ExecuteCommand("SET IDENTITY_INSERT tbl_productos OFF");

                    // Eliminar del historial
                    dc.ExecuteCommand("DELETE FROM tbl_productos_eliminado WHERE pro_id = {0}", pro_id);
                }
                catch
                {
                    try { dc.ExecuteCommand("SET IDENTITY_INSERT tbl_productos OFF"); } catch { }
                    throw;
                }
                finally
                {
                    if (dc.Connection.State == System.Data.ConnectionState.Open)
                        dc.Connection.Close();
                }
            }
        }
    }
}
