using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirLineApi.Models
{
    public class ResponseDB
    {

        public ResponseDB()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Propiedades

        /// <summary>
        /// Identificador de la respuesta.
        /// </summary>
        public Boolean Resp
        {
            get;
            set;
        }

        /// <summary>
        /// jsonData array data (string)
        /// </summary>
        public string jsonData
        {
            get;
            set;
        }

        /// <summary>
        /// Contenido del mensaje.
        /// </summary>
        public string Msg
        {
            get;
            set;
        }

        /// <summary>
        /// Rows
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Rows error
        /// </summary>
        public int Error
        {
            get;
            set;
        }

        public DataTable dtResult
        {
            get;
            set;
        }

        public Guid guidResult
        {
            get;
            set;
        }

        #endregion
    }

}
