using Data.Contexts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProjectRepository
    {
        private readonly ApplicationContext _context;

        public ProjectRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Project> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects .Include(p => p.ProjectManager) .ToListAsync();
        }
        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Project>> GetFilteredProjectsAsync(
            string name,
            DateTime? startDateFrom,
            DateTime? startDateTo,
            int? priority,
            string sortField,
            bool sortDescending)
        {
            var query = _context.Projects
                .Include(p => p.ProjectManager)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            if (startDateFrom.HasValue)
                query = query.Where(p => p.StartDate >= startDateFrom.Value);

            if (startDateTo.HasValue)
                query = query.Where(p => p.StartDate <= startDateTo.Value);

            if (priority.HasValue)
                query = query.Where(p => p.Priority == priority.Value);

            query = sortField switch
            {
                "StartDate" => sortDescending
                    ? query.OrderByDescending(p => p.StartDate)
                    : query.OrderBy(p => p.StartDate),
                "Priority" => sortDescending
                    ? query.OrderByDescending(p => p.Priority)
                    : query.OrderBy(p => p.Priority),
                _ => sortDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name)
            };

            return await query.ToListAsync();
        }
    }
}
