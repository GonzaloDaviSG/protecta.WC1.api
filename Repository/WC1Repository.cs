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

        internal ResponseDTO SaveDetail(EntityDetail entityDetail, int nId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, nId, ParameterDirection.Input);
                OracleParameter P_SDETAILTYPE = new OracleParameter("P_SDETAILTYPE", OracleDbType.Varchar2, entityDetail.detailType, ParameterDirection.Input);
                OracleParameter P_STEXT = new OracleParameter("P_STEXT", OracleDbType.Varchar2, entityDetail.text, ParameterDirection.Input);
                OracleParameter P_STITLE = new OracleParameter("P_STITLE", OracleDbType.Varchar2, entityDetail.title, ParameterDirection.Input);


                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NID_COINCIDENCIA, P_SDETAILTYPE, P_STEXT, P_STITLE, P_NCODE, P_SMESSAGE };
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA_DETAIL(:P_NID_COINCIDENCIA, :P_SDETAILTYPE, :P_STEXT, :P_STITLE, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();

                this.context.Database.CloseConnection();
                return respo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal ResponseDTO SaveWebLinks(File file, int nId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, nId, ParameterDirection.Input);
                OracleParameter P_SCAPTION = new OracleParameter("P_SCAPTION", OracleDbType.Varchar2, file.caption, ParameterDirection.Input);
                OracleParameter P_STAGS = new OracleParameter("P_STAGS", OracleDbType.Varchar2, string.Join(',', file.tags), ParameterDirection.Input);
                OracleParameter P_SURI = new OracleParameter("P_SURI", OracleDbType.Varchar2, file.uri, ParameterDirection.Input);


                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NID_COINCIDENCIA, P_SCAPTION, P_STAGS, P_SURI, P_NCODE, P_SMESSAGE };
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA_WEBLINKS(:P_NID_COINCIDENCIA, :P_SCAPTION, :P_STAGS, :P_SURI, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();

                this.context.Database.CloseConnection();
                return respo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal ResponseDTO SaveDetailSources(ActionEntitySource sourse, int nId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, nId, System.Data.ParameterDirection.Input);
                OracleParameter P_SABBREVIATION = new OracleParameter("P_SABBREVIATION", OracleDbType.Varchar2, sourse.abbreviation, ParameterDirection.Input);
                OracleParameter P_SCREATIONDATE = new OracleParameter("P_SCREATIONDATE", OracleDbType.Varchar2, sourse.creationDate, ParameterDirection.Input);
                OracleParameter P_SIMPORTIDENTIFIER = new OracleParameter("P_SIMPORTIDENTIFIER", OracleDbType.Varchar2, sourse.importIdentifier, ParameterDirection.Input);
                OracleParameter P_SNAME = new OracleParameter("P_SNAME", OracleDbType.Varchar2, sourse.name, ParameterDirection.Input);
                OracleParameter P_SIDENTIFIER = new OracleParameter("P_SIDENTIFIER", OracleDbType.Varchar2, sourse.identifier, ParameterDirection.Input);
                OracleParameter P_SPROVIDERSOURCESTATUS = new OracleParameter("P_SPROVIDERSOURCESTATUS", OracleDbType.Varchar2, sourse.providerSourceStatus, ParameterDirection.Input);
                OracleParameter P_SREGIONOFAUTHORITY = new OracleParameter("P_SREGIONOFAUTHORITY", OracleDbType.Varchar2, sourse.regionOfAuthority, ParameterDirection.Input);
                OracleParameter P_SSUBSCRIPTIONCATEGORY = new OracleParameter("P_SSUBSCRIPTIONCATEGORY", OracleDbType.Varchar2, sourse.subscriptionCategory, ParameterDirection.Input);
                OracleParameter P_SPROVIDERCODE = new OracleParameter("P_SPROVIDERCODE", OracleDbType.Varchar2, sourse.provider == null ? "" : sourse.provider.code, ParameterDirection.Input);
                OracleParameter P_SPROVIDERIDENTIFIER = new OracleParameter("P_SPROVIDERIDENTIFIER", OracleDbType.Varchar2, sourse.provider == null ? "" : sourse.provider.identifier, ParameterDirection.Input);
                OracleParameter P_SPROVIDERMASTER = new OracleParameter("P_SPROVIDERMASTER", OracleDbType.Varchar2, sourse.provider == null ? "0" : sourse.provider.master == "true" ? "1" : "0", ParameterDirection.Input);
                OracleParameter P_SPROVIDERNAME = new OracleParameter("P_SPROVIDERNAME", OracleDbType.Varchar2, sourse.provider == null ? "" : sourse.provider.name, ParameterDirection.Input);
                bool isObjectCategory = sourse.type == null ? false : sourse.type.category == null ? false : true;
                OracleParameter P_STYPECATEGORYDESCRIPTION = new OracleParameter("P_STYPECATEGORYDESCRIPTION", OracleDbType.Varchar2, (isObjectCategory ? sourse.type.category.description : ""), ParameterDirection.Input);
                OracleParameter P_STYPECATEGORYIDENTIFIER = new OracleParameter("P_STYPECATEGORYIDENTIFIER", OracleDbType.Varchar2, (isObjectCategory ? sourse.type.category.identifier : ""), ParameterDirection.Input);
                OracleParameter P_STYPECATEGORYNAME = new OracleParameter("P_STYPECATEGORYNAME", OracleDbType.Varchar2, (isObjectCategory ? sourse.type.category.name : ""), ParameterDirection.Input);
                bool isObjectType = sourse.type == null ? false : true;
                OracleParameter P_STYPEIDENTIFIER = new OracleParameter("P_STYPEIDENTIFIER", OracleDbType.Varchar2, (isObjectType ? sourse.type.identifier : ""), ParameterDirection.Input);
                OracleParameter P_STYPENAME = new OracleParameter("P_STYPENAME", OracleDbType.Varchar2, (isObjectType ? sourse.type.name : ""), ParameterDirection.Input);

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NID_COINCIDENCIA, P_SABBREVIATION, P_SCREATIONDATE, P_SIMPORTIDENTIFIER, P_SNAME, P_SIDENTIFIER, P_SPROVIDERSOURCESTATUS,
                    P_SREGIONOFAUTHORITY, P_SSUBSCRIPTIONCATEGORY, P_SPROVIDERCODE, P_SPROVIDERIDENTIFIER, P_SPROVIDERMASTER, P_SPROVIDERNAME, P_STYPECATEGORYDESCRIPTION, P_STYPECATEGORYIDENTIFIER,
                    P_STYPECATEGORYNAME, P_STYPEIDENTIFIER, P_STYPENAME, P_NCODE, P_SMESSAGE};
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA_DETAIL_SOURCE(:P_NID_COINCIDENCIA, :P_SABBREVIATION, :P_SCREATIONDATE, :P_SIMPORTIDENTIFIER, :P_SNAME, :P_SIDENTIFIER, :P_SPROVIDERSOURCESTATUS,
                    :P_SREGIONOFAUTHORITY, :P_SSUBSCRIPTIONCATEGORY, :P_SPROVIDERCODE, :P_SPROVIDERIDENTIFIER, :P_SPROVIDERMASTER, :P_SPROVIDERNAME, :P_STYPECATEGORYDESCRIPTION, :P_STYPECATEGORYIDENTIFIER,
                    :P_STYPECATEGORYNAME, :P_STYPEIDENTIFIER, :P_STYPENAME, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();

                this.context.Database.CloseConnection();
                return respo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        internal ResponseDTO deshabilitarResultado(string sCaseId)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                OracleParameter P_SCASEID = new OracleParameter("P_SCASEID", OracleDbType.NVarchar2, sCaseId, System.Data.ParameterDirection.Input);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                OracleParameter[] parameters = new OracleParameter[] { P_SCASEID, P_NCODE, P_SMESSAGE };
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_DESHABILITARCOINCIDENCIAS(:P_SCASEID, :P_NCODE, :P_SMESSAGE);
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

                throw;
            }

            return respo;
        }

        internal ResponseDTO eliminarCoincidenciastemporales(ResquestAlert item)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NPERIODO_PROCESO = new OracleParameter("P_NPERIODO_PROCESO", OracleDbType.Int32, item.periodId, ParameterDirection.Input);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NPERIODO_PROCESO, P_NCODE,P_SMESSAGE};
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_DEL_COINCIDENCIA_TEMP(:P_NPERIODO_PROCESO, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();

                this.context.Database.CloseConnection();
                return respo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal ResponseDTO SaveProfile(ResponseProfileDTO item, string resulId, int id)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NID_COINCIDENCIA = new OracleParameter("P_NID_COINCIDENCIA", OracleDbType.Int32, id, ParameterDirection.Input);
                OracleParameter P_SRESULTID = new OracleParameter("P_SRESULTID", OracleDbType.Varchar2, resulId, ParameterDirection.Input);
                OracleParameter P_SACTIVE = new OracleParameter("P_SACTIVE", OracleDbType.Varchar2, item.active == "true" ? "1" : "0", ParameterDirection.Input);
                OracleParameter P_SCATEGORY = new OracleParameter("P_SCATEGORY", OracleDbType.Varchar2, item.category, ParameterDirection.Input);
                OracleParameter P_SCOMMENTS = new OracleParameter("P_SCOMMENTS", OracleDbType.Varchar2, item.comments, ParameterDirection.Input);
                OracleParameter P_SCREATIONDATE = new OracleParameter("P_SCREATIONDATE", OracleDbType.Varchar2, item.creationDate, ParameterDirection.Input);
                OracleParameter P_SDELETIONDATE = new OracleParameter("P_SDELETIONDATE", OracleDbType.Varchar2, item.deletionDate, ParameterDirection.Input);
                OracleParameter P_SDESCRIPTION = new OracleParameter("P_SDESCRIPTION", OracleDbType.Varchar2, item.description, ParameterDirection.Input);
                OracleParameter P_SENTITYID = new OracleParameter("P_SENTITYID", OracleDbType.Varchar2, item.entityId, ParameterDirection.Input);
                OracleParameter P_SEXTERNALIMPORTID = new OracleParameter("P_SEXTERNALIMPORTID", OracleDbType.Varchar2, item.externalImportId, ParameterDirection.Input);

                OracleParameter P_SLASTADJUNTCHANGEDATE = new OracleParameter("P_SLASTADJUNTCHANGEDATE", OracleDbType.Varchar2, item.lastAdjunctChangeDate, ParameterDirection.Input);
                OracleParameter P_SMODIFICATIONDATE = new OracleParameter("P_SMODIFICATIONDATE", OracleDbType.Varchar2, item.modificationDate, ParameterDirection.Input);
                OracleParameter P_SSOURCEDESCRIPTION = new OracleParameter("P_SSOURCEDESCRIPTION", OracleDbType.Varchar2, item.sourceDescription, ParameterDirection.Input);
                OracleParameter P_SSUBCATEGORY = new OracleParameter("P_SSUBCATEGORY", OracleDbType.Varchar2, item.subCategory, ParameterDirection.Input);
                OracleParameter P_SUPDATECATEGORY = new OracleParameter("P_SUPDATECATEGORY", OracleDbType.Varchar2, item.updateCategory, ParameterDirection.Input);
                OracleParameter P_SGENDER = new OracleParameter("P_SGENDER", OracleDbType.Varchar2, item.gender, ParameterDirection.Input);
                OracleParameter P_SAGEASOFDATE = new OracleParameter("P_SAGEASOFDATE", OracleDbType.Varchar2, item.ageAsOfDate, ParameterDirection.Input);
                OracleParameter P_SISDECEASED = new OracleParameter("P_SISDECEASED", OracleDbType.Varchar2, item.isDeceased == "true" ? "1" : "0", ParameterDirection.Input);
                OracleParameter P_SAGE = new OracleParameter("P_SAGE", OracleDbType.Varchar2, item.age, ParameterDirection.Input);
                OracleParameter P_SENTITYTYPE = new OracleParameter("P_SENTITYTYPE", OracleDbType.Varchar2, item.entityType, ParameterDirection.Input);

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NID_COINCIDENCIA, P_SRESULTID, P_SACTIVE, P_SCATEGORY, P_SCOMMENTS, P_SCREATIONDATE, P_SDELETIONDATE,
                P_SDESCRIPTION,P_SENTITYID,P_SEXTERNALIMPORTID,P_SLASTADJUNTCHANGEDATE,P_SMODIFICATIONDATE,P_SSOURCEDESCRIPTION,P_SSUBCATEGORY,P_SUPDATECATEGORY,P_SGENDER,
                P_SAGEASOFDATE,P_SISDECEASED,P_SAGE,P_SENTITYTYPE,P_NCODE,P_SMESSAGE};
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_COINCIDENCIA_PROFILE(:P_NID_COINCIDENCIA, :P_SRESULTID, :P_SACTIVE, :P_SCATEGORY, :P_SCOMMENTS, :P_SCREATIONDATE, :P_SDELETIONDATE,
                            :P_SDESCRIPTION, :P_SENTITYID, :P_SEXTERNALIMPORTID, :P_SLASTADJUNTCHANGEDATE, :P_SMODIFICATIONDATE, :P_SSOURCEDESCRIPTION, :P_SSUBCATEGORY, :P_SUPDATECATEGORY, :P_SGENDER,
                            :P_SAGEASOFDATE, :P_SISDECEASED, :P_SAGE, :P_SENTITYTYPE, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();

                this.context.Database.CloseConnection();
                return respo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal ResponseDTO spcarga_coincidencias(ResquestAlert item)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                OracleParameter P_NPERIODO_PROCESO = new OracleParameter("P_NPERIODO_PROCESO", OracleDbType.Int32, item.periodId, ParameterDirection.Input);
                OracleParameter P_NIDREGIMEN = new OracleParameter("P_NIDREGIMEN", OracleDbType.Int32, 1, ParameterDirection.Input);
                OracleParameter P_NIDALERTA = new OracleParameter("P_NIDALERTA", OracleDbType.Int32, item.alertId, ParameterDirection.Input);
                OracleParameter P_NIDGRUPOSENAL = new OracleParameter("P_NIDGRUPOSENAL", OracleDbType.Int32, 1, ParameterDirection.Input);
                OracleParameter P_CLIENT = new OracleParameter("P_CLIENT", OracleDbType.Varchar2, item.sClient, ParameterDirection.Input);
                OracleParameter P_NIDUSUARIO = new OracleParameter("P_NIDUSUARIO", OracleDbType.Int32, item.nIdUsuario, ParameterDirection.Input);
                OracleParameter P_NTIPOCARGA = new OracleParameter("P_NTIPOCARGA", OracleDbType.Int32, item.tipoCargaId, ParameterDirection.Input);               

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);

                OracleParameter[] parameters = new OracleParameter[] { P_NPERIODO_PROCESO, P_NIDREGIMEN, P_NIDALERTA, P_NIDGRUPOSENAL, P_CLIENT, P_NIDUSUARIO, P_NTIPOCARGA, P_NCODE,P_SMESSAGE};
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_UPD_COINCIDENCIA(:P_NPERIODO_PROCESO, :P_NIDREGIMEN, :P_NIDALERTA, :P_NIDGRUPOSENAL, :P_CLIENT, 
                        :P_NIDUSUARIO, :P_NTIPOCARGA, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);

                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
            return respo;
        }

        internal ResponseDTO SaveResultCoincidencias(ResponseWc1 responseWc1, ResquestAlert item, int id, string caseSystemId, string caseId, string biography)
        {

            ResponseDTO response = new ResponseDTO();
            for (int i = 0; i < responseWc1.categories.Count; i++)
            {
                List<Dictionary<string, dynamic>> documents = new List<Dictionary<string, dynamic>>();
                if (responseWc1.identityDocuments.Count > 0)
                {
                    Dictionary<string, dynamic> doc;
                    for (int j = 0; j < responseWc1.identityDocuments.Count; j++)
                    {
                        doc = new Dictionary<string, dynamic>();
                        doc["number"] = responseWc1.identityDocuments[j].number;
                        doc["type"] = responseWc1.identityDocuments[j].locationType.type.Contains("DNI") ? "DNI" : responseWc1.identityDocuments[j].locationType.type.Contains("RUC") ? "RUC" : "";
                        documents.Add(doc);
                    }
                }
                else
                {
                    Dictionary<string, dynamic> doc = new Dictionary<string, dynamic>();
                    doc["number"] = "0";
                    doc["type"] = "XX";
                    documents.Add(doc);
                }
                String[] palabras = responseWc1.matchedTerm.Split(' ');
                for (int j = 0; j < documents.Count; j++)
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

                        OracleParameter P_SNOM_COMPLETO = new OracleParameter("P_SNOM_COMPLETO", OracleDbType.NVarchar2, responseWc1.primaryName.Trim().ToUpper(), System.Data.ParameterDirection.Input);
                        OracleParameter P_SMATCHEDTERM = new OracleParameter("P_SMATCHEDTERM", OracleDbType.NVarchar2, responseWc1.matchedTerm.Trim().ToUpper(), System.Data.ParameterDirection.Input);
                        OracleParameter P_SNUM_DOCUMENT = new OracleParameter("P_SNUM_DOCUMENT", OracleDbType.NVarchar2, documents[j]["number"], System.Data.ParameterDirection.Input);
                        OracleParameter P_STIPO_DOCUMENT = new OracleParameter("P_STIPO_DOCUMENT", OracleDbType.NVarchar2, documents[j]["type"], System.Data.ParameterDirection.Input);
                        OracleParameter P_SCARGO_PEP_EXTERNO = new OracleParameter("P_SCARGO_PEP_EXTERNO", OracleDbType.NVarchar2, biography, System.Data.ParameterDirection.Input);

                        OracleParameter P_NTIPOCARGA = new OracleParameter("P_NTIPOCARGA", OracleDbType.Int32, item.tipoCargaId, System.Data.ParameterDirection.Input);
                        OracleParameter P_SNOMBREBUSQUEDA = new OracleParameter("P_SNOMBREBUSQUEDA", OracleDbType.NVarchar2, responseWc1.submittedTerm, System.Data.ParameterDirection.Input);

                        OracleParameter P_SCASESYSTEMID = new OracleParameter("P_SCASESYSTEMID", OracleDbType.NVarchar2, caseSystemId, System.Data.ParameterDirection.Input);
                        OracleParameter P_SCASEID = new OracleParameter("P_SCASEID", OracleDbType.NVarchar2, caseId, System.Data.ParameterDirection.Input);
                        OracleParameter P_SCLIENT = new OracleParameter("P_SCLIENT", OracleDbType.NVarchar2, item.sClient, System.Data.ParameterDirection.Input);
                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);
                        OracleParameter[] parameters = new OracleParameter[] { P_NIDALERTA, P_NPERIODO_PROCESO, P_NIDRESULTADO, P_NMACHTSTRENGTHVALUE, P_NIDTIPOLISTA,
                        P_SORIGEN,P_SNOM_COMPLETO,P_SMATCHEDTERM,P_SNUM_DOCUMENT,P_STIPO_DOCUMENT,P_SCARGO_PEP_EXTERNO,P_NTIPOCARGA,P_SNOMBREBUSQUEDA,P_SCASESYSTEMID,P_SCASEID,P_SCLIENT, P_NCODE, P_SMESSAGE };
                        var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_INS_WC1_RESULT_COINCIDENCIA(:P_NIDALERTA, :P_NPERIODO_PROCESO, :P_NIDRESULTADO,:P_NMACHTSTRENGTHVALUE, :P_NIDTIPOLISTA,
                        :P_SORIGEN,:P_SNOM_COMPLETO,:P_SMATCHEDTERM,:P_SNUM_DOCUMENT,:P_STIPO_DOCUMENT,:P_SCARGO_PEP_EXTERNO,:P_NTIPOCARGA, :P_SNOMBREBUSQUEDA, :P_SCASESYSTEMID, :P_SCASEID, :P_SCLIENT, :P_NCODE, :P_SMESSAGE);
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal List<ObjCaseDTO> getCase(string name)
        {
            List<ObjCaseDTO> List = new List<ObjCaseDTO>();
            try
            {
                //matchedNameType
                OracleParameter P_SNAME = new OracleParameter("P_SNAME", OracleDbType.Varchar2, name, ParameterDirection.Input);
                OracleParameter P_LISTCASEID = new OracleParameter("P_LISTCASEID", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                OracleParameter[] parameters = new OracleParameter[] { P_SNAME, P_LISTCASEID };
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_GET_EXIST_CASE(:P_SNAME, :P_LISTCASEID);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);
                OracleDataReader odr = ((OracleRefCursor)P_LISTCASEID.Value).GetDataReader();
                while (odr.Read())
                {
                    ObjCaseDTO item = new ObjCaseDTO();
                    item.SCaseId = odr["SCASEID"].ToString();
                    item.SCaseSystemId = odr["SCASESYSTEMID"].ToString();
                    List.Add(item);
                }
                odr.Close();
                this.context.Database.CloseConnection();
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal ResponseDTO ins_tratamiento_cliente(ResquestAlert item)
        {
            ResponseDTO respo = new ResponseDTO();
            try
            {
                //matchedNameType
                OracleParameter P_NPERIODO_PROCESO = new OracleParameter("P_NPERIODO_PROCESO", OracleDbType.Int32, item.periodId, ParameterDirection.Input);
                OracleParameter P_NIDALERTA = new OracleParameter("P_NIDALERTA", OracleDbType.Int16, 2, ParameterDirection.Input);
                OracleParameter P_NIDGRUPOSENAL = new OracleParameter("P_NIDGRUPOSENAL", OracleDbType.Int16, 1, ParameterDirection.Input);
                OracleParameter P_NTIPOCARGA = new OracleParameter("P_NTIPOCARGA", OracleDbType.Int16, item.tipoCargaId, ParameterDirection.Input);
                OracleParameter P_SCLIENT = new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, item.sClient, ParameterDirection.Input);
                OracleParameter P_NIDUSUARIO_MODIFICA = new OracleParameter("P_NIDUSUARIO_MODIFICA", OracleDbType.Int32, item.nIdUsuario, ParameterDirection.Input);
                OracleParameter P_NIDPROVEEDOR = new OracleParameter("P_NIDPROVEEDOR", OracleDbType.Int16, 4, ParameterDirection.Input);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter[] parameters = new OracleParameter[] { P_NPERIODO_PROCESO, P_NIDALERTA , P_NIDGRUPOSENAL, P_NTIPOCARGA, P_SCLIENT
                , P_NIDUSUARIO_MODIFICA, P_NIDPROVEEDOR, P_NCODE, P_SMESSAGE};
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1.SP_GET_EXIST_CASE(:P_NPERIODO_PROCESO, :P_NIDALERTA , :P_NIDGRUPOSENAL, :P_NTIPOCARGA, :P_SCLIENT
                        , :P_NIDUSUARIO_MODIFICA, :P_NIDPROVEEDOR, :P_NCODE, :P_SMESSAGE);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);
                respo.nCode = Convert.ToInt32(P_NCODE.Value.ToString());
                respo.sMessage = P_SMESSAGE.Value.ToString();
                this.context.Database.CloseConnection();
                return respo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal List<ObjCaseDTO> getCaseJD(string name)
        {
            List<ObjCaseDTO> List = new List<ObjCaseDTO>();
            try
            {
                //matchedNameType
                OracleParameter P_SNAME = new OracleParameter("P_SNAME", OracleDbType.Varchar2, name, ParameterDirection.Input);
                OracleParameter P_LISTCASEID = new OracleParameter("P_LISTCASEID", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                OracleParameter[] parameters = new OracleParameter[] { P_SNAME, P_LISTCASEID };
                var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1_JD.SP_GET_EXIST_CASE(:P_SNAME, :P_LISTCASEID);
                    END;
                    ";
                this.context.Database.OpenConnection();
                this.context.Database.ExecuteSqlCommand(query, parameters);
                OracleDataReader odr = ((OracleRefCursor)P_LISTCASEID.Value).GetDataReader();
                while (odr.Read())
                {
                    ObjCaseDTO item = new ObjCaseDTO();
                    item.SCaseId = odr["SCASEID"].ToString();
                    item.SCaseSystemId = odr["SCASESYSTEMID"].ToString();
                    List.Add(item);
                }
                odr.Close();
                this.context.Database.CloseConnection();
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseDTO SaveResultJD(ResponseWc1 item, string SystemCaseId)
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
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1_JD.SP_INS_WC1_COINCIDENCIA(:P_SSYSTEMCASEID, :P_SRESULTID, :P_SREFERENCEID, :P_SMATCHSTRENGTH, :P_SMATCHEDTERM, :P_SSUBMITTEDTERM, 
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

        internal ResponseDTO SaveResultCoincidenciasJD(ResponseWc1 responseWc1, ResquestAlert item, int id, string caseSystemId, string caseId, string biography)
        {

            ResponseDTO response = new ResponseDTO();
            for (int i = 0; i < responseWc1.categories.Count; i++)
            {
                List<Dictionary<string, dynamic>> documents = new List<Dictionary<string, dynamic>>();
                if (responseWc1.identityDocuments.Count > 0)
                {
                    Dictionary<string, dynamic> doc;
                    for (int j = 0; j < responseWc1.identityDocuments.Count; j++)
                    {
                        doc = new Dictionary<string, dynamic>();
                        doc["number"] = responseWc1.identityDocuments[j].number.Length > 11 ? "0" : responseWc1.identityDocuments[j].number;
                        doc["type"] = responseWc1.identityDocuments[j].locationType.type.Contains("DNI") ? "DNI" : responseWc1.identityDocuments[j].locationType.type.Contains("RUC") ? "RUC" : "";
                        documents.Add(doc);
                    }
                }
                else
                {
                    Dictionary<string, dynamic> doc = new Dictionary<string, dynamic>();
                    doc["number"] = "0";
                    doc["type"] = "XX";
                    documents.Add(doc);
                }
                String[] palabras = responseWc1.matchedTerm.Split(' ');
                for (int j = 0; j < documents.Count; j++)
                {
                    try
                    {
                        int category = responseWc1.categories[i] == "PEP" ? 2 : 1;
                        int valorPorcentaje = responseWc1.matchStrength == "WEAK" ? 25 : responseWc1.matchStrength == "MEDIUM" ? 50 : responseWc1.matchStrength == "STRONG" ? 75 : responseWc1.matchStrength == "EXACT" ? 100 : 0;
                        //OracleParameter P_NIDALERTA = new OracleParameter("P_NIDALERTA", OracleDbType.Int32, int.Parse(item.alertId), System.Data.ParameterDirection.Input);
                        OracleParameter P_NPERIODO_PROCESO = new OracleParameter("P_NPERIODO_PROCESO", OracleDbType.Int32, int.Parse(item.periodId), System.Data.ParameterDirection.Input);
                        OracleParameter P_NIDRESULTADO = new OracleParameter("P_NIDRESULTADO", OracleDbType.Int32, id, System.Data.ParameterDirection.Input);
                        //OracleParameter P_SMACHTSTRENGTH = new OracleParameter("P_SMACHTSTRENGTH", OracleDbType.NVarchar2, responseWc1.matchStrength, System.Data.ParameterDirection.Input);
                        OracleParameter P_NMACHTSTRENGTHVALUE = new OracleParameter("P_NMACHTSTRENGTHVALUE", OracleDbType.Int32, valorPorcentaje, System.Data.ParameterDirection.Input);
                        OracleParameter P_NIDTIPOLISTA = new OracleParameter("P_NIDTIPOLISTA", OracleDbType.Int32, category, System.Data.ParameterDirection.Input);
                        OracleParameter P_SORIGEN = new OracleParameter("P_SORIGEN", OracleDbType.NVarchar2, responseWc1.categories[i], System.Data.ParameterDirection.Input);

                        OracleParameter P_SNOM_COMPLETO = new OracleParameter("P_SNOM_COMPLETO", OracleDbType.NVarchar2, responseWc1.primaryName.Trim().ToUpper(), System.Data.ParameterDirection.Input);
                        OracleParameter P_SMATCHEDTERM = new OracleParameter("P_SMATCHEDTERM", OracleDbType.NVarchar2, responseWc1.matchedTerm.Trim().ToUpper(), System.Data.ParameterDirection.Input);
                        OracleParameter P_SNUM_DOCUMENT = new OracleParameter("P_SNUM_DOCUMENT", OracleDbType.NVarchar2, documents[j]["number"], System.Data.ParameterDirection.Input);
                        OracleParameter P_STIPO_DOCUMENT = new OracleParameter("P_STIPO_DOCUMENT", OracleDbType.NVarchar2, documents[j]["type"], System.Data.ParameterDirection.Input);
                        OracleParameter P_SCARGO_PEP_EXTERNO = new OracleParameter("P_SCARGO_PEP_EXTERNO", OracleDbType.NVarchar2, biography, System.Data.ParameterDirection.Input);

                        OracleParameter P_NTIPOCARGA = new OracleParameter("P_NTIPOCARGA", OracleDbType.Int32, 1, System.Data.ParameterDirection.Input);
                        OracleParameter P_SNOMBREBUSQUEDA = new OracleParameter("P_SNOMBREBUSQUEDA", OracleDbType.NVarchar2, responseWc1.submittedTerm, System.Data.ParameterDirection.Input);

                        OracleParameter P_SCASESYSTEMID = new OracleParameter("P_SCASESYSTEMID", OracleDbType.NVarchar2, caseSystemId, System.Data.ParameterDirection.Input);
                        OracleParameter P_SCASEID = new OracleParameter("P_SCASEID", OracleDbType.NVarchar2, caseId, System.Data.ParameterDirection.Input);
                        OracleParameter P_SCLIENT = new OracleParameter("P_SCLIENT", OracleDbType.NVarchar2, "", System.Data.ParameterDirection.Input);
                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);
                        OracleParameter[] parameters = new OracleParameter[] {  P_NPERIODO_PROCESO, P_NIDRESULTADO, P_NMACHTSTRENGTHVALUE, P_NIDTIPOLISTA,
                        P_SORIGEN,P_SNOM_COMPLETO,P_SMATCHEDTERM,P_SNUM_DOCUMENT,P_STIPO_DOCUMENT,P_SCARGO_PEP_EXTERNO,P_NTIPOCARGA,P_SNOMBREBUSQUEDA,P_SCASESYSTEMID,P_SCASEID,P_SCLIENT, P_NCODE, P_SMESSAGE };
                        var query = @"
                    BEGIN
                        LAFT.PKG_LAFT_IMPORTAR_DATA_WC1_JD.SP_INS_WC1_RESULT_COINCIDENCIA( :P_NPERIODO_PROCESO, :P_NIDRESULTADO,:P_NMACHTSTRENGTHVALUE, :P_NIDTIPOLISTA,
                        :P_SORIGEN,:P_SNOM_COMPLETO,:P_SMATCHEDTERM,:P_SNUM_DOCUMENT,:P_STIPO_DOCUMENT,:P_SCARGO_PEP_EXTERNO,:P_NTIPOCARGA, :P_SNOMBREBUSQUEDA, :P_SCASESYSTEMID, :P_SCASEID, :P_SCLIENT, :P_NCODE, :P_SMESSAGE);
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
            }
            return response;
        }
    }
}
