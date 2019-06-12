using Soneta.Business;
using Soneta.Zadania;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soneta
{
    public static class ZapiszOdczytFeature
    {
        public static void ZapiszFeature(string Nazwa, object value, ref Session session)
        {
            session.Global.Features[Nazwa] = value;
        }

        public static object OdczytFeature(string Nazwa, ref Session session)
        {
            return session.Global.Features[Nazwa];
        }

        public static object OdczytFeatureObject(string Nazwa, ref Row Typ)
        {
            return Typ.Features[Nazwa];
        }
    }
}
