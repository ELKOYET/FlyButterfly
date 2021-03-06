using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyButterfly.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<ProjectsModel> Projects { get; set; }
        public DbSet<ProfessionModel> Professions { get; set; }
        public DbSet<RiskChanceModel> RiskChances { get; set; }
        public DbSet<RiskInfluenceModel> RiskInfluences { get; set; }
        public DbSet<RiskReaction> RiskReactions { get; set; }
        public DbSet<RiskModel> RiskModels { get; set; } 


        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminLogin = "admin";
            string adminPassword = "admin";




            // создаем роли
            RoleModel adminRole = new RoleModel { ID = 1, Name = adminRoleName };
            RoleModel userRole = new RoleModel { ID = 2, Name = userRoleName };

            // и юзера
            UserModel adminUser = new UserModel { ID = 1, Login = adminLogin, Password =Crypto.SHA1( adminPassword), RoleId = adminRole.ID, Name = "Кирилл5к" };

            modelBuilder.Entity<RoleModel>().HasData(new RoleModel[] { adminRole, userRole });
            modelBuilder.Entity<UserModel>().HasData(new UserModel[] { adminUser });


            


            // ------------------------------------------------------------------------

           


            base.OnModelCreating(modelBuilder);
        }
    }
}
