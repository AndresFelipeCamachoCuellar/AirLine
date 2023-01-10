using AirLineApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AirLineApi.Util
{
    public static class General
    {
        /*
        Author: Juan David Bonilla Á
        Date: 05/03/2022
        Desc: Valida campos vacios
        Params: JArray con los strings
        Return: True si no hay campos vacios, si no, false
        */
        public static bool ValidateEmptyFields(JArray arrFields)
        {
            try
            {
                foreach (string stField in arrFields)
                {
                    if (String.IsNullOrEmpty(stField))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*
        Author: Juan David Bonilla Á
        Date: 05/03/2022
        Desc: Valida si hay algún filtro diferente de vacío
        Params: JArray con los strings de los filtros
        Return: True si hay algún filtro aplicado, si no, false
        */
        public static bool HasFilter(JArray arrFields)
        {
            try
            {
                foreach (string stField in arrFields)
                {
                    if (!String.IsNullOrEmpty(stField))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*
        Author: Juan David Bonilla Á
        Date: 29/03/2022
        Desc: Valida en un array de strings si alguno es vacío y lo vuelve nulo
        Params: this (string[])
        Return: El nuevo array corregido
        */
        public static string[] SetEmptyToNull(this string[] myStrings)
        {
            JArray arrResponse = new JArray();

            for (int i = 0; i < myStrings.Length; i++)
            {
                if (string.IsNullOrEmpty(myStrings[i]))
                {
                    arrResponse.Add(null);
                }
                else
                {
                    arrResponse.Add(myStrings[i]);
                }
            }

            return arrResponse.ParseToStringArray();
        }

        /*
        Author: Juan David Bonilla Á
        Date: 05/03/2022
        Desc: Convierte un JArray a Array nativo de string
        */
        public static string[] ParseToStringArray(this JArray array)
        {
            return array.Select(jv => (string)jv).ToArray();
        }

        /*
        Author: Juan David Bonilla Á
        Date: 03/04/2022
        Desc: Valida si la fecha ingresada es un día festivo o no
        Return: true si es un día festivo, de lo contrario false
        */
        public static bool IsFestDay(string strDate)
        {
            Boolean blIsFestDay = false;

            try
            {
                Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);

                string strQuery = "SELECT DateFest FROM mgo.FestDay";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 10000, "", strQuery, null, null, "NO_PAGINATE", "Array");

                if (dbResponse.Resp)
                {
                    if (dbResponse.Count > 0)
                    {
                        JArray arrResult = JArray.Parse(dbResponse.jsonData);

                        for (int i = 0; i < arrResult.Count; i++)
                        {
                            if (DateTime.Parse(JArray.Parse(arrResult[i].ToString())[0].ToString()).ToString("yyyy-MM-dd").Equals(strDate))
                            {
                                blIsFestDay = true;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Error en la consulta del día festivo");
                    }
                }
                else
                {
                    throw new Exception(dbResponse.Msg);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return blIsFestDay;
        }
    }
}
