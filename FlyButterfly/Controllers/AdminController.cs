using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FlyButterfly.Models;

namespace FlyButterfly.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private UserModel _User { get { return _context.Users.FirstOrDefault(u => u.Login == User.Identity.Name); } }

        private ApplicationContext _context;

        public AdminController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult AdminPanel()
        {
            return View(_User);
        }

        //Профессии
        #region Professions
        [HttpGet]
        public IActionResult Professions(int? errorCode)
        {
            switch (errorCode)
            {
                case 1: { ModelState.AddModelError("", "Есть пользователи с такой профессией"); break; }
                default: break;
            }

            ViewBag.Professions = _context.Professions.OrderBy(x => x.Name).ToArray();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Professions(ProfessionModel profession)
        {
            ProfessionModel pr = await _context.Professions.FirstOrDefaultAsync(x => x.Name == profession.Name);

            if (ModelState.IsValid)
            {
                if (pr == null)
                {
                    _context.Professions.Add(profession);
                    _context.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Уже существует");
                }
            }

            ViewBag.Professions = _context.Professions.OrderBy(x => x.Name).ToArray();
            return View();
        }


        [HttpPost]
        public IActionResult DeleteProfession(int id)
        {
            ProfessionModel pr = _context.Professions.FirstOrDefault(x => x.ID == id);
            if (_context.Users.FirstOrDefault(x => x.Profession == pr) != null)
            {
                return RedirectToAction("Professions", new { errorCode = 1 });
            }


            _context.Professions.Remove(pr);
            _context.SaveChanges();

            return RedirectToAction("Professions");
        }
        #endregion Professions

        #region Projects
        [HttpGet]
        public IActionResult Projects()
        {
            ViewBag.Projects = _context.Projects.ToArray();
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Projects(ProjectsModel project)
        {
            ProjectsModel pr = await _context.Projects.FirstOrDefaultAsync(x => x.Name == project.Name);

            if (ModelState.IsValid)
            {
                if (pr == null)
                {
                    _context.Projects.Add(project);
                    _context.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Уже существует");
                }
            }

            ViewBag.Projects = _context.Projects.OrderBy(x => x.Name).ToArray();
            return View();
        }

        [HttpPost]
        public IActionResult DeleteProject(int id)
        {
            ProjectsModel pr = _context.Projects.FirstOrDefault(x => x.ID == id);

            if (pr.Users.Count > 0)
            {
                return RedirectToAction("Projects", new { errorCode = 1 });
            }


            _context.Projects.Remove(pr);
            _context.SaveChanges();

            return RedirectToAction("Projects");
        }
        #endregion

        #region Risks
        [HttpGet]
        public IActionResult Risks(int projectId)
        {
            if (_context.Projects.Find(projectId) == null)
                return NotFound();

            ViewBag.projectId = projectId;
            ViewBag.Influences = new SelectList(_context.RiskInfluences.ToArray(), "ID", "Name");
            ViewBag.Reactions = new SelectList(_context.RiskReactions.ToArray(), "ID", "Name");
            ViewBag.Chances = new SelectList(_context.RiskChances.ToArray(), "ID", "Name");
            ViewBag.Users = new SelectList(_context.Users.ToArray(), "ID", "Name");
            ViewBag.Types = new SelectList(_context.RiskTypes.ToArray(), "ID", "Name");

            ViewBag.Risks = _context.RiskModels.Where(x => x.Project.ID == projectId).ToArray();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Risks(RiskModel risk, bool edit, int projectId, int UserId,
            int InfluenceId, int ReactionId, int ChanceId, int TypeId)
        {
            var _Name = risk.Name;
            var _User = await _context.Users.FindAsync(UserId);
            var _Influence = await _context.RiskInfluences.FindAsync(InfluenceId);
            var _Reaction = await _context.RiskReactions.FindAsync(ReactionId);
            var _Chance = await _context.RiskChances.FindAsync(ChanceId);
            var _Type = await _context.RiskTypes.FindAsync(TypeId);
            var _Discription = risk.Discription;

            bool valid = !(_User == null || _Influence == null || _Reaction == null || _Chance == null || _Type == null || string.IsNullOrEmpty(_Discription) || string.IsNullOrEmpty(_Name));


            ProjectsModel pr = await _context.Projects.FindAsync(projectId);

            if (ModelState.IsValid)
            {
                if (pr != null)
                {
                    if (!valid)
                    {
                        ViewBag.projectId = projectId;
                        ViewBag.Influences = new SelectList(_context.RiskInfluences.ToArray(), "ID", "Name");
                        ViewBag.Reactions = new SelectList(_context.RiskReactions.ToArray(), "ID", "Name");
                        ViewBag.Chances = new SelectList(_context.RiskChances.ToArray(), "ID", "Name");
                        ViewBag.Users = new SelectList(_context.Users.ToArray(), "ID", "Name");
                        ViewBag.Types = new SelectList(_context.RiskTypes.ToArray(), "ID", "Name");

                        ViewBag.Risks = _context.RiskModels.Where(x => x.Project.ID == projectId).ToArray();
                        ModelState.AddModelError("", "Не все необходимые поля заполнены");
                        return View();
                    }
                    if (edit)
                    {
                        var rs = pr.Risks.FirstOrDefault(x => x.ID == risk.ID);
                        rs.Name = risk.Name;
                        rs.User = await _context.Users.FindAsync(UserId);
                        rs.Influence = await _context.RiskInfluences.FindAsync(InfluenceId);
                        rs.Reaction = await _context.RiskReactions.FindAsync(ReactionId);
                        rs.Chance = await _context.RiskChances.FindAsync(ChanceId);
                        rs.Type = await _context.RiskTypes.FindAsync(TypeId);
                        rs.Discription = risk.Discription;
                    }
                    else
                    {
                        var r = _context.RiskModels.FirstOrDefault(x => x.Project.ID==projectId && x.Name == risk.Name);
                        if (r == null)
                        {
                            var Rsk = new RiskModel()
                            {
                                Name = risk.Name,
                                Discription = risk.Discription,
                                User = await _context.Users.FindAsync(UserId),
                                Influence = await _context.RiskInfluences.FindAsync(InfluenceId),
                                Reaction = await _context.RiskReactions.FindAsync(ReactionId),
                                Chance = await _context.RiskChances.FindAsync(ChanceId),
                                Type = await _context.RiskTypes.FindAsync(TypeId),
                                Project = pr
                            };
                            pr.Risks.Add(Rsk);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Одноименный риск уже существует в проекте");
                        }
                    }

                    _context.SaveChanges();
                }


                else
                {
                    return NotFound();
                }
            }

            ViewBag.projectId = projectId;
            ViewBag.Influences = new SelectList(_context.RiskInfluences.ToArray(), "ID", "Name");
            ViewBag.Reactions = new SelectList(_context.RiskReactions.ToArray(), "ID", "Name");
            ViewBag.Chances = new SelectList(_context.RiskChances.ToArray(), "ID", "Name");
            ViewBag.Users = new SelectList(_context.Users.ToArray(), "ID", "Name");
            ViewBag.Types = new SelectList(_context.RiskTypes.ToArray(), "ID", "Name");

            ViewBag.Risks = _context.RiskModels.Where(x => x.Project.ID == projectId).ToArray();

            return View();
        }


        [HttpPost]
        public IActionResult DeleteRisk(int id)
        {
            RiskModel r = _context.RiskModels.Find(id);
            int prId = r.Project.ID;
            _context.RiskModels.Remove(r);
            _context.SaveChanges();

            return RedirectToAction("Risks", "Admin", new { projectId = prId });
        }
        #endregion Risks

    }
}
