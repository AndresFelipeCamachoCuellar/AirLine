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
    public class CompanyController : ControllerBase
    {
        [HttpGet("/[controller]/Companies")]
        public Response GetCCompanies()
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

                string strQuery = @"SELECT CompanyID, Name, Status, 
                                    (CASE WHEN Status = 'A' THEN 'Activo' WHEN Status = 'I' THEN 'Inactivo' ELSE 'Desconocido' END) AS StatusDes,
                                    CreatedBy,
                                    CreationDate,
                                    ModifiedBy,
                                    ModificationDate
                                    FROM Company";

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

        [HttpPost("/[controller]/CreateCompany")]
        public Response CreateCompany([FromBody] Request objRequestParams)
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

                string[] arrParams = new string[] { };
                string[] arrValues = new string[] { };

                string strQuery = @"SELECT * FROM Company";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 500, "", strQuery, arrParams, arrValues, "Name", "DataTable");

                if (dbResponse.Resp)
                {
                    JArray arrCities = JArray.Parse(JsonConvert.SerializeObject(dbResponse.dtResult, Formatting.None));

                    foreach (JObject city in arrCities)
                    {
                        if (city["Name"].ToString().ToLower().Trim().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Equals(StrName.ToLower().Trim().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")))
                        {
                            throw new Exception("Ya existe una empresa con este nombre");
                        }
                    }

                    string strStatus = objRequestParam["Status"].ToString();
                    string strCreatedby = objRequestParam["CreatedBy"].ToString();
                    arrParams = new string[] { "@Name", "@Status", "@CreatedBy" };
                    arrValues = new string[] { StrName, strStatus, strCreatedby };

                    strQuery = @"INSERT INTO Company (Name, Status, CreatedBy) OUTPUT INSERTED.CompanyID 
                                VALUES (@Name, @Status, @CreatedBy)";

                    dbResponse = objConnection.InsData(strQuery, arrParams, arrValues);
                    if (dbResponse.Resp)
                    {
                        if (dbResponse.Count > 0)
                        {
                            objResponse.Resp = true;
                            objResponse.Type = "Success";
                            objResponse.Msg = "Empresa creada";
                        }
                        else
                        {
                            objResponse.Resp = false;
                            objResponse.Type = "Error";
                            objResponse.Msg = "No se pudo guardar la Empresa";
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

        [HttpPut("/[controller]/UpdateCompany")]
        public Response UpdateCompany([FromBody] Request objRequestParams)
        {
            Response objResponse = new Response()
            {
                Resp = true
            };
            try
            {
                Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);
                JObject objRequestParam = JObject.Parse(objRequestParams.Data);

                string strCompanyID = objRequestParam["CompanyID"].ToString();
                string strName = objRequestParam["Name"].ToString();
                string strStatus = objRequestParam["Status"].ToString();
                string strModifiedBy = objRequestParam["ModifiedBy"].ToString();


                string[] arrParams = new string[] { "@CompanyID", "@Name", "@Status", "@ModifiedBy" };
                string[] arrValues = new string[] { strCompanyID, strName, strStatus, strModifiedBy };

                string strQuery = @"UPDATE Company SET Name = @Name, Status = @Status, ModifiedBy = @ModifiedBy WHERE CompanyID = @CompanyID";

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
