using System.Linq;
using System.Web.Mvc;
using Ideastrike.Helpers.Attributes;
using Ideastrike.Models;
using Ideastrike.Models.Repositories;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Web;
using Ideastrike.Models.ViewModels;
using System.Collections.Generic;
using Ideastrike.Helpers;

namespace Ideastrike.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdeaRepository _ideas;
        private readonly IUserRepository _users;
        private readonly ISettingsRepository _settings;

        public HomeController(IIdeaRepository ideas, IUserRepository users, ISettingsRepository settings)
        {
            _ideas = ideas;
            _users = users;
            _settings = settings;
        }

        public ActionResult Index()
        {
            ViewBag.Selected = SelectedTab.Popular;
            ViewBag.PageCount = (_ideas.Count + _settings.PageSize - 1) / _settings.PageSize;
            ViewBag.SortOrder = "Popular";
            return View();
        }

        public ActionResult GetIdeas(int CurrentPage, string SortOrder)
        {
            var query = _ideas.GetAll();

            switch (SortOrder)
            {
                case "New" :
                    query = query.OrderByDescending(i => i.Time);
                    break;
                case "Top":
                    query = query = query.OrderByDescending(i => i.Votes.Sum(s => s.Value));
                    break;
                case "Popular":
                    query = query = query.OrderBy(i => i.Id);
                    break;
                default:
                    query = query.OrderBy(i => i.Id);
                    break;
            }

            query = query.Skip(_settings.PageSize * (CurrentPage - 1)).Take(_settings.PageSize);

            List<IdeaListViewModel> ideaList = new List<IdeaListViewModel>();
            foreach (var i in query)
            {
                ideaList.Add((IdeaListViewModel)i);
            }

            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = JsonConvert.SerializeObject(ideaList);

            return jsonNetResult;
        }

        [IdeastrikeAuthorize(Roles = "Administrators")] // this should match up with a claim on the user 
        public ActionResult Secured()
        {
            return View("Index");
        }

        public ActionResult Top()
        {
            ViewBag.Selected = SelectedTab.Top;
            ViewBag.PageCount = (_ideas.Count + _settings.PageSize - 1) / _settings.PageSize;
            ViewBag.SortOrder = "Top";
            return View("Index");
        }

        public ActionResult New()
        {
            ViewBag.Selected = SelectedTab.New;
            ViewBag.PageCount = (_ideas.Count + _settings.PageSize - 1) / _settings.PageSize;
            ViewBag.SortOrder = "New";
            return View("Index");
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.WelcomeMessage = _settings.WelcomeMessage;
            ViewBag.Title = _settings.SiteTitle;

            base.OnActionExecuted(filterContext);
        }

    }
}
