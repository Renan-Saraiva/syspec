using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SysPec.Data.Models;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Configuration;

namespace SysPec.App.Helpers
{
    public class Current
    {
        public static string CriadorNome 
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    object o = HttpContext.Current.Session["CriadorNome"];
                    if (o != null)
                        return (string)o;
                    else
                    {
                        string name = Criador.Get(HttpContext.Current.User.Identity.GetUserId()).Nome;
                        Current.CriadorNome = name;
                        return name;
                    }
                }
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["CriadorNome"] = value;
            }
        }

        public static int CriadorId
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    object o = HttpContext.Current.Session["CriadorId"];
                    if (o != null)
                        return (int)o;
                    else
                    {
                        int Id = Criador.Get(HttpContext.Current.User.Identity.GetUserId()).Id;
                        Current.CriadorId = Id;
                        return Id;
                    }
                }
                return 0;
            }
            set
            {
                HttpContext.Current.Session["CriadorId"] = value;
            }
        }

        public static int FazendaId
        {
            get
            {
                object o = HttpContext.Current.Session["FazendaId"];
                if (o != null)
                    return (int)o;
                else
                {
                    Fazendas fazendas = Fazendas;
                    if (fazendas != null && fazendas.Count > 0)
                    {
                        FazendaId = fazendas[0].Id;
                        return fazendas[0].Id;
                    }
                    else
                        return 0;
                }                    
            }
            set
            {
                HttpContext.Current.Session["FazendaId"] = value;
            }
        }

        public static Fazendas Fazendas 
        {
            get
            {
                object o = HttpContext.Current.Session["Fazendas"];
                if (o != null)
                    return (Fazendas)o;
                else
                {
                    Fazendas fazendas = Fazendas.List(Helpers.Current.CriadorId);
                    if (fazendas != null) 
                    {
                        Fazendas = fazendas;
                        return fazendas;
                    }
                    else
                        return new Fazendas();
                }
            }
            set
            {
                HttpContext.Current.Session["Fazendas"] = value;
            }
        }

        public static Racoes Racoes 
        {
            get
            {
                object o = HttpContext.Current.Session["Racoes"];
                if (o != null)
                    return (Racoes)o;
                else
                {
                    Racoes racoes = Racoes.ListByCriador(CriadorId);
                    if (racoes != null)
                    {
                        Racoes = racoes;
                        return racoes;
                    }
                    else
                        return new Racoes();
                }
            }
            set
            {
                HttpContext.Current.Session["Racoes"] = value;
            }
        }

        public static Vacinas Vacinas 
        {
            get
            {
                object o = HttpContext.Current.Session["Vacinas"];
                if (o != null)
                    return (Vacinas)o;
                else
                {
                    Vacinas vacinas = Vacinas.ListByCriador(CriadorId);
                    if (vacinas != null)
                    {
                        Vacinas = vacinas;
                        return vacinas;
                    }
                    else
                        return new Vacinas();
                }
            }
            set
            {
                HttpContext.Current.Session["Vacinas"] = value;
            }
        }

        public static void CleanCurrentData()
        {
            HttpContext.Current.Session.Remove("CriadorNome");
            HttpContext.Current.Session.Remove("CriadorId");
            HttpContext.Current.Session.Remove("FazendaId");
            HttpContext.Current.Session.Remove("Fazendas");
            HttpContext.Current.Session.Remove("Racoes");
            HttpContext.Current.Session.Remove("Vacinas");
        }
    }
}