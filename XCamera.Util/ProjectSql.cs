using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XCamera.Util
{

    public partial class ProjectSql
    {
        /// <summary>
        /// build the project path, create the directory if it does not exist
        /// </summary>
        /// <param name="szProjectNameToLoad"></param>
        /// <returns></returns>
        public static string BuildProjectPath(string szProjectNameToLoad)
        {
            string szProjectPath = Path.Combine(ProjectUtil.szBasePath, szProjectNameToLoad);
            if (!Directory.Exists(szProjectPath))
            {
                Directory.CreateDirectory(szProjectPath);
            }
            return szProjectPath;
        }
        /// <summary>
        /// build the fulle file name of the SqLite database
        /// </summary>
        /// <param name="szProjectNameToLoad"></param>
        /// <returns></returns>
        public static string BuildDbPath(string szProjectNameToLoad)
        {
            string szProjectPath = BuildProjectPath(szProjectNameToLoad);
            return Path.Combine(szProjectPath, szProjectNameToLoad + ".db");
        }
        public static Boolean DbExists(string szProjectNameToLoad)
        {
            return File.Exists(BuildDbPath(szProjectNameToLoad));
        }
        /// <summary>
        /// Delete the project <para/>
        /// All files and the directory are removed
        /// </summary>
        /// <param name="szProjectNameToDelete">error messages or an empty string</param>
        /// <returns></returns>
        public static string Delete(Boolean bForce, string szProjectNameToDelete)
        {
            string szRet = "";
            string szProjectPath = BuildProjectPath(szProjectNameToDelete);
            if (bForce)
            {
                string[] dateien = Directory.GetFiles(szProjectPath, "*.*");
                foreach (var datei in dateien)
                {
                    try
                    {
                        File.Delete(datei);
                    }
                    catch (Exception)
                    {
                        szRet += "Kann " + Path.GetFileName(datei) + " nicht löschen." + Environment.NewLine;
                    }
                }
                try
                {
                    Directory.Delete(szProjectPath, true);
                }
                catch (Exception)
                {
                    szRet += "Kann " + szProjectPath + " nicht löschen." + Environment.NewLine;
                }
            }
            if (bForce)
            {
                string datei = szProjectPath + ".deleted";
                try
                {
                    File.Delete(datei);
                }
                catch (Exception)
                {
                    szRet += "Kann " + Path.GetFileName(datei) + " nicht löschen." + Environment.NewLine;
                }
            }
            if (Directory.Exists(szProjectPath))
            {
                try
                {
                    File.WriteAllText(szProjectPath + ".deleted", "deleted on" + DateTime.Now.ToLongDateString());
                }
                catch (Exception)
                {
                    szRet += "Kann " + szProjectPath + ".deleted nicht anlegen." + Environment.NewLine;
                }
            }
            return szRet;
        }
        /// <summary>
        /// Get all imagesin the project directory.<para/>
        /// There may be found images on disk, but not in the database, or found in the database but on disk
        /// </summary>
        /// <param name="szProjectName"></param>
        /// <returns></returns>
        public static string[] GetImages(string szProjectName)
        {
            string szProjectPath = BuildProjectPath(szProjectName);
            return Directory.GetFiles(szProjectPath, "*.jpg");
        }


        public string szProjectName { get; set; } = "Sample";
        public string szProjectPath { get; set; }
        public string szProjectFile { get; set; }
        public string szTempProjectPath { get; set; }

        public SqlGebaeude sqlGebaeude { get; set; }
        public SqlEtage sqlEtage { get; set; }
        public SqlWohnung sqlWohnung { get; set; }
        public SqlZimmer sqlZimmer { get; set; }

        public SQLiteConnection database { get; set; }

        public ProjectSql(string szProjectNameToLoad)
        {
            this.szProjectName = szProjectNameToLoad;
            szProjectPath = Path.Combine(ProjectUtil.szBasePath, szProjectName);
            if (!Directory.Exists(szProjectPath))
            {
                Directory.CreateDirectory(szProjectPath);
            }
            szProjectFile = Path.Combine(szProjectPath, szProjectName + ".db");

            database = new SQLiteConnection(szProjectFile,false);
            
            database.CreateTable<Zusatz>();
            database.CreateTable<Gebaeude>();
            database.CreateTable<Etage>();
            database.CreateTable<Wohnung>();
            database.CreateTable<Zimmer>();
            database.CreateTable<Kommentar>();
            database.CreateTable<Bild_Zusatz>();
            database.CreateTable<Bild_Gebaeude>();
            database.CreateTable<Bild_Etage>();
            database.CreateTable<Bild_Wohnung>();
            database.CreateTable<Bild_Zimmer>();
            database.CreateTable<Bild_Kommentar>();
            database.CreateTable<Bild>();

            if( GetBilderChanged().Count > 0 )
            {
                Config.current.SetProjectStatus(this.szProjectName, STATUS.CHANGED);
            }
            sqlGebaeude = new SqlGebaeude(this);
            sqlEtage = new SqlEtage(this);
            sqlWohnung = new SqlWohnung(this);
            sqlZimmer = new SqlZimmer(this);
        }
        
        public string[] ListAllFiles()
        {
            return Directory.GetFiles(this.szProjectPath, "*.*");
            
        }
        public MetaData GetMetaData()
        {
            MetaData metaData = new MetaData();
            metaData.gebaeudeListe = sqlGebaeude.GetListe();
            metaData.etageListe = sqlEtage.GetListe();
            metaData.wohnungiste = sqlWohnung.GetListe();
            metaData.zimmerListe = sqlZimmer.GetListe();
            metaData.kommentarListe = GetKommentarListe();

            return metaData;

        }
        public List<string> GetImages()
        {
            List<string> imgList = new List<string>();

            return imgList;
        }
        public string GetImageFullName(string szImage, string szAdditionalDir="")
        {
            if (string.IsNullOrWhiteSpace(szAdditionalDir))
            {
                return Path.Combine(szProjectPath, szImage);
            }
            return Path.Combine(szProjectPath, szAdditionalDir, szImage);
        }
        public string GetImageFullThumbName(string szImage)
        {
            return Path.Combine(szProjectPath, "thumb_" +Path.GetFileName(szImage));
        }

        public void DeleteImage(string szImageName)
        {
            var biResult = GetBildId(szImageName,DateTime.Now);
            SetDeleted( biResult.BildId);
        }

        public Boolean IsDirty()
        {
            // are there any change images?
            return GetBilderChanged().Count > 0;
        }
        public string GetTempDir()
        {
            return szTempProjectPath;
        }
        public BildInfo GetBildInfo(string szImageName, DateTime CaptureDate)
        {
            BildInfo bi = new BildInfo();
            bi.BildName = szImageName;
            var biResult = GetBildId(szImageName, CaptureDate);
            bi.BildId = biResult.BildId;
            bi.bBildIdFound = biResult.bBildIdFound;
            bi.CaptureDate = biResult.dtCaptureDate;

            sqlGebaeude.GetForBild(bi.BildId, bi);
            sqlWohnung.GetForBild(bi.BildId, bi);
            sqlEtage.GetForBild(bi.BildId, bi);
            sqlZimmer.GetForBild(bi.BildId, bi);
            GetKommentarForBild(bi.BildId, bi);

            return bi;
        }

        public class GetBildIdResult
        {
            public int BildId { get; set; }
            public Boolean bBildIdFound { get; set; } = false;
            public DateTime dtCaptureDate { get; set; }

        }
        public GetBildIdResult GetBildId(string szImageName, DateTime dtCaptureDate)
        {
            GetBildIdResult ret = new GetBildIdResult();
            var bildListe = database.Query<Bild>("SELECT * FROM [Bild] WHERE [Name] = '" + szImageName + "'");
            
            if (bildListe.Count > 0)
            {
                ret.bBildIdFound = true;
                ret.BildId= bildListe[0].ID;
                ret.dtCaptureDate = bildListe[0].CaptureDate;
                if( ret.dtCaptureDate  == DateTime.MinValue)
                {
                    // should we update the BILD when the CaptureDate is missing?
                    ret.dtCaptureDate = dtCaptureDate;
                }
            }
            else
            {
                ret.bBildIdFound = false;
                ret.dtCaptureDate = dtCaptureDate;
                Bild bild = new Bild
                {
                    Name = szImageName,
                    CaptureDate = ret.dtCaptureDate
                };
                database.Insert(bild);
                ret.BildId = database.ExecuteScalar<int>("select last_insert_rowid();");
            }
            return ret;
        }
        public Boolean HasDeleted()
        {
            string szSql = "select Count(*) from BILD where status & 1";
            int iCnt = database.ExecuteScalar<int>(szSql);
            return false;
        }
        public void ClearDeleted()
        {
        }
        public Boolean IsDeleted(int bildID)
        {
            return HasStatus(bildID,STATUS.DELETED);
        }
        public Boolean IsNew(int bildID)
        {
            return HasStatus(bildID, STATUS.NEW);
        }
        public Boolean IsChanged(int bildID)
        {
            return HasStatus(bildID, STATUS.CHANGED);
        }
        public void SetDeleted(int bildID)
        {
            SetStatus(bildID, STATUS.DELETED | STATUS.CHANGED);
        }
        public void SetNew(int bildID)
        {
            SetStatus(bildID, STATUS.NEW | STATUS.CHANGED);
        }
        public void SetChanged(int bildID)
        {
            SetStatus(bildID, STATUS.CHANGED);
        }

        public Boolean HasStatus(int bildID, STATUS bsCheck)
        {
            Bild bild = GetBild(bildID);
            if (bild != null)
            {
                STATUS bs = (STATUS)bild.Status;

                return bs.HasFlag(bsCheck);
            }
            return false;
        }
        public void SetStatus(int bildID, STATUS bsSet)
        {
            Bild bild = GetBild(bildID);
            if (bild != null)
            {
                bild.Status = (int)bsSet;
                database.Update(bild);

            }
        }
        public void ClearStatus(int bildID, STATUS bsSet)
        {
            Bild bild = GetBild(bildID);
            if (bild != null)
            {
                bild.Status &= ~(int)bsSet;
                database.Update(bild);

            }
        }
        
        public void SetCaptureDate(int bildId, DateTime captureDate)
        {
            Bild bild = GetBild(bildId);
            if( bild != null )
            {
                bild.CaptureDate = captureDate;
                database.Update(bild);
            }
        }
        
        public int AddBild(string szImageName)
        {
            Bild bild = new Bild();
            bild.Name = szImageName;
            database.Insert(bild);
            return database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public Bild GetBild(int id)
        {
            return database.Table<Bild>().Where(x => x.ID == id).SingleOrDefault();
        }
        
        public List<Bild> GetBilderChanged()
        {
            string szSql = "SELECT * FROM Bild where Status & " + (int)STATUS.CHANGED;
            return database.Query<Bild>(szSql);

        }
        public void Patch()
        {
            string szSql = "SELECT * FROM Bild where CaptureDate is null";
            List<Bild> bldList = database.Query<Bild>(szSql);
            foreach(var bild in bldList)
            {
                DateTime dtUseDatTime = DateTime.MinValue;
                try
                {
                    // IMG_20190217_183147.jpg
                    string szName = Path.GetFileNameWithoutExtension(bild.Name);
                    string[] teile = szName.Split('_');
                    if (teile.Length > 1)
                    {
                        string yy = teile[teile.Length - 2].Substring(0, 4);
                        string MM = teile[teile.Length - 2].Substring(4, 2);
                        string dd = teile[teile.Length - 2].Substring(6, 2);

                        string HH = teile[teile.Length - 1].Substring(0, 2);
                        string mm = teile[teile.Length - 1].Substring(2, 2);
                        string ss = teile[teile.Length - 1].Substring(4, 2);
                        int iyy, iMM, idd, iHH, imm, iss;
                        if (int.TryParse(yy, out iyy) &&
                        int.TryParse(MM, out iMM) &&
                        int.TryParse(dd, out idd) &&
                        int.TryParse(HH, out iHH) &&
                        int.TryParse(mm, out imm) &&
                        int.TryParse(ss, out iss))
                        {
                            DateTime dtNew = new DateTime(iyy, iMM, idd, iHH, imm, iss);
                            dtUseDatTime = dtNew;
                        }
                    }
                }
                catch (Exception)
                {
                }

                if (dtUseDatTime == DateTime.MinValue)
                {
                    FileInfo fi = new FileInfo(GetImageFullName(bild.Name));
                    dtUseDatTime = fi.CreationTime;
                }

                if (dtUseDatTime != DateTime.MinValue)
                {
                    bild.CaptureDate = dtUseDatTime;
                    database.Update(bild);

                }
            }

        }
        public List<Bild> GetBilder(DateTime? dtStart, DateTime? dtEnd,int gebaeudeId = -1, int etageId = -1, int wohnungId = -1, int zimmerId = -1)
        {
            string szSql = "SELECT * FROM Bild ";
            string szWhere = "";
            if (dtEnd != null)
            {
                dtEnd = dtEnd.Value.AddDays(1);
            }
            if (gebaeudeId >= 0)
            {
                szSql += " LEFT JOIN BILD_GEBAEUDE on Bild.ID = BILD_GEBAEUDE.BildID ";
                szWhere = " where ";
            }
            if (etageId >= 0)
            {
                szSql += " LEFT JOIN BILD_ETAGE on Bild.ID = BILD_ETAGE.BildID ";
                szWhere = " where ";
            }
            if (wohnungId >= 0)
            {
                szSql += " LEFT JOIN BILD_WOHNUNG on Bild.ID = BILD_WOHNUNG.BildID ";
                szWhere = " where ";
            }
            if (zimmerId >= 0)
            {
                szSql += " LEFT JOIN BILD_ZIMMER on Bild.ID = BILD_ZIMMER.BildID ";
                szWhere = " where ";
            }
            if (dtStart > DateTime.MinValue)
            {
                szWhere = " where ";
            }
            if (dtEnd < DateTime.MaxValue)
            {
                szWhere = " where ";
            }
            string szAnd = "";
            // we exclude the deleted images at the end, so we need always the where clause
            szWhere = " where ";

            if ( !string.IsNullOrEmpty(szWhere))
            {
                szSql += szWhere;
                if (dtStart != null)
                {
                    szSql += szAnd + " BILD.CaptureDate >= '" + dtStart?.ToString("yyyy-MM-dd") + "'";
                    szAnd = " and ";
                }
                if (dtEnd != null)
                {
                    szSql += szAnd + " BILD.CaptureDate <= '" + dtEnd?.ToString("yyyy-MM-dd") + "'";
                    szAnd = " and ";
                }
                if (gebaeudeId >= 0)
                {
                    szSql += szAnd + " BILD_GEBAEUDE.GebaeudeID = " + gebaeudeId.ToString();
                    szAnd = " and ";
                }
                if (etageId >= 0)
                {
                    szSql += szAnd + " BILD_ETAGE.EtageID = " + etageId.ToString();
                    szAnd = " and ";
                }
                if (wohnungId >= 0)
                {
                    szSql += szAnd + " BILD_WOHNUNG.WohnungID = " + wohnungId.ToString();
                    szAnd = " and ";
                }
                if (zimmerId >= 0)
                {
                    szSql += szAnd + " BILD_ZIMMER.ZimmerID = " + zimmerId.ToString();
                    szAnd = " and ";
                }
                // exclude the deleted images
                szSql += szAnd + " not status & 1";
                szAnd = " and ";
            }

            szSql += " ORDER BY BILD.CaptureDate DESC";
            return database.Query<Bild>(szSql);
        }
    }
}
