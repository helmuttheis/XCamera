using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace XCamera.Util
{
    [Flags]
    public enum STATUS
    {
        NONE=0,
        DELETED=1,
        NEW=2,
        CHANGED=4
    }
    public class BildInfo
    {
        public int BildId { get; set; }
        public string BildName { get; set; }
        public Boolean bBildIdFound { get; set; }
        public int GebaeudeId { get; set; }
        public string GebaeudeBezeichnung { get; set; }
        public int EtageId { get; set; }
        public string EtageBezeichnung { get; set; }
        public int WohnungId { get; set; }
        public string WohnungBezeichnung { get; set; }
        public int ZimmerId { get; set; }
        public string ZimmerBezeichnung { get; set; }
        public int KommentarId { get; set; }
        public string KommentarBezeichnung { get; set; }
    }
    public class MetaData
    {
        public List<Gebaeude> gebaeudeListe { get; set; }
        public List<Etage> etageListe { get; set; }
        public List<Wohnung> wohnungiste { get; set; }
        public List<Zimmer> zimmerListe { get; set; }
        public List<Kommentar> kommentarListe { get; set; }
    }
    public class Zusatz
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Bezeichnung { get; set; }
        public override string ToString()
        {
            return Bezeichnung;
        }
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
        public int Status { get; set; }

        public override string ToString()
        {
            return Bezeichnung;
        }
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
        public int Status { get; set; }
        public override string ToString()
        {
            return Bezeichnung;
        }
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
        public int Status { get; set; }
        public override string ToString()
        {
            return Bezeichnung;
        }
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
        public int Status { get; set; }
        public override string ToString()
        {
            return Bezeichnung;
        }
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
        public int Status { get; set; }
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
        public int Status { get; set; }
    }

}
