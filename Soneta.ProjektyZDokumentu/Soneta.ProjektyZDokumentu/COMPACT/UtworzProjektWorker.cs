using System;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta;
using Soneta.Handel;
using Soneta.CRM;
using Soneta.Zadania;
using Soneta.Types;
using System.Linq;


[assembly: Worker(typeof(UtworzProjektWorker), typeof(DokumentHandlowy))]
namespace Soneta
{
    public class UtworzProjektWorker : ContextBase
    {
        public UtworzProjektWorker(Context context) : base(context) { }
        [Context]
        public UtworzProjektWorkerParams @params { get; set; }
        [Context]
        public Context Ctx { get; set; }
        [Context]
        public DokumentHandlowy DokH { get; set; }

        // TODO -> Należy podmienić podany opis akcji na bardziej czytelny dla uzytkownika
        [Action("Utwórz projekt", Mode = ActionMode.SingleSession | ActionMode.ConfirmSave | ActionMode.Progress, Target = ActionTarget.ToolbarWithText, Icon = ActionIcon.Book)]
        public void TworzenieProjektu()
        {
            using (var t = Ctx.Session.Logout(true))
            {
                var ZM = Soneta.Zadania.ZadaniaModule.GetInstance(t);
                var CRMM = Soneta.CRM.CRMModule.GetInstance(t);

                //projekt
                var projekt = new Soneta.Zadania.Projekt();
                ZM.Projekty.AddRow(projekt);
                string definicjaProjektu = Session.Global.Features["DefProjektu"].ToString();
                projekt.Definicja = ZM.DefProjektow.WgSymbolu[definicjaProjektu];
                projekt.Prowadzacy = @params.Prowadzacy;

                if (@params.CzyZmienicNazwe)
                    projekt.Nazwa = @params.NazwaProjektu;
                else
                    projekt.Nazwa = $"{DokH.Opis} {DokH.Kontrahent.Nazwa} {DokH.Numer.NumerPelny}";

                if (@params.CzyPrzeniescPrzychod)
                    projekt.Przychod = DokH.BruttoCy;

                projekt.Kontrahent = DokH.Kontrahent;

                //Tworzenie zadania do utworzonego projektu

                var zadanie = new Zadanie();
                ZM.Zadania.AddRow(zadanie);

                //Parametry zadania

                string definicjaZadania = Session.Global.Features["DefZadania"].ToString();
                zadanie.Definicja = ZM.DefZadan.WgSymbolu[definicjaZadania];
                zadanie.DataOd = @params.DataRozpoczecia;
                zadanie.Projekt = projekt;
                zadanie.Wykonujacy = @params.Prowadzacy;
                zadanie.Przychod = DokH.BruttoCy;
                zadanie.Opis = $"Zadanie dla dokumentu {DokH.Numer.NumerPelny} kontrahenta {DokH.Kontrahent} ";

                // Dodanie dokh do zadania

                DokumentCRM dcrm = new Soneta.Zadania.DokumentCRM();
                ZadaniaModule.GetInstance(t.Session).DokumentyCRM.AddRow(dcrm);
                dcrm.Dokument = DokH;
                dcrm.Zadanie = zadanie;

                t.CommitUI();
            }
        }

        public static bool IsVisibleTworzenieProjektu(DokumentHandlowy DokH)
        {
            return true;
            /*
             

            if (String.IsNullOrWhiteSpace(DokH.Session.Global.Features["DefDokHandlowego"].ToString()))
                return DokH.Definicja.Symbol == "FV";
           else
                return DokH.Definicja.Symbol == DokH.Session.Global.Features["DefDokHandlowego"].ToString();
                */
        }
    }
    [Caption("Tworzenie projektu ...")]
    public class UtworzProjektWorkerParams : ContextBase
    {
        public UtworzProjektWorkerParams(Context context) : base(context)
        {

            Prowadzacy = context.Session.Login.Operator;
            DataRozpoczecia = Date.Today;
        }

        public Soneta.Business.App.Operator Prowadzacy { get; set; }
        public object GetListProwadzacy()
        {
            return new LookupInfo.EnumerableItem(
                "Operators",
                Soneta.Business.Db.BusinessModule.GetInstance(Session).Operators.Cast<Soneta.Business.App.Operator>().
                    Where(x => !x.Locked && !x.IsOperatorNet).ToList().OrderBy(x => x.Name),
                new[] { "FullName" });
        }

        public bool CzyZmienicNazwe { get; set; }
        public string NazwaProjektu { get; set; }
        public Date DataRozpoczecia { get; set; }
        public bool CzyPrzeniescPrzychod { get; set; }

    }

}
