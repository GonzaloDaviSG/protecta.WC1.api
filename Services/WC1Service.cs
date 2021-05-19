using Newtonsoft.Json;
using protecta.WC1.api.DTO;
using protecta.WC1.api.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace protecta.WC1.api.Services
{
    public class WC1Service
    {
        public WC1Service()
        {

        }

        public List<ResponseWc1> Create(RequestWC1DTO item) {
            item.item.groupId = Config.AppSetting["WordlCheckOne:groupId"];
            string values = this.createCase(item);

            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(values);
            string values2 = this.confirmCase(obj.caseId.ToString());

            dynamic obj2 = Newtonsoft.Json.JsonConvert.DeserializeObject(values2);
            string values3 = this.getResults(obj.caseSystemId.ToString());

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseWc1>>(values3);
        }

        internal object Resolution(RequestWC1ResolutionDTO item)
        {
            string sRequest = JsonConvert.SerializeObject(item.item);
            string sMethod = $"cases/{item.caseSystemId}/results/resolution";
            string response = this.putReques(sMethod, sRequest);
            return response;
        }

        private string createCase(RequestWC1DTO item) {
            string sRequest = JsonConvert.SerializeObject(item.item);
            //string sRequest = JsonConvert.SerializeObject($"{{\"secondaryFields\":[],\"entityType\":\"INDIVIDUAL\",\"customFields\":[],\"groupId\":\"5jb8bs1tdnwv1fnb5aqmq6kyc\",\"providerTypes\":[\"WATCHLIST\"],\"name\":\"putin\"}}");
            string sMethod = "cases/screeningRequest";
            string response = this.postReques(sMethod, sRequest);
            return response;
        }
        private string confirmCase(string caseId)
        {
            string sMethod = $"caseReferences";
            string response = this.getReques(sMethod, "?caseId="+ caseId);
            return response;
        }
        private string getResults(string caseSystemId)
        {
            string sMethod = $"cases";
            string response = this.getReques(sMethod + "/" + caseSystemId + "/results");
            return response;
        }
        public string postReques(string sMethod, string sRequest) {
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
                Thread.Sleep(2000);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) { 
                    Stream Answer = response.GetResponseStream();
                    StreamReader _Answer = new StreamReader(Answer);
                    string jsontxt = _Answer.ReadToEnd();
                    return jsontxt;
                }
                    ;
                
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
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream Answer = response.GetResponseStream();
                StreamReader _Answer = new StreamReader(Answer);
                string jsontxt = _Answer.ReadToEnd();
                return jsontxt;
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
                    Stream Answer = response.GetResponseStream();
                    StreamReader _Answer = new StreamReader(Answer);
                    string jsontxt = _Answer.ReadToEnd();
                    return jsontxt;
                }
                    ;

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
        
    }
}
