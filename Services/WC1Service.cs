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
        public int ValidPorcentage;
        public int ValidPorcentageDemanda;
        public List<String> ValidTipDoc;

        List<Properties> FiltrosOrganization;
        List<Properties> FiltrosIndivual;
        WC1Repository _repository;
        RequestWc1 objDefault;
        List<int> records = new List<int>();
        public WC1Service()
        {
            ValidPorcentage = 90;
            ValidPorcentageDemanda = 75;
            ValidTipDoc = new List<String> { "1", "2" };
            _repository = new WC1Repository();
            objDefault = new RequestWc1();
            objDefault.providerTypes = new List<string>() { "WATCHLIST" };
            objDefault.entityType = "INDIVIDUAL";
            objDefault.groupId = Config.AppSetting["WordlCheckOne:groupId"];
            objDefault.customFields = new List<Properties>();
            objDefault.secondaryFields = new List<Properties>();
            objDefault.nameTransposition = true;
            FiltrosIndivual = new List<Properties>();
            FiltrosOrganization = new List<Properties>();
            FiltrosIndivual.Add(new Properties() { typeId = "SFCT_5", value = "PER" });
            FiltrosOrganization.Add(new Properties() { typeId = "SFCT_6", value = "PER" });
            //objDefault.secondaryFields.Add(new Properties() { typeId = "SFCT_6", value = "PER" });
            records.Add(1);
            records.Add(2);
            records.Add(3);
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
        public string archivateCase(string caseSystemId)
        {
            //string sRequest = JsonConvert.SerializeObject($"{{\"secondaryFields\":[],\"entityType\":\"INDIVIDUAL\",\"customFields\":[],\"groupId\":\"5jb8bs1tdnwv1fnb5aqmq6kyc\",\"providerTypes\":[\"WATCHLIST\"],\"name\":\"putin\"}}");
            string sMethod = $"cases/{caseSystemId}/archive";
            string response = "";
            do
            {
                response = this.putReques(sMethod, "");
            } while (response == "429");
            return response;
        }
        public string deleteCase(string caseSystemId)
        {
            //string sRequest = JsonConvert.SerializeObject($"{{\"secondaryFields\":[],\"entityType\":\"INDIVIDUAL\",\"customFields\":[],\"groupId\":\"5jb8bs1tdnwv1fnb5aqmq6kyc\",\"providerTypes\":[\"WATCHLIST\"],\"name\":\"putin\"}}");
            string sMethod = $"cases/{caseSystemId}";
            string response = "";
            do
            {
                response = this.deleteReques(sMethod, "");
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

        internal Task<Dictionary<string, string>> deleteCases(CasesEntity _item = null)
        {
            Task<Dictionary<string, string>> response = null;
            try
            {
                response = Task.Run(async () =>
                {
                    Dictionary<string, string> resp = new Dictionary<string, string>();

                    List<CasesEntity> items = new List<CasesEntity>();
                    if (_item != null)
                    {
                        items.Add(_item);
                    }
                    else {
                        items = _repository.getCases();
                    }
                    string resDelete = "";
                    string resArchivate = "";
                    for (int i = 0; i < items.Count; i++)
                    {
                        resDelete = "";
                        resArchivate = "";
                        if (string.IsNullOrWhiteSpace(items[i].caseSystemId))
                        {
                            string _resConfirm = confirmCase(items[i].caseId);
                            if (!string.IsNullOrWhiteSpace(_resConfirm))
                            {
                                CasesEntity item = JsonConvert.DeserializeObject<CasesEntity>(_resConfirm);
                                items[i].caseSystemId = item.caseSystemId;
                            }
                        }
                        resArchivate = archivateCase(items[i].caseSystemId);
                        if (resArchivate == "204")
                            resDelete = deleteCase(items[i].caseSystemId);
                        if (resDelete == "204")
                            items[i].isFinish = true;
                        else
                            items[i].isFinish = false;
                    }
                    int countError = items.Count(t => !t.isFinish);
                    if (countError > 0)
                    {
                        resp["Mensaje"] = "Error en " + countError + " casos de " + items.Count;
                        resp["code"] = "1";
                        log.Info("los errores estan en los siguientes casos : " + string.Join(",", items.FindAll(t => !t.isFinish).Select(t => t.caseId).ToArray()));
                    }
                    else
                    {
                        resp["Mensaje"] = "se han Eliminado " + items.Count + " casos.";
                        resp["code"] = "0";
                    }
                    if (_item == null)
                        _repository.deleteCases(items.FindAll(t => t.isFinish).Select(t => t.caseId).ToArray());
                    return resp;
                });
                response.Wait();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
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
            try
            {

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
                log.Error(ex.Message);
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
                Thread.Sleep(500);
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
                log.Error(ex.Message);
                // Handle error
            }
            return "";
        }
        public string deleteReques(string sMethod, string sRequest = "")
        {
            var url = Config.AppSetting["WordlCheckOne:protocol"] + Config.AppSetting["WordlCheckOne:gateWayHost"] + Config.AppSetting["WordlCheckOne:gateWayUrl"];
            //string json = sRequest;
            DateTime dDate = DateTime.UtcNow;
            string date = dDate.ToString("R");
            //byte[] byte1 = Encoding.UTF8.GetBytes(json);
            //int NLength = byte1.Length;
            string firma = $"(request-target): delete {Config.AppSetting["WordlCheckOne:gateWayUrl"]}{sMethod}\nhost: {Config.AppSetting["WordlCheckOne:gateWayHost"]}\ndate: {date}";
            string base64 = this.generateAuthHeader(firma);
            string sAuthorisation = $"Signature keyId=\"{Config.AppSetting["WordlCheckOne:appKey"]}\",algorithm=\"hmac-sha256\",headers=\"(request-target) host date\" ,signature=\"{base64}\"";

            var request = (HttpWebRequest)WebRequest.Create(url + sMethod + sRequest);
            request.Method = "DELETE";
            request.Date = dDate;
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Authorization", sAuthorisation);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return (int)response.StatusCode + "";
                };

            }
            catch (WebException ex)
            {
                log.Error(ex.Message);
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
            //string firma = $"(request-target): put {Config.AppSetting["WordlCheckOne:gateWayUrl"]}{sMethod}\nhost: {Config.AppSetting["WordlCheckOne:gateWayHost"]}\ndate: {date}\ncontent-type: {Config.AppSetting["WordlCheckOne:content"]}\ncontent-length: {NLength}\n{json}";
            string firma = $"(request-target): put {Config.AppSetting["WordlCheckOne:gateWayUrl"]}{sMethod}\nhost: {Config.AppSetting["WordlCheckOne:gateWayHost"]}\ndate: {date}";
            string base64 = this.generateAuthHeader(firma);
            //string sAuthorisation = $"Signature keyId=\"{Config.AppSetting["WordlCheckOne:appKey"]}\",algorithm=\"hmac-sha256\",headers=\"(request-target) host date content-type content-length\" ,signature=\"{base64}\"";
            string sAuthorisation = $"Signature keyId=\"{Config.AppSetting["WordlCheckOne:appKey"]}\",algorithm=\"hmac-sha256\",headers=\"(request-target) host date \" ,signature=\"{base64}\"";

            var request = (HttpWebRequest)WebRequest.Create(url + sMethod);
            request.Method = "PUT";
            //request.ContentType = "application/json";
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
                //Thread.Sleep(2000);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    return (int)response.StatusCode + "";
                };

            }
            catch (WebException ex)
            {
                log.Error(ex.Message);
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
                log.Error(ex.Message);
                throw;
            }
            return response;
        }
        internal async Task<ListResponseDTO> getCoincidenceNotPep(ResquestAlert item)
        {

            string caseId = "";
            string caseSystemId = "";
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
            List<Dictionary<string, string>> filters = new List<Dictionary<string, string>>();
            response.Data = new List<DataEntity>();
            bool isExist = false;
            await Task.Run(() =>
            {
                try
                {

                    filters = this.prepareFilter(item);
                    for (int f = 0; f < filters.Count; f++)
                    {
                        List<ResponseWc1> _items = new List<ResponseWc1>();
                        item.tipo = filters[f]["tipoEntidad"];
                        item.typeDocument = filters[f]["typeDocument"];
                        objDefault = getRequest(item);
                        string result = createCase(objDefault);
                        _items = getResponse(result, item, true, out caseId, out caseSystemId);
                        items.AddRange(_items);
                    }
                    for (int i = 0; i < items.Count; i++)
                    {
                        DataEntity dataitem = new DataEntity();
                        if (items[i].categories.Contains("PEP"))
                        {
                            response.isPep = true;
                            dataitem.name = items[i].matchedTerm;
                            dataitem.percentage = getPorcentaje(items[i].matchStrength);
                            dataitem.percentageScore = double.Parse(items[i].matchScore);
                            List<IdentityDocuments> _items = items[i].identityDocuments.FindAll(t => t.type == "DNI");
                            if (_items.Count > 0)
                            {
                                dataitem.documentNumber = this.validarDni(_items[0].number);
                                dataitem.documentType = _items[0].type ?? "-";
                            }
                            response.Data.Add(dataitem);
                        }
                        else
                            if (getPorcentaje(items[i].matchStrength) == 100)
                            response.isOtherList = true;
                        if (item.idDocNumber != "")
                            if (!response.isIdNumber)
                                response.isIdNumber = items[i].identityDocuments.Exists(t => validarDni(t.number) == item.idDocNumber);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error  :" + ex.Message);
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
            List<ResquestAlert> itemsBusqueda = new List<ResquestAlert>();
            List<CoincidenciaDemanda> coincidencias = new List<CoincidenciaDemanda>();
            ResponseDTO profiles = new ResponseDTO();
            try
            {
                List<Dictionary<string, dynamic>> listas = _repository.getListasPorFuente();
                listas = listas.FindAll(t => int.Parse(t["NIDPROVEEDOR"].ToString()) == 4);
                List<CoincidenciaDemanda> response = new List<CoincidenciaDemanda>();
                if (item.tipoBusqueda == 2)
                {

                    itemsBusqueda = _repository.getDemandas(item.codBusqueda);
                    if (itemsBusqueda.Count > 0)
                    {
                        for (int i = 0; i < itemsBusqueda.Count; i++)
                        {
                            List<CoincidenciaDemanda> _coincidencias = null;
                            _coincidencias = getCoincidenciasDemanda(itemsBusqueda[i], true);
                            coincidencias.AddRange(_coincidencias);
                        }
                    }
                    log.Info("luego de obtener todas las coincidencias");
                    for (int i = 0; i < itemsBusqueda.Count; i++)
                    {
                        for (int j = 0; j < listas.Count; j++)
                        {
                            CoincidenciaDemanda _item = null;
                            List<CoincidenciaDemanda> _coincidencias = coincidencias.FindAll(t => t.SNOMBRE_BUSQUEDA == itemsBusqueda[i].name);
                            for (int x = 0; x < _coincidencias.Count; x++)
                            {
                                _item = new CoincidenciaDemanda();
                                if (_coincidencias[x].NIDTIPOLISTA == int.Parse(listas[j]["NIDTIPOLISTA"].ToString()) && _coincidencias[x].SNOMBRE_BUSQUEDA == itemsBusqueda[i].name)
                                {

                                    _coincidencias[x].SCOINCIDENCIA = "CON COINCIDENCIA";
                                    _coincidencias[x].SDESTIPOLISTA = listas[j]["SDESTIPOLISTA"].ToString();
                                    _coincidencias[x].SDESPROVEEDOR = listas[j]["SDESPROVEEDOR"].ToString();
                                    _coincidencias[x].NIDPROVEEDOR = int.Parse(listas[j]["NIDPROVEEDOR"].ToString());
                                    _coincidencias[x].SCOINCIDENCIA = "CON COINCIDENCIA";
                                    _coincidencias[x].STIPOCOINCIDENCIA = "NOMBRE";
                                    _item = _coincidencias[x];
                                    _item.SUSUARIO_BUSQUEDA = item.usuario;
                                    response.Add(_item);
                                }
                                else
                                {
                                    _item.SNOMBRE_BUSQUEDA = itemsBusqueda[i].name;
                                    _item.NIDTIPOLISTA = int.Parse(listas[j]["NIDTIPOLISTA"].ToString());
                                    _item.SDESTIPOLISTA = listas[j]["SDESTIPOLISTA"].ToString();
                                    _item.NIDPROVEEDOR = int.Parse(listas[j]["NIDPROVEEDOR"].ToString());
                                    _item.SDESPROVEEDOR = listas[j]["SDESPROVEEDOR"].ToString();
                                    _item.SUSUARIO_BUSQUEDA = item.usuario;
                                    _item.SCOINCIDENCIA = "SIN COINCIDENCIA";
                                    response.Add(_item);
                                }
                            }
                            if (_coincidencias.Count == 0)
                            {
                                _item = new CoincidenciaDemanda();
                                _item.SNOMBRE_BUSQUEDA = itemsBusqueda[i].name;
                                _item.NIDTIPOLISTA = int.Parse(listas[j]["NIDTIPOLISTA"].ToString());
                                _item.SDESTIPOLISTA = listas[j]["SDESTIPOLISTA"].ToString();
                                _item.NIDPROVEEDOR = int.Parse(listas[j]["NIDPROVEEDOR"].ToString());
                                _item.SDESPROVEEDOR = listas[j]["SDESPROVEEDOR"].ToString();
                                _item.SUSUARIO_BUSQUEDA = item.usuario;
                                _item.SCOINCIDENCIA = "SIN COINCIDENCIA";
                                response.Add(_item);

                            }
                        }
                    }
                }
                else
                {
                    coincidencias = getCoincidenciasDemanda(item, false);
                    for (int j = 0; j < listas.Count; j++)
                    {
                        CoincidenciaDemanda _item = null;
                        for (int x = 0; x < coincidencias.Count; x++)
                        {
                            _item = new CoincidenciaDemanda();
                            if (coincidencias[x].NIDTIPOLISTA == int.Parse(listas[j]["NIDTIPOLISTA"]) && coincidencias[x].SNOMBRE_BUSQUEDA == item.name)
                            {
                                coincidencias[x].SCOINCIDENCIA = "CON COINCIDENCIA";
                                coincidencias[x].SDESTIPOLISTA = listas[j]["SDESTIPOLISTA"].ToString();
                                coincidencias[x].SDESPROVEEDOR = listas[j]["SDESPROVEEDOR"].ToString();
                                coincidencias[x].NIDPROVEEDOR = int.Parse(listas[j]["NIDPROVEEDOR"].ToString());
                                _item.SUSUARIO_BUSQUEDA = item.usuario;
                                coincidencias[x].SCOINCIDENCIA = "CON COINCIDENCIA";
                                coincidencias[x].STIPOCOINCIDENCIA = "NOMBRE";
                                _item = coincidencias[x];
                                response.Add(_item);
                            }
                            else
                            {
                                _item.SNOMBRE_BUSQUEDA = item.name;
                                _item.NIDTIPOLISTA = int.Parse(listas[j]["NIDTIPOLISTA"].ToString());
                                _item.SDESTIPOLISTA = listas[j]["SDESTIPOLISTA"].ToString();
                                _item.NIDPROVEEDOR = int.Parse(listas[j]["NIDPROVEEDOR"].ToString());
                                _item.SDESPROVEEDOR = listas[j]["SDESPROVEEDOR"].ToString();
                                _item.SUSUARIO_BUSQUEDA = item.usuario;
                                _item.SCOINCIDENCIA = "SIN COINCIDENCIA";
                                response.Add(_item);
                            }
                        }
                        if (coincidencias.Count == 0)
                        {
                            _item = new CoincidenciaDemanda();
                            _item.SNOMBRE_BUSQUEDA = item.name;
                            _item.NIDTIPOLISTA = int.Parse(listas[j]["NIDTIPOLISTA"].ToString());
                            _item.SDESTIPOLISTA = listas[j]["SDESTIPOLISTA"].ToString();
                            _item.NIDPROVEEDOR = int.Parse(listas[j]["NIDPROVEEDOR"].ToString());
                            _item.SDESPROVEEDOR = listas[j]["SDESPROVEEDOR"].ToString();
                            _item.SUSUARIO_BUSQUEDA = item.usuario;
                            _item.SCOINCIDENCIA = "SIN COINCIDENCIA";
                            response.Add(_item);

                        }
                    }
                }
                respuesta.sMessage = "Termino la busqueda satisfactoriamente.";
                respuesta.nCode = 0;
                respuesta.Items = new List<CoincidenciaDemanda>();
                respuesta.Items.AddRange(response);
                return respuesta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                respuesta.sMessage = ex.Message;
                respuesta.nCode = 1;
                return respuesta;
            }
        }
        public List<CoincidenciaDemanda> getCoincidenciasDemanda(ResquestAlert item, bool isMasive)
        {

            string caseId = "";
            string caseSystemId = "";
            List<CoincidenciaDemanda> _items = new List<CoincidenciaDemanda>();
            CasesEntity elementCase = new CasesEntity();

            CoincidenciaDemanda _item = null;
            List<ResponseWc1> items = new List<ResponseWc1>();
            List<Dictionary<string, string>> filters = new List<Dictionary<string, string>>();
            filters = this.prepareFilter(item);
            log.Info("antes de crear los filtres " + item.name);
            for (int f = 0; f < filters.Count; f++)
            {
                item.tipo = filters[f]["tipoEntidad"];
                item.typeDocument = filters[f]["typeDocument"];
                objDefault = this.getRequest(item);
                string result = createCase(objDefault);
                items = this.getResponse(result, item, true, out caseId, out caseSystemId);
                elementCase.caseId = caseId;
                elementCase.caseSystemId = caseSystemId;
                log.Info("luego de obtener el getresponse  " + item.name);
                if (items.Count > 0)
                {

                    ResponseProfileDTO profile = new ResponseProfileDTO();
                    for (int i = 0; i < items.Count; i++)
                    {
                        _item = new CoincidenciaDemanda();
                        _item.SNOMBRE_COMPLETO = items[i].primaryName;
                        _item.SNOMBRE_BUSQUEDA = item.name;
                        _item.SNOMBRE_TERMINO = items[i].matchedTerm.ToString();
                        if (items[i].categories.Count > 0)
                        {
                            _item.NIDTIPOLISTA = items[i].categories.Contains("PEP") ? 2 : 1;
                        }
                        _item.DFECHA_BUSQUEDA = DateTime.Now.ToString();
                        _item.SCARGO = "-";
                        _item.STIPO_PERSONA = filters[f]["tipoPersona"];
                        _item.SNUM_DOCUMENTO = "-";
                        _item.STIPO_DOCUMENTO = "-";
                        List<Dictionary<string, dynamic>> documents = new List<Dictionary<string, dynamic>>();
                        if (items[i].identityDocuments.Count > 0)
                        {
                            for (int j = 0; j < items[i].identityDocuments.Count; j++)
                            {
                                if (items[i].identityDocuments[j].locationType.type.Contains("RUC"))
                                {
                                    _item.SNUM_DOCUMENTO = items[i].identityDocuments[j].number.Length == 11 ? items[i].identityDocuments[j].number : "-";
                                    _item.STIPO_DOCUMENTO = "RUC";
                                }
                                else if (items[i].identityDocuments[j].locationType.type.Contains("DNI"))
                                {
                                    _item.SNUM_DOCUMENTO = validarDni(items[i].identityDocuments[j].number);
                                    _item.STIPO_DOCUMENTO = "DNI";
                                }
                            }
                        }
                        //if (_item.NIDTIPOLISTA == 2)
                        //{
                            string resultado = this.getProfiles(items[i].referenceId);
                            profile = JsonConvert.DeserializeObject<ResponseProfileDTO>(resultado);
                            if (profile.details.Count > 0)
                                for (int j = 0; j < profile.details.Count; j++)
                                {
                                    if (profile.details[j].detailType == "BIOGRAPHY")
                                        _item.SCARGO = profile.details[j].text;
                                    if (profile.details[j].detailType == "REPORTS")
                                        _item.SINFORMACION = profile.details[j].text;
                                }
                        //}
                        if (!string.IsNullOrWhiteSpace(items[i].matchScore))
                        {
                            if (double.Parse(items[i].matchScore) > 0)
                                _item.SPORCEN_COINCIDENCIA = ((int)double.Parse(items[i].matchScore)).ToString();
                        }
                        else
                        {
                            _item.SPORCEN_COINCIDENCIA = "0";
                        }
                        //_item["SPORCEN_COINCIDENCIA"] = items[i].matchStrength == "STRONG" ? 75 : items[i].matchStrength == "EXACT" ? 100 : 0;
                        //_item["SPORCEN_SCORE"] = double.Parse(items[i].matchScore);
                        _items.Add(_item);
                    }
                }
            }
            this.deleteCases(elementCase);
            return _items;
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
                log.Error(ex.Message);
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
                log.Error(ex.Message);
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

            List<Dictionary<string, string>> filters = new List<Dictionary<string, string>>();
            _response.sStatus = "NOT FOUND";
            _response.nCode = 0;
            bool isCreate = false;
            await Task.Run(() =>
            {
                string result = "";
                string caseId = "";
                string caseSystemId = "";
                List<ResponseWc1> items = new List<ResponseWc1>();
                Case = _repository.getCase(item.name);

                filters = this.prepareFilter(item);
                for (int f = 0; f < filters.Count; f++)
                {
                    List<ResponseWc1> _items = new List<ResponseWc1>();
                    item.tipo = filters[f]["tipoEntidad"];
                    item.typeDocument = filters[f]["typeDocument"];
                    objDefault = this.getRequest(item);
                    if (Case.Count == 0)
                    {
                        result = createCase(objDefault);
                        _items = this.getResponse(result, item, true, out caseId, out caseSystemId);
                        if (caseId != "")
                            isCreate = true;
                    }
                    else
                    {
                        result = getResults(Case[0].SCaseSystemId);
                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            _items = this.getResponse(result, item, false, out caseId, out caseSystemId);
                            caseSystemId = Case[0].SCaseSystemId;
                            caseId = Case[0].SCaseId;
                        }
                    }
                    items.AddRange(_items);
                }





                try
                {
                    if (items != null && items.Count > 0)
                    {
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
                    log.Error(ex.Message);
                    throw ex;
                }
            });
            return _response;
        }
        public string validarDni(string input)
        {
            if (input != "")
            {
                var numCaracteres = input.Length;
                if (numCaracteres < 8)
                {
                    var numRestantes = 8 - numCaracteres;
                    for (int i = 0; i < numRestantes; i++)
                    {
                        input = "0" + input;
                    }
                }
            }
            else
                input = "-";
            return input;
        }
        private List<Dictionary<string, string>> prepareFilter(ResquestAlert item)
        {
            List<Dictionary<string, string>> filters = new List<Dictionary<string, string>>();
            Dictionary<string, string> filter = new Dictionary<string, string>();

            if (item.typeDocument == "2" || (item.typeDocument == "1" && item.idDocNumber.StartsWith("20")))
            {
                filter["tipoEntidad"] = item.typeDocument == "2" ? "INDIVIDUAL" : "ORGANISATION";
                filter["tipoPersona"] = item.typeDocument == "2" ? "PERSONA NATURAL" : "EMPRESA  (PERSONA JURÍDICA)";
                filter["typeDocument"] = item.typeDocument;
                filters.Add(filter);
            }
            else if (!String.IsNullOrWhiteSpace(item.idDocNumber) && (item.idDocNumber.Length > 8 && item.idDocNumber.StartsWith("20")) && String.IsNullOrWhiteSpace(item.typeDocument))
            {
                filter["tipoEntidad"] = item.idDocNumber.Length <= 8 ? "INDIVIDUAL" : "ORGANISATION";
                filter["tipoPersona"] = item.idDocNumber.Length <= 8 ? "PERSONA NATURAL" : "EMPRESA  (PERSONA JURÍDICA)";
                filter["typeDocument"] = item.idDocNumber.Length <= 8 ? "2" : "1";
                filters.Add(filter);
            }
            else
            {
                filter["tipoEntidad"] = "INDIVIDUAL";
                filter["tipoPersona"] = "PERSONA NATURAL";
                filter["typeDocument"] = "2";
                filters.Add(filter);
                filter = new Dictionary<string, string>();
                filter["tipoEntidad"] = "ORGANISATION";
                filter["tipoPersona"] = "EMPRESA  (PERSONA JURÍDICA)";
                filter["typeDocument"] = "1";
                filters.Add(filter);
            }
            return filters;
        }
        private RequestWc1 getRequest(ResquestAlert item)
        {
            objDefault.name = item.name;
            objDefault.entityType = item.tipo;
            objDefault.secondaryFields = new List<Properties>();
            if (item.tipo == "ORGANISATION")
            {
                if (ValidTipDoc.Contains(item.typeDocument))
                    objDefault.secondaryFields.AddRange(FiltrosOrganization);
            }
            else
            {
                objDefault.secondaryFields.AddRange(FiltrosIndivual);
                if (!String.IsNullOrWhiteSpace(item.tipoSex))
                    if (!(int.Parse(item.tipoSex) > 2))
                        objDefault.secondaryFields.Add(new Properties() { typeId = "SFCT_1", value = (item.tipoSex == "1" ? "FEMALE" : "MALE") });
                if (!String.IsNullOrWhiteSpace(item.birthday))
                    objDefault.secondaryFields.Add(new Properties() { typeId = "SFCT_2", dateTimeValue = item.birthday });
            }
            return objDefault;
        }
        private List<ResponseWc1> getResponse(string result, ResquestAlert item, bool isCase, out string caseId, out string caseSystemId)
        {
            List<ResponseWc1> items = new List<ResponseWc1>();
            string objResult = "";
            if (!String.IsNullOrWhiteSpace(result))
            {
                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                caseSystemId = isCase ? obj.caseSystemId.ToString() : "";
                caseId = isCase ? obj.caseId.ToString() : "";
                objResult = isCase ? obj.results.ToString() : obj.ToString();


            }
            else
            {
                caseSystemId = "";
                caseId = "";
            }
            items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>((objResult).ToString());
            if (items != null && items.Count > 0)
            {
                List<ResponseWc1> itemsCoincidenceDoc = items
                                                       .FindAll(t =>
                                                           t.identityDocuments
                                                           .FindAll(d => d.locationType.type == (item.typeDocument == "1" ? "PE-RUC" : "PE-DNI") && (validarDni(d.number) == item.idDocNumber)).Count > 0
                                                       );
                if (itemsCoincidenceDoc.Count > 0)
                {
                    items = itemsCoincidenceDoc;
                }
                else
                {
                    List<ResponseWc1> itemsCoincidencefilter = new List<ResponseWc1>();
                    if (item.tipo != "ORGANISATION")
                    {

                        items = items.FindAll(t =>
                                                                t.secondaryFieldResults.Exists(t2 =>
                                                                                               t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_5"));
                        if (!String.IsNullOrEmpty(item.tipoSex))
                            if (!(int.Parse(item.tipoSex) > 2))
                                items = itemsCoincidencefilter.FindAll(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_1"));
                        if (!String.IsNullOrEmpty(item.birthday))
                            items = itemsCoincidencefilter.FindAll(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_2"));
                        items = items.FindAll(t => double.Parse(t.matchScore) >= ValidPorcentage);
                    }
                    else
                    {
                        items = items.FindAll(t => !(t.categories.FindAll(t2 => t2 == "PEP").Count > 0));
                        if (ValidTipDoc.Contains(item.typeDocument))
                            itemsCoincidencefilter = items.FindAll(t => t.secondaryFieldResults.Exists(t2 => t2.fieldResult == "MATCHED" && t2.typeId == "SFCT_6"));
                        else
                        {
                            itemsCoincidencefilter = items;
                        }
                        items = itemsCoincidencefilter.FindAll(t => double.Parse(t.matchScore) >= ValidPorcentage);
                    }
                }
            }

            return items;
        }
    }
}
