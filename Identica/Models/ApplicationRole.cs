using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identica.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        {

        }

        public ApplicationRole(string roleName) : base(roleName)
        {

        }

        public ApplicationRole(string roleName,string description, DateTime createdDate) : base(roleName)
        {
            Description = description;
            CreatedDate = createdDate;
        }


        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    
}
