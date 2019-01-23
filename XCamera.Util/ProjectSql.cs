using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XCamera.Util
{
    public class ProjectSql
    {
        public static string szProjectName { get; set; } = "Sample";
        public string szProjectPath { get; set; }
        public string szProjectFile { get; set; }
        private string szBasePath { get; set; }
        public string szTempProjectPath { get; set; }

        readonly SQLiteConnection database;

        public ProjectSql(string szBasePath)
        {
            this.szBasePath = szBasePath;
            szProjectPath = Path.Combine(szBasePath, szProjectName);
            if (!Directory.Exists(szProjectPath))
            {
                Directory.CreateDirectory(szProjectPath);
            }
            szProjectFile = Path.Combine(szProjectPath, szProjectName + ".db");

            database = new SQLiteConnection(szProjectFile);
            
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
        }
        public int GetBildId(string szFullImageName)
        {
            var bildListe = database.Query<Bild>("SELECT * FROM [Bild] WHERE [Name] = '" + szFullImageName + "'");
            int bildID;
            if (bildListe.Count > 0)
            {
                bildID = bildListe[0].ID;
            }
            else
            {
                Bild bild = new Bild
                {
                    Name = szFullImageName
                };
                database.Insert(bild);
                bildID = database.ExecuteScalar<int>("select last_insert_rowid();");
            }
            return bildID;
        }
        public string GetComment(string szFullImageName)
        {
            int bildID = GetBildId(szFullImageName);
            
            return GetComment(bildID);
        }
        public string GetComment(int id)
        {
            var kommentarListe = database.Query<Kommentar>("SELECT * FROM [Kommentar] left join Bild_Kommentar WHERE Kommentar.ID = Bild_Kommentar.KommentarID and Bild_Kommentar.BildID = " + id.ToString());
                
            if (kommentarListe.Count > 0 )
            {
                return kommentarListe[0].Bezeichnung;
            }
            return "";
        }
        public void SetComment(string szFullImageName, string szComment)
        {
            int bildID = GetBildId(szFullImageName);
            SetComment(bildID, szComment);
        }
        public void SetComment(int id, string szComment)
        {
            var kommentarListe = database.Query<Kommentar>("SELECT * FROM [Kommentar] left join Bild_Kommentar WHERE Kommentar.ID = Bild_Kommentar.KommentarID and Bild_Kommentar.BildID = " + id.ToString());

            if (kommentarListe.Count == 0)
            {
                Kommentar kommentar = new Kommentar();
                kommentar.Bezeichnung = szComment;
                database.Insert(kommentar);
                int kommentarID = database.ExecuteScalar<int>("select last_insert_rowid();");
                Bild_Kommentar bk = new Bild_Kommentar { BildID = id, KommentarID = kommentarID };
                database.Insert(bk);
            }
            else
            {
                Kommentar kommentar = kommentarListe[0];
                kommentar.Bezeichnung = szComment;
                database.Update(kommentar);
            }
        }
        public void SetGebaeude(int bildId, int gebaeudeId)
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
        public void SetEtage(int bildId, int etageId)
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
        public void SetWohnung(int bildId, int wohnungId)
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
        public void SetZimmer(int bildId, int zimmerId)
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
        public int AddBild(string szFullImageName)
        {
            Bild bild = new Bild();
            bild.Name = szFullImageName;
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
    }
    public class Zusatz
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Bezeichnung { get; set; }
    }
    public class Bild_Zusatz
    {
        [PrimaryKey]
        public int BildID { get; set; }
        public int ZusatzID { get; set; }
    }

    public class Gebaeude
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Bezeichnung { get; set; }
    }
    public class Bild_Gebaeude
    {
        [PrimaryKey]
        public int BildID { get; set; }
        public int GebaeudeID { get; set; }
    }

    public class Etage
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Bezeichnung { get; set; }
    }
    public class Bild_Etage
    {
        [PrimaryKey]
        public int BildID { get; set; }
        public int EtageID { get; set; }
    }

    public class Wohnung
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Bezeichnung { get; set; }
    }
    public class Bild_Wohnung
    {
        [PrimaryKey]
        public int BildID { get; set; }
        public int WohnungID { get; set; }
    }
    public class Zimmer
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Bezeichnung { get; set; }
    }
    public class Bild_Zimmer
    {
        [PrimaryKey]
        public int BildID { get; set; }
        public int ZimmerID { get; set; }
    }
    public class Kommentar
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Bezeichnung { get; set; }
    }
    public class Bild_Kommentar
    {
        [PrimaryKey]
        public int BildID { get; set; }
        public int KommentarID { get; set; }
    }

    public class Bild
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}
