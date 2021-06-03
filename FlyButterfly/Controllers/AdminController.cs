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
        public  IActionResult DeleteProfession(int id)
        {
            ProfessionModel pr = _context.Professions.FirstOrDefault(x => x.ID == id);
            if (_context.Users.FirstOrDefault(x => x.Profession == pr ) != null)
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
            if (_context.Users.FirstOrDefault(x => x.Projects[id] == pr) != null)
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
        public IActionResult Risks(int id)
        {
            ViewBag.ProjectId = id;
            ViewBag.Users = _context.Users.ToArray();
            ViewBag.Risks = _context.RiskModels.ToArray();
            ViewBag.Influences = _context.RiskInfluences.ToArray();
            ViewBag.Chances = _context.RiskChances.ToArray();
            ViewBag.Reactions = _context.RiskReactions.ToArray();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Risks(RiskModel risk, int ProjectId, bool edit)
        {
            ProjectsModel pr = await _context.Projects.FindAsync( ProjectId);

            if (ModelState.IsValid)
            {
                if (pr != null)
                {
                    if (!( risk.Name != null && risk.Discription != null && risk.Influence != null && risk.Chance != null && risk.Reaction != null && risk.User != null) )
                    {
                        ModelState.AddModelError("","Заполнены не все поля");
                        ViewBag.Users = _context.Users.ToArray();
                        ViewBag.Risks = _context.RiskModels.ToArray();
                        ViewBag.Influences = _context.RiskInfluences.ToArray();
                        ViewBag.Chances = _context.RiskChances.ToArray();
                        ViewBag.Reactions = _context.RiskReactions.ToArray();
                        return View();

                    }

                    if (edit )
                    {
                       var rs = pr.Risks.FirstOrDefault(x => x.ID == risk.ID);
                        rs.Name = risk.Name;
                        rs.Reaction = risk.Reaction;
                        rs.Influence = risk.Influence;
                        rs.Chance = risk.Chance;
                        rs.User = risk.User;
                        rs.Discription = risk.Discription;
                    }
                    else 
                    {
                        _context.RiskModels.Add(risk);
                    }
                    _context.SaveChanges();
                }
                else
                {
                    return NotFound(); 
                }
            }

            ViewBag.Users = _context.Users.ToArray();
            ViewBag.Risks = _context.RiskModels.ToArray();
            ViewBag.Influences = _context.RiskInfluences.ToArray();
            ViewBag.Chances = _context.RiskChances.ToArray();
            ViewBag.Reactions = _context.RiskReactions.ToArray();
            return View();
        }

        #endregion
        ////Статусы проектов
        //#region ProjectStatuses
        //[HttpGet]
        //public IActionResult ProjectStatuses(int? errorCode)
        //{
        //    switch (errorCode)
        //    {
        //        case 1: { ModelState.AddModelError("", "Есть проекты с таким статусом"); break; }
        //        default: break;
        //    }

        //    ViewBag.ProjectStatuses = _context.Project_Statuses.OrderBy(x => x.Name).ToArray();
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult ProjectStatuses(Project_Status status)
        //{
        //    Project_Status st = _context.Project_Statuses.FirstOrDefault(x => x.Name == status.Name);

        //    if (ModelState.IsValid)
        //    {
        //        if (st == null)
        //        {
        //            _context.Project_Statuses.Add(status);
        //            _context.SaveChanges();
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Уже существует :)");
        //        }
        //    }

        //    ViewBag.ProjectStatuses = _context.Project_Statuses.OrderBy(x => x.Name).ToArray();
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> DeleteProjectStatus(int id)
        //{
        //    var status = await CRUD.Find(_context.Project_Statuses, id);
        //    if (status.Name == "Created" || status.Name == "Done") return RedirectToAction("ProjectStatuses");


        //    if (_context.Projects.FirstOrDefault(x => x.Status.Id == id) != null)
        //    {
        //        return RedirectToAction("ProjectStatuses", new { errorCode = 1 });
        //    }

        //    Project_Status st = _context.Project_Statuses.FirstOrDefault(x => x.Id == id);
        //    _context.Project_Statuses.Remove(st);
        //    _context.SaveChanges();

        //    return RedirectToAction("ProjectStatuses");
        //}
        //#endregion ProjectStatuses

        ////Статусы задач
        //#region TaskStatuses
        //[HttpGet]
        //public IActionResult TaskStatuses(int? errorCode)
        //{
        //    switch (errorCode)
        //    {
        //        case 1: { ModelState.AddModelError("", "Есть задачи с таким статусом"); break; }
        //        default: break;
        //    }
        //    ViewBag.TaskStatuses = _context.Task_Statuses.OrderBy(x => x.Name).ToArray();
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> TaskStatuses(Task_Status status)
        //{
        //    Task_Status st = await CRUD.First(_context.Task_Statuses, x => x.Name == status.Name);

        //    if (ModelState.IsValid)
        //    {
        //        if (st == null)
        //        {
        //            CRUD.Create(status, _context.Task_Statuses);
        //            _context.SaveChanges();
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Уже существует :)");
        //        }
        //    }

        //    ViewBag.TaskStatuses = _context.Task_Statuses.OrderBy(x => x.Name).ToArray();
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> DeleteTaskStatus(int id)
        //{
        //    var status = await CRUD.Find(_context.Task_Statuses, id);
        //    if (status.Name == "Created" || status.Name == "Done") return RedirectToAction("TaskStatuses");


        //    if (_context.Project_Tasks.FirstOrDefault(x => x.Status.Id == id) != null)
        //    {
        //        return RedirectToAction("TaskStatuses", new { errorCode = 1 });
        //    }

        //    Task_Status st = await CRUD.Find(_context.Task_Statuses, id);
        //    CRUD.Remove(st, _context.Task_Statuses);
        //    _context.Task_Statuses.Remove(st);
        //    _context.SaveChanges();

        //    return RedirectToAction("TaskStatuses");
        //}
        //#endregion TaskStatuses
        ////-------------------------

        ////Проекты
        //#region Projects
        //[HttpGet]
        //public IActionResult Projects(string projectName, int? StatusId)
        //{
        //    Project[] projects = null;
        //    if (!string.IsNullOrEmpty(projectName))
        //    {
        //        projects = _context.Projects.Where(x => x.Name.Contains(projectName)).
        //       OrderBy(x => x.Name).ToArray();
        //    }

        //    if (StatusId == null) StatusId = 0;

        //    projects = projects != null ? (StatusId != 0 ? projects.Where(x => x.Status.Id == StatusId).ToArray() : projects) : (StatusId != 0 ?
        //       _context.Projects.Where(x => x.Status.Id == StatusId).OrderBy(x => x.Name).ToArray() : _context.Projects.OrderBy(x => x.Name).ToArray());


        //    ViewBag.Statuses = new SelectList(_context.Project_Statuses.ToArray(), "Id", "Name");
        //    return View(projects);
        //}

        //// Добавление 
        //[HttpGet]
        //public IActionResult AddProject()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddProject(Project project)
        //{
        //    Project pr = _context.Projects.FirstOrDefault(x => x.Name == project.Name);

        //    if (ModelState.IsValid)
        //    {
        //        if (pr == null)
        //        {
        //            project.Status = await CRUD.First(_context.Project_Statuses, x => x.Name == "Created") ?? null;
        //            _context.Projects.Add(project);
        //            _context.SaveChanges();
        //            return RedirectToAction("Projects");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Уже существует :)");
        //        }
        //    }

        //    return View();

        //}


        //#endregion Projects

        ////Проект (редактирование)
        //#region EditProject

        //[HttpGet]
        //public async Task<IActionResult> Project(int id)
        //{
        //    var pr = await CRUD.Find(_context.Projects, id);
        //    if (pr == null) return NotFound();
        //    return View(pr);
        //}

        //[HttpGet]
        //public async Task<IActionResult> EditProject(int id)
        //{
        //    var pr = await CRUD.Find(_context.Projects, id);
        //    if (pr == null) return NotFound();

        //    ViewBag.Statuses = new SelectList(_context.Project_Statuses.ToArray(), "Id", "Name", pr.Status.Id);
        //    return View(pr);
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditProject(Project project, int StatusId)
        //{
        //    Project pr = _context.Projects.FirstOrDefault(x => x.Id == project.Id);

        //    if (ModelState.IsValid)
        //    {
        //        if (pr != null)
        //        {
        //            Project other = _context.Projects.FirstOrDefault(x => x.Name == project.Name) ?? null;
        //            if (other != null && other.Id != pr.Id)
        //            {
        //                ModelState.AddModelError("", "Выбери другое название :)");
        //                return View();
        //            }

        //            pr.Name = project.Name;
        //            pr.Description = project.Description;
        //            pr.StartDate = project.StartDate;
        //            pr.EndDate = project.EndDate;
        //            var status = await CRUD.Find(_context.Project_Statuses, StatusId);
        //            if (status != null) pr.Status = status;

        //            _context.Projects.Update(pr);
        //            _context.SaveChanges();
        //            return RedirectToAction("Project", new { id = pr.Id });
        //        }
        //    }

        //    ViewBag.Statuses = new SelectList(_context.Project_Statuses.ToArray(), "Id", "Name", pr.Status.Id);

        //    return View();

        //}


        //#endregion EditProject

        ////Команда проекта
        //#region ProjectTeam

        //[HttpGet]
        //public async Task<IActionResult> ProjectTeam(int id, int? ProfessionId, int? errorCode)
        //{
        //    switch (errorCode)
        //    {
        //        case 1: { ModelState.AddModelError("", "Этот исполнитель назначен на задачи"); break; }
        //        default: break;
        //    }

        //    if (ProfessionId == null) ProfessionId = 0;

        //    Profession prof = null;
        //    if (ProfessionId != 0) prof = await CRUD.Find(_context.Professions, (int)ProfessionId);

        //    Project proj = _context.Projects.FirstOrDefault(x => x.Id == id);

        //    ViewBag.AvaliableWorkers = ProfessionId != 0 ? _context.Users.ToList().Where(x => x.Professions.Contains(prof)).Except(proj.Project_Workers.Select(x => x.Worker)) : _context.Users.ToList().Except(proj.Project_Workers.Select(x => x.Worker));

        //    ViewBag.Professions = new SelectList(_context.Professions.ToArray(), "Id", "Name");

        //    return View(proj);
        //}

        //[HttpPost]
        //public async Task<IActionResult> ProjectTeam(int project_id, int worker_id)
        //{
        //    var proj = await CRUD.Find(_context.Projects, project_id);
        //    var worker = await CRUD.First(_context.Users, x => x.Id == worker_id);

        //    proj.Project_Workers.Add(new Project_Worker { Worker = worker, Project = proj });
        //    _context.SaveChanges();

        //    return RedirectToAction("ProjectTeam", new { id = project_id });
        //}

        //[HttpPost]
        //public async Task<IActionResult> ProjectTeamRemove(int project_id, int worker_id)
        //{
        //    var proj = await CRUD.Find(_context.Projects, project_id);
        //    var worker = await CRUD.First(_context.Project_Workers, x => x.Project_Id == project_id && x.Worker_Id == worker_id);

        //    if (proj == null) return NotFound();

        //    if (worker != null)
        //    {
        //        if (proj.Tasks.FirstOrDefault(x => x.Project_Worker == worker) != null)
        //        {
        //            return RedirectToAction("ProjectTeam", new { id = project_id, errorCode = 1 });
        //        }
        //        else
        //        {
        //            proj.Project_Workers.Remove(worker);
        //            _context.SaveChanges();
        //            return RedirectToAction("ProjectTeam", new { id = project_id });
        //        }
        //    }

        //    return RedirectToAction("ProjectTeam", new { id = project_id });

        //}


        //#endregion ProjectTeam

        ////Задачи по проекту
        //#region ProjectTasks

        //[HttpGet]
        //public async Task<IActionResult> ProjectTasks(int id, string TaskName, int? StatusId)
        //{
        //    var pr = await CRUD.Find(_context.Projects, id);
        //    if (pr == null) return NotFound();

        //    if (StatusId == null) StatusId = 0;

        //    Project_Task[] tasks = null;
        //    if (!string.IsNullOrEmpty(TaskName))
        //    {
        //        tasks = pr.Tasks.Where(x => x.Name.ToLower().Contains(TaskName.ToLower())).
        //       OrderBy(x => x.Name).ToArray();
        //    }

        //    tasks = tasks != null ? (StatusId != 0 ? tasks.Where(x => x.Status.Id == StatusId).ToArray() : tasks) : (StatusId != 0 ?
        //        pr.Tasks.Where(x => x.Status.Id == StatusId).OrderBy(x => x.Name).ToArray() : pr.Tasks.OrderBy(x => x.Name).ToArray());


        //    ViewBag.Statuses = new SelectList(_context.Task_Statuses.ToArray(), "Id", "Name");

        //    ViewBag.projectId = id;

        //    return View(tasks);
        //}

        //[HttpGet]
        //public async Task<IActionResult> ProjectTask(int id)
        //{
        //    return View(await CRUD.Find(_context.Project_Tasks, id));
        //}



        //[HttpGet]
        //public async Task<IActionResult> AddTask(int projectId)
        //{
        //    var pr = await CRUD.Find(_context.Projects, projectId);
        //    if (pr == null) return NotFound();

        //    ViewBag.projectId = projectId;
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddTask(Project_Task task, int projectId)
        //{
        //    var pr = await CRUD.Find(_context.Projects, projectId);
        //    if (pr == null) return NotFound();

        //    ViewBag.projectId = projectId;

        //    if (ModelState.IsValid)
        //    {
        //        Project_Task tsk = await CRUD.First(_context.Project_Tasks, x => x.Name == task.Name);
        //        if (tsk == null)
        //        {
        //            task.Status = await CRUD.First(_context.Task_Statuses, x => x.Name == "Created") ?? null;

        //            pr.Tasks.Add(task);
        //            _context.SaveChanges();
        //            return RedirectToAction("ProjectTask", new { id = task.Id });
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Уже существует :)");
        //        }

        //    }
        //    return View();

        //}


        //[HttpGet]
        //public async Task<IActionResult> EditTask(int id, string WorkerName, int? ProfessionId)
        //{
        //    var task = await CRUD.Find(_context.Project_Tasks, id);
        //    if (task == null) return NotFound();

        //    if (ProfessionId == null) ProfessionId = 0;

        //    Project_Worker[] workers = null;
        //    if (!string.IsNullOrEmpty(WorkerName))
        //    {
        //        workers = task.Project.Project_Workers.Where(
        //            x => (x.Worker.Name != null ? x.Worker.Name.ToLower().Contains(WorkerName.ToLower()) : false) ||
        //            (x.Worker.Surname != null ? x.Worker.Surname.ToLower().Contains(WorkerName.ToLower()) : false)).
        //            OrderBy(x => x.Worker.Name).ToArray();
        //    }

        //    workers = workers != null ? (ProfessionId != 0 ? workers.Where(x => x.Worker.Professions.Contains(_context.Professions.Find(ProfessionId))).ToArray() : workers) :
        //        (ProfessionId != 0 ? task.Project.Project_Workers.Where(x => x.Worker.Professions.Contains(_context.Professions.Find(ProfessionId))).OrderBy(x => x.Worker.Name).ToArray() :
        //        task.Project.Project_Workers.OrderBy(x => x.Worker.Name).ToArray());

        //    ViewBag.Professions = new SelectList(_context.Professions.ToArray(), "Id", "Name");

        //    ViewBag.Workers = workers.Select(x => x.Worker);

        //    ViewBag.Statuses = new SelectList(_context.Task_Statuses.ToArray(), "Id", "Name", task.Status.Id);

        //    //ViewBag.Workers =;
        //    return View(task);
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditTask(Project_Task task, int WorkerId, int StatusId)
        //{
        //    Project_Task tsk = await CRUD.Find(_context.Project_Tasks, task.Id);
        //    if (ModelState.IsValid)
        //    {
        //        if (tsk != null)
        //        {
        //            var t = await CRUD.First(_context.Project_Tasks, x => x.Name == task.Name);
        //            if (t == null || t.Id == task.Id)
        //            {
        //                tsk.Name = task.Name;
        //                tsk.Description = task.Description;
        //                var status = await CRUD.Find(_context.Task_Statuses, StatusId);
        //                if (status != null) tsk.Status = status;
        //                tsk.WorkHours = task.WorkHours;
        //                tsk.Project_Worker = tsk.Project.Project_Workers.Find(x => x.Worker_Id == WorkerId) ?? null;

        //                CRUD.Update(tsk, _context.Project_Tasks);
        //                _context.SaveChanges();
        //                return RedirectToAction("ProjectTasks", new { id = tsk.Project.Id });
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Возможно такая задача уже существует :)");
        //            }
        //        }
        //    }

        //    ViewBag.Professions = new SelectList(_context.Professions.ToArray(), "Id", "Name");
        //    ViewBag.Workers = tsk.Project.Project_Workers.Select(x => x.Worker).OrderBy(x => x.Name);
        //    ViewBag.Statuses = new SelectList(_context.Task_Statuses.ToArray(), "Id", "Name", tsk.Status.Id);

        //    return View(tsk);


        //}

        //#endregion ProjectTasks

        ////Статистика
        //#region ProjectsStatistic

        //[HttpGet]
        //public IActionResult ReportConfigure()
        //{
        //    return View(_context.Projects.ToArray());
        //}

        //[HttpPost]
        //public IActionResult ReportConfigure(DateTime start, DateTime end, bool fullReport)
        //{


        //    if (end < start)
        //    {
        //        ModelState.Clear();
        //        ModelState.AddModelError("", "Начало проекта позже конца!");
        //        return View(_context.Projects.ToArray());
        //    }

        //    Project[] projects = _context.Projects.Where(x => x.StartDate >= start && x.EndDate <= end).ToArray();

        //    if (projects.Length == 0) return View(_context.Projects.ToArray());


        //    Dictionary<string, int> pc = new Dictionary<string, int>();
        //    for (int i = 0; i < projects.Length; i++)
        //    {
        //        int sum = 0;
        //        for (int j = 0; j < projects[i].Tasks.Count; j++)
        //        {
        //            var tsk = projects[i].Tasks[j];
        //            if (tsk.Project_Worker == null || tsk.WorkHours == 0) continue;
        //            sum += tsk.Project_Worker.Worker.CostPerHour * (int)tsk.WorkHours;
        //        }
        //        pc.Add(projects[i].Name, sum);
        //    }

        //    var statuses = projects.Select(x => x.Status.Name).ToArray();

        //    ReportModel report = new ReportModel();

        //    report.start = start;
        //    report.end = end;
        //    report.projectsCosts = pc.Values.ToArray();
        //    report.projectsNames = pc.Keys.ToArray();
        //    report.projectsStatuses = statuses;
        //    report.fullReport = fullReport;

        //    if (report.projectsCosts != null || report.projectsNames != null || report.projectsStatuses != null)
        //        return RedirectToAction("ReportView", report);

        //    else return View(_context.Projects.ToArray());
        //}

        //[HttpGet]
        //public IActionResult ReportView(ReportModel report)
        //{
        //    return View(report);
        //}
        //#endregion 


        //// Сделать в меню админа кнопку все исполнители, а на страничке - пару полезных кнопок (Получить адреса всех исполнителей (например для отправки на почту всем)) Мб* // Мб добавить разного рода поиск туда же (по зарплате ...)


    }
}
