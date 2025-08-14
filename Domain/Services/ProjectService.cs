using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class ProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ProjectService(IProjectRepository projectRepository, IEmployeeRepository employeeRepository)
        {
            _projectRepository = projectRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Project> CreateProjectAsync(
            string name,
            string customerCompany,
            string executorCompany,
            DateTime startDate,
            DateTime endDate,
            int priority,
            int? projectManagerId,
            IEnumerable<int> employeeIds)
        {
            var project = new Project
            {
                Name = name,
                CustomerCompany = customerCompany,
                ExecutorCompany = executorCompany,
                StartDate = startDate,
                EndDate = endDate,
                Priority = priority
            };

            if (projectManagerId.HasValue)
            {
                var manager = await _employeeRepository.GetByIdAsync(projectManagerId.Value);
                if (manager == null)
                    throw new ArgumentException("Manager not found");
                project.ProjectManager = manager;
            }

            if (employeeIds != null && employeeIds.Any())
            {
                var employees = await _employeeRepository.GetAllAsync();
                project.Employees = employees.Where(e => employeeIds.Contains(e.Id)).ToList();
            }

            await _projectRepository.AddAsync(project);
            return project;
        }
        public async Task<Project> GetByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                throw new KeyNotFoundException("Проект не найден");
            return project;
        }
        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _projectRepository.GetAllAsync();
        }
        public async Task UpdateProjectAsync(
            int projectId,
            string name,
            string customerCompany,
            string executorCompany,
            DateTime startDate,
            DateTime endDate,
            int priority,
            int? projectManagerId,
            IEnumerable<int> employeeIds)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                throw new KeyNotFoundException("Проект не найден");

            // Обновление основных полей
            project.Name = name;
            project.CustomerCompany = customerCompany;
            project.ExecutorCompany = executorCompany;
            project.StartDate = startDate;
            project.EndDate = endDate;
            project.Priority = priority;

            // Обновление руководителя
            if (projectManagerId.HasValue)
            {
                var manager = await _employeeRepository.GetByIdAsync(projectManagerId.Value);
                if (manager == null)
                    throw new ArgumentException("Руководитель проекта не найден");
                project.ProjectManager = manager;
            }
            else
            {
                project.ProjectManager = null;
            }

            // Обновление сотрудников
            if (employeeIds != null)
            {
                var allEmployees = await _employeeRepository.GetAllAsync();
                project.Employees = allEmployees
                    .Where(e => employeeIds.Contains(e.Id))
                    .ToList();

                if (project.Employees.Count != employeeIds.Count())
                    throw new ArgumentException("Один или несколько сотрудников не найдены");
            }
            else
            {
                project.Employees.Clear();
            }

            await _projectRepository.UpdateAsync(project);
        }
        public async Task DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                throw new KeyNotFoundException("Проект не найден");

            await _projectRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Project>> GetFilteredProjectsAsync(
            string name,
            DateTime? startDateFrom,
            DateTime? startDateTo,
            int? priority,
            string sortField,
            bool sortDescending)
        {
            return await _projectRepository.GetFilteredProjectsAsync(
                name,
                startDateFrom,
                startDateTo,
                priority,
                sortField,
                sortDescending);
        }
        public async Task AddEmployeeToProjectAsync(int projectId, int employeeId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (project == null || employee == null)
                throw new KeyNotFoundException("Проект или сотрудник не найден");

            if (project.Employees.Any(e => e.Id == employeeId))
                throw new InvalidOperationException("Сотрудник уже участвует в проекте");

            project.Employees.Add(employee);
            await _projectRepository.UpdateAsync(project);
        }
        public async Task RemoveEmployeeFromProjectAsync(int projectId, int employeeId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            var employee = project?.Employees.FirstOrDefault(e => e.Id == employeeId);

            if (project == null || employee == null)
                throw new KeyNotFoundException("Проект или сотрудник не найден");

            project.Employees.Remove(employee);
            await _projectRepository.UpdateAsync(project);
        }
    }
}
