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

    public class ProjectSql
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

        readonly SQLiteConnection database;

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

        }
        
        public string[] ListAllFiles()
        {
            return Directory.GetFiles(this.szProjectPath, "*.*");
            
        }
        public MetaData GetMetaData()
        {
            MetaData metaData = new MetaData();
            metaData.gebaeudeListe = GetGebaeudeListe();
            metaData.etageListe = GetEtagenListe();
            metaData.wohnungiste = GetWohnungListe();
            metaData.zimmerListe = GetZimmerListe();
            metaData.kommentarListe = GetKommentarListe();

            return metaData;

        }
        public List<string> GetImages()
        {
            List<string> imgList = new List<string>();

            return imgList;
        }
        public string GetImageFullName(string szImage)
        {
            return Path.Combine(szProjectPath, szImage);
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

            GetGebaeudeForBild(bi.BildId, bi);
            GetWohnungForBild(bi.BildId, bi);
            GetEtageForBild(bi.BildId, bi);
            GetZimmerForBild(bi.BildId, bi);
            GetKommentarForBild(bi.BildId, bi);

            return bi;
        }
        public int GetGebaeudeForBild(int bildId, BildInfo bi = null)
        {
            var ret = database.Table<Bild_Gebaeude>().Where(x => x.BildID == bildId).SingleOrDefault();
            if (ret != null)
            {
                if (bi != null)
                {
                    bi.GebaeudeId = ret.GebaeudeID;
                    bi.GebaeudeBezeichnung = GetGebaeude(ret.GebaeudeID).Bezeichnung;
                }
                return ret.GebaeudeID;
            }
            return -1;
        }
        public int GetEtageForBild(int bildId, BildInfo bi = null)
        {
            var ret = database.Table<Bild_Etage>().Where(x => x.BildID == bildId).SingleOrDefault();
            if (ret != null)
            {
                if (bi != null)
                {
                    bi.EtageId = ret.EtageID;
                    bi.EtageBezeichnung = GetEtage(ret.EtageID).Bezeichnung;
                }
                return ret.EtageID;
            }
            return -1;
        }
        public int GetWohnungForBild(int bildId, BildInfo bi = null)
        {
            var ret = database.Table<Bild_Wohnung>().Where(x => x.BildID == bildId).SingleOrDefault();
            if (ret != null)
            {
                if (bi != null)
                {
                    bi.WohnungId = ret.WohnungID;
                    bi.WohnungBezeichnung = GetWohnung(ret.WohnungID).Bezeichnung;
                }
                return ret.WohnungID;
            }
            return -1;
        }
        public int GetZimmerForBild(int bildId, BildInfo bi = null)
        {
            var ret = database.Table<Bild_Zimmer>().Where(x => x.BildID == bildId).SingleOrDefault();
            if (ret != null)
            {
                if( bi != null )
                {
                    bi.ZimmerId = ret.ZimmerID;
                    bi.ZimmerBezeichnung = GetZimmer(ret.ZimmerID).Bezeichnung;
                }
                return ret.ZimmerID;
            }
            return -1;
        }
        public int GetKommentarForBild(int bildId, BildInfo bi = null)
        {
            var ret = database.Table<Bild_Kommentar>().Where(x => x.BildID == bildId).SingleOrDefault();
            if (ret != null)
            {
                if (bi != null)
                {
                    bi.KommentarId = ret.KommentarID;
                    bi.KommentarBezeichnung = GetKommentar(ret.BildID);
                }
                return ret.KommentarID;
            }
            return -1;
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

        public string GetKommentar(string szImageName)
        {
            var biResult = GetBildId(szImageName,DateTime.Now);
            
            return GetKommentar(biResult.BildId);
        }
        public string GetKommentar(int bildId)
        {
            var kommentarListe = database.Query<Kommentar>("SELECT * FROM [Kommentar] left join Bild_Kommentar WHERE Kommentar.ID = Bild_Kommentar.KommentarID and Bild_Kommentar.BildID = " + bildId.ToString());
                
            if (kommentarListe.Count > 0 )
            {
                return kommentarListe[0].Bezeichnung;
            }
            return "";
        }
        public void SetComment(string szImageName, string szComment)
        {
            var biResult = GetBildId(szImageName,DateTime.Now);
            SetComment(biResult.BildId, szComment);
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

        public void SetComment(int bildId, string szComment)
        {
            var kommentarListe = database.Query<Kommentar>("SELECT * FROM [Kommentar] left join Bild_Kommentar WHERE Kommentar.ID = Bild_Kommentar.KommentarID and Bild_Kommentar.BildID = " + bildId.ToString());

            if (kommentarListe.Count == 0)
            {
                Kommentar kommentar = new Kommentar();
                kommentar.Bezeichnung = szComment;
                database.Insert(kommentar);
                int kommentarID = database.ExecuteScalar<int>("select last_insert_rowid();");
                Bild_Kommentar bk = new Bild_Kommentar { BildID = bildId, KommentarID = kommentarID };
                database.Insert(bk);
            }
            else
            {
                Kommentar kommentar = kommentarListe[0];
                kommentar.Bezeichnung = szComment;
                database.Update(kommentar);
            }
            SetChanged(bildId);
        }
        public void SetGebaeude(int bildId, int gebaeudeId)
        {
            if (gebaeudeId < 0)
            {
                database.Delete<Bild_Gebaeude>(bildId);
            }
            else
            {
                var liste = database.Query<Bild_Gebaeude>(
                    "SELECT * FROM [Bild_Gebaeude] WHERE BildID = " + bildId.ToString());

                if (liste.Count == 0)
                {
                    var eintrag = new Bild_Gebaeude();
                    eintrag.BildID = bildId;
                    eintrag.GebaeudeID = gebaeudeId;
                    database.Insert(eintrag);

                }
                else
                {
                    var eintrag = liste[0];
                    eintrag.GebaeudeID = gebaeudeId;
                    database.Update(eintrag);
                }
            }
            SetChanged(bildId);

        }
        public void SetEtage(int bildId, int etageId)
        {
            if (etageId < 0)
            {
                database.Delete<Bild_Etage>(bildId);
            }
            else
            {
                var liste = database.Query<Bild_Etage>(
                "SELECT * FROM [Bild_Etage] WHERE BildID = " + bildId.ToString());

                if (liste.Count == 0)
                {
                    var eintrag = new Bild_Etage();
                    eintrag.BildID = bildId;
                    eintrag.EtageID = etageId;
                    database.Insert(eintrag);
                }
                else
                {
                    var eintrag = liste[0];
                    eintrag.EtageID = etageId;
                    database.Update(eintrag);
                }
            }
            SetChanged(bildId);
        }
        public void SetWohnung(int bildId, int wohnungId)
        {
            if (wohnungId < 0)
            {
                database.Delete<Bild_Wohnung>(bildId);
            }
            else
            {
                var liste = database.Query<Bild_Wohnung>(
                "SELECT * FROM [Bild_Wohnung] WHERE BildID = " + bildId.ToString());

                if (liste.Count == 0)
                {
                    var eintrag = new Bild_Wohnung();
                    eintrag.BildID = bildId;
                    eintrag.WohnungID = wohnungId;
                    database.Insert(eintrag);

                }
                else
                {
                    var eintrag = liste[0];
                    eintrag.WohnungID = wohnungId;
                    database.Update(eintrag);
                }
            }
            SetChanged(bildId);
        }
        public void SetZimmer(int bildId, int zimmerId)
        {
            if (zimmerId < 0)
            {
                database.Delete<Bild_Zimmer>(bildId);
            }
            else
            {

                var liste = database.Query<Bild_Zimmer>(
                "SELECT * FROM [Bild_Zimmer] WHERE BildID = " + bildId.ToString());

                if (liste.Count == 0)
                {
                    var eintrag = new Bild_Zimmer();
                    eintrag.BildID = bildId;
                    eintrag.ZimmerID = zimmerId;
                    database.Insert(eintrag);

                }
                else
                {
                    var eintrag = liste[0];
                    eintrag.ZimmerID = zimmerId;
                    database.Update(eintrag);
                }
            }
            SetChanged(bildId);
        }
        public int AddBild(string szImageName)
        {
            Bild bild = new Bild();
            bild.Name = szImageName;
            database.Insert(bild);
            return database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public int AddGebaeude(string szGebaeude)
        {
            Gebaeude gebaeude = new Gebaeude();
            gebaeude.Bezeichnung = szGebaeude;
            database.Insert(gebaeude);
            return database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public Gebaeude EnsureGebaeude(string szGebaeude)
        {
            if (string.IsNullOrWhiteSpace(szGebaeude))
            {
                return null;
            }

            Gebaeude gebaeude = GetGebaeude(szGebaeude);
            if (gebaeude == null)
            {
                gebaeude = GetGebaeude(AddGebaeude(szGebaeude));
            }
            return gebaeude;
        }
        public Etage EnsureEtage(string szEtage)
        {
            if (string.IsNullOrWhiteSpace(szEtage))
            {
                return null;
            }

            Etage etage = GetEtage(szEtage);
            if (etage == null)
            {
                etage = GetEtage(AddEtage(szEtage));
            }
            return etage;
        }
        public Wohnung EnsureWohnung(string szWohnung)
        {
            if (string.IsNullOrWhiteSpace(szWohnung))
            {
                return null;
            }

            Wohnung wohnung = GetWohnung(szWohnung);
            if (wohnung == null)
            {
                wohnung = GetWohnung(AddWohnung(szWohnung));
            }
            return wohnung;
        }
        public Zimmer EnsureZimmer(string szZimmer)
        {
            if( string.IsNullOrWhiteSpace(szZimmer) )
            {
                return null;
            }
            Zimmer zimmer = GetZimmer(szZimmer);
            if (zimmer == null)
            {
                zimmer = GetZimmer(AddZimmer(szZimmer));
            }
            return zimmer;
        }
        public int AddEtage(string szEtage)
        {
            Etage etage = new Etage();
            etage.Bezeichnung = szEtage;
            database.Insert(etage);
            return database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public int AddWohnung(string szWohnung)
        {
            Wohnung wohnung = new Wohnung();
            wohnung.Bezeichnung = szWohnung;
            database.Insert(wohnung);
            return database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public int AddZimmer(string szZimmer)
        {
            Zimmer zimmer = new Zimmer();
            zimmer.Bezeichnung = szZimmer;
            database.Insert(zimmer);
            return database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public List<Gebaeude> GetGebaeudeListe()
        {
            return database.Table<Gebaeude>().ToList();
        }
        public List<Etage> GetEtagenListe()
        {
            return database.Table<Etage>().ToList();
        }
        public List<Wohnung> GetWohnungListe()
        {
            return database.Table<Wohnung>().ToList();
        }
        public List<Zimmer> GetZimmerListe()
        {
            return database.Table<Zimmer>().ToList();
        }
        public List<Kommentar> GetKommentarListe()
        {
            return database.Table<Kommentar>().ToList();
        }
        public Gebaeude GetGebaeude(string szBezeichnung)
        {
            return database.Table<Gebaeude>().Where(x => x.Bezeichnung.Equals(szBezeichnung)).SingleOrDefault();
        }
        public Bild GetBild(int id)
        {
            return database.Table<Bild>().Where(x => x.ID == id).SingleOrDefault();
        }
        public Gebaeude GetGebaeude(int id)
        {
            return database.Table<Gebaeude>().Where(x => x.ID == id).SingleOrDefault();
        }
        public Etage GetEtage(string szBezeichnung)
        {
            return database.Table<Etage>().Where(x => x.Bezeichnung.Equals(szBezeichnung)).SingleOrDefault();
        }
        public Etage GetEtage(int id)
        {
            return database.Table<Etage>().Where(x => x.ID == id).SingleOrDefault();
        }
        public Wohnung GetWohnung(string szBezeichnung)
        {
            return database.Table<Wohnung>().Where(x => x.Bezeichnung.Equals(szBezeichnung)).SingleOrDefault();
        }
        public Wohnung GetWohnung(int id)
        {
            return database.Table<Wohnung>().Where(x => x.ID == id).SingleOrDefault();
        }
        public Zimmer GetZimmer(string szBezeichnung)
        {
            return database.Table<Zimmer>().Where(x => x.Bezeichnung.Equals(szBezeichnung)).SingleOrDefault();
        }
        public Zimmer GetZimmer(int id)
        {
            return database.Table<Zimmer>().Where(x => x.ID == id).SingleOrDefault();
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
