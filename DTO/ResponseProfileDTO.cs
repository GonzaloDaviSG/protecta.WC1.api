using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    [Serializable]
    [DataContract]
    public class ResponseProfileDTO
    {
        //[DataMember]
        //public string actions { get; set;}
        [DataMember]
        public string active { get; set; }
        [DataMember]
        public List<Address> addresses { get; set; }
        [DataMember]
        public List<EntityAssociates> associates { get; set; }
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public string comments { get; set; }
        //[DataMember]
        //public string contacts { get; set; }
        [DataMember]
        public List<CountryLinks> countryLinks { get; set; }
        [DataMember]
        public string creationDate { get; set; }
        [DataMember]
        public string deletionDate { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public List<EntityDetail> details { get; set; }
        [DataMember]
        public string entityId { get; set; }
        [DataMember]
        public string externalImportId { get; set; }
        //[DataMember]
        //public string files { get; set; }
        [DataMember]
        public List<IdentityDocuments> identityDocuments { get; set; }
        //[DataMember]
        //public string images { get; set; }
        [DataMember]
        public string lastAdjunctChangeDate { get; set; }
        [DataMember]
        public string modificationDate { get; set; }
        [DataMember]
        public List<EntityNames> names { get; set; }
        //[DataMember]
        //public string previousCountryLinks { get; set; }
        [DataMember]
        public Provider provider { get; set; }
        [DataMember]
        public string sourceDescription { get; set; }
        //[DataMember]
        //public string sourceUris { get; set; }
        [DataMember]
        public List<ActionEntitySource> sources { get; set; }
        [DataMember]
        public string subCategory { get; set; }
        [DataMember]
        public string updateCategory { get; set; }
        [DataMember]
        public EntityUpdatedDates updatedDates { get; set; }
        [DataMember]
        public List<File> weblinks { get; set; }
        [DataMember]
        public string gender { get; set; }
        //[DataMember]
        //public string roles { get; set; }
        [DataMember]
        public string ageAsOfDate { get; set; }
        [DataMember]
        public string isDeceased { get; set; }
        [DataMember]
        public List<Events> events { get; set; }
        //[DataMember]
        //public string previousRoles { get; set; }
        [DataMember]
        public string age { get; set; }
        [DataMember]
        public string entityType { get; set; }
    }
}
