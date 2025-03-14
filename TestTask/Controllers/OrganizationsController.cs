using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Models;
using TestTask.Models.Entities;

namespace TestTask.Controllers
{
    public class OrganizationsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public OrganizationsController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(OrganizationViewModel organizationModel)
        {
            if (!ModelState.IsValid)
            {
                return View(organizationModel);
            }

            var organization = new Organization
            {
                INN = organizationModel.INN,
                Name = organizationModel.Name,
                Phone = organizationModel.Phone,
                Email = organizationModel.Email,
            };

            await _dbContext.Organizations.AddAsync(organization);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
                {
                    ModelState.AddModelError("INN", "Организация с таким ИНН уже существует");
                    return View(organizationModel);
                }
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var organizations = await _dbContext.Organizations.ToListAsync();

            return View(organizations);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
                return View();
            var organization = await _dbContext.Organizations.FindAsync(id);
            return View(organization);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Organization organizationModel)
        {
            var organization = await _dbContext.Organizations.FindAsync(organizationModel.Id);
            if (organization is not null)
            {
                organization.Name = organizationModel.Name;
                organization.INN = organizationModel.INN;
                organization.Phone = organizationModel.Phone;
                organization.Email = organizationModel.Email;
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var organization = await _dbContext.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (organization is not null)
            {
                _dbContext.Remove(organization);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }
    }

}
