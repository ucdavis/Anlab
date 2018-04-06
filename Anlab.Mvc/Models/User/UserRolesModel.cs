using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Models.Roles;

namespace AnlabMvc.Models.User
{
    public class UserRolesModel
    {
        public Anlab.Core.Domain.User User { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLabUser { get; set; }
        public bool IsReports { get; set; }
    }

    public class EditAdminModel
    {
        public Anlab.Core.Domain.User User { get; set; }
        public string RoleCode { get; set; }
        public bool IsActive { get; set; }
    }
}
