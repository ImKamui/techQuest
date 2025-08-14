using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> GetByIdAsync(int id);
        Task<IEnumerable<Project>> GetAllAsync();
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(int id);
        Task<IEnumerable<Project>> GetFilteredProjectsAsync(string name, DateTime? startDateFrom,
            DateTime? startDateTo, int? priority, string sortField, bool sortDescending);
    }
}
