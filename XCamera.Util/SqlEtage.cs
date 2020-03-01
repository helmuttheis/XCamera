using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCamera.Util
{
    public class SqlEtage : ISql
    {
        readonly ProjectSql projectSql;
        private SQLiteConnection database;
        public SqlEtage(ProjectSql projectSql)
        {
            this.projectSql = projectSql;
            database = projectSql.database;
        }
        public void Set(int bildId, int etageId)
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
            projectSql.SetChanged(bildId);
        }
        public int Add(string szEtage)
        {
            Etage etage = new Etage();
            etage.Bezeichnung = szEtage;
            database.Insert(etage);
            return database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public IDbObject Ensure(string szEtage)
        {
            if (string.IsNullOrWhiteSpace(szEtage))
            {
                return null;
            }

            Etage etage = Get(szEtage) as Etage;
            if (etage == null)
            {
                etage = Get(Add(szEtage)) as Etage;
            }
            return etage;
        }
        public IDbObject Get(string szBezeichnung)
        {
            return database.Table<Etage>().Where(x => x.Bezeichnung.Equals(szBezeichnung)).SingleOrDefault() ;
        }
        public IDbObject Get(int id)
        {
            return database.Table<Etage>().Where(x => x.ID == id).SingleOrDefault();
        }
        public List<Etage> GetListe()
        {
            return database.Table<Etage>().ToList();
        }

        public int GetForBild(int bildId, BildInfo bi = null)
        {
            var ret = database.Table<Bild_Etage>().Where(x => x.BildID == bildId).SingleOrDefault();
            if (ret != null)
            {
                if (bi != null)
                {
                    bi.EtageId = ret.EtageID;
                    bi.EtageBezeichnung = Get(ret.EtageID).Bezeichnung;
                }
                return ret.EtageID;
            }
            return -1;
        }
        public List<Grouped> GetUsed()
        {
            string szSql = "SELECT Count(*) as iCnt, ID, Bezeichnung FROM Etage ";
            szSql += " LEFT JOIN BILD_ETAGE on Etage.ID = BILD_ETAGE.EtageID ";
            szSql += " Group BY ID";
            return database.Query<Grouped>(szSql).ToList();
        }
        public Boolean Delete(IDbObject etage)
        {
            Boolean bRet = false;
            if (etage != null)
            {
                if (!IsUsed(etage.ID))
                {
                    database.Delete<Etage>(etage.ID);
                    bRet = true;
                }
            }

            return bRet;
        }
        public Boolean IsUsed(int id)
        {
            var result = database.Table<Bild_Etage>().Where(x => x.EtageID == id);

            return result.Count() != 0;
        }
        public void Edit(Etage etage, string newBezeichnung)
        {
            var eintrag = etage;
            eintrag.Bezeichnung = newBezeichnung;
            projectSql.database.Update(eintrag);
        }
    }
}
