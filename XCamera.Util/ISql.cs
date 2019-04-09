using System;
using System.Collections.Generic;
using System.Text;

namespace XCamera.Util
{
    public interface  ISql
    {
        void Set(int bildId, int ID);
        int Add(string szBezeichnung);
        IDbObject Ensure(string szBezeichnung);
        IDbObject Get(int id);
        IDbObject Get(string szBezeichnung);
        int GetForBild(int bildId, BildInfo bi = null);
        List<Grouped> GetUsed();
        Boolean Delete(IDbObject obj);
        Boolean IsUsed(int id);
    }
}
