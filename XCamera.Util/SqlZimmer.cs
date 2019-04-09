using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCamera.Util
{
    public class SqlZimmer : ISql
    {
        readonly ProjectSql projectSql;
        private SQLiteConnection database;
        public SqlZimmer(ProjectSql projectSql)
        {
            this.projectSql = projectSql;
            database = projectSql.database;
        }
        public void Set(int bildId, int zimmerId)
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
            projectSql.SetChanged(bildId);
        }
        public int Add(string szZimmer)
        {
            Zimmer zimmer = new Zimmer();
            zimmer.Bezeichnung = szZimmer;
            database.Insert(zimmer);
            return database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public IDbObject Ensure(string szZimmer)
        {
            if (string.IsNullOrWhiteSpace(szZimmer))
            {
                return null;
            }
            IDbObject zimmer = Get(szZimmer);
            if (zimmer == null)
            {
                zimmer = Get(Add(szZimmer));
            }
            return zimmer;
        }
        public List<Zimmer> GetListe()
        {
            return database.Table<Zimmer>().ToList();
        }
        public List<Grouped> GetUsed()
        {
            string szSql = "SELECT Count(*) as iCnt, ID, Bezeichnung FROM Zimmer ";
            szSql += " LEFT JOIN BILD_ZIMMER on Zimmer.ID = BILD_ZIMMER.ZimmerID ";
            szSql += " Group BY ID";
            return database.Query<Grouped>(szSql).ToList();
        }
        public IDbObject Get(string szBezeichnung)
        {
            return database.Table<Zimmer>().Where(x => x.Bezeichnung.Equals(szBezeichnung)).SingleOrDefault();
        }
        public IDbObject Get(int id)
        {
            return database.Table<Zimmer>().Where(x => x.ID == id).SingleOrDefault();
        }
        public int GetForBild(int bildId, BildInfo bi = null)
        {
            var ret = database.Table<Bild_Zimmer>().Where(x => x.BildID == bildId).SingleOrDefault();
            if (ret != null)
            {
                if (bi != null)
                {
                    bi.ZimmerId = ret.ZimmerID;
                    bi.ZimmerBezeichnung = Get(ret.ZimmerID).Bezeichnung;
                }
                return ret.ZimmerID;
            }
            return -1;
        }
        public Boolean Delete(IDbObject zimmer)
        {
            Boolean bRet = false;
            if (zimmer != null)
            {
                if (!IsUsed(zimmer.ID))
                {
                    database.Delete<Zimmer>(zimmer.ID);
                    bRet = true;
                }
            }

            return bRet;
        }
        public Boolean IsUsed(int id)
        {
            var result = database.Table<Bild_Zimmer>().Where(x => x.ZimmerID == id);

            return result.Count() != 0;
        }
    }
}
