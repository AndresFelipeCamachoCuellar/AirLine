using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using AirLineApi.Models;
using System.Linq;
using Newtonsoft.Json;

namespace AirLineApi.Util
{
    public class Connection
    {
        public Connection(string strSQLPass, string strServer, string strDBName, string strSQLUser)
        {
            try
            {
                SqlConnectionStringBuilder bldr = new SqlConnectionStringBuilder(strConnection)
                {
                    IntegratedSecurity = true,
                    DataSource = strServer,
                    InitialCatalog = strDBName,
                    UserID = strSQLUser,
                    Password = strSQLPass
                };

                strConnection = bldr.ConnectionString;
                blError = false;
            }
            catch (Exception)
            {
                blError = true;
            }
        }

        public ResponseDB InsData(string strQuery, string[] arrayParam, string[] arrayValue)
        {
            ResponseDB objResponseDB = new ResponseDB
            {
                Resp = true
            };

            SqlConnection objSQLConnection;

            try
            {
                objSQLConnection = new SqlConnection(strConnection);

                try
                {
                    objSQLConnection.Open();

                    if (objSQLConnection.State != System.Data.ConnectionState.Open)
                    {
                        objResponseDB.Resp = false;
                        objResponseDB.Msg = "The connection status is: " + objSQLConnection.State.ToString();

                        objSQLConnection.Close();
                    }
                    else
                    {
                        try
                        {
                            objResponseDB.Count = 0;
                            objResponseDB.Error = 0;

                            try
                            {
                                SqlCommand cmd = new SqlCommand(strQuery, objSQLConnection);

                                cmd.CommandText = strQuery;

                                for (int j = 0; j < arrayParam.Length; j++)
                                {
                                    if (arrayValue[j] != null)
                                    {
                                        cmd.Parameters.Add(new SqlParameter(arrayParam[j], arrayValue[j]));
                                    }
                                    else
                                    {
                                        cmd.Parameters.Add(new SqlParameter(arrayParam[j], DBNull.Value));
                                    }
                                }

                                Guid guidPK = (Guid)cmd.ExecuteScalar();

                                if (guidPK == Guid.Empty)
                                {
                                    objResponseDB.Error++;
                                }
                                else
                                {
                                    objResponseDB.guidResult = guidPK;
                                    objResponseDB.Count++;
                                }

                                //Guid guidPK = new Guid();

                                //var result = cmd.ExecuteScalar();

                                //if (result is Guid)
                                //{
                                //    guidPK = (Guid)result;
                                //}
                                //else
                                //{
                                //    guidPK = Guid.Parse("00000000-0000-0000-0000-000000000001");
                                //}
                            }
                            catch (Exception innerEx)
                            {
                                objResponseDB.Msg = innerEx.Message;
                                objResponseDB.Error++;
                            }

                            objSQLConnection.Close();

                        }
                        catch (SqlException ex)
                        {
                            // 5. Close the connection
                            if (objSQLConnection != null)
                            {
                                objResponseDB.Resp = false;
                                objResponseDB.Msg = ex.Message;
                                objSQLConnection.Close();
                            }
                        }

                    }

                }
                // Catch errors specific to the Open method
                catch (SqlException ex)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Error opening the connection:: " + ex.Message;
                }
                catch (InvalidOperationException ix)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Invalid Operation error: " + ix.Message;
                }
                catch (ConfigurationErrorsException cx)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Configuration error: " + cx.Message;
                }

            }
            catch (ArgumentException ax)  // there was something wrong in the connection string.
            {
                objResponseDB.Resp = false;
                objResponseDB.Msg = "Error creating the connection: " + ax.Message;
            }

            return objResponseDB;
        }

        public ResponseDB UpdData(string strQuery, string[] arrayParam, string[] arrayValue)
        {
            ResponseDB objResponseDB = new ResponseDB
            {
                Resp = true
            };

            SqlConnection objSQLConnection;

            try
            {
                objSQLConnection = new SqlConnection(strConnection);

                try
                {
                    objSQLConnection.Open();

                    if (objSQLConnection.State != System.Data.ConnectionState.Open)
                    {
                        objResponseDB.Resp = false;
                        objResponseDB.Msg = "The connection status is: " + objSQLConnection.State.ToString();

                        objSQLConnection.Close();
                    }
                    else
                    {

                        try
                        {

                            objResponseDB.Count = 0;
                            objResponseDB.Error = 0;

                            try
                            {

                                SqlCommand cmd = new SqlCommand(strQuery, objSQLConnection);

                                cmd.CommandText = strQuery;

                                for (int j = 0; j < arrayParam.Length; j++)
                                {
                                    if (arrayValue[j] != null)
                                    {
                                        cmd.Parameters.Add(new SqlParameter(arrayParam[j], arrayValue[j]));
                                    }
                                    else
                                    {
                                        cmd.Parameters.Add(new SqlParameter(arrayParam[j], DBNull.Value));
                                    }
                                }

                                int nuRows = cmd.ExecuteNonQuery();

                                if (nuRows == 0)
                                {
                                    objResponseDB.Error++;
                                }
                                else
                                {
                                    objResponseDB.Count = nuRows;
                                }

                            }
                            catch (Exception innerEx)
                            {
                                objResponseDB.Msg = innerEx.Message;
                                objResponseDB.Error++;
                            }


                            objSQLConnection.Close();

                        }
                        catch (SqlException ex)
                        {
                            // 5. Close the connection
                            if (objSQLConnection != null)
                            {
                                objResponseDB.Resp = false;
                                objResponseDB.Msg = ex.Message;
                                objSQLConnection.Close();
                            }
                        }

                    }

                }
                // Catch errors specific to the Open method
                catch (SqlException ex)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Error opening the connection:: " + ex.Message;
                }
                catch (InvalidOperationException ix)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Invalid Operation error: " + ix.Message;
                }
                catch (ConfigurationErrorsException cx)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Configuration error: " + cx.Message;
                }

            }
            catch (ArgumentException ax)  // there was something wrong in the connection string.
            {
                objResponseDB.Resp = false;
                objResponseDB.Msg = "Error creating the connection: " + ax.Message;
            }

            return objResponseDB;
        }

        public ResponseDB InsMultipleData(string strQuery, List<string> lstParam, List<List<string>> lstRows, string strValColAux, string strIdColAux)
        {
            ResponseDB objResponseDB = new ResponseDB
            {
                Resp = true
            };

            SqlConnection objSQLConnection;

            try
            {
                objSQLConnection = new SqlConnection(strConnection);

                try
                {
                    objSQLConnection.Open();

                    if (objSQLConnection.State != System.Data.ConnectionState.Open)
                    {
                        objResponseDB.Resp = false;
                        objResponseDB.Msg = "The connection status is: " + objSQLConnection.State.ToString();

                        objSQLConnection.Close();
                    }
                    else
                    {
                        try
                        {
                            objResponseDB.Count = 0;
                            objResponseDB.Error = 0;

                            for (var i = 0; i < lstRows.Count; i++)
                            {
                                try
                                {

                                    SqlCommand cmd = new SqlCommand(strQuery, objSQLConnection);

                                    cmd.CommandText = strQuery;

                                    int nuEmptyCols = 0;
                                    
                                    for (int j = 0; j < lstParam.Count; j++)
                                    {
                                        if (lstRows[i][j].Equals(""))
                                        {
                                            nuEmptyCols++;
                                        }
                                    }

                                    if (nuEmptyCols < lstRows[i].Count) {

                                        if (!strIdColAux.Equals("") && !strValColAux.Equals(""))
                                        {
                                            cmd.Parameters.Add(new SqlParameter(strIdColAux, strValColAux));
                                        }

                                        for (int j = 0; j < lstParam.Count; j++)
                                        {
                                            cmd.Parameters.Add(new SqlParameter(lstParam[j], lstRows[i][j]));
                                        }

                                        int nuAffectedRows = cmd.ExecuteNonQuery();

                                        if (nuAffectedRows == 0)
                                        {
                                            objResponseDB.Error++;
                                        }
                                        else
                                        {
                                            objResponseDB.Count++;
                                        }
                                    }

                                }
                                catch (Exception innerEx)
                                {
                                    objResponseDB.Msg = innerEx.Message;
                                    objResponseDB.Error++;
                                }

                            }

                            objResponseDB.Resp = true;

                            objSQLConnection.Close();

                        }
                        catch (SqlException ex)
                        {
                            // 5. Close the connection
                            if (objSQLConnection != null)
                            {
                                objResponseDB.Resp = false;
                                objResponseDB.Msg = ex.Message;
                                objSQLConnection.Close();
                            }
                        }

                    }

                }
                // Catch errors specific to the Open method
                catch (SqlException ex)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Error opening the connection:: " + ex.Message;
                }
                catch (InvalidOperationException ix)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Invalid Operation error: " + ix.Message;
                }
                catch (ConfigurationErrorsException cx)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Configuration error: " + cx.Message;
                }

            }
            catch (ArgumentException ax)  // there was something wrong in the connection string.
            {
                objResponseDB.Resp = false;
                objResponseDB.Msg = "Error creating the connection: " + ax.Message;
            }

            return objResponseDB;
        }

        public ResponseDB InsMultipleData(string strQuery, string[] arrayParam, string[] arrayRows, string strSplit, string strValColAux, string strIdColAux)
        {
            ResponseDB objResponseDB = new ResponseDB
            {
                Resp = true
            };

            SqlConnection objSQLConnection;

            try
            {
                objSQLConnection = new SqlConnection(strConnection);

                try
                {
                    objSQLConnection.Open();

                    if (objSQLConnection.State != System.Data.ConnectionState.Open)
                    {
                        objResponseDB.Resp = false;
                        objResponseDB.Msg = "The connection status is: " + objSQLConnection.State.ToString();

                        objSQLConnection.Close();
                    }
                    else
                    {
                        try
                        {
                            objResponseDB.Count = 0;
                            objResponseDB.Error = 0;

                            for (var i = 0; i < arrayRows.Length; i++)
                            {
                                if (arrayRows[i] != "")
                                {

                                    try
                                    {

                                        string[] arrayValue = arrayRows[i].Split(';');

                                        SqlCommand cmd = new SqlCommand(strQuery, objSQLConnection);

                                        cmd.CommandText = strQuery;

                                        if (!strIdColAux.Equals("") && !strValColAux.Equals(""))
                                        {
                                            cmd.Parameters.Add(new SqlParameter(strIdColAux, strValColAux));
                                        }

                                        for (int j = 0; j < arrayParam.Length; j++)
                                        {
                                            cmd.Parameters.Add(new SqlParameter(arrayParam[j], arrayValue[j]));
                                        }

                                        int nuAffectedRows = cmd.ExecuteNonQuery();

                                        if (nuAffectedRows == 0)
                                        {
                                            objResponseDB.Error++;
                                        }
                                        else
                                        {
                                            objResponseDB.Count++;
                                        }

                                    }
                                    catch (Exception innerEx)
                                    {
                                        objResponseDB.Resp = false;
                                        objResponseDB.Msg = innerEx.Message;
                                        objResponseDB.Error++;
                                    }

                                }
                            }

                            objSQLConnection.Close();

                        }
                        catch (SqlException ex)
                        {
                            // 5. Close the connection
                            if (objSQLConnection != null)
                            {
                                objResponseDB.Resp = false;
                                objResponseDB.Msg = ex.Message;
                                objSQLConnection.Close();
                            }
                        }

                    }

                }
                // Catch errors specific to the Open method
                catch (SqlException ex)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Error opening the connection:: " + ex.Message;
                }
                catch (InvalidOperationException ix)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Invalid Operation error: " + ix.Message;
                }
                catch (ConfigurationErrorsException cx)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Configuration error: " + cx.Message;
                }

            }
            catch (ArgumentException ax)  // there was something wrong in the connection string.
            {
                objResponseDB.Resp = false;
                objResponseDB.Msg = "Error creating the connection: " + ax.Message;
            }

            return objResponseDB;
        }
        //DESDE nuIni HASTA nuEnd,COLUMNAS,EL QUERY,PARAMETROS, PARAMENTROS, NO PAGINATE, TIPO DE RESPUESTA
        public ResponseDB getRespFromQuery(int nuIni, int nuEnd, string strQueryCols, string strQuery, string[] arrayParam, string[] arrayValue, string strOrderByCol, string strRespType)
        {
            ResponseDB objResponseDB = new ResponseDB
            {
                Resp = true
            };

            SqlConnection objSQLConnection;

            try
            {
                objSQLConnection = new SqlConnection(strConnection);

                try
                {
                    objSQLConnection.Open();

                    if (objSQLConnection.State != System.Data.ConnectionState.Open)
                    {
                        objResponseDB.Resp = false;
                        objResponseDB.Msg = "The connection status is: " + objSQLConnection.State.ToString();

                        objSQLConnection.Close();
                    }
                    else
                    {
                        SqlDataReader rdr = null;

                        try
                        {
                            bool blCount = true;

                            if (!strOrderByCol.Equals("NO_PAGINATE"))
                            {
                                #region QueryCount

                                string strCountQuery = "SELECT COUNT(*) AS ROWS " + strQuery;
                                SqlCommand cmdCount = new SqlCommand(strCountQuery, objSQLConnection);

                                DataSet dsInfoCount = new DataSet();
                                SqlDataAdapter adapterCount = new SqlDataAdapter();

                                cmdCount.CommandText = strCountQuery;

                                if (arrayParam != null)
                                {
                                    for (int i = 0; i < arrayParam.Length; i++)
                                    {
                                        SqlParameter objParam = new SqlParameter();
                                        objParam.ParameterName = arrayParam[i];
                                        objParam.Value = arrayValue[i];
                                        cmdCount.Parameters.Add(objParam);
                                    }
                                }

                                adapterCount.SelectCommand = cmdCount;
                                adapterCount.Fill(dsInfoCount);

                                if (dsInfoCount.Tables.Count > 0 && dsInfoCount.Tables[0].Rows.Count > 0)
                                {
                                    objResponseDB.Count = int.Parse(dsInfoCount.Tables[0].Rows[0][0].ToString());
                                }
                                else
                                {
                                    blCount = false;
                                }

                                #endregion

                                strQuery = strQueryCols+" "+ strQuery;
                                strQuery = SQLUtil.GetPaginatedSQL(nuIni, nuEnd, strQuery, "ORDER BY " + strOrderByCol);
                            }
                            else
                            {
                                strQuery = strQueryCols + " " + strQuery;
                            }

                            if (blCount) {

                                #region QueryData
                                SqlCommand cmd = new SqlCommand(strQuery, objSQLConnection);

                                DataSet dsInfo = new DataSet();
                                SqlDataAdapter adapter = new SqlDataAdapter();

                                cmd.CommandText = strQuery;

                                if (arrayParam != null)
                                {
                                    for (int i = 0; i < arrayParam.Length; i++)
                                    {
                                        SqlParameter objParam = new SqlParameter();
                                        objParam.ParameterName = arrayParam[i];
                                        objParam.Value = arrayValue[i];
                                        cmd.Parameters.Add(objParam);
                                    }
                                }

                                adapter.SelectCommand = cmd;
                                adapter.Fill(dsInfo);

                                if (dsInfo.Tables.Count > 0 && dsInfo.Tables[0].Rows.Count > 0)
                                {
                                    switch (strRespType)
                                    {
                                        case "DataTable":
                                            objResponseDB.dtResult = dsInfo.Tables[0];
                                            break;
                                        case "Array":

                                            string[][] stringRepresentation = dsInfo.Tables[0].Rows
                                            .OfType<DataRow>()
                                            .Select(r => dsInfo.Tables[0].Columns
                                                .OfType<DataColumn>()
                                                .Select(c => r[c.ColumnName].ToString())
                                                .ToArray())
                                            .ToArray();

                                            objResponseDB.jsonData = JsonConvert.SerializeObject(stringRepresentation, Formatting.None);

                                            break;
                                    }

                                    if (strOrderByCol.Equals("NO_PAGINATE"))
                                    {
                                        objResponseDB.Count = dsInfo.Tables[0].Rows.Count;
                                    }
                                }
                                else
                                {
                                    objResponseDB.Count = 0;
                                }

                                #endregion QueryData
                            }
                        }
                        catch (SqlException ex)
                        {
                            // close the reader
                            if (rdr != null)
                            {
                                rdr.Close();
                            }

                            // 5. Close the connection
                            if (objSQLConnection != null)
                            {
                                objSQLConnection.Close();
                            }

                            objResponseDB.Resp = false;
                            objResponseDB.Msg = ex.Message;
                        }

                        objSQLConnection.Close();
                    }

                }
                // Catch errors specific to the Open method
                catch (SqlException ex)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Error al tratar de conectarse a la base de datos:: " + ex.Message;
                }
                catch (InvalidOperationException ix)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Operaci??n no v??lida en la base de datos:" + ix.Message;
                }
                catch (ConfigurationErrorsException cx)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Error de configuraci??n de la base de datos: " + cx.Message;
                }

            }
            catch (ArgumentException ax)  // there was something wrong in the connection string.
            {
                objResponseDB.Resp = false;
                objResponseDB.Msg = "Error al crear la conexi??n a la base de datos: " + ax.Message;
            }

            return objResponseDB;
        }

        public ResponseDB DelData(string strQuery, string[] arrayParam, string[] arrayValue)
        {
            ResponseDB objResponseDB = new ResponseDB
            {
                Resp = true
            };

            SqlConnection objSQLConnection;

            try
            {
                objSQLConnection = new SqlConnection(strConnection);

                try
                {
                    objSQLConnection.Open();

                    if (objSQLConnection.State != System.Data.ConnectionState.Open)
                    {
                        objResponseDB.Resp = false;
                        objResponseDB.Msg = "The connection status is: " + objSQLConnection.State.ToString();

                        objSQLConnection.Close();
                    }
                    else
                    {

                        try
                        {

                            objResponseDB.Count = 0;
                            objResponseDB.Error = 0;

                            try
                            {

                                SqlCommand cmd = new SqlCommand(strQuery, objSQLConnection);

                                cmd.CommandText = strQuery;

                                for (int j = 0; j < arrayParam.Length; j++)
                                {
                                    cmd.Parameters.Add(new SqlParameter(arrayParam[j], arrayValue[j]));
                                }

                                int nuRows = cmd.ExecuteNonQuery();

                                if (nuRows == 0)
                                {
                                    objResponseDB.Error++;
                                }
                                else
                                {
                                    objResponseDB.Count = nuRows;
                                }

                            }
                            catch (Exception innerEx)
                            {
                                objResponseDB.Msg = innerEx.Message;
                                objResponseDB.Error++;
                            }


                            objSQLConnection.Close();

                        }
                        catch (SqlException ex)
                        {
                            // 5. Close the connection
                            if (objSQLConnection != null)
                            {
                                objResponseDB.Resp = false;
                                objResponseDB.Msg = ex.Message;
                                objSQLConnection.Close();
                            }
                        }

                    }

                }
                // Catch errors specific to the Open method
                catch (SqlException ex)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Error opening the connection:: " + ex.Message;
                }
                catch (InvalidOperationException ix)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Invalid Operation error: " + ix.Message;
                }
                catch (ConfigurationErrorsException cx)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Configuration error: " + cx.Message;
                }

            }
            catch (ArgumentException ax)  // there was something wrong in the connection string.
            {
                objResponseDB.Resp = false;
                objResponseDB.Msg = "Error creating the connection: " + ax.Message;
            }

            return objResponseDB;
        }
        /// <summary>
        /// Contenido del mensaje.
        /// </summary>
        public string strConnection = "";

        /// <summary>
        /// Error Conexi??n
        /// </summary>
        public Boolean blError = false;

        /// <summary>
        /// Descripci??n Error Conexi??n
        /// </summary>
        public string strErrorMsg = "";

        public int nuPagination = 100;
    }
}
