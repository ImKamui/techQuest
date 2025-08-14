using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }

        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
    }
}
