using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SysPec.Data.Models;
using SysPec.Data.Estatisticas;

namespace SysPec.App.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index(int? FazendaId)
        {
            if (FazendaId.HasValue && FazendaId.Value > 0) 
                Helpers.Current.FazendaId = FazendaId.Value;

            if (Helpers.Current.FazendaId > 0)
            {
                Fazenda f = Fazenda.Get(Helpers.Current.FazendaId);
                int Animais, Racoes, Pastos, Lotes;
                f.GetSumario(out Animais, out Lotes, out Racoes, out Pastos);
                ViewBag.Animais = Animais;
                ViewBag.Racoes = Racoes;
                ViewBag.Pastos = Pastos;
                ViewBag.Lotes = Lotes;
                ViewBag.Capacidades = Capacidades.ListByCriador(Helpers.Current.CriadorId);
            }                
            return View();
        }

        public ActionResult ListAnimais() 
        {
            return View();
        }

        public ActionResult ListLotes()
        {
            return View();
        }

        public ActionResult ListRacoes()
        {
            return View();
        }

        public ActionResult ListPastos() 
        {
            return View();
        }
    }
}