using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SysPec.Data.Models;

namespace SysPec.App.Controllers
{
    [Authorize]
    public class AnimalController : Controller
    {
        [HttpGet]
        public ActionResult Index(int? AnimalId, AnimalMessageId? message, bool? ApresentaBusca)
        {
            ViewBag.StatusMessage =
                    message == AnimalMessageId.AddSuccess ? "Animal adicionado com sucesso."
                    : message == AnimalMessageId.DisableSucess ? "Animal desabilitado com sucesso."
                    : message == AnimalMessageId.Error ? "Ocorreu um erro durante a operação. Tente novamente mais tarde."
                    : "";

            if (ApresentaBusca.HasValue && ApresentaBusca.Value)
            {
                ViewBag.ApresentaBusca = true;
            }
            else
            {
                ViewBag.ApresentaBusca = false;
                ViewBag.AnimalExiste = AnimalId.HasValue && AnimalId.Value > 0;
                if (ViewBag.AnimalExiste)
                {
                    return View(Animal.Get(AnimalId.Value));
                }
                else
                {
                    Lotes lotes = Lotes.List(Helpers.Current.FazendaId);
                    Lotes lotesDisponiveis = new Lotes();
                    foreach (Lote l in lotes)
                    {
                        if (LoteController._QtdMaxAnimais > Lote.GetQtnAnimais(l.Id))
                        {
                            lotesDisponiveis.Add(l);
                        }
                    }
                    ViewBag.LotesDisponiveis = lotesDisponiveis;
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(Animal model)
        {
            AnimalMessageId message = AnimalMessageId.Error;
            int id = 0;
            try
            {
                if (model != null)
                {
                    model.Add();
                    message = AnimalMessageId.AddSuccess;
                    id = model.Id;
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { message = AnimalMessageId.Error });
            }
            return RedirectToAction("Index", new { AnimalId = id, message = message });
        }

        [HttpGet]
        public ActionResult Buscar()
        {
            return RedirectToAction("Index", new { ApresentaBusca = true });
        }

        [HttpGet]
        public PartialViewResult BuscaFormulario()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Buscar(int TipoBusca, string ValorBusca)
        {
            Animais animais = new Animais();
            switch (TipoBusca)
            {
                case 1:
                    animais = Animais.ListAnimaisByCodigo(ValorBusca, Helpers.Current.FazendaId);
                    break;
                case 2:
                    animais = Animais.ListByLoteName(ValorBusca, Helpers.Current.FazendaId);
                    break;
            }
            TempData["animais"] = animais;
            TempData["ValorBusca"] = ValorBusca;
            return View();
        }

        [HttpPost]
        public ActionResult Desabilitar(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    Animal.Disable(Id);
                    return RedirectToAction("Index", new { message = AnimalMessageId.DisableSucess });
                }
                else
                    return RedirectToAction("Index", new { message = AnimalMessageId.Error });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { message = AnimalMessageId.Error });
            }
        }

        [HttpGet]
        public PartialViewResult ListByLote(int LoteId)
        {
            ViewBag.Titulo = "Animais do Lote";
            ViewBag.Animais = Animais.ListByLote(LoteId);
            return PartialView("Listar");
        }

        [HttpGet]
        public PartialViewResult ListByFazenda(int FazendaId)
        {
            ViewBag.Titulo = "Animais da Fazenda";
            ViewBag.Animais = Animais.ListByFazenda(FazendaId);
            return PartialView("Listar");
        }

        [HttpGet]
        public PartialViewResult Listar()
        {
            return PartialView();
        }

        public enum AnimalMessageId
        {
            AddSuccess,
            DisableSucess,
            Error
        }
    }
}