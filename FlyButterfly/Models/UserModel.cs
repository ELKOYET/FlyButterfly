using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlyButterfly.Models
{
    public class UserModel
    {
        [Key, Column(Order = 1)]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }


        public string Login { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public virtual RoleModel Role { get; set; }
        public virtual ProfessionModel Profession { get; set; }
        public virtual List<ProjectsModel> Projects { get; set; }


    }
}
