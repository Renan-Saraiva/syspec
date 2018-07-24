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
    public class AplicacaoController : Controller
    {
        [HttpGet]
        public ActionResult PorAnimal(int? AnimalId)
        {
            ViewBag.AnimalExiste = AnimalId.HasValue && AnimalId > 0;
            if (ViewBag.AnimalExiste)
                ViewBag.Animal = Animal.Get(AnimalId.Value);
            else 
            {
                Animais animais = Animais.ListByFazenda(Current.FazendaId);
                if (animais != null && animais.Count > 0)
                    ViewBag.Animais = animais;
                else
                    return RedirectToAction("ApresentaMensagem", new { menssagem = "Não existe animais disponíveis", message = MessageType.Warning });
            }
            return View();
        }

        [HttpGet]
        public ActionResult PorLote(int? LoteId)
        {
            ViewBag.LoteExiste = LoteId.HasValue && LoteId > 0;
            if (ViewBag.LoteExiste)
                ViewBag.Lote = Lote.Get(LoteId.Value);
            else 
            {
                Lotes lotes = Lotes.List(Current.FazendaId);
                ViewBag.Lotes = new Lotes();
                if (lotes != null && lotes.Count > 0)
                    foreach (Lote l in lotes)
                        if (Lote.GetQtnAnimais(l.Id) > 0)
                            ViewBag.Lotes.Add(l);
                if (ViewBag.Lotes.Count <= 0)
                    return RedirectToAction("ApresentaMensagem", new { menssagem = "Não existe lotes com animais disponíveis", message = MessageType.Warning });
            }
            return View();
        }

        [HttpGet]
        public ActionResult Editar()
        {
            Vacinas vacinas = Current.Vacinas;
            if (vacinas.Count <= 0)
                return RedirectToAction("ApresentaMensagem", new { menssagem = "Não existe vacinas cadastradas", message = MessageType.Warning });
            ViewBag.Vacinas = vacinas;
            return PartialView();
        }

        [HttpPost]
        public ActionResult Registrar(Aplicacao model) 
        {
            try
            {
                if (model != null)
                {
                    if (model.Animal > 0)
                    {
                        model.Add();
                    }
                    else
                    {
                        model.AddReplicandoParaLote();
                    }
                    return RedirectToAction("ApresentaMensagem", new { menssagem = "Vacinação registrada com sucesso!", message = MessageType.Sucess });
                }
                else 
                {
                    return RedirectToAction("ApresentaMensagem", new { menssagem = "Não foi possível registrar a vacinação. Dados invalídos!", message = MessageType.Warning });
                }
            }
            catch (Exception) 
            {
                return RedirectToAction("ApresentaMensagem", new { menssagem = "Desulpe! Não foi possível registrar a vacinação. Erro desconhecido!", message = MessageType.Error });
            }
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
            ViewBag.ApresentaDadoAnimais = false;
            ViewBag.Titulo = "Vacinas Aplicadas";
            ViewBag.Aplicacoes = Aplicacoes.ListByAnimal(AnimalId);
            return PartialView("Listar");
        }

        [HttpGet]
        public PartialViewResult ListByVacina(int VacinaId)
        {
            ViewBag.ApresentaDadoAnimais = true;
            Vacina vacina =  Vacina.Get(VacinaId);
            string titulo = string.Empty;
            if (vacina != null)
                titulo = string.Format("Animais vacinados ({0})", vacina.Nome);
            else
                titulo = "Animais vacinados";
            ViewBag.Titulo = titulo;
            ViewBag.Aplicacoes = Aplicacoes.ListByVacina(VacinaId,Current.FazendaId);
            return PartialView("Listar");
        }

        [HttpGet]
        public PartialViewResult Listar()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult Vacinacao()
        {
            ViewBag.Vacinas = Current.Vacinas;
            ViewBag.ApresantaResuldado = false;
            return View();
        }

        [HttpPost]
        public ActionResult Vacinacao(int VacinaId)        
        {
            ViewBag.Vacinas = Current.Vacinas;
            ViewBag.ApresantaResuldado = true;
            Vacina vacina = Vacina.Get(VacinaId);
            Aplicacoes aplicacoes = Aplicacoes.ListByVacina(VacinaId, Current.FazendaId);
            ViewBag.Aplicacoes = aplicacoes;
            ViewBag.ApresentaDadoAnimais = true;
            ViewBag.QuantidadeFazendaAnimais = Animais.ListCountAnimaisByFazenda(Current.FazendaId);
            return View();
        }

        [HttpGet]
        public PartialViewResult VacinacaoFormulario()
        {
            ViewBag.Vacinas = Current.Vacinas;
            return PartialView();
        }
    }
}