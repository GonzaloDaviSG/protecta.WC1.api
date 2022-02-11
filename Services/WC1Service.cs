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
using System.Diagnostics;

namespace protecta.WC1.api.Services
{
    public class WC1Service
    {
        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<String> ValidPorcentage;
        public List<String> ValidPorcentageDemanda;
        WC1Repository _repository;
        RequestWc1 objDefault;
        List<int> records = new List<int>();
        bool istest;
        public WC1Service()
        {
            ValidPorcentage = new List<String> { "EXACT" };
            ValidPorcentageDemanda = new List<String> { "STRONG", "EXACT" };
            _repository = new WC1Repository();
            objDefault = new RequestWc1();
            objDefault.providerTypes = new List<string>() { "WATCHLIST" };
            objDefault.entityType = "INDIVIDUAL";
            objDefault.groupId = Config.AppSetting["WordlCheckOne:groupId"];
            objDefault.customFields = new List<Properties>();
            objDefault.secondaryFields = new List<Properties>();
            objDefault.nameTransposition = true;
            objDefault.secondaryFields.Add(new Properties() { typeId = "SFCT_5", value = "PER" });
            istest = false;
            records.Add(1);
            records.Add(2);
            records.Add(3);
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

        internal Task<ResponseDTO> GetCoincidenceMassive(ResquestAlert item)
        {
            throw new NotImplementedException();
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
                    if (item.sources != null)
                        if (item.sources.Count > 0)
                            for (int j = 0; j < item.sources.Count; j++)
                                _repository.SaveSources(item.sources[j], response.nId);
                    if (item.categories != null)
                        if (item.categories.Count > 0)
                            for (int j = 0; j < item.categories.Count; j++)
                                _repository.SaveCategories(item.categories[j], response.nId);
                    if (item.events != null)
                        if (item.events.Count > 0)
                            for (int j = 0; j < item.events.Count; j++)
                                _repository.SaveEvents(item.events[j], response.nId);
                    if (item.countryLinks != null)
                        if (item.countryLinks.Count > 0)
                            for (int j = 0; j < item.countryLinks.Count; j++)
                                _repository.SaveCountryLinks(item.countryLinks[j], response.nId);
                    if (item.identityDocuments != null)
                        if (item.identityDocuments.Count > 0)
                            for (int j = 0; j < item.identityDocuments.Count; j++)
                                _repository.SaveIdentityDocuments(item.identityDocuments[j], response.nId);
                    if (profile.sources != null)
                        if (profile.sources.Count > 0)
                            for (int j = 0; j < profile.sources.Count; j++)
                                _repository.SaveDetailSources(profile.sources[j], response.nId);
                    if (profile.weblinks != null)
                        if (profile.weblinks.Count > 0)
                            for (int j = 0; j < profile.weblinks.Count; j++)
                                _repository.SaveWebLinks(profile.weblinks[j], response.nId);
                    if (profile.details != null)
                        if (profile.details.Count > 0)
                        {
                            response.informacionComplementaria = new Dictionary<string, dynamic>();
                            for (int j = 0; j < profile.details.Count; j++)
                            {
                                _repository.SaveDetail(profile.details[j], response.nId);
                                if (profile.details[j].detailType == "REPORTS")
                                    response.informacionComplementaria["SINFORMACION"] = profile.details[j].text;
                                if (profile.details[j].detailType == "BIOGRAPHY")
                                    response.informacionComplementaria["SCARGO"] = profile.details[j].text;
                            }
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
            log.Info("entro");
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
                    if (result != "")
                    {
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
                                if (getPorcentaje(items[i].matchStrength) == 100)
                                response.isOtherList = true;
                            if (item.idDocNumber != "")
                                if (!response.isIdNumber)
                                    response.isIdNumber = items[i].identityDocuments.Exists(t => t.number == item.idDocNumber);

                        }
                    }
                    else {
                        log.Info("resultado  :" + result);
                    }
                }
                catch (Exception ex)
                {
                    log.Info("Error  :" + ex.Message);
                    throw ex;
                }
            });
            log.Info("acabo");
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
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                        objResult = obj.results.ToString();
                        caseSystemId = obj.caseSystemId.ToString();
                        caseId = obj.caseId.ToString();
                        if (caseId != "")
                            isCreate = true;
                    }
                }
                else
                {
                    result = getResults(Case[0].SCaseSystemId);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                        objResult = obj.ToString();
                        caseSystemId = Case[0].SCaseSystemId;
                        caseId = Case[0].SCaseId;
                    }
                }
                List<ResponseWc1> items = new List<ResponseWc1>();

                try
                {
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>((objResult).ToString());
                    if (items != null && items.Count > 0)
                    {
                        items = items.FindAll(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_5"));
                        items = items.FindAll(t => ValidPorcentage.Contains(t.matchStrength));
                        //List<Dictionary<string, dynamic>> _items = new List<Dictionary<string, dynamic>>();
                        //Dictionary<string, dynamic> _item;
                        //for (int k = 0; k < items.Count; k++)
                        //{
                        //    _item = new Dictionary<string, dynamic>();
                        //    _item["name"] = items[k].matchedTerm;
                        //    _item["numdocument"] = items[k].matchedTerm;
                        //    List<Dictionary<string, dynamic>> documents = new List<Dictionary<string, dynamic>>();
                        //    if (items[k].identityDocuments.Count > 0)
                        //    {
                        //        Dictionary<string, dynamic> doc;
                        //        for (int j = 0; j < items[k].identityDocuments.Count; j++)
                        //        {
                        //            _item["number"] = items[k].identityDocuments[j].number;
                        //            _item["type"] = items[k].identityDocuments[j].locationType.type.Contains("DNI") ? "DNI" : items[k].identityDocuments[j].locationType.type.Contains("RUC") ? "RUC" : "";
                        //         }
                        //    }
                        //    else
                        //    {
                        //        Dictionary<string, dynamic> doc = new Dictionary<string, dynamic>();
                        //        doc["number"] = "0";
                        //        doc["type"] = "XX";
                        //        documents.Add(doc);
                        //    }
                        //    _item["name"] = items[k].matchedTerm;

                        //}
                        System.Console.WriteLine("individuo :" + item.name + " cantidad :" + items.Count);
                        if (items.Count > 0)
                        {
                            if (!isCreate)
                            {
                                // _repository.deshabilitarResultado(Case[0].SCaseId);
                            }
                            _response.data = items.Select(t => string.Join(",", t.categories.Distinct())).ToList();
                            List<string> _categorys = _response.data.FindAll(t => t.Contains(","));
                            _response.data = _response.data.FindAll(t => !t.Contains(","));
                            for (int i = 0; i < _categorys.Count; i++)
                            {
                                _response.data.AddRange(_categorys[i].Split(",").ToList());
                            }
                            _response.data = _response.data.Distinct().ToList();
                            if (item.tipoCargaId == "2")
                                response = _repository.eliminarCoincidenciastemporales(item);
                            for (int i = 0; i < items.Count; i++)
                            {
                                _response.sStatus = isCreate ? "OK" : "UPDATE";
                                try
                                {
                                    string biography = "";
                                    items[i].categories = items[i].categories.Distinct().ToList();
                                    response = this.SaveResult(items[i], caseSystemId);
                                    biography = response.sMessage;
                                    items[i].matchedTerm = this.formatearNombre(items[i].matchedTerm.Replace(',', ' ').Split(' '));
                                    items[i].primaryName = this.formatearNombre(items[i].primaryName.Replace(',', ' ').Split(' '));
                                    response = _repository.SaveResultCoincidencias(items[i], item, response.nId, caseSystemId, caseId, biography);
                                    _response.sMessage = response.sMessage;
                                }
                                catch (Exception ex)
                                {
                                    _response.sMessage = response.sMessage;
                                    System.Console.WriteLine("error : " + i + " - " + items[i].primaryName + " - " + ex.Message);
                                }
                            }
                            if (item.tipoCargaId == "2")
                                response = _repository.spcarga_coincidencias(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("error : " + " - " + ex.Message);
                }
            });
            return _response;
        }

        private string formatearNombre(string[] word)
        {
            List<String> nombre = new List<String>();
            List<String> apellidos = new List<String>();
            for (var x = 0; x < word.Length; x++)
            {
                if (word[x] == word[x].ToUpper())
                    apellidos.Add(word[x]);
                else
                    nombre.Add(word[x]);
            }
            return String.Join(" ", apellidos) + ' ' + String.Join(" ", nombre);
        }
        internal async Task<ResponseDTO> GetDemandaSearch(ResquestAlert item)
        {
            ResponseDTO respuesta = new ResponseDTO();

            List<Dictionary<string, dynamic>> itemsBusqueda = new List<Dictionary<string, dynamic>>();
            List<Dictionary<string, dynamic>> coincidencias = new List<Dictionary<string, dynamic>>();
            ResponseDTO profiles = new ResponseDTO();
            try
            {
                if (item.tipoBusqueda == 2)
                {
                    itemsBusqueda = _repository.getDemandas(item.codBusqueda);
                    if (itemsBusqueda.Count > 0)
                    {
                        for (int i = 0; i < itemsBusqueda.Count; i++)
                        {
                            List<Dictionary<string, dynamic>> _coincidencias = null;
                            _coincidencias = getCoincidenciasDemanda(itemsBusqueda[i], true);
                            coincidencias.AddRange(_coincidencias);
                        }
                    }
                }
                else
                {
                    Dictionary<string, dynamic> _item = new Dictionary<string, dynamic>();
                    _item["SNOMBRE_COMPLETO"] = item.name;
                    _item["STIPO_DOCUMENTO"] = item.typeDocument;
                    coincidencias = getCoincidenciasDemanda(_item, false);
                }
                respuesta.sMessage = "Termino la busqueda satisfactoriamente.";
                respuesta.nCode = 0;
                respuesta.Items = new List<Dictionary<string, dynamic>>();
                respuesta.Items.AddRange(coincidencias);
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.sMessage = ex.Message;
                respuesta.nCode = 1;
                return respuesta;
            }
        }
        public List<Dictionary<string, dynamic>> getCoincidenciasDemanda(Dictionary<string, dynamic> item, bool isMasive)
        {

            string objResult = "";
            string tipoEntidad = "";
            string tipoPersona = "";
            string tipoDocumento = "";
            List<Dictionary<string, dynamic>> _items = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> _item = null;
            List<ResponseWc1> items = new List<ResponseWc1>();
            if (isMasive)
            {
                tipoEntidad = item["STIPO_DOCUMENTO"] == "RUC" ? "ORGANISATION" : "INDIVIDUAL";
                tipoPersona = item["STIPO_DOCUMENTO"] == "RUC" ? "EMPRESA  (PERSONA JURÍDICA)" : "PERSONA NATURAL";
                tipoDocumento = item["STIPO_DOCUMENTO"] == "RUC" ? "RUC" : "DNI";
            }
            else
            {
                tipoEntidad = item["STIPO_DOCUMENTO"] == "1" ? "ORGANISATION" : "INDIVIDUAL";
                tipoPersona = item["STIPO_DOCUMENTO"] == "1" ? "EMPRESA  (PERSONA JURÍDICA)" : "PERSONA NATURAL";
                tipoDocumento = item["STIPO_DOCUMENTO"] == "1" ? "RUC" : "DNI";
            }
            objDefault.name = item["SNOMBRE_COMPLETO"];
            objDefault.entityType = tipoEntidad;
            if (tipoEntidad == "ORGANISATION")
                objDefault.secondaryFields = new List<Properties>();
            string result = createCase(objDefault);
            if (!String.IsNullOrWhiteSpace(result))
            {
                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                objResult = obj.results.ToString();
            }
            items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>((objResult).ToString());
            if (items.Count > 0)
            {
                if (tipoEntidad != "ORGANISATION")
                    items = items.FindAll(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_5"));
                items = items.FindAll(t => ValidPorcentageDemanda.Contains(t.matchStrength));
                ResponseProfileDTO profile = new ResponseProfileDTO();
                for (int i = 0; i < items.Count; i++)
                {
                    _item = new Dictionary<string, dynamic>();
                    _item["SNOMBRE_COMPLETO"] = items[i].primaryName;
                    _item["SNOMBRE_BUSQUEDA"] = item["SNOMBRE_COMPLETO"];
                    _item["SNOMBRE_TERMINO"] = items[i].matchedTerm.ToString();
                    if (items[i].categories.Count > 0)
                    {
                        _item["SLISTA"] = items[i].categories.Distinct().ToString() == "PEP" ? "LISTAS PEP" : "LISTAS INTERNACIONALES";
                    }
                    _item["DFECHA_BUSQUEDA"] = DateTime.Now.ToString();
                    _item["SCARGO"] = "-";
                    _item["STIPO_PERSONA"] = tipoPersona;
                    _item["SNUM_DOCUMENTO"] = "-";
                    _item["STIPO_DOCUMENTO"] = "-";
                    List<Dictionary<string, dynamic>> documents = new List<Dictionary<string, dynamic>>();
                    if (items[i].identityDocuments.Count > 0)
                    {
                        for (int j = 0; j < items[i].identityDocuments.Count; j++)
                        {
                            if (items[i].identityDocuments[j].locationType.type.Contains("RUC"))
                            {
                                _item["SNUM_DOCUMENTO"] = items[i].identityDocuments[j].number.Length == 11 ? items[i].identityDocuments[j].number : "-";
                                _item["STIPO_DOCUMENTO"] = "RUC";
                            }
                            else if (items[i].identityDocuments[j].locationType.type.Contains("DNI"))
                            {
                                _item["SNUM_DOCUMENTO"] = items[i].identityDocuments[j].number.Length == 8 ? items[i].identityDocuments[j].number : items[i].identityDocuments[j].number.Length == 7 ? "0" + items[i].identityDocuments[j].number : "-";
                                _item["STIPO_DOCUMENTO"] = "DNI";
                            }
                        }
                    }
                    if (_item["SLISTA"] == "LISTAS PEP")
                    {
                        string resultado = this.getProfiles(items[i].referenceId);
                        profile = JsonConvert.DeserializeObject<ResponseProfileDTO>(resultado);
                        if (profile.details.Count > 0)
                            for (int j = 0; j < profile.details.Count; j++)
                            {
                                if (profile.details[j].detailType == "REPORTS")
                                    _item["SCARGO"] = profile.details[j].text;
                            }
                    }
                    _item["SPORCEN_COINCIDENCIA"] = items[i].matchStrength == "STRONG" ? 75 : items[i].matchStrength == "EXACT" ? 100 : 0;
                    _items.Add(_item);
                }
            }
            return _items;
        }
        internal async Task<ResponseDTO> alertsProcessJD(ResquestAlert item)
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
                Case = _repository.getCaseJD(item.name);
                objDefault.name = item.name;
                objDefault.entityType = item.tipo;
                if (item.tipo == "ORGANISATION")
                    objDefault.secondaryFields = new List<Properties>();
                if (Case.Count == 0)
                {
                    result = createCase(objDefault);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                        objResult = obj.results.ToString();
                        caseSystemId = obj.caseSystemId.ToString();
                        caseId = obj.caseId.ToString();
                        if (caseId != "")
                            isCreate = true;
                    }
                }
                else
                {
                    result = getResults(Case[0].SCaseSystemId);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                        objResult = obj.ToString();
                        caseSystemId = Case[0].SCaseSystemId;
                        caseId = Case[0].SCaseId;
                    }
                }
                List<ResponseWc1> items = new List<ResponseWc1>();

                try
                {
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>((objResult).ToString());
                    if (items != null && items.Count > 0)
                    {
                        if (item.tipo != "ORGANISATION")
                            items = items.FindAll(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_5"));
                        items = items.FindAll(t => ValidPorcentage.Contains(t.matchStrength));
                        System.Console.WriteLine("individuo :" + item.name + " cantidad :" + items.Count);
                        if (items.Count > 0)
                        {
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
                                    response = this.SaveResultJD(items[i], caseSystemId);
                                    biography = response.sMessage;
                                    items[i].matchedTerm = this.formatearNombre(items[i].matchedTerm.Replace(',', ' ').Split(' '));
                                    items[i].primaryName = this.formatearNombre(items[i].primaryName.Replace(',', ' ').Split(' '));
                                    response = _repository.SaveResultCoincidenciasJD(items[i], item, response.nId, caseSystemId, caseId, biography);
                                    _response.sMessage = response.sMessage;
                                }
                                catch (Exception ex)
                                {
                                    _response.sMessage = response.sMessage;
                                    System.Console.WriteLine("error : " + i + " - " + items[i].primaryName + " - " + ex.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("error : " + " - " + ex.Message);
                }
            });
            return _response;
        }
        ResponseDTO SaveResultJD(ResponseWc1 item, string SystemCaseId)
        {
            ResponseDTO response = new ResponseDTO();
            ResponseProfileDTO profile = new ResponseProfileDTO();
            string resultado = this.getProfiles(item.referenceId);
            profile = JsonConvert.DeserializeObject<ResponseProfileDTO>(resultado);
            response = _repository.SaveResultJD(item, SystemCaseId);
            try
            {
                if (response.nId > 0)
                {
                    if (profile.details.Count > 0)
                        for (int j = 0; j < profile.details.Count; j++)
                        {
                            if (profile.details[j].detailType == "REPORTS")
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
        public async Task<ResponseDTO> getClients(ResquestAlert item)
        {
            List<ResponseDTO> _responses = new List<ResponseDTO>();
            ResponseDTO _response = new ResponseDTO();
            ResponseDTO _responsereturn = new ResponseDTO();
            _repository.deleteCoincidenciastmp(item.periodId);
            List<Dictionary<string, dynamic>> items = new List<Dictionary<string, dynamic>>();
            try
            {
                if (item.tipoCargaId == "1")
                {
                    List<Dictionary<string, dynamic>> grupos = _repository.GetGrupoSenal();
                    for (int j = 0; j < grupos.Count; j++)
                    {
                        item.grupoSenalId = grupos[j]["NIDGRUPOSENAL"].ToString();
                        item.items = _repository.getClientsForGroups(item);
                        List<string> names = item.items.Select(t => t["name"]).Distinct().ToList();
                        if (names.Count > 0)
                        {
                            for (int i = 0; i < names.Count; i++)
                            {
                                bool isruc = item.items.FindAll(t => t["name"] == names[i]).Exists(t => t["tipo"] == "1");
                                item.tipo = isruc ? "ORGANISATION" : "INDIVIDUAL";
                                _response = await searchCoincidence(item, names[i]);
                                _responses.Add(_response);
                            }
                        }
                    }
                }
                else
                {
                    _response = await searchCoincidence(item);
                    _responses.Add(_response);
                }
                if (_response.nCode == 0)
                {
                    _response = _repository.spcarga_coincidencias(item);
                }
                int fountCount = _responses.FindAll(t => t.sStatus != "NOT FOUND").Count;
                if (fountCount > 0)
                {
                    _responsereturn.nCode = 0;
                    _responsereturn.sMessage = "Si encontro coincidencia";
                }
                else
                {
                    _responsereturn.nCode = 0;
                    _responsereturn.sMessage = "No encontro coincidencia";
                }
                return _responsereturn;
            }
            catch (Exception ex)
            {
                _response = new ResponseDTO();
                _response.nCode = 1;
                _response.sMessage = ex.Message;
                return _response;
            }

        }
        public async Task<ResponseDTO> getClientsAutomatic(ResquestAlert item)
        {
            List<ResponseDTO> _responses = new List<ResponseDTO>();
            ResponseDTO _response = new ResponseDTO();
            ResponseDTO _responsereturn = new ResponseDTO();
            try
            {
                _response = await searchCoincidence(item);
                _responses.Add(_response);
                int fountCount = _responses.FindAll(t => t.sStatus != "NOT FOUND").Count;
                if (fountCount > 0)
                {
                    _responsereturn.nCode = 0;
                    _responsereturn.sMessage = "Si encontro coincidencia";
                }
                else
                {
                    _responsereturn.nCode = 0;
                    _responsereturn.sMessage = "No encontro coincidencia";
                }
                return _responsereturn;
            }
            catch (Exception ex)
            {
                _response = new ResponseDTO();
                _response.nCode = 1;
                _response.sMessage = ex.Message;
                return _response;
            }
        }
        public async Task<ResponseDTO> searchCoincidence(ResquestAlert item, dynamic name = null)
        {
            List<ObjCaseDTO> Case = new List<ObjCaseDTO>();
            ResponseDTO _response = new ResponseDTO();
            ResponseDTO response = new ResponseDTO();
            _response.sStatus = "NOT FOUND";
            _response.nCode = 0;
            bool isCreate = false;
            await Task.Run(() =>
            {
                string result = "";
                string objResult = "";
                string caseId = "";
                string caseSystemId = "";
                if (!istest)//validacion para tomar los json de prueba
                {
                    Case = _repository.getCase(item.name);
                    objDefault.name = item.name;
                    objDefault.entityType = item.tipo;
                    if (item.tipo == "ORGANISATION")
                        objDefault.secondaryFields = new List<Properties>();
                    if (Case.Count == 0)
                    {
                        result = createCase(objDefault);
                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                            objResult = obj.results.ToString();
                            caseSystemId = obj.caseSystemId.ToString();
                            caseId = obj.caseId.ToString();
                            if (caseId != "")
                                isCreate = true;
                        }
                    }
                    else
                    {
                        result = getResults(Case[0].SCaseSystemId);
                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                            objResult = obj.ToString();
                            caseSystemId = Case[0].SCaseSystemId;
                            caseId = Case[0].SCaseId;
                        }
                    }
                }
                else
                {
                    string path = "";
                    if (item.tipo == "ORGANISATION")
                    {
                        path = AppDomain.CurrentDomain.BaseDirectory + "JsonTest/Organizacion.json";
                    }
                    else
                    {
                        path = AppDomain.CurrentDomain.BaseDirectory + "JsonTest/Individuo.json";
                    }
                    path = path.Replace("\\bin\\Debug\\netcoreapp2.1", "");
                    using (StreamReader jsonStream = System.IO.File.OpenText(path))
                    {
                        objResult = jsonStream.ReadToEnd();
                    }
                }
                List<ResponseWc1> items = new List<ResponseWc1>();

                try
                {
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>((objResult).ToString());
                    if (items != null && items.Count > 0)
                    {
                        if (item.tipo != "ORGANISATION")
                            items = items.FindAll(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_5"));
                        items = items.FindAll(t => ValidPorcentage.Contains(t.matchStrength));
                        System.Console.WriteLine("individuo :" + item.name + " cantidad :" + items.Count);
                        if (items.Count > 0)
                        {
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
                                string biography = "";
                                string informacion = "";
                                items[i].categories = items[i].categories.Distinct().ToList();
                                response = this.SaveResult(items[i], caseSystemId);
                                biography = response.informacionComplementaria["SCARGO"];
                                informacion = response.informacionComplementaria["SINFORMACION"];
                                items[i].matchedTerm = this.formatearNombre(items[i].matchedTerm.Replace(',', ' ').Split(' '));
                                items[i].primaryName = this.formatearNombre(items[i].primaryName.Replace(',', ' ').Split(' '));
                                response = _repository.SaveResultCoincidencias(items[i], item, response.nId, caseSystemId, caseId, biography, informacion);
                                _response.sMessage = response.sMessage;
                                _response.nCode = response.nCode;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return _response;
        }
    }
}
