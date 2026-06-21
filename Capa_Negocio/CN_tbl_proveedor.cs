using System;
using System.Collections.Generic;
using System.Linq;
using Capa_Datos;
using System.Data.Linq.Mapping;

namespace Capa_Negocio
{
    public class CN_tbl_proveedor
    {
        public static List<tbl_proveedor> traerproveedores()
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor
                    .Where(x => x.prov_estado == 'A')
                    .ToList();
            }
        }

        public static tbl_proveedor traerproveedorxid(int id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor
                    .FirstOrDefault(x => x.prov_id == id);
            }
        }

        public static void save(tbl_proveedor proveedor)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                proveedor.prov_estado = 'A';

                dc.tbl_proveedor.InsertOnSubmit(proveedor);
                dc.SubmitChanges();
            }
        }

        public static void modify(tbl_proveedor proveedor)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                var prov = dc.tbl_proveedor
                    .FirstOrDefault(x => x.prov_id == proveedor.prov_id);

                if (prov != null)
                {
                    prov.prov_nombre = proveedor.prov_nombre;
                    prov.prov_ruc = proveedor.prov_ruc;
                    prov.prov_telefono = proveedor.prov_telefono;
                    prov.prov_correo = proveedor.prov_correo;

                    dc.SubmitChanges();
                }
            }
        }

        public static void elimiLog(tbl_proveedor proveedor)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                var prov = dc.tbl_proveedor
                    .FirstOrDefault(x => x.prov_id == proveedor.prov_id);

                if (prov != null)
                {
                    prov.prov_estado = 'I';
                    dc.SubmitChanges();
                }
            }
        }

        //Eliminar Fisico
        public static void elimiFis(tbl_proveedor proveedor)
        {
            using (var dc = new DataClasses1DataContext())
            {
                var prov = dc.tbl_proveedor.FirstOrDefault(x => x.prov_id == proveedor.prov_id);
                if (prov == null) return;

                // Proceso para "volver al campo anterior" de la tabla hija
                var productos = dc.tbl_productos.Where(p => p.prov_id == prov.prov_id).ToList();

                foreach (var producto in productos)
                {
                    producto.prov_id = null;   
                }

                // Finalmente eliminamos el proveedor
                dc.tbl_proveedor.DeleteOnSubmit(prov);
                dc.SubmitChanges();
            }
        }

        // ==================== MÉTODOS PARA IMPORTACIÓN EXCEL ====================

        /// <summary>
        /// Busca proveedor por nombre (incluye inactivos y del historial si es necesario)
        /// </summary>
        public static tbl_proveedor ObtenerPorNombre(string nombre)
        {
            using (var dc = new DataClasses1DataContext())
            {
                // Primero buscar en la tabla activa
                var proveedor = dc.tbl_proveedor
                    .FirstOrDefault(p => p.prov_nombre.Trim().ToLower() == nombre.Trim().ToLower());

                if (proveedor != null)
                    return proveedor;

                // Si no existe, podríamos reactivar uno del historial (opcional)
                return null;
            }
        }

        /// <summary>
        /// Actualizar proveedor existente
        /// </summary>
        public static void Actualizar(tbl_proveedor proveedor)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                var provExistente = dc.tbl_proveedor
                    .FirstOrDefault(x => x.prov_id == proveedor.prov_id);

                if (provExistente != null)
                {
                    provExistente.prov_nombre = proveedor.prov_nombre;
                    provExistente.prov_ruc = proveedor.prov_ruc;
                    provExistente.prov_telefono = proveedor.prov_telefono;
                    provExistente.prov_correo = proveedor.prov_correo;
                    provExistente.prov_estado = proveedor.prov_estado ?? 'A';

                    dc.SubmitChanges();
                }
            }
        }

        // ==================== VALIDACIONES DE UNICIDAD ====================

        public static bool ExisteNombre(string nombre)
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor.Any(p => p.prov_nombre.Trim().ToLower() == nombre.Trim().ToLower()
                                               && p.prov_estado == 'A');
            }
        }

        public static bool ExisteRUC(string ruc)
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor.Any(p => p.prov_ruc.Trim() == ruc.Trim()
                                               && p.prov_estado == 'A');
            }
        }

        public static bool ExisteTelefono(string telefono)
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor.Any(p => p.prov_telefono.Trim() == telefono.Trim()
                                               && p.prov_estado == 'A');
            }
        }

        public static bool ExisteCorreo(string correo)
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor.Any(p => p.prov_correo.Trim().ToLower() == correo.Trim().ToLower()
                                               && p.prov_estado == 'A');
            }
        }

        // ==================== NUEVOS MÉTODOS ====================

        public static List<tbl_proveedor> traerTodosProveedores()
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor
                         .OrderByDescending(x => x.prov_id)
                         .ToList();
            }
        }

        public static void CambiarEstado(int id, char nuevoEstado)
        {
            using (var dc = new DataClasses1DataContext())
            {
                var prov = dc.tbl_proveedor.FirstOrDefault(x => x.prov_id == id);
                if (prov != null)
                {
                    prov.prov_estado = nuevoEstado;
                    dc.SubmitChanges();
                }
            }
        }

        //EDITAR PROVEEDOR CON VALIDACIONES DE UNICIDAD
        public static bool ExisteNombreEditar(
    string nombre,
    int id)
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor.Any(p =>
                    p.prov_nombre.Trim().ToLower() ==
                    nombre.Trim().ToLower()
                    &&
                    p.prov_id != id
                );
            }
        }

        public static bool ExisteRUCEditar(
            string ruc,
            int id)
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor.Any(p =>
                    p.prov_ruc.Trim() ==
                    ruc.Trim()
                    &&
                    p.prov_id != id
                );
            }
        }

        public static bool ExisteTelefonoEditar(
            string telefono,
            int id)
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor.Any(p =>
                    p.prov_telefono.Trim() ==
                    telefono.Trim()
                    &&
                    p.prov_id != id
                );
            }
        }

        public static bool ExisteCorreoEditar(
            string correo,
            int id)
        {
            using (var dc = new DataClasses1DataContext())
            {
                return dc.tbl_proveedor.Any(p =>
                    p.prov_correo.Trim().ToLower() ==
                    correo.Trim().ToLower()
                    &&
                    p.prov_id != id
                );
            }
        }

        // ==================== GUARDAR CON CONTROL DE ID (Secuencia) ====================

        /// <summary>
        /// Guarda un proveedor respetando la secuencia del último ID registrado
        /// </summary>
        public static void GuardarConSecuencia(tbl_proveedor proveedor)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                proveedor.prov_estado = 'A';

                // Si el ID es 0, dejamos que SQL Server maneje el IDENTITY
                if (proveedor.prov_id == 0)
                {
                    dc.tbl_proveedor.InsertOnSubmit(proveedor);
                    dc.SubmitChanges();
                }
                else
                {
                    // Si queremos forzar un ID específico (por ejemplo desde historial)
                    dc.ExecuteCommand("SET IDENTITY_INSERT tbl_proveedor ON");
                    dc.tbl_proveedor.InsertOnSubmit(proveedor);
                    dc.SubmitChanges();
                    dc.ExecuteCommand("SET IDENTITY_INSERT tbl_proveedor OFF");
                }
            }
        }

        public static void ReactivarProveedorSiExisteEnHistorial(int prov_id)
        {
            using (var dc = new DataClasses1DataContext())
            {
                // 1. Verificar si ya existe (activo o inactivo)
                var proveedorExistente = dc.tbl_proveedor.FirstOrDefault(p => p.prov_id == prov_id);
                if (proveedorExistente != null)
                {
                    if (proveedorExistente.prov_estado == 'I')
                    {
                        proveedorExistente.prov_estado = 'A';
                        dc.SubmitChanges();
                    }
                    return; // Ya existe, no hacer nada más
                }

                // 2. Buscar en el historial
                var historial = dc.ExecuteQuery<ProveedorEliminadoDTO>(
                @"SELECT
                    prov_id,
                    prov_nombre,
                    prov_estado,
                    prov_ruc,
                    prov_telefono,
                    prov_correo
                  FROM tbl_proveedor_eliminado
                  WHERE prov_id = {0}",
                prov_id).FirstOrDefault();

                if (historial == null) return; // No está en historial

                // 3. Reactivar con IDENTITY_INSERT (con mejor manejo)
                try
                {
                    dc.Connection.Open();

                    dc.ExecuteCommand("SET IDENTITY_INSERT tbl_proveedor ON");

                    dc.ExecuteCommand(@"
                        INSERT INTO tbl_proveedor
                        (
                            prov_id,
                            prov_nombre,
                            prov_estado,
                            prov_ruc,
                            prov_telefono,
                            prov_correo
                        )
                        VALUES
                        (
                            {0},
                            {1},
                            {2},
                            {3},
                            {4},
                            {5}
                        )",
                            historial.prov_id,
                            historial.prov_nombre,
                            historial.prov_estado,
                            historial.prov_ruc,
                            historial.prov_telefono,
                            historial.prov_correo);



                    dc.ExecuteCommand("SET IDENTITY_INSERT tbl_proveedor OFF");

                    // Eliminar del historial
                    dc.ExecuteCommand("DELETE FROM tbl_proveedor_eliminado WHERE prov_id = {0}", prov_id);
                }
                catch (Exception)
                {
                    // Asegurar que se apague aunque falle
                    try { dc.ExecuteCommand("SET IDENTITY_INSERT tbl_proveedor OFF"); } catch { }
                    throw;
                }
                finally
                {
                    if (dc.Connection.State == System.Data.ConnectionState.Open)
                        dc.Connection.Close();
                }
            }
        }

        public class ProveedorEliminadoDTO
        {
            public int prov_id { get; set; }
            public string prov_nombre { get; set; }
            public char prov_estado { get; set; }
            public string prov_ruc { get; set; }
            public string prov_telefono { get; set; }
            public string prov_correo { get; set; }
        }
    }
}