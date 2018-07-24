using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SysPec.Data.Models;
using SysPec.App.Helpers;

namespace SysPec.App.Controllers
{
    [Authorize]
    public class AlimentacaoController : Controller
    {
        // GET: Alimentacao
        public ActionResult Index()
        {
            Lotes lotes = Lotes.List(Current.FazendaId);
            ViewBag.Lotes = new Lotes();
            if (lotes != null && lotes.Count > 0)
                foreach (Lote l in lotes)
                    if (Lote.GetQtnAnimais(l.Id) > 0)
                        ViewBag.Lotes.Add(l);
            if (ViewBag.Lotes.Count <= 0)
                return RedirectToAction("ApresentaMensagem", new { menssagem = "Não existe lotes com animais disponíveis", message = MessageType.Warning });
            return View();
        }

        [HttpGet]
        public ActionResult PorLote(int LoteId)
        {
            ViewBag.LoteExiste = LoteId > 0;
            if (ViewBag.LoteExiste) 
            {
                ViewBag.Lote = Lote.Get(LoteId);                
                Pastos pastos = Pastos.List(Current.FazendaId);
                int QuantidadeAtualLote = Lote.GetQtnAnimais(ViewBag.Lote.Id);
                ViewBag.Pastos = new Pastos();
                if (pastos != null && pastos.Count > 0)
                    foreach (Pasto p in pastos)
                        if ((p.GetQtnAnimais() + QuantidadeAtualLote) < p.QtdAnimaisSuporte)
                            ViewBag.Pastos.Add(p);

                if (ViewBag.Pastos.Count <= 0)
                    return RedirectToAction("ApresentaMensagem", new { menssagem = "Não existe pastos disponíveis", message = MessageType.Warning });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Alimentacao model)
        {
            try
            {
                if (model != null)
                {
                    if (model.Animal > 0)
                        model.Add();
                    else
                        model.AddReplicandoParaLote();
                    return RedirectToAction("ApresentaMensagem", new { menssagem = "Alimentação registrada com sucesso!", message = MessageType.Sucess });
                }
                else
                {
                    return RedirectToAction("ApresentaMensagem", new { menssagem = "Não foi possível registrar a alimentação. Dados invalídos!", message = MessageType.Warning });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ApresentaMensagem", new { menssagem = "Desulpe! Não foi possível registrar a alimentação. Erro desconhecido!", message = MessageType.Error });
            }
        }

        [HttpGet]
        public ActionResult Editar()
        {
            Pastos pastos = Pastos.List(Current.FazendaId);
            if (pastos.Count <= 0)
                return RedirectToAction("ApresentaMensagem", new { menssagem = "Não existe pastos cadastradas", message = MessageType.Warning });
            ViewBag.Pastos = pastos;
            return PartialView();
        }

        [HttpGet]
        public ActionResult ApresentaMensagem(string menssagem, MessageType message)
        {
            ViewBag.Menssagem = menssagem;
            ViewBag.MessageType = message;
            return View();
        }

        [HttpGet]
        public PartialViewResult ListByAnimal(int AnimalId)
        {
            ViewBag.Titulo = "Historico Alimentar";
            Alimentacoes A = Alimentacoes.ListByAnimal(AnimalId);
            ViewBag.Alimentacoes = A;
            ViewBag.ApresentaDadoAnimais = false;

            if (A != null && A.Count > 0) 
            {
                ViewBag.ApresentaGrafico = true;
                ViewBag.Datas = TransformaDataEmString(A);
                ViewBag.Pesos = TransformaPesoEmString(A);
            }
            return PartialView("Listar");
        }

        [HttpGet]
        public PartialViewResult Listar()
        {
            return PartialView();
        }

        private string TransformaDataEmString(Alimentacoes Al) 
        {
            string r = string.Empty;
            if (Al != null & Al.Count > 0) 
            {
                foreach (Alimentacao A in Al)
                    r += string.Format("'{0}',",A.CriadoEm.ToString("dd/MM/yyyy"));
                r = r.Substring(0,r.Length -1);
            }
            return r;
        }

        private string TransformaPesoEmString(Alimentacoes Al)
        {
            string r = string.Empty;
            if (Al != null & Al.Count > 0)
            {
                foreach (Alimentacao A in Al)
                    r += string.Format("{0},", A.Peso);
                r = r.Substring(0, r.Length - 1);
            }
            return r;
        }
    }

    public enum MessageType
    {
        Error,
        Sucess,
        Warning
    }

}