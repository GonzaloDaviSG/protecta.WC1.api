using Newtonsoft.Json;
using protecta.WC1.api.DTO;
using protecta.WC1.api.Repository;
using protecta.WC1.api.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace protecta.WC1.api.Services
{
    public class WC1Service
    {
        WC1Repository _repository;
        RequestWc1 objDefault;
        public WC1Service()
        {
            _repository = new WC1Repository();
            objDefault = new RequestWc1();
            objDefault.providerTypes = new List<string>() { "WATCHLIST" };
            objDefault.entityType = "INDIVIDUAL";
            objDefault.groupId = Config.AppSetting["WordlCheckOne:groupId"];
            objDefault.customFields = new List<Properties>();
            objDefault.secondaryFields = new List<Properties>();
            objDefault.nameTransposition = true;
            objDefault.secondaryFields.Add(new Properties() { typeId = "SFCT_5", value = "PER" });
        }

        public List<ResponseWc1> Create(RequestWc1 item)
        {
            item.groupId = Config.AppSetting["WordlCheckOne:groupId"];
            string values = this.createCase(item);

            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(values);
            string values2 = this.confirmCase(obj.caseId.ToString());

            dynamic obj2 = Newtonsoft.Json.JsonConvert.DeserializeObject(values2);
            string values3 = this.getResults(obj.caseSystemId.ToString());

            List<ResponseWc1> items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>(values3);
            //ResponseDTO _item = this.SaveResult(item, obj.caseSystemId.ToString());
            return items;
        }

        internal Dictionary<string, string> listCountry()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            string sMethod = $"/reference/countries";
            string response = this.getReques(sMethod);
            list = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            return list;
        }



        internal object cargaMassive()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            string sMethod = $"/reference/nationalities";
            string response = this.getReques(sMethod);
            list = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            return list;
        }

        internal Task<ListResponseDTO> setClienteTratamiento(ResquestAlert item)
        {
            throw new NotImplementedException();
        }

        internal object Resolution(RequestWC1ResolutionDTO item)
        {
            string sRequest = JsonConvert.SerializeObject(item.item);
            string sMethod = $"cases/{item.caseSystemId}/results/resolution";
            string response = this.putReques(sMethod, sRequest);
            return response;
        }


        private string createCase(RequestWc1 item)
        {
            string sRequest = JsonConvert.SerializeObject(item);
            //string sRequest = JsonConvert.SerializeObject($"{{\"secondaryFields\":[],\"entityType\":\"INDIVIDUAL\",\"customFields\":[],\"groupId\":\"5jb8bs1tdnwv1fnb5aqmq6kyc\",\"providerTypes\":[\"WATCHLIST\"],\"name\":\"putin\"}}");
            string sMethod = "cases/screeningRequest";
            string response = "";
            do
            {
                response = this.postReques(sMethod, sRequest);
            } while (response == "429");
            return response;
        }
        public string getProfiles(string referenceId)
        {
            //string sRequest = JsonConvert.SerializeObject($"{{\"secondaryFields\":[],\"entityType\":\"INDIVIDUAL\",\"customFields\":[],\"groupId\":\"5jb8bs1tdnwv1fnb5aqmq6kyc\",\"providerTypes\":[\"WATCHLIST\"],\"name\":\"putin\"}}");
            string sMethod = $"reference/profile/{referenceId}";
            string response = "";
            do
            {
                response = this.getReques(sMethod, "");
            } while (response == "429");
            return response;
        }
        private string confirmCase(string caseId)
        {
            string sMethod = $"caseReferences";
            string response = "";
            do
            {
                response = this.getReques(sMethod, "?caseId=" + caseId);
            } while (response == "429");
            return response;
        }
        private string getResults(string caseSystemId)
        {
            string sMethod = $"cases";
            string response = "";
            do
            {
                response = this.getReques(sMethod + "/" + caseSystemId + "/results");
            } while (response == "429");
            return response;
        }
        public string postReques(string sMethod, string sRequest)
        {
            var url = Config.AppSetting["WordlCheckOne:protocol"] + Config.AppSetting["WordlCheckOne:gateWayHost"] + Config.AppSetting["WordlCheckOne:gateWayUrl"];
            string json = sRequest;
            DateTime dDate = DateTime.UtcNow;
            string date = dDate.ToString("R");
            byte[] byte1 = Encoding.UTF8.GetBytes(json);
            int NLength = byte1.Length;
            string firma = $"(request-target): post {Config.AppSetting["WordlCheckOne:gateWayUrl"]}{sMethod}\nhost: {Config.AppSetting["WordlCheckOne:gateWayHost"]}\ndate: {date}\ncontent-type: {Config.AppSetting["WordlCheckOne:content"]}\ncontent-length: {NLength}\n{json}";
            string base64 = this.generateAuthHeader(firma);
            string sAuthorisation = $"Signature keyId=\"{Config.AppSetting["WordlCheckOne:appKey"]}\",algorithm=\"hmac-sha256\",headers=\"(request-target) host date content-type content-length\" ,signature=\"{base64}\"";

            var request = (HttpWebRequest)WebRequest.Create(url + sMethod);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = NLength;
            //request.ContentLength = firma.Length;
            request.Date = dDate;
            //request.Accept = "*/*";
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Authorization", sAuthorisation);
            Stream newStream = request.GetRequestStream();
            newStream.Write(byte1, 0, NLength);
            try
            {
                //Thread.Sleep(1000);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string jsontxt = "";
                    if ((int)response.StatusCode == 429)
                    {
                        return (int)response.StatusCode + "";
                    }
                    Stream Answer = response.GetResponseStream();
                    StreamReader _Answer = new StreamReader(Answer);
                    jsontxt = _Answer.ReadToEnd();
                    return jsontxt;
                }
            }
            catch (WebException ex)
            {
                // Handle error
            }
            return "";
        }
        public string getReques(string sMethod, string sRequest = "")
        {
            var url = Config.AppSetting["WordlCheckOne:protocol"] + Config.AppSetting["WordlCheckOne:gateWayHost"] + Config.AppSetting["WordlCheckOne:gateWayUrl"];
            //string json = sRequest;
            DateTime dDate = DateTime.UtcNow;
            string date = dDate.ToString("R");
            //byte[] byte1 = Encoding.UTF8.GetBytes(json);
            //int NLength = byte1.Length;
            string firma = $"(request-target): get {Config.AppSetting["WordlCheckOne:gateWayUrl"]}{sMethod}\nhost: {Config.AppSetting["WordlCheckOne:gateWayHost"]}\ndate: {date}";
            string base64 = this.generateAuthHeader(firma);
            string sAuthorisation = $"Signature keyId=\"{Config.AppSetting["WordlCheckOne:appKey"]}\",algorithm=\"hmac-sha256\",headers=\"(request-target) host date\" ,signature=\"{base64}\"";

            var request = (HttpWebRequest)WebRequest.Create(url + sMethod + sRequest);
            request.Method = "GET";
            request.Date = dDate;
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Authorization", sAuthorisation);
            try
            {
                Thread.Sleep(2000);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string jsontxt = "";
                    if ((int)response.StatusCode == 429)
                    {
                        return (int)response.StatusCode + "";
                    }
                    Stream Answer = response.GetResponseStream();
                    StreamReader _Answer = new StreamReader(Answer);
                    jsontxt = _Answer.ReadToEnd();
                    return jsontxt;
                };

            }
            catch (WebException ex)
            {
                // Handle error
            }
            return "";
        }
        public string putReques(string sMethod, string sRequest)
        {
            var url = Config.AppSetting["WordlCheckOne:protocol"] + Config.AppSetting["WordlCheckOne:gateWayHost"] + Config.AppSetting["WordlCheckOne:gateWayUrl"];
            string json = sRequest;
            DateTime dDate = DateTime.UtcNow;
            string date = dDate.ToString("R");
            byte[] byte1 = Encoding.UTF8.GetBytes(json);
            int NLength = byte1.Length;
            string firma = $"(request-target): put {Config.AppSetting["WordlCheckOne:gateWayUrl"]}{sMethod}\nhost: {Config.AppSetting["WordlCheckOne:gateWayHost"]}\ndate: {date}\ncontent-type: {Config.AppSetting["WordlCheckOne:content"]}\ncontent-length: {NLength}\n{json}";
            string base64 = this.generateAuthHeader(firma);
            string sAuthorisation = $"Signature keyId=\"{Config.AppSetting["WordlCheckOne:appKey"]}\",algorithm=\"hmac-sha256\",headers=\"(request-target) host date content-type content-length\" ,signature=\"{base64}\"";

            var request = (HttpWebRequest)WebRequest.Create(url + sMethod);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.ContentLength = NLength;
            //request.ContentLength = firma.Length;
            request.Date = dDate;
            //request.Accept = "*/*";
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Authorization", sAuthorisation);
            Stream newStream = request.GetRequestStream();
            newStream.Write(byte1, 0, NLength);
            try
            {
                Thread.Sleep(2000);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string jsontxt = "";
                    if ((int)response.StatusCode == 429)
                    {
                        return (int)response.StatusCode + "";
                    }
                    Stream Answer = response.GetResponseStream();
                    StreamReader _Answer = new StreamReader(Answer);
                    jsontxt = _Answer.ReadToEnd();
                    return jsontxt;
                };

            }
            catch (WebException ex)
            {
                // Handle error
            }
            return "";
        }
        public string generateAuthHeader(string message)
        {
            string apisecret = Config.AppSetting["WordlCheckOne:appSecrect"];
            byte[] secretKey = Encoding.UTF8.GetBytes(apisecret);
            HMACSHA256 hmac = new HMACSHA256(secretKey);
            hmac.Initialize();

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            byte[] rawHmac = hmac.ComputeHash(bytes);
            Console.WriteLine("---rawHmac---");
            string hex = BitConverter.ToString(rawHmac).Replace("-", "");
            Console.WriteLine(hex);
            return (Convert.ToBase64String(rawHmac));
        }

        ResponseDTO SaveResult(ResponseWc1 item, string SystemCaseId)
        {
            ResponseDTO response = new ResponseDTO();
            ResponseProfileDTO profile = new ResponseProfileDTO();
            string resultado = this.getProfiles(item.referenceId);
            profile = JsonConvert.DeserializeObject<ResponseProfileDTO>(resultado);
            response = _repository.SaveResult(item, SystemCaseId);
            try
            {
                if (response.nId > 0)
                {
                    _repository.SaveProfile(profile, item.resultId, response.nId);
                    if (item.sources.Count > 0)
                        for (int j = 0; j < item.sources.Count; j++)
                            _repository.SaveSources(item.sources[j], response.nId);
                    if (item.categories.Count > 0)
                        for (int j = 0; j < item.categories.Count; j++)
                            _repository.SaveCategories(item.categories[j], response.nId);
                    if (item.events.Count > 0)
                        for (int j = 0; j < item.events.Count; j++)
                            _repository.SaveEvents(item.events[j], response.nId);
                    if (item.countryLinks.Count > 0)
                        for (int j = 0; j < item.countryLinks.Count; j++)
                            _repository.SaveCountryLinks(item.countryLinks[j], response.nId);
                    if (item.identityDocuments.Count > 0)
                        for (int j = 0; j < item.identityDocuments.Count; j++)
                            _repository.SaveIdentityDocuments(item.identityDocuments[j], response.nId);
                    if (profile.sources.Count > 0)
                        for (int j = 0; j < profile.sources.Count; j++)
                            _repository.SaveDetailSources(profile.sources[j], response.nId);
                    if (profile.weblinks.Count > 0)
                        for (int j = 0; j < profile.weblinks.Count; j++)
                            _repository.SaveWebLinks(profile.weblinks[j], response.nId);
                    if (profile.details.Count > 0)
                        for (int j = 0; j < profile.details.Count; j++)
                        {
                            _repository.SaveDetail(profile.details[j], response.nId);
                            if (profile.details[j].detailType == "BIOGRAPHY")
                                response.sMessage = profile.details[j].text;
                        }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return response;
        }

        internal ResponseDTO procesoCoincidencia()
        {

            ResponseDTO response = new ResponseDTO();
            List<ResponseWc1> items;
            List<string> List = _repository.ListIndividuos();
            try
            {
                //for (int i = 0; i < List.Count; i++)
                //{
                objDefault.name = "Alan Gabriel Ludwig García Pérez";//List[i];
                string result = createCase(objDefault);
                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>((obj.results).ToString());

                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return response;
        }
        internal async Task<ListResponseDTO> getCoincidenceNotPep(ResquestAlert item)
        {
            ListResponseDTO response = new ListResponseDTO();
            if (String.IsNullOrWhiteSpace(item.name))
            {
                response.isError = true;
                response.messageError = "Ingrese un nombre de un cliente";
                return response;
            }
            if (String.IsNullOrWhiteSpace(item.idDocNumber))
            {
                response.isError = true;
                response.messageError = "Ingrese un numero de documento";
                return response;
            }
            List<ResponseWc1> items = new List<ResponseWc1>();
            response.Data = new List<DataEntity>();
            bool isExist = false;
            await Task.Run(() =>
            {
                try
                {
                    objDefault.name = item.name;
                    string result = createCase(objDefault);
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>((obj.results).ToString());
                    items = items.Where(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED")).ToList();
                    for (int i = 0; i < items.Count; i++)
                    {
                        DataEntity dataitem = new DataEntity();
                        if (items[i].categories.Contains("PEP"))
                        {
                            response.isPep = true;
                            dataitem.name = items[i].matchedTerm;
                            dataitem.percentage = getPorcentaje(items[i].matchStrength);
                            List<IdentityDocuments> _items = items[i].identityDocuments.FindAll(t => t.type == "DNI");
                            if (_items.Count > 0)
                            {
                                dataitem.documentNumber = _items[0].number;
                                dataitem.documentType = _items[0].type;
                            }
                            response.Data.Add(dataitem);
                        }
                        else
                            response.isOtherList = true;
                        if (item.idDocNumber != "")
                            if (!response.isIdNumber)
                                response.isIdNumber = items[i].identityDocuments.Exists(t => t.number == item.idDocNumber);

                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
            return response;
        }
        public int getPorcentaje(string termn)
        {
            int value = 0;
            switch (termn)
            {
                case "WEAK":
                    value = 25;
                    break;
                case "MEDIUM":
                    value = 50;
                    break;
                case "STRONG":
                    value = 75;
                    break;
                case "EXACT":
                    value = 100;
                    break;
            }
            return value;
        }
        public async Task<ResponseDTO> alertsProcess(ResquestAlert item)
        {
            List<ObjCaseDTO> Case = new List<ObjCaseDTO>();
            ResponseDTO _response = new ResponseDTO();
            ResponseDTO response = new ResponseDTO();
            _response.sStatus = "NOT FOUND";
            bool isCreate = false;
            await Task.Run(() =>
            {
                string result = "";
                string objResult = "";
                string caseId = "";
                string caseSystemId = "";
                Case = _repository.getCase(item.name);
                objDefault.name = item.name;
                if (Case.Count == 0)
                {
                    result = createCase(objDefault);
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                    objResult = obj.results.ToString();
                    caseSystemId = obj.caseSystemId.ToString();
                    caseId = obj.caseId.ToString();
                    if (caseId != "")
                        isCreate = true;
                }
                else
                {
                    result = getResults(Case[0].SCaseSystemId);
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                    objResult = obj.ToString();
                    caseSystemId = Case[0].SCaseSystemId;
                    caseId = Case[0].SCaseId;
                }
                List<ResponseWc1> items = new List<ResponseWc1>();

                try
                {
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>((objResult).ToString());
                    items = items.FindAll(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_5"));
                    //items = items.Where(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_5")).ToList();
                    System.Console.WriteLine("individuo :" + item.name + " cantidad :" + items.Count);
                    if (items.Count > 0)
                    {
                        if (!isCreate)
                        {
                            _repository.deshabilitarResultado(Case[0].SCaseId);
                        }
                        _response.data = items.Select(t => string.Join(",", t.categories.Distinct())).ToList();
                        List<string> _categorys = _response.data.FindAll(t => t.Contains(","));
                        _response.data = _response.data.FindAll(t => !t.Contains(","));
                        for (int i = 0; i < _categorys.Count; i++)
                        {
                            _response.data.AddRange(_categorys[i].Split(",").ToList());
                        }
                        _response.data = _response.data.Distinct().ToList();

                        for (int i = 0; i < items.Count; i++)
                        {
                            _response.sStatus = isCreate ? "OK" : "UPDATE";
                            try
                            {
                                string biography = "";
                                items[i].categories = items[i].categories.Distinct().ToList();
                                response = this.SaveResult(items[i], caseSystemId);
                                biography = response.sMessage;
                                response = _repository.SaveResultCoincidencias(items[i], item, response.nId, caseSystemId, caseId, biography);
                                _response.sMessage = response.sMessage;
                            }
                            catch (Exception ex)
                            {
                                _response.sMessage = response.sMessage;
                                System.Console.WriteLine("error : " + i + " - " + items[i].primaryName + " - " + ex.Message);
                            }
                        }
                        if(item.tipoCargaId == "2")
                            response = _repository.ins_tratamiento_cliente(item);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("error : " + " - " + ex.Message);
                }
            });
            return _response;
        }

        //public Microsoft.AspNetCore.Mvc.ActionResult getprueba()
        //{

        //    return new System.Web.Http.Results.ResponseMessageResult(
        //     Request.CreateErrorResponse(
        //         (HttpStatusCode)422,
        //         new HttpError("Something goes wrong")
        //     )
        // );
        //}
    }
}
