using Capa_Datos;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace Capa_Negocio
{
    public class CN_tbl_fotos_productos
    {
        // ============================
        // LISTAR TODAS LAS IMÁGENES
        // ============================
        public static List<tbl_fotos_productos> ListarTodas()
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<tbl_fotos_productos>(f => f.tbl_productos);

                dc.LoadOptions = options;

                return dc.tbl_fotos_productos
                    .OrderByDescending(f => f.fpro_id)
                    .ToList();
            }
        }

        // ============================
        // LISTAR POR PRODUCTO
        // ============================
        public static List<tbl_fotos_productos> ListarPorProducto(int pro_id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                return dc.tbl_fotos_productos
                    .Where(f => f.pro_id == pro_id)
                    .OrderBy(f => f.fpro_es_principal == true ? 0 : 1)
                    .ToList();
            }
        }

        // ============================
        // GUARDAR NUEVA IMAGEN
        // ============================
        public static int Guardar(tbl_fotos_productos foto)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                dc.tbl_fotos_productos.InsertOnSubmit(foto);
                dc.SubmitChanges();
                return foto.fpro_id;
            }
        }

        // ============================
        // ELIMINAR FÍSICO
        // ============================
        public static void EliminarFisico(int fpro_id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                var foto = dc.tbl_fotos_productos.FirstOrDefault(f => f.fpro_id == fpro_id);
                if (foto != null)
                {
                    dc.tbl_fotos_productos.DeleteOnSubmit(foto);
                    dc.SubmitChanges();
                }
            }
        }

        // ============================
        // VERIFICAR SI EL PRODUCTO YA TIENE FOTO PRINCIPAL
        // ============================
        public static bool TieneFotoPrincipal(int pro_id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                return dc.tbl_fotos_productos
                    .Any(f => f.pro_id == pro_id && f.fpro_es_principal == true);
            }
        }

        // ============================
        // MARCAR COMO PRINCIPAL (Desmarca las anteriores)
        // ============================
        public static void MarcarComoPrincipal(int fpro_id, int pro_id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                // Desmarcar todas las fotos del producto
                var fotos = dc.tbl_fotos_productos.Where(f => f.pro_id == pro_id).ToList();
                foreach (var f in fotos)
                {
                    f.fpro_es_principal = false;
                }

                // Si se pasó un ID válido, marcar esa como principal
                if (fpro_id > 0)
                {
                    var fotoPrincipal = fotos.FirstOrDefault(f => f.fpro_id == fpro_id);
                    if (fotoPrincipal != null)
                    {
                        fotoPrincipal.fpro_es_principal = true;
                    }
                }

                dc.SubmitChanges();
            }
        }

        public static tbl_fotos_productos ObtenerPorId(int fpro_id)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<tbl_fotos_productos>(f => f.tbl_productos);  // ← Importante
                dc.LoadOptions = options;

                return dc.tbl_fotos_productos
                          .FirstOrDefault(f => f.fpro_id == fpro_id);
            }
        }

        // ============================
        // ACTUALIZAR FOTO
        // ============================
        public static void Actualizar(tbl_fotos_productos foto)
        {
            using (DataClasses1DataContext dc = new DataClasses1DataContext())
            {
                var fotoExistente = dc.tbl_fotos_productos.FirstOrDefault(f => f.fpro_id == foto.fpro_id);
                if (fotoExistente != null)
                {
                    fotoExistente.pro_id = foto.pro_id;
                    fotoExistente.fpro_ruta_imagen = foto.fpro_ruta_imagen;
                    fotoExistente.fpro_nombre_archivo = foto.fpro_nombre_archivo;
                    fotoExistente.fpro_es_principal = foto.fpro_es_principal;
                    fotoExistente.fpro_fecha_subida = foto.fpro_fecha_subida;

                    dc.SubmitChanges();
                }
            }
        }
    }
}