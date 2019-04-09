using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCamera.Util
{
    public partial class ProjectSql
    {
        public string GetKommentar(string szImageName)
        {
            var biResult = GetBildId(szImageName, DateTime.Now);

            return GetKommentar(biResult.BildId);
        }
        public string GetKommentar(int bildId)
        {
            var kommentarListe = database.Query<Kommentar>("SELECT * FROM [Kommentar] left join Bild_Kommentar WHERE Kommentar.ID = Bild_Kommentar.KommentarID and Bild_Kommentar.BildID = " + bildId.ToString());

            if (kommentarListe.Count > 0)
            {
                return kommentarListe[0].Bezeichnung;
            }
            return "";
        }
        public void SetComment(string szImageName, string szComment)
        {
            var biResult = GetBildId(szImageName, DateTime.Now);
            SetComment(biResult.BildId, szComment);
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
        public List<Kommentar> GetKommentarListe()
        {
            return database.Table<Kommentar>().ToList();
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
        public List<Grouped> GetUsedKommentar()
        {
            string szSql = "SELECT Count(*) as iCnt, ID, Bezeichnung FROM Kommentar ";
            szSql += " LEFT JOIN BILD_KOMMENTAR on Kommentar.ID = BILD_KOMMENTAR.KommentarID ";
            szSql += " Group BY ID";
            return database.Query<Grouped>(szSql).ToList();
        }
        public Boolean DeleteKommentar(Kommentar kommentar)
        {
            Boolean bRet = false;
            if (kommentar != null)
            {
                if (!IsUsedKommentar(kommentar.ID))
                {
                    database.Delete<Kommentar>(kommentar.ID);
                    bRet = true;
                }
            }

            return bRet;
        }
        public Boolean IsUsedKommentar(int id)
        {
            var result = database.Table<Bild_Kommentar>().Where(x => x.KommentarID == id);

            return result.Count() != 0;
        }

    }
}
