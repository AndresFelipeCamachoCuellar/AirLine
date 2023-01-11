using AirLineApi.Models;
using AirLineApi.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AirLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirplaneController : ControllerBase
    {
        [HttpGet("/[controller]/GetAirplanes")]
        public Response GetAirplanes()
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

                string strQuery = @"SELECT AIP.AirPlaneID, AIP.CompanyID, COM.Name AS NAME_COMPANY, AIP.Name AS NAME_AIRPLANE, AIP.Status,
                                    (CASE WHEN AIP.Status = 'A' THEN 'Activo' WHEN AIP.Status = 'I' THEN 'Inactivo' ELSE 'Desconocido' END) AS StatusDes 
                                    FROM AirPlane AIP 
                                    INNER JOIN Company COM ON AIP.CompanyID = COM.CompanyID";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 500, "", strQuery, arrParams, arrValues, "COM.Name", "DataTable");

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

        [HttpPost("/[controller]/CreateAirplane")]
        public Response CreateAirplane([FromBody] Request objRequestParams)
        {
            Response objResponse = new Response()
            {
                Resp = true
            };
            try
            {
                Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);
                JObject objRequestParam = JObject.Parse(objRequestParams.Data);

                string StrName = objRequestParam["Name"].ToString();
                string strCompanyID = objRequestParam["CompanyID"].ToString();

                string[] arrParams = new string[] { "@CompanyID" };
                string[] arrValues = new string[] { strCompanyID };

                string strQuery = @"SELECT * FROM AirPlane WHERE CompanyID = @CompanyID";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 500, "", strQuery, arrParams, arrValues, "Name", "DataTable");

                if (dbResponse.Resp)
                {
                    if (dbResponse.Count > 0)
                    {
                        JArray arrCities = JArray.Parse(JsonConvert.SerializeObject(dbResponse.dtResult, Formatting.None));

                        foreach (JObject city in arrCities)
                        {
                            if (city["Name"].ToString().ToLower().Trim().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Equals(StrName.ToLower().Trim().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")))
                            {
                                throw new Exception("Ya existe un avion con este nombre");
                            }
                        }
                    }
                   

                    string strStatus = objRequestParam["Status"].ToString();
                    string strCreatedby = objRequestParam["CreatedBy"].ToString();
                    arrParams = new string[] { "@CompanyID", "@Name", "@Status", "@CreatedBy" };
                    arrValues = new string[] { strCompanyID, StrName, strStatus, strCreatedby };

                    strQuery = @"INSERT INTO AirPlane(CompanyID, Name, Status, CreatedBy) OUTPUT INSERTED.AirPlaneID 
                                 VALUES (@CompanyID, @Name, @Status, @CreatedBy)";

                    dbResponse = objConnection.InsData(strQuery, arrParams, arrValues);
                    if (dbResponse.Resp)
                    {
                        if (dbResponse.Count > 0)
                        {
                            objResponse.Resp = true;
                            objResponse.Type = "Success";
                            objResponse.Msg = "Avion creado";
                        }
                        else
                        {
                            objResponse.Resp = false;
                            objResponse.Type = "Error";
                            objResponse.Msg = "No se pudo guardar el avion";
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

        [HttpPut("/[controller]/UpdateAirplane")]
        public Response UpdateAirplane([FromBody] Request objRequestParams)
        {
            Response objResponse = new Response()
            {
                Resp = true
            };
            try
            {
                Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);
                JObject objRequestParam = JObject.Parse(objRequestParams.Data);

                string strAirplaneID = objRequestParam["AirplaneID"].ToString();
                string strCompanyID = objRequestParam["CompanyID"].ToString();
                string strName = objRequestParam["Name"].ToString();
                string strStatus = objRequestParam["Status"].ToString();
                string strModifiedBy = objRequestParam["ModifiedBy"].ToString();


                string[] arrParams = new string[] { "@AirplaneID", "@CompanyID", "@Name", "@Status", "@ModifiedBy" };
                string[] arrValues = new string[] { strAirplaneID, strCompanyID, strName, strStatus, strModifiedBy };

                string strQuery = @"UPDATE AirPlane SET Name = @Name, Status = @Status, CompanyID = @CompanyID, ModifiedBy = @ModifiedBy, ModificationDate = GETDATE() WHERE AirPlaneID = @AirplaneID";

                ResponseDB dbResponse = objConnection.UpdData(strQuery, arrParams, arrValues);

                if (dbResponse.Resp)
                {
                    if (dbResponse.Count > 0)
                    {
                        objResponse.Resp = true;
                        objResponse.Type = "Success";
                        objResponse.Msg = "Empresa actualizada";
                    }
                    else
                    {
                        objResponse.Resp = false;
                        objResponse.Type = "Error";
                        objResponse.Msg = "No se pudo actualizar la Empresa";
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
