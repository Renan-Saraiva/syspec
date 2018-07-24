using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SysPec.Data.Models;

namespace SysPec.App.Controllers
{
    [Authorize]
    public class FazendaController : Controller
    {
        [HttpGet]
        public ActionResult Index(int? FazendaId, FazendaMessageId? message)
        {
            ViewBag.StatusMessage =
                message == FazendaMessageId.AddSuccess ? "Fazenda adicionada com sucesso."
                : message == FazendaMessageId.SaveSucess? "Fazenda atualizada com sucesso."
                : message == FazendaMessageId.Error? "Ocorreu um erro durante a operação. Tente novamente mais tarde."
                : "";

            ViewBag.FazendaExiste = FazendaId.HasValue && FazendaId > 0;
            if (ViewBag.FazendaExiste)
                return View(Fazenda.Get(FazendaId.Value));
            return View();
        }

        [HttpPost]
        public ActionResult Index(Fazenda fazenda, [Bind(Prefix = "Pasto")]Pasto pasto)
        {
            FazendaMessageId message = FazendaMessageId.Error;
            int id = 0;
            try
            {
                if (fazenda != null)
                {
                    if (fazenda.Id > 0)
                    {
                        fazenda.Save();
                        id = fazenda.Id;
                        message = FazendaMessageId.SaveSucess;
                    }
                    else
                    {
                        fazenda.Criador = Helpers.Current.CriadorId;
                        fazenda.Add();
                        id = fazenda.Id;
                        message = FazendaMessageId.AddSuccess;
                        Helpers.Current.Fazendas = Fazendas.List(Helpers.Current.CriadorId);
                        Helpers.Current.FazendaId = id;
                    }
                    if (pasto != null && !string.IsNullOrEmpty(pasto.Nome) && pasto.QtdAnimaisSuporte > 0)
                    {
                        pasto.Fazenda = fazenda.Id;
                        pasto.Add();
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { message = FazendaMessageId.Error });
            }
            return RedirectToAction("Index", new { FazendaId = id, message = message });
        }

        [HttpGet]
        public PartialViewResult Listar()
        {
            ViewBag.Fazendas = Fazendas.List(SysPec.App.Helpers.Current.CriadorId);
            return PartialView();
        }

        public enum FazendaMessageId
        {
            AddSuccess,
            SaveSucess,
            Error
        }
    }
}