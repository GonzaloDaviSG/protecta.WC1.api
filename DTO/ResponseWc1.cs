using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    [Serializable]
    [DataContract]
    public class ResponseWc1
    {
        [DataMember]
        public string resultId { get; set; }
        [DataMember]
        public string referenceId { get; set; }
        [DataMember]
        public string matchStrength { get; set; }
        [DataMember]
        public string matchedTerm { get; set; }
        [DataMember]
        public string submittedTerm { get; set; }
        [DataMember]
        public string matchedNameType { get; set; }
        [DataMember]
        public List<string> secondaryFieldResults { get; set; }
        [DataMember]
        public List<string> sources { get; set; }
        [DataMember]
        public List<string> categories { get; set; }
        [DataMember]
        public string creationDate { get; set; }
        [DataMember]
        public string modificationDate { get; set; }
        [DataMember]
        public string resolution { get; set; }
        [DataMember]
        public ResultReview resultReview { get; set; }
        [DataMember]
        public string primaryName { get; set; }
        [DataMember]
        public List<Events> events { get; set; }
        [DataMember]
        public List<CountryLinks> countryLinks {get; set;}
        [DataMember]
        public List<IdentityDocuments> identityDocuments { get; set; }
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public string providerType { get; set; }
        [DataMember]
        public string gender { get; set; }
    }

    public class IdentityDocuments
    {
        [DataMember]
        public string entity { get; set; }
        [DataMember]
        public string expiryDate { get; set; }
        [DataMember]
        public string issueDate { get; set; }
        [DataMember]
        public string issuer { get; set; }
        [DataMember]
        public LocationType locationType { get; set; }
        [DataMember]
        public string number { get; set; }
        [DataMember]
        public string type { get; set; }
    }

    public class LocationType
    {
        [DataMember]
        public Country country { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string type { get; set; }
    }

    public class CountryLinks
    {
        [DataMember]
        public Country country { get; set; }
        [DataMember]
        public string countryText { get; set; }
        [DataMember]
        public string type { get; set; }
    }

    public class Country
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string name { get; set; }
    }

    public class ResultReview
    {
        [DataMember]
        public bool reviewRequired { get; set; }
        [DataMember]
        public string reviewRequiredDate { get; set; }
        [DataMember]
        public string reviewRemark { get; set; }
        [DataMember]
        public string reviewDate { get; set; }
    }

    public class Events
    {
        [DataMember]
        public Address address { get; set; }
        [DataMember]
        public List<string> allegedAddresses { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string fullDate { get; set; }
        [DataMember]
        public string month { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string year { get; set; }
    }

    public class Address
    {
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public Country country { get; set; }
        [DataMember]
        public string postCode { get; set; }
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string street { get; set; }
    }
}
