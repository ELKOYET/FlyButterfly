using FlyButterfly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyButterfly.Controllers
{
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {

        private UserModel _User { get { return _context.Users.FirstOrDefault(u => u.Login == User.Identity.Name); } }

        private ApplicationContext _context;

        public UserController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult UserPanel()
        {
            return View(_User);
        }

        #region Projects
        [HttpGet]
        public IActionResult Projects(string name)
        {
            var FilteredProjects = _context.RiskModels.Where(x => x.User == _User).Select(x => x.Project);

            if (!string.IsNullOrEmpty(name)) FilteredProjects = FilteredProjects.Where(x => x.Name.Contains(name));

            ViewBag.Projects = FilteredProjects.ToArray();

            return View();
        }
        #endregion Projects


        #region Risks
        [HttpGet]
        public IActionResult Risks(int projectId, string name, int? TypeId, int? ChanceId)
        {
            if (_context.Projects.Find(projectId) == null)
                return NotFound();

            IQueryable<RiskModel> FilteredRisks = _context.RiskModels.Where(x => x.Project.ID == projectId && x.User == _User);

            if (!string.IsNullOrEmpty(name)) FilteredRisks = FilteredRisks.Where(x => x.Name.Contains(name));
            if (TypeId != null) FilteredRisks = FilteredRisks.Where(x => x.Type.ID == TypeId);
            if (ChanceId != null) FilteredRisks = FilteredRisks.Where(x => x.Chance.ID == ChanceId);

            ViewBag.projectId = projectId;

            ViewBag.Risks = FilteredRisks.ToArray();

            ViewBag.Chances = new SelectList(_context.RiskChances.ToArray(), "ID", "Name");
            ViewBag.Types = new SelectList(_context.RiskTypes.ToArray(), "ID", "Name");

            return View();
        }
        #endregion Risks


        #region Settings
        [HttpGet]
        public IActionResult Settings()
        {

            var user = _User;
            var settings = new SettingsModel
            {
                Name = user.Name,
                Surname = user.Surname,
            };
            ViewBag.Professions = new SelectList(_context.Professions.ToArray(), "ID", "Name", _User.Profession.ID);

            return View(settings);
        }

        [HttpPost]
        public IActionResult Settings(SettingsModel settings, int ProfessionId)
        {
            if (ProfessionId < 1)
            {
                ModelState.AddModelError("", "Не выбрана профессия");
            }
            else if (ModelState.IsValid)
            {

                _User.Name = settings.Name;
                _User.Surname = settings.Surname;
                _User.Profession = _context.Professions.Find(ProfessionId);

                _context.SaveChanges();

            }

            ViewBag.Professions = new SelectList(_context.Professions.ToArray(), "ID", "Name", _User.Profession.ID);

            return View(settings);


        }

        #endregion Settings
    }
}

