using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos;
using System.Data.Linq;

namespace Capa_Negocio
{
    public class CN_tbl_tipo_usuario
    {
        private static DataClasses1DataContext dc = new DataClasses1DataContext();

        //consulta que trae toda la información de la tabla tipo_usuario
        public  static List<tbl_tipo_usuario> ListarTipoUsuario()
        {
            var query = dc.tbl_tipo_usuario.Where(tu => tu.tusu_estado == 'A').ToList();
            return query;
        }
    }
}
