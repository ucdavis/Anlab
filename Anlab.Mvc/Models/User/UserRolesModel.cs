using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;

namespace AnlabMvc.Models.User
{
    public class UserRolesModel
    {
        public Anlab.Core.Domain.User User { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsUser { get; set; }
    }
}
