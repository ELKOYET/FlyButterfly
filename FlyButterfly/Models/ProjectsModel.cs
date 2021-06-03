using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlyButterfly.Models
{
    public class ProjectsModel
    {
        public int ID { get; set; }
        public string Name { get; set; }


        [Required(ErrorMessage = "Не введена дата начала")]
        public DateTime Start_Date { get; set; }

        [Required(ErrorMessage = "Не введена дата окончания")]
        public DateTime End_Date { get; set; }

        public virtual List<UserModel> Users { get; set; }
        public virtual List<RiskModel> Risks { get; set; }

    }

}
