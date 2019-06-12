using System;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta;
using Soneta.Zadania;
using Soneta.Handel;
using Soneta.Towary;
using Soneta.Magazyny;

[assembly: Worker(typeof(ZakladkaUstawieniaExtender))]
namespace Soneta
{
    public class ZakladkaUstawieniaExtender
    {
        [Context]

        public Session Session { get; set; }



        public Soneta.Zadania.DefProjektu DefProjektu
        {
            get
            {
                var session = Session;
                return (DefProjektu)ZapiszOdczytFeature.OdczytFeature("DefProjektu", ref session);

            }
            set
            {
                var session = Session;
                ZapiszOdczytFeature.ZapiszFeature("DefProjektu", value, ref session);
            }
        }

        public Soneta.Zadania.DefZadania DefZadania
        {
            get
            {
                var session = Session;
                return (DefZadania)ZapiszOdczytFeature.OdczytFeature("DefZadania", ref session);

            }
            set
            {
                var session = Session;
                ZapiszOdczytFeature.ZapiszFeature("DefZadania", value, ref session);
            }
        }

        public Soneta.Handel.DefDokHandlowego DefDokHandlowego
        {
            get
            {
                var session = Session;
                return (DefDokHandlowego)ZapiszOdczytFeature.OdczytFeature("DefDokHandlowego", ref session);

            }
            set
            {
                var session = Session;
                ZapiszOdczytFeature.ZapiszFeature("DefDokHandlowego", value, ref session);
            }
        }

        public Towar TowarDoFakturyCyklicznej
        {
            get
            {
                var session = Session;
                return (Towar)ZapiszOdczytFeature.OdczytFeature("TowarDoFakturyCyklicznej", ref session);

            }
            set
            {
                var session = Session;
                ZapiszOdczytFeature.ZapiszFeature("TowarDoFakturyCyklicznej", value, ref session);
            }
        }

        public Magazyn Magazyn
        {
            get
            {
                var session = Session;
                return (Magazyn)ZapiszOdczytFeature.OdczytFeature("Magazyn", ref session);

            }
            set
            {
                var session = Session;
                ZapiszOdczytFeature.ZapiszFeature("Magazyn", value, ref session);
            }
        }



    }


}
