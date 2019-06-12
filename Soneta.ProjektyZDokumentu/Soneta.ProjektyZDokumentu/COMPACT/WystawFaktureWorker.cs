using System;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta.COMPACT;
using Soneta.Zadania;
using Soneta.Handel;
using Soneta;
using Soneta.Core;
using Soneta.CRM;
using Soneta.Towary;
using Soneta.Types;

[assembly: Worker(typeof(WystawFaktureWorker), typeof(Zadania))]
namespace Soneta.COMPACT
{
    public class WystawFaktureWorker : ContextBase
    {
        public WystawFaktureWorker(Context context) : base(context) { }

        [Context]
        public Context Ctx { get; set; }
        [Context]
        public Zadanie[] Tablica_zadanie { get; set; }
        [Context]
        public WystawFaktureWorkerParams @params { get; set; }


        // TODO -> Należy podmienić podany opis akcji na bardziej czytelny dla uzytkownika
        [Action("Wystaw fakture", Mode = ActionMode.SingleSession | ActionMode.ConfirmSave | ActionMode.Progress, Target = ActionTarget.ToolbarWithText, Icon = ActionIcon.AcceptDoc)]
        public DokumentHandlowy ToDo()
        {

            using (var t = Ctx.Session.Logout(true))
            {

                var session = t.Session;
                var ZM = Soneta.Zadania.ZadaniaModule.GetInstance(t);
                var CRMM = Soneta.CRM.CRMModule.GetInstance(t);
                var HM = Soneta.Handel.HandelModule.GetInstance(t);

                //Projekt 

                var projekt = new Soneta.Zadania.Projekt();
                ZM.Projekty.AddRow(projekt);
                projekt.Definicja = (DefProjektu)ZapiszOdczytFeature.OdczytFeature("DefProjektu", ref session);
                var zparams = ((ZadaniaParams)Ctx[typeof(ZadaniaParams)]);
                Weryfikacja(zparams);

                projekt.Nazwa = $"{@params.kontrahent.NazwaFormatowana} {@params.okres.ToString()}";
                projekt.Kontrahent = @params.kontrahent;
                projekt.DataOd = @params.okres.From;
                projekt.DataDo = @params.okres.To;

                //Dokument handlowy

                var DokH = new DokumentHandlowy();
                HM.DokHandlowe.AddRow(DokH);

                DokH.Definicja = (DefDokHandlowego)ZapiszOdczytFeature.OdczytFeature("DefDokHandlowego", ref session);
                DokH.Magazyn = (Magazyny.Magazyn)ZapiszOdczytFeature.OdczytFeature("Magazyn", ref session);
                

                DokH.Kontrahent = @params.kontrahent;
                DokH.Opis = $"Faktura za usługi w okresie {@params.okres.From}-{@params.okres.To}";
                var time = Soneta.Types.Time.Zero;
                var PodsumowanieWartosci = SumowanieWartosciCRM.Przychód;

                foreach (var Zad in Tablica_zadanie)
                {
                    if (Zad.Kontrahent == @params.kontrahent && !(bool)Zad.Features["Zafakturowane"])
                    {
                        Row RowZad = Zad;

                        Zad.Projekt = projekt;
                        if (!(bool)ZapiszOdczytFeature.OdczytFeatureObject("Bezplatne", ref RowZad))
                            time += (Soneta.Types.Time)ZapiszOdczytFeature.OdczytFeatureObject("Czas pracy", ref RowZad);

                        DokumentCRM dcrm = new Soneta.Zadania.DokumentCRM();
                        ZadaniaModule.GetInstance(t.Session).DokumentyCRM.AddRow(dcrm);
                        dcrm.Dokument = DokH;
                        dcrm.Zadanie = Zad;
                        dcrm.SumowanieWartosci = PodsumowanieWartosci;

                        if (SumowanieWartosciCRM.Przychód == PodsumowanieWartosci)
                            PodsumowanieWartosci = SumowanieWartosciCRM.Brak;
                    }
                }

                PozycjaDokHandlowego PozDokH = new PozycjaDokHandlowego(DokH);
                HM.PozycjeDokHan.AddRow(PozDokH);
                PozDokH.Towar = (Towar)ZapiszOdczytFeature.OdczytFeature("TowarDoFakturyCyklicznej", ref session);
                PozDokH.Ilosc = new Quantity(time.TotalHours);

                projekt.Przychod = DokH.BruttoCy;

                t.CommitUI();
                return DokH;
            }
        }
        public void Weryfikacja(ZadaniaParams zadaniaParams)
        {

            //if (zadaniaParams.Kontrahent == null)
            //    throw new Exception("Uzupełnij kontrahenta w filtrze");

            if (zadaniaParams.Okres == FromTo.Empty)
                throw new Exception("Uzupełnij okres w filtrze");
        }
    }
    public class WystawFaktureWorkerParams : ContextBase
    {
        public WystawFaktureWorkerParams(Context context) : base(context)
        {
            var zparams = ((ZadaniaParams)context[typeof(ZadaniaParams)]);
            
                kontrahent = (Kontrahent)zparams.Kontrahent;
            
                okres = zparams.Okres;
        }
        [Caption("Wybierz kontrahenta: ")]
        public Kontrahent kontrahent { get; set; }
        [Caption("Wybierz okres: ")]
        public FromTo okres { get; set; }
    }

}
