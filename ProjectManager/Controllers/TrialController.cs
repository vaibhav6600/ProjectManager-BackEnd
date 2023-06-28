using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Context;
using ProjectManager.DTOs;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrialController : ControllerBase
    {
        ProjectDBContext _context;
        public TrialController(ProjectDBContext dBContext) 
        {
            _context = dBContext;
        }

        [HttpPost]
        [Route("addproject")]
        public async Task<ActionResult<IEnumerable<Project>>> addproject([FromBody]Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
            return Ok(project);
        }

        [HttpGet]
        [Route("listproject")]
        public async Task<ActionResult<IEnumerable<Project>>> ListProjects()
        {
            return Ok(_context.Projects.Include(p => p.Manager).ToList());
        }

        [HttpPost]
        [Route("emailpassword")]
        public async Task<ActionResult<IEnumerable<Employee>>> EmailPassword([FromBody]EmployeeDTO employee)
        {
            return Ok(_context.Employees.Where(e => e.Email == employee.Email && e.Password == employee.Password));
        }

        [HttpPost]
        [Route("addtask")]
        public async Task<ActionResult<IEnumerable<Projecttask>>> AddTask([FromBody]Projecttask projecttask)
        {
            _context.Projecttasks.Add(projecttask);
            _context.SaveChanges();
            return Ok(projecttask);
        }

        [HttpGet]
        [Route("listtasks")]
        public async Task<ActionResult<IEnumerable<Project>>> ListTask()
        {
            return Ok(_context.Projects.Include(p => p.Projecttasks).ToList());
        }

        [HttpPut]
        [Route("updatemanager/{id}/{managerid}")]
        public async Task<ActionResult<IEnumerable<Project>>> UpdateManager([FromRoute] int id, [FromRoute]int managerid)
        {
            Project project = await _context.Projects.FindAsync(id);
            if(id != null)
            {
                project.Managerid = managerid;
                _context.Entry(project).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return Ok(project);
        }

        [HttpGet]
        [Route("taskbyprojectid/{id}")]
        public async Task<ActionResult<IEnumerable<Project>>> TaskByProjectId([FromRoute]int id)
        {
            Project project = await _context.Projects.FindAsync(id);
            if(project != null)
            {
                _context.Projecttasks.Include(p => p.Project).Where(p => p.Projectid == id).ToList(); 
            }
            return Ok(project);
        }

        [HttpGet]
        [Route("taskbyemployeeid/{id}")]
        public async Task<ActionResult<IEnumerable<Project>>> TaskByEmployeeId([FromRoute] int id)
        {
            Project project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projecttasks.Include(p => p.Project).Where(p => p.Employeeid == id).ToList();
            }
            return Ok(project);
        }

        [HttpDelete]
        [Route("deletetask/{id}")]
        public async Task<ActionResult<IEnumerable<Projecttask>>> DeleteTask([FromRoute]int id)
        {
            Projecttask projecttask = await _context.Projecttasks.FindAsync(id);
            if(projecttask != null)
            {
                _context.Projecttasks.Remove(projecttask);
                _context.SaveChangesAsync();

            }
            return Ok(projecttask);
        }

        [HttpDelete]
        [Route("deleteproject/{id}")]
        public async Task<ActionResult<IEnumerable<Project>>> DeleteProject([FromRoute] int id)
        {
            List<Projecttask> projecttasks = _context.Projecttasks.Where(t => t.Projectid == id).ToList();
            List<Projectmodule> projectmodules = _context.Projectmodules.Where(t => t.Projectid == id).ToList();
            foreach(Projecttask projecttask in projecttasks)
            {
                _context.Projecttasks.Remove(projecttask);
            }
            foreach(Projectmodule projectmodule in projectmodules)
            {
                _context.Projectmodules.Remove(projectmodule);
            }
            Project project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            _context.SaveChangesAsync();
            return Ok(project);
        }
    }
}
