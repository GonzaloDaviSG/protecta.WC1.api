using protecta.WC1.api.DTO;
using protecta.WC1.api.Repository.Interfaces;
using System;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace protecta.WC1.api.Repository
{
    public class WC1Repository : IWC1Repository
    {
        private DB.ApplicationDbContext context;
        public WC1Repository()
        {
            this.context = new DB.ApplicationDbContext(DB.ApplicationDB.UsarOracle());
        }

        public ResponseDTO SaveResult(ResponseWc1 item, string SystemCaseId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_SSYSTEMCASEID = new OracleParameter("P_SSYSTEMCASEID", OracleDbType.NVarchar2, SystemCaseId, ParameterDirection.Input);
                OracleParameter P_SRESULTID = new OracleParameter("P_SRESULTID", OracleDbType.NVarchar2, item.resultId, ParameterDirection.Input);
                OracleParameter P_SREFERENCEID = new OracleParameter("P_SREFERENCEID", OracleDbType.NVarchar2, item.referenceId, ParameterDirection.Input);
                OracleParameter P_SMATCHSTRENGTH = new OracleParameter("P_SMATCHSTRENGTH", OracleDbType.NVarchar2, item.matchStrength, ParameterDirection.Input);
                OracleParameter P_SMATCHEDTERM = new OracleParameter("P_SMATCHEDTERM", OracleDbType.NVarchar2, item.matchedTerm, ParameterDirection.Input);
                OracleParameter P_SSUBMITTEDTERM = new OracleParameter("P_SSUBMITTEDTERM", OracleDbType.NVarchar2, item.submittedTerm, ParameterDirection.Input);
                OracleParameter P_SMATCHEDNAMETYPE = new OracleParameter("P_SMATCHEDNAMETYPE", OracleDbType.NVarchar2, item.matchedNameType, ParameterDirection.Input);
                OracleParameter P_SCREATIONDATE = new OracleParameter("P_SCREATIONDATE", OracleDbType.NVarchar2, item.creationDate, ParameterDirection.Input);
                OracleParameter P_SMODIFICATIONDATE = new OracleParameter("P_SMODIFICATIONDATE", OracleDbType.NVarchar2, item.modificationDate, ParameterDirection.Input);
                OracleParameter P_SRESOLUTION = new OracleParameter("P_SRESOLUTION", OracleDbType.NVarchar2, item.resolution, ParameterDirection.Input);
                OracleParameter P_SPRIMARYNAME = new OracleParameter("P_SPRIMARYNAME", OracleDbType.NVarchar2, item.primaryName, ParameterDirection.Input);
                OracleParameter P_SCATEGORY = new OracleParameter("P_SCATEGORY", OracleDbType.NVarchar2, item.category, ParameterDirection.Input);
                OracleParameter P_SPROVIDERTYPE = new OracleParameter("P_SPROVIDERTYPE", OracleDbType.NVarchar2, item.providerType, ParameterDirection.Input);
                OracleParameter P_SGENDER = new OracleParameter("P_SGENDER", OracleDbType.NVarchar2, item.gender, ParameterDirection.Input);

                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_SSYSTEMCASEID, P_SRESULTID, P_SREFERENCEID, P_SMATCHSTRENGTH , P_SMATCHEDTERM, P_SSUBMITTEDTERM,
                P_SMATCHEDNAMETYPE, P_SCREATIONDATE ,P_SMODIFICATIONDATE ,P_SRESOLUTION , P_SPRIMARYNAME , P_SCATEGORY , P_SPROVIDERTYPE, P_SGENDER,
                P_NID_COINCIDENCIA, P_NCODE, P_SMESSAGE};
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA(:P_SSYSTEMCASEID, :P_SRESULTID, :P_SREFERENCEID, :P_SMATCHSTRENGTH, :P_SMATCHEDTERM, :P_SSUBMITTEDTERM, 
                        :P_SMATCHEDNAMETYPE, :P_SCREATIONDATE, :P_SMODIFICATIONDATE, :P_SRESOLUTION, :P_SPRIMARYNAME, :P_SCATEGORY, :P_SPROVIDERTYPE, :P_SGENDER,
                        :P_NID_COINCIDENCIA, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.nId = Convert.ToInt32(P_NID_COINCIDENCIA.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return respo;
        }

        internal void SaveSources(string sSources, int nId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, nId, ParameterDirection.Input);
                OracleParameter P_SSOURCES = new OracleParameter("P_SSOURCES", OracleDbType.NVarchar2, sSources, ParameterDirection.Input);

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NID_COINCIDENCIA, P_SSOURCES, P_NCODE, P_SMESSAGE };
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA_SOURCE(:P_NID_COINCIDENCIA, :P_SSOURCES, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void SaveCategories(string sCategories, int nId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, nId, ParameterDirection.Input);
                OracleParameter P_SCATEGORIES = new OracleParameter("P_SCATEGORIES", OracleDbType.NVarchar2, sCategories, ParameterDirection.Input);

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NID_COINCIDENCIA, P_SCATEGORIES, P_NCODE, P_SMESSAGE };
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA_CATEGORIES(:P_NID_COINCIDENCIA, :P_SCATEGORIES, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void SaveEvents(Events events, int nId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, nId, ParameterDirection.Input);
                OracleParameter P_SADDRESS = new OracleParameter("P_SADDRESS", OracleDbType.NVarchar2, events.address, ParameterDirection.Input);
                OracleParameter P_NDAY = new OracleParameter("P_NDAY", OracleDbType.Int32, events.day, ParameterDirection.Input);
                OracleParameter P_SFULLDATE = new OracleParameter("P_SFULLDATE", OracleDbType.NVarchar2, events.fullDate, ParameterDirection.Input);
                OracleParameter P_NMONTH = new OracleParameter("P_NMONTH", OracleDbType.Int32, events.month, ParameterDirection.Input);
                OracleParameter P_STYPE = new OracleParameter("P_STYPE", OracleDbType.NVarchar2, events.type, ParameterDirection.Input);
                OracleParameter P_NYEAR = new OracleParameter("P_NYEAR", OracleDbType.Int32, events.year, ParameterDirection.Input);


                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NID_COINCIDENCIA, P_SADDRESS, P_NDAY, P_SFULLDATE, P_NMONTH, P_STYPE, P_NYEAR, P_NCODE, P_SMESSAGE };
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA_EVENTS(:P_NID_COINCIDENCIA, :P_SADDRESS, :P_NDAY, :P_SFULLDATE, :P_NMONTH, :P_STYPE, :P_NYEAR, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.nId = Convert.ToInt32(P_NID_COINCIDENCIA.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();

                this.context.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void SaveCountryLinks(CountryLinks countryLinks, int nId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, nId, ParameterDirection.Input);
                OracleParameter P_SCOUNTRYCODE = new OracleParameter("P_SCOUNTRYCODE", OracleDbType.NVarchar2, countryLinks.country.code, ParameterDirection.Input);
                OracleParameter P_SCOUNTRYNAME = new OracleParameter("P_SCOUNTRYNAME", OracleDbType.NVarchar2, countryLinks.country.name, ParameterDirection.Input);
                OracleParameter P_SCOUNTRYTEXT = new OracleParameter("P_SCOUNTRYTEXT", OracleDbType.NVarchar2, countryLinks.countryText, ParameterDirection.Input);
                OracleParameter P_STYPE = new OracleParameter("P_STYPE", OracleDbType.NVarchar2, countryLinks.type, ParameterDirection.Input);

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NID_COINCIDENCIA, P_SCOUNTRYCODE, P_SCOUNTRYNAME, P_SCOUNTRYTEXT, P_STYPE, P_NCODE, P_SMESSAGE };
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA_COUNTRYLINKS(:P_NID_COINCIDENCIA, :P_SCOUNTRYCODE, :P_SCOUNTRYNAME, :P_SCOUNTRYTEXT, :P_STYPE, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();

                this.context.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal List<string> ListIndividuos()
        {
            ResponseDTO response = new ResponseDTO();
            List<string> Lista = new List<string>();
            try
            {
                OracleParameter P_LISTA = new OracleParameter("P_LISTA", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.GET_LAFT_SEARCH_INDIVIDUES(:P_LISTA);
                    END;
                    ";
                OracleParameter[] parameters = new OracleParameter[] { P_LISTA };
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);
                OracleDataReader odr = ((OracleRefCursor)P_LISTA.Value).GetDataReader();
                while (odr.Read())
                {
                    string item = odr["SNOM_COMPLETO"].ToString();
                    Lista.Add(item);
                }
                odr.Close();
                this.context.Database.CloseConnection();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Lista;
        }

        internal ResponseDTO SaveResultCoincidencias(ResponseWc1 responseWc1, ResquestAlert item , int id)
        {
            ResponseDTO response = new ResponseDTO();
            for (int i = 0; i < responseWc1.categories.Count; i++)
            {
                try
                {
                    int category = responseWc1.categories[i] == "PEP" ? 2 : 1;
                    int valorPorcentaje = responseWc1.matchStrength == "WEAK" ? 25 : responseWc1.matchStrength == "MEDIUM" ? 50 : responseWc1.matchStrength == "STRONG" ? 75 : responseWc1.matchStrength == "EXACT" ? 100 : 0;
                    OracleParameter P_NIDALERTA = new OracleParameter("P_NIDALERTA", OracleDbType.Int32, int.Parse(item.alertId), System.Data.ParameterDirection.Input);
                    OracleParameter P_NPERIODO_PROCESO = new OracleParameter("P_NPERIODO_PROCESO", OracleDbType.Int32, int.Parse(item.periodId), System.Data.ParameterDirection.Input);
                    OracleParameter P_NIDRESULTADO = new OracleParameter("P_NIDRESULTADO", OracleDbType.Int32, id, System.Data.ParameterDirection.Input);
                    //OracleParameter P_SMACHTSTRENGTH = new OracleParameter("P_SMACHTSTRENGTH", OracleDbType.NVarchar2, responseWc1.matchStrength, System.Data.ParameterDirection.Input);
                    OracleParameter P_NMACHTSTRENGTHVALUE = new OracleParameter("P_NMACHTSTRENGTHVALUE", OracleDbType.Int32, valorPorcentaje, System.Data.ParameterDirection.Input);
                    OracleParameter P_NIDTIPOLISTA = new OracleParameter("P_NIDTIPOLISTA", OracleDbType.Int32, category, System.Data.ParameterDirection.Input);
                    OracleParameter P_SORIGEN = new OracleParameter("P_SORIGEN", OracleDbType.NVarchar2, responseWc1.categories[i], System.Data.ParameterDirection.Input);
                    OracleParameter P_SMATCHEDTERM = new OracleParameter("P_SMATCHEDTERM", OracleDbType.NVarchar2, responseWc1.matchedTerm, System.Data.ParameterDirection.Input);
                    OracleParameter P_NTIPOCARGA = new OracleParameter("P_NTIPOCARGA", OracleDbType.Int32, item.tipoCargaId, System.Data.ParameterDirection.Input);
                    OracleParameter P_SNOMBREBUSQUEDA = new OracleParameter("P_SNOMBREBUSQUEDA", OracleDbType.NVarchar2, responseWc1.submittedTerm, System.Data.ParameterDirection.Input);
                    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);
                    OracleParameter[] parameters = new OracleParameter[] { P_NIDALERTA, P_NPERIODO_PROCESO, P_NIDRESULTADO, P_NMACHTSTRENGTHVALUE, P_NIDTIPOLISTA,
                        P_SORIGEN,P_SMATCHEDTERM,P_NTIPOCARGA,P_SNOMBREBUSQUEDA, P_NCODE, P_SMESSAGE };
                    var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_RESULT_COINCIDENCIA(:P_NIDALERTA, :P_NPERIODO_PROCESO, :P_NIDRESULTADO,:P_NMACHTSTRENGTHVALUE, :P_NIDTIPOLISTA,
                        :P_SORIGEN,:P_SMATCHEDTERM,:P_NTIPOCARGA,:P_SNOMBREBUSQUEDA, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                    this.context.Database.OpenConnection();
                    this.context.Database.ExecuteSqlCommand(query, parameters);

                    response.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                    response.sMessage = P_SMESSAGE.Value.ToString();
                    this.context.Database.CloseConnection();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }

        internal void SaveIdentityDocuments(IdentityDocuments identityDocuments, int nId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, nId, System.Data.ParameterDirection.Input);
                OracleParameter P_SENTITY = new OracleParameter("P_SENTITY", OracleDbType.NVarchar2, identityDocuments.entity, ParameterDirection.Input);
                OracleParameter P_SEXPIRYDATE = new OracleParameter("P_SEXPIRYDATE", OracleDbType.NVarchar2, identityDocuments.expiryDate, ParameterDirection.Input);
                OracleParameter P_SISSUEDATE = new OracleParameter("P_SISSUEDATE", OracleDbType.NVarchar2, identityDocuments.issueDate, ParameterDirection.Input);
                OracleParameter P_SISSUER = new OracleParameter("P_SISSUER", OracleDbType.NVarchar2, identityDocuments.issuer, ParameterDirection.Input);
                OracleParameter P_SLOCATIONTYPECOUNTRYNAME = new OracleParameter("P_SLOCATIONTYPECOUNTRYNAME", OracleDbType.NVarchar2, identityDocuments.locationType.country.name, ParameterDirection.Input);
                OracleParameter P_SLOCATIONTYPECOUNTRYCODE = new OracleParameter("P_SLOCATIONTYPECOUNTRYCODE", OracleDbType.NVarchar2, identityDocuments.locationType.country.code, ParameterDirection.Input);
                OracleParameter P_SLOCATIONTYPENAME = new OracleParameter("P_SLOCATIONTYPENAME", OracleDbType.NVarchar2, identityDocuments.locationType.name, ParameterDirection.Input);
                OracleParameter P_SLOCATIONTYPETYPE = new OracleParameter("P_SLOCATIONTYPETYPE", OracleDbType.NVarchar2, identityDocuments.locationType.type, ParameterDirection.Input);
                OracleParameter P_SNUMBER = new OracleParameter("P_SNUMBER", OracleDbType.NVarchar2, identityDocuments.number, ParameterDirection.Input);
                OracleParameter P_STYPE = new OracleParameter("P_STYPE", OracleDbType.NVarchar2, identityDocuments.type, ParameterDirection.Input);

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NID_COINCIDENCIA, P_SENTITY, P_SEXPIRYDATE, P_SISSUEDATE, P_SISSUER, P_SLOCATIONTYPECOUNTRYNAME,
                    P_SLOCATIONTYPECOUNTRYCODE, P_SLOCATIONTYPENAME, P_SLOCATIONTYPETYPE, P_SNUMBER, P_STYPE, P_NCODE, P_SMESSAGE};
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA_IDENTITYDOCUMENTS(:P_NID_COINCIDENCIA, :P_SENTITY, :P_SEXPIRYDATE, :P_SISSUEDATE, :P_SISSUER, :P_SLOCATIONTYPECOUNTRYNAME,
                                :P_SLOCATIONTYPECOUNTRYCODE, :P_SLOCATIONTYPENAME, :P_SLOCATIONTYPETYPE, :P_SNUMBER, :P_STYPE, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();

                this.context.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
