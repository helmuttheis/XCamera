using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCamera.Util
{
    public class SqlGebaeude: ISql
    {
        readonly ProjectSql projectSql;
        private SQLiteConnection database;
        public SqlGebaeude(ProjectSql projectSql)
        {
            this.projectSql = projectSql;
            database = projectSql.database;
        }
        public void Set(int bildId, int ID)
        {
            if (ID < 0)
            {
                projectSql.database.Delete<Bild_Gebaeude>(bildId);
            }
            else
            {
                var liste = projectSql.database.Query<Bild_Gebaeude>(
                    "SELECT * FROM [Bild_Gebaeude] WHERE BildID = " + bildId.ToString());

                if (liste.Count == 0)
                {
                    var eintrag = new Bild_Gebaeude();
                    eintrag.BildID = bildId;
                    eintrag.GebaeudeID = ID;
                    projectSql.database.Insert(eintrag);

                }
                else
                {
                    var eintrag = liste[0];
                    eintrag.GebaeudeID = ID;
                    projectSql.database.Update(eintrag);
                }
            }
            projectSql.SetChanged(bildId);

        }
        public int Add(string szBezeichnung)
        {
            Gebaeude gebaeude = new Gebaeude();
            gebaeude.Bezeichnung = szBezeichnung;
            projectSql.database.Insert(gebaeude);
            return projectSql.database.ExecuteScalar<int>("select last_insert_rowid();");
        }
        public IDbObject Ensure(string szBezeichnung)
        {
            if (string.IsNullOrWhiteSpace(szBezeichnung))
            {
                return null;
            }

            Gebaeude gebaeude = Get(szBezeichnung) as Gebaeude;
            if (gebaeude == null)
            {
                gebaeude = (Gebaeude)Get(Add(szBezeichnung));
            }
            return gebaeude;
        }
        public IDbObject Get(int id)
        {
            return projectSql.database.Table<Gebaeude>().Where(x => x.ID == id).SingleOrDefault();
        }
        public IDbObject Get(string szBezeichnung)
        {
            return projectSql.database.Table<Gebaeude>().Where(x => x.Bezeichnung.Equals(szBezeichnung)).SingleOrDefault();
        }
        public int GetForBild(int bildId, BildInfo bi = null)
        {
            var ret = projectSql.database.Table<Bild_Gebaeude>().Where(x => x.BildID == bildId).SingleOrDefault();
            if (ret != null)
            {
                if (bi != null)
                {
                    bi.GebaeudeId = ret.GebaeudeID;
                    bi.GebaeudeBezeichnung = Get(ret.GebaeudeID).Bezeichnung;
                }
                return ret.GebaeudeID;
            }
            return -1;
        }
        public List<Grouped> GetUsed()
        {
            string szSql = "SELECT Count(*) as iCnt, ID, Bezeichnung FROM Gebaeude ";
            szSql += " LEFT JOIN BILD_GEBAEUDE on Gebaeude.ID = BILD_GEBAEUDE.GebaeudeID ";
            szSql += " Group BY ID";
            return projectSql.database.Query<Grouped>(szSql).ToList();
        }
        public List<Gebaeude> GetListe()
        {
            return projectSql.database.Table<Gebaeude>().ToList();
        }

        public Boolean Delete(IDbObject gebaeude)
        {
            Boolean bRet = false;
            if (gebaeude != null)
            {
                if (!IsUsed(gebaeude.ID))
                {
                    projectSql.database.Delete<Gebaeude>(gebaeude.ID);
                    bRet = true;
                }
            }

            return bRet;
        }
        public Boolean IsUsed(int id)
        {
            var result = projectSql.database.Table<Bild_Gebaeude>().Where(x => x.GebaeudeID == id);

            return result.Count() != 0;
        }
    }
}
