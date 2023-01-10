using AirLineApi.Models;
using AirLineApi.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Data.Common;

namespace AirLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        [HttpGet("/[controller]/Cities")]
        public Response GetCities()
        {
            Response objResponse = new Response()
            {
                Resp = true
            };
            try
            {
                Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);

                
                string[] arrParams = new string[] { };
                string[] arrValues = new string[] { };

                string strQuery = @"SELECT * FROM CITY";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 500, "", strQuery, arrParams, arrValues, "Name", "DataTable");

                if (dbResponse.Resp)
                {
                    if (dbResponse.Count > 0)
                    {
                        objResponse.Count = dbResponse.Count;
                        objResponse.Data = JsonConvert.SerializeObject(dbResponse.dtResult, Formatting.None);
                    }
                    else
                    {
                        objResponse.Resp = false;
                        objResponse.Type = "Error";
                        objResponse.Msg = "No se encontro ningun usuario asociado a tu empresa";
                    }
                }
                else
                {
                    throw new Exception(dbResponse.Msg);
                }

            }
            catch (Exception exc)
            {
                objResponse.Resp = false;
                objResponse.Type = "Error";
                objResponse.Msg = "Ocurrió un error inesperado, por favor consulte al administrador";
                objResponse.Detail = exc.Message;
            }

            return objResponse;
        }

        [HttpPost("/[controller]/CreateCity")]
        public Response CreateCity([FromBody] Request objRequestParams)
        {
            Response objResponse = new Response()
            {
                Resp = true
            };
            try
            {
                Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);
                JObject objRequestParam = JObject.Parse(objRequestParams.Data);

                string CityName = objRequestParam["Name"].ToString();

                string[] arrParams = new string[] { };
                string[] arrValues = new string[] { };

                string strQuery = @"SELECT * FROM CITY";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 500, "", strQuery, arrParams, arrValues, "Name", "DataTable");

                if (dbResponse.Resp)
                {
                    JArray arrCities = JArray.Parse(JsonConvert.SerializeObject(dbResponse.dtResult, Formatting.None));

                    foreach (JObject city in arrCities)
                    {
                        if (city["Name"].ToString().ToLower().Trim().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Equals(CityName.ToLower().Trim().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")))
                        {
                            throw new Exception("Ya existe una ciudad con este nombre");
                        }
                    }

                    string strStatus = objRequestParam["Status"].ToString();
                    string strCreatedby = objRequestParam["CreatedBy"].ToString();
                    arrParams = new string[] { "@Name", "@Status", "@CreatedBy" };
                    arrValues = new string[] { CityName, strStatus, strCreatedby };

                    strQuery = @"INSERT INTO City (Name, Status, CreatedBy) OUTPUT INSERTED.CityID 
                                VALUES (@Name, @Status, @CreatedBy)";

                    dbResponse = objConnection.InsData(strQuery, arrParams, arrValues);
                    if (dbResponse.Resp)
                    {
                        if (dbResponse.Count > 0)
                        {
                            objResponse.Resp = true;
                            objResponse.Type = "Success";
                            objResponse.Msg = "Ciudad creada";
                        }
                        else
                        {
                            objResponse.Resp = false;
                            objResponse.Type = "Error";
                            objResponse.Msg = "No se pudo guardar la ciudad";
                        }
                    }
                    else
                    {
                        throw new Exception(dbResponse.Msg);
                    }
                }
                else
                {
                    throw new Exception(dbResponse.Msg);
                }

            }
            catch (Exception exc)
            {
                objResponse.Resp = false;
                objResponse.Type = "Error";
                objResponse.Msg = "Ocurrió un error inesperado, por favor consulte al administrador";
                objResponse.Detail = exc.Message;
            }

            return objResponse;
            
        }

        [HttpPut("/[controller]/UpdateCity")]
        public Response UpdateCity([FromBody] Request objRequestParams)
        {
            Response objResponse = new Response()
            {
                Resp = true
            };
            try
            {
                Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);
                JObject objRequestParam = JObject.Parse(objRequestParams.Data);

                string strCityID = objRequestParam["CityID"].ToString();
                string strName = objRequestParam["Name"].ToString();
                string strStatus = objRequestParam["Status"].ToString();
                string strModifiedBy = objRequestParam["ModifiedBy"].ToString();


                string[] arrParams = new string[] { "@CityID", "@Name", "@Status", "@ModifiedBy" };
                string[] arrValues = new string[] { strCityID, strName, strStatus, strModifiedBy };

                string strQuery = @"UPDATE City SET Name = @Name, Status = @Status, ModifiedBy = @ModifiedBy WHERE CityID = @CityID";

                ResponseDB dbResponse = objConnection.UpdData(strQuery, arrParams, arrValues);

                if (dbResponse.Resp)
                {
                    if (dbResponse.Count > 0)
                    {
                        objResponse.Resp = true;
                        objResponse.Type = "Success";
                        objResponse.Msg = "Ciudad actualizada";
                    }
                    else
                    {
                        objResponse.Resp = false;
                        objResponse.Type = "Error";
                        objResponse.Msg = "No se pudo actualizar la ciudad";
                    }
                }
                else
                {
                    throw new Exception(dbResponse.Msg);
                }
            }
            catch (Exception exc)
            {
                objResponse.Resp = false;
                objResponse.Type = "Error";
                objResponse.Msg = "Ocurrió un error inesperado, por favor consulte al administrador";
                objResponse.Detail = exc.Message;
            }
            return objResponse;
        }
    }
}
