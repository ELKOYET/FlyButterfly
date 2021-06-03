using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyButterfly.Models
{
    public class RiskModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public virtual UserModel User { get; set; }
        public virtual RiskChanceModel Chance { get; set; }
        public virtual RiskInfluenceModel Influence { get; set; }
        public virtual ProjectsModel Project { get; set; }
        public virtual RiskReaction Reaction { get; set; }
    }
}
