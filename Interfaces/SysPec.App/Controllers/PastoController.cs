using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SysPec.Data.Models;

namespace SysPec.App.Controllers
{
    [Authorize]
    public class PastoController : Controller
    {
        [HttpGet]
        public ActionResult Index(int? PastoId, PastoMessageId? message)
        {
            ViewBag.StatusMessage =
                    message == PastoMessageId.AddSuccess ? "Pasto adicionado com sucesso."
                    : message == PastoMessageId.SaveSucess ? "Pasto atualizado com sucesso."
                    : message == PastoMessageId.Error ? "Ocorreu um erro durante a operação. Tente novamente mais tarde."
                    : "";

            ViewBag.PastoExiste = PastoId.HasValue && PastoId > 0;
            if (ViewBag.PastoExiste)
                return View(Pasto.Get(PastoId.Value));

            return View();
        }

        [HttpPost]
        public ActionResult Index(Pasto pasto)
        {
            PastoMessageId message = PastoMessageId.Error;
            int id = 0;
            try
            {
                if (pasto != null)
                {
                    if (pasto.Id > 0)
                    {
                        pasto.Save();
                        id = pasto.Id;
                        message = PastoMessageId.SaveSucess;
                    }
                    else
                    {
                        pasto.Fazenda = SysPec.App.Helpers.Current.FazendaId;
                        pasto.Add();
                        id = pasto.Id;
                        message = PastoMessageId.AddSuccess;
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { message = PastoMessageId.Error });
            }
            return RedirectToAction("Index", new { PastoId = id, message = message });
        }

        [HttpGet]
        public PartialViewResult Listar(int? FazendaId)
        {
            if (FazendaId.HasValue && FazendaId > 0)
                ViewBag.Pastos = Pastos.List(FazendaId.Value);
            else 
                ViewBag.Pastos = Pastos.List(SysPec.App.Helpers.Current.FazendaId);

            return PartialView();
        }

        [HttpGet]
        public PartialViewResult Capacidade()
        {
            ViewBag.Pastos = Pastos.List(SysPec.App.Helpers.Current.FazendaId);              
            return PartialView();
        }

        public enum PastoMessageId
        {
            AddSuccess,
            SaveSucess,
            Error
        }
    }
}