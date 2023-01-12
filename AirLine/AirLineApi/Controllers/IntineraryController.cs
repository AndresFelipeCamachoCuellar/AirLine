using AirLineApi.Models;
using AirLineApi.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace AirLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntineraryController : ControllerBase
    {
        [HttpGet("/[controller]/GetItineraries")]
        public Response GetItineraries()
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

                string strQuery = @"SELECT 
                                    	FLI.FlyIntineraryID, FLI.AirPlaneID, AIP.Name AS NAME_AIRPLAIN, 
                                    	FLI.Origin, ORI.Name AS ORIGIN_NAME, FLI.Destiny, DEST.Name AS DESTINY_NAME,
                                    	FLI.FlyDate, FLI.TakeOffTime, FLI.ArrivalDate, FLI.ArrivalTime, FLI.Status,
										CAST(CONCAT(CAST(FLI.FlyDate as nvarchar), ' ', CONVERT(nvarchar, FLI.TakeOffTime, 8)) AS datetime) AS TAKEOFF_DATE,
										CAST(CONCAT(CAST(FLI.ArrivalDate as nvarchar), ' ', CONVERT(nvarchar, FLI.ArrivalTime, 8)) AS datetime) AS ARRIVAL_DATE
                                    FROM FlyIntinerary FLI
                                    INNER JOIN AirPlane AIP ON FLI.AirPlaneID = AIP.AirPlaneID
                                    LEFT JOIN City ORI ON FLI.Origin = ORI.CityID
                                    LEFT JOIN City DEST ON FLI.Destiny = DEST.CityID";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 500, "", strQuery, arrParams, arrValues, "FLI.FlyDate DESC", "DataTable");

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

        [HttpPost("/[controller]/CreateItinerary")]
        public Response CreateItinerary([FromBody] Request objRequestParams)
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
                string strTakeOffDate = objRequestParam["TakeOffDate"].ToString();
                string strArrivalDate = objRequestParam["ArrivalDate"].ToString();
                string strTakeOffTime = objRequestParam["TakeOffTime"].ToString();
                string strArrivalTime = objRequestParam["ArrivalTime"].ToString();
                string strOrigin = objRequestParam["Origin"].ToString();
                string strDestiny = objRequestParam["Destiny"].ToString();
                string strStatus = objRequestParam["Status"].ToString();
                string strCreatedBy = objRequestParam["CreadtedBy"].ToString();

                string[] arrParams = new string[] { "@AirplaneID", "@TakeOffDate", "@TakeOffTime", "@ArrivalDate", "@ArrivalTime", "@Origin", "@Destiny", "@Status", "@CreatedBy" };
                string[] arrValues = new string[] { strAirplaneID, strTakeOffDate, strTakeOffTime, strArrivalDate, strArrivalTime, strOrigin, strDestiny, strStatus, strCreatedBy };

                string strQuery = @"SELECT FlyIntineraryID 
                                    FROM FlyIntinerary 
                                    WHERE 
                                    AirPlaneID = @AirplaneID 
                                    AND (CAST(CONCAT(@TakeOffDate, ' ', @TakeOffTime) AS datetime) 
                                    BETWEEN CAST(CONCAT(CAST(FlyDate as nvarchar), ' ', CONVERT(nvarchar, TakeOffTime, 8)) AS datetime) 
                                    AND CAST(CONCAT(CAST(ArrivalDate as nvarchar), ' ', CONVERT(nvarchar, ArrivalTime, 8)) AS datetime) 
                                    OR 
                                    CAST(CONCAT(@ArrivalDate, ' ', @ArrivalTime) AS datetime) 
                                    BETWEEN CAST(CONCAT(CAST(FlyDate as nvarchar), ' ', CONVERT(nvarchar, TakeOffTime, 8)) AS datetime) 
                                    AND CAST(CONCAT(CAST(ArrivalDate as nvarchar), ' ', CONVERT(nvarchar, ArrivalTime, 8)) AS datetime))";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 500, "", strQuery, arrParams, arrValues, "FlyIntineraryID", "DataTable");
                if (dbResponse.Resp)
                {
                    if (dbResponse.Count > 0)
                    {
                        objResponse.Resp = false;
                        objResponse.Type = "Error";
                        objResponse.Msg = "las fechas de vuelo se cruzan con otro intinerario";
                    }
                    else
                    {
                        strQuery = @"INSERT INTO FlyIntinerary (AirPlaneID, Origin, Destiny, FlyDate, TakeOffTime, ArrivalDate, ArrivalTime, Status, CreatedBy) OUTPUT INSERTED.FlyIntineraryID
                                     VALUES (@AirplaneID, @Origin, @Destiny, @TakeOffDate, @TakeOffTime, @ArrivalDate, @ArrivalTime, @Status, @CreatedBy)";

                        dbResponse = objConnection.InsData(strQuery, arrParams, arrValues);

                        if (dbResponse.Resp)
                        {
                            if (dbResponse.Count > 0)
                            {
                                objResponse.Resp = true;
                                objResponse.Type = "Success";
                                objResponse.Msg = "vuelo creado";
                            }
                            else
                            {
                                objResponse.Resp = false;
                                objResponse.Type = "Error";
                                objResponse.Msg = "No se pudo guardar el vuelo";
                            }
                        }
                        else
                        {
                            throw new Exception(dbResponse.Msg);
                        }
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

        [HttpGet("/[controller]/GetMostvisited")]
        public Response GetMostvisited()
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

                string strQuery = @"EXEC MOST_VISITED_DESTINIES";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 500, "", strQuery, arrParams, arrValues, "NO_PAGINATE", "DataTable");

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

        [HttpGet("/[controller]/GetFliedHours")]
        public Response GetFliedHours(string strAirPlaneID)
        {
            Response objResponse = new Response()
            {
                Resp = true
            };
            try
            {
                Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);


                string[] arrParams = new string[] { "@AirplaneID" };
                string[] arrValues = new string[] { strAirPlaneID };

                string strQuery = @"EXEC Airplane_Hours_Flown @AirPlane = @AirplaneID";

                ResponseDB dbResponse = objConnection.getRespFromQuery(0, 500, "", strQuery, arrParams, arrValues, "NO_PAGINATE", "DataTable");

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
    }
}
