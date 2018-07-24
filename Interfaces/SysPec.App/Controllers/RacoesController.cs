using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SysPec.Data.Models;

namespace SysPec.App.Controllers
{
    [Authorize]
    public class RacaoController : Controller
    {
        // GET: Racoes
        [HttpGet]
        public ActionResult Index(int? RacaoId, RacoesMessageId? message)
        {
            ViewBag.StatusMessage =
            message == RacoesMessageId.AddSuccess ? "Ração adicionada com sucesso."
            : message == RacoesMessageId.SaveSucess ? "Ração atualizada com sucesso."
            : message == RacoesMessageId.Error ? "Ocorreu um erro durante a operação. Tente novamente mais tarde."
            : "";

            ViewBag.RacaoExiste = RacaoId.HasValue && RacaoId > 0;
            if (ViewBag.RacaoExiste)
                return View(Racao.Get(RacaoId.Value));
            return View();
        }

        [HttpPost]
        public ActionResult Index(Racao racao)
        {
            RacoesMessageId message = RacoesMessageId.Error;
            int id = 0;
            try
            {
                if (racao != null)
                {
                    if (racao.Id > 0)
                    {
                        racao.Save();
                        id = racao.Id;
                        message = RacoesMessageId.SaveSucess;
                    }
                    else
                    {
                        racao.Criador = Helpers.Current.CriadorId;
                        racao.Add();
                        id = racao.Id;
                        Helpers.Current.Racoes = Racoes.ListByCriador(Helpers.Current.CriadorId);
                        message = RacoesMessageId.AddSuccess;
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { message = VacinaMessageId.Error });
            }
            return RedirectToAction("Index", new { RacaoId = id, message = message });
        }

        [HttpGet]
        public PartialViewResult Listar()
        {
            ViewBag.Racoes = Racoes.ListByCriador(Helpers.Current.CriadorId);
            return PartialView();
        }
    }

    public enum RacoesMessageId
    {
        AddSuccess,
        SaveSucess,
        Error
    }
}