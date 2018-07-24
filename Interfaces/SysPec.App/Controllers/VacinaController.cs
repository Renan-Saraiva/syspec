using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SysPec.Data.Models;

namespace SysPec.App.Controllers
{
    [Authorize]
    public class VacinaController : Controller
    {
        [HttpGet]
        public ActionResult Index(int? VacinaId, VacinaMessageId? message)
        {
            ViewBag.StatusMessage =
            message == VacinaMessageId.AddSuccess ? "Vacina adicionada com sucesso."
            : message == VacinaMessageId.SaveSucess ? "Vacina atualizada com sucesso."
            : message == VacinaMessageId.Error ? "Ocorreu um erro durante a operação. Tente novamente mais tarde."
            : "";

            ViewBag.VacinaExiste = VacinaId.HasValue && VacinaId > 0;
            if (ViewBag.VacinaExiste)
                return View(Vacina.Get(VacinaId.Value));
            return View();
        }

        [HttpPost]
        public ActionResult Index(Vacina vacina)
        {
            VacinaMessageId message = VacinaMessageId.Error;
            int id = 0;
            try
            {
                if (vacina != null)
                {
                    if (vacina.Id > 0)
                    {
                        vacina.Save();
                        id = vacina.Id;
                        message = VacinaMessageId.SaveSucess;
                    }
                    else
                    {
                        vacina.Criador = Helpers.Current.CriadorId;
                        vacina.Add();
                        id = vacina.Id;
                        Helpers.Current.Vacinas = Vacinas.ListByCriador(Helpers.Current.CriadorId);
                        message = VacinaMessageId.AddSuccess;
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { message = VacinaMessageId.Error });
            }
            return RedirectToAction("Index", new { VacinaId = id, message = message });
        }

        [HttpGet]
        public PartialViewResult Listar() 
        {
            ViewBag.Vacinas = Vacinas.ListByCriador(Helpers.Current.CriadorId);
            return PartialView();
        }
    }

    public enum VacinaMessageId
    {
        AddSuccess,
        SaveSucess,
        Error
    }
}