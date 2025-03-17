using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XCamera.Util
{
    public class SqlWohnung : ISql
    {
        readonly ProjectSql projectSql;
        private SQLiteConnection database;
        public SqlWohnung(ProjectSql projectSql)
        {
            this.projectSql = projectSql;
            database = projectSql.database;
        }
        public void Set(int bildId, int wohnungId)
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
            projectSql.SetChanged(bildId);
        }
        public IDbObject Ensure(string szWohnung)
        {
            if (string.IsNullOrWhiteSpace(szWohnung))
            {
                return null;
            }

            IDbObject wohnung = Get(szWohnung);
            if (wohnung == null)
            {
                wohnung = Get(Add(szWohnung));
            }
            return wohnung;
        }
        public int Add(string szWohnung)
        {
            Wohnung wohnung = new Wohnung();
            wohnung.Bezeichnung = szWohnung;
            database.Insert(wohnung);
            return database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public List<Wohnung> GetListe()
        {
            var list = database.Table<Wohnung>().ToList();
            list.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
            return list;
        }
        public IDbObject Get(string szBezeichnung)
        {
            return database.Table<Wohnung>().Where(x => x.Bezeichnung.Equals(szBezeichnung)).SingleOrDefault();
        }
        public IDbObject Get(int id)
        {
            return database.Table<Wohnung>().Where(x => x.ID == id).SingleOrDefault();
        }
        public int GetForBild(int bildId, BildInfo bi = null)
        {
            var ret = database.Table<Bild_Wohnung>().Where(x => x.BildID == bildId).SingleOrDefault();
            if (ret != null)
            {
                if (bi != null)
                {
                    bi.WohnungId = ret.WohnungID;
                    bi.WohnungBezeichnung = Get(ret.WohnungID).Bezeichnung;
                }
                return ret.WohnungID;
            }
            return -1;
        }
        public List<Grouped> GetUsed()
        {
            string szSql = "SELECT Count(*) as iCnt, ID, Bezeichnung FROM Wohnung ";
            szSql += " LEFT JOIN BILD_WOHNUNG on Wohnung.ID = BILD_WOHNUNG.WohnungID ";
            szSql += " Group BY ID";
            return database.Query<Grouped>(szSql).ToList();
        }
        public Boolean Delete(IDbObject wohnung)
        {
            Boolean bRet = false;
            if (wohnung != null)
            {
                if (!IsUsed(wohnung.ID))
                {
                    database.Delete<Wohnung>(wohnung.ID);
                    bRet = true;
                }
            }

            return bRet;
        }
        public Boolean IsUsed(int id)
        {
            var result = database.Table<Bild_Wohnung>().Where(x => x.WohnungID == id);

            return result.Count() != 0;
        }
        public void Edit(Wohnung wohnung, string newBezeichnung)
        {
            var eintrag = wohnung;
            eintrag.Bezeichnung = newBezeichnung;
            projectSql.database.Update(eintrag);
        }
    }
}
