using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SysPec.Data.Models;

namespace SysPec.App.Controllers
{
    [Authorize]
    public class LoteController : Controller
    {
        public const int _QtdMaxAnimais = 100;

        [HttpGet]
        public ActionResult Index(int? LoteId, LoteMessageId? message)
        {
            ViewBag.StatusMessage =
                message == LoteMessageId.AddSuccess ? "Lote adicionado com sucesso."
                : message == LoteMessageId.SaveSucess ? "Lote atualizado com sucesso."
                : message == LoteMessageId.Error ? "Ocorreu um erro durante a operação. Tente novamente mais tarde."
                : message == LoteMessageId.AddAnimaisSucess ? "Animais adicionados com sucesso."
                : "";

            ViewBag.LoteExiste = LoteId.HasValue && LoteId > 0;
            if (ViewBag.LoteExiste) 
                return View(new LoteModelBag { Lote = Lote.Get(LoteId.Value) });
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoteModelBag model)
        {
            LoteMessageId message = LoteMessageId.Error;
            int id = 0;
            try
            {
                if (model != null)
                {
                    if (model.Lote.Id > 0)
                    {
                        model.Lote.Save();
                        id = model.Lote.Id;
                        message = LoteMessageId.SaveSucess;
                    }
                    else
                    {

                        model.Lote.Fazenda = SysPec.App.Helpers.Current.FazendaId;
                        model.Lote.Add();
                        id = model.Lote.Id;
                        addAnimaisToLote(model.Lote, model.Animal, model.QtdDeAnimais);
                        message = LoteMessageId.AddSuccess;
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { message = LoteMessageId.Error });
            }
            return RedirectToAction("Index", new { LoteId = id, message = message });
        }

        [HttpGet]
        public PartialViewResult AddAnimaisToLote(int? LoteId)
        {
            ViewBag.LoteExiste = LoteId.HasValue && LoteId > 0;
            if (ViewBag.LoteExiste)
            {
                ViewBag.QtdAnimais = Lote.GetQtnAnimais(LoteId.Value);
                ViewBag.QtdAnimaisPermitida = _QtdMaxAnimais - ViewBag.QtdAnimais;
            }
            else 
            {
                ViewBag.QtdAnimais = 0;
                ViewBag.QtdAnimaisPermitida = _QtdMaxAnimais;
            }

            return PartialView();
        }

        [HttpPost]
        public ActionResult AddAnimaisToLote(LoteModelBag model) 
        {
            LoteMessageId message = LoteMessageId.Error;
            int id = 0;
            try
            {
                addAnimaisToLote(model.Lote, model.Animal, model.QtdDeAnimais);
                message = LoteMessageId.AddAnimaisSucess;
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { message = LoteMessageId.Error });
            }
            return RedirectToAction("Index", new { LoteId = id, message = message });
        }

        [HttpGet]
        public PartialViewResult Listar(int? FazendaId)
        {
            if (FazendaId.HasValue && FazendaId > 0)
                ViewBag.Lotes = Lotes.List(FazendaId.Value);
            else
                ViewBag.Lotes = Lotes.List(SysPec.App.Helpers.Current.FazendaId);

            return PartialView();
        }

        [HttpGet]
        public ActionResult ListarAnimaisDoLote(int LoteId)
        {
            ViewBag.LoteId = LoteId;
            return View();
        }

        private void addAnimaisToLote(Lote lote, Animal modelo, int Quantidade)
        {
            Animais animais = new Animais();
            for (int i = 1; i <= Quantidade; i++)
                animais.Add(new Animal
                {
                    NascidoEm = modelo.NascidoEm,
                    Peso = modelo.Peso,
                    Raca = modelo.Raca,
                    Sexo = modelo.Sexo
                });

            animais.AddAnimais(SysPec.App.Helpers.Current.FazendaId, lote.Id);   
        }
    }

    public enum LoteMessageId
    {
        AddSuccess,
        SaveSucess,
        AddAnimaisSucess,
        Error
    }
}