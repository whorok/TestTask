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

        [HttpGet]
        public async Task<IActionResult> Upsert(Guid id)
        {
            if (id == Guid.Empty)
                return View();
            var organization = await _dbContext.Organizations.FindAsync(id);
            if (organization == null)
                return NotFound();

            var organizationViewModel = new OrganizationViewModel
            {
                Id = organization.Id,
                INN = organization.INN,
                Name = organization.Name,
                Phone = organization.Phone,
                Email = organization.Email,
            };
            return View(organizationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(OrganizationViewModel organizationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(organizationViewModel);
            }

            // Проверка уникальности ИНН (если требуется)
            if (await _dbContext.Organizations.AnyAsync(o =>
                    o.Id != organizationViewModel.Id && o.INN == organizationViewModel.INN))
            {
                ModelState.AddModelError("INN", "ИНН уже существует");
                return View(organizationViewModel);
            }

            // Выбор между созданием и обновлением
            if (organizationViewModel.Id == Guid.Empty)
                CreateOrganization(organizationViewModel);
            else
                await UpdateOrganization(organizationViewModel);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Save", "Ошибка сохранения данных");
            }
            catch
            {
                return NotFound();
            }

            return RedirectToAction("List");
        }

        private async Task UpdateOrganization(OrganizationViewModel organizationViewModel)
        {
            var organization = await _dbContext.Organizations.FindAsync(organizationViewModel.Id);
            if (organization is not null)
            {
                organization.INN = organizationViewModel.INN;
                organization.Name = organizationViewModel.Name;
                organization.Phone = organizationViewModel.Phone;
                organization.Email = organizationViewModel.Email;
            }
        }

        private void CreateOrganization(OrganizationViewModel organizationViewModel)
        {
            var organization = new Organization
            {
                INN = organizationViewModel.INN,
                Name = organizationViewModel.Name,
                Phone = organizationViewModel.Phone,
                Email = organizationViewModel.Email,
            };
            _dbContext.Organizations.Add(organization);
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            var organizations = await _dbContext.Organizations
                .Select(x => new OrganizationViewModel
                {
                    Id = x.Id,
                    INN = x.INN,
                    Name = x.Name,
                    Email = x.Email,
                })
                .ToListAsync();

            return View(organizations);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(List<Guid> selectedIds)
        {
            if (selectedIds.Count == 0)
                return RedirectToAction("List");

            var organizationsToDelete = _dbContext.Organizations.Where(o => selectedIds.Contains(o.Id));
            _dbContext.RemoveRange(organizationsToDelete);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }

        [HttpPost("Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var organization = await _dbContext.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (organization is null) return NotFound();

            _dbContext.Remove(organization);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }
    }
}