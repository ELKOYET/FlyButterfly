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
        public DbSet<ChanceModel> RiskChances { get; set; }
        public DbSet<InfluenceModel> RiskInfluences { get; set; }
        public DbSet<ReactionModel> RiskReactions { get; set; }
        public DbSet<RiskModel> RiskModels { get; set; }
        public DbSet<RiskTypeModel> RiskTypes { get; set; }


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
            UserModel adminUser = new UserModel { ID = 1, Login = adminLogin, Password = Crypto.SHA1(adminPassword), RoleId = adminRole.ID, Name = "Кирилл5к" };

            modelBuilder.Entity<RoleModel>().HasData(new RoleModel[] { adminRole, userRole });
            modelBuilder.Entity<UserModel>().HasData(new UserModel[] { adminUser });


            modelBuilder.Entity<ChanceModel>().HasData(new ChanceModel[] {
                new ChanceModel { ID = 1, Name="Низкая" },
                new ChanceModel { ID = 2, Name = "Ниже среднего" },
                new ChanceModel { ID = 3, Name = "Средняя" },
                new ChanceModel { ID = 4, Name = "Выше среднего" },
                new ChanceModel { ID = 5, Name = "Высокая" },
            });

            modelBuilder.Entity<InfluenceModel>().HasData(new InfluenceModel[] {
                new InfluenceModel { ID = 1, Name = "Низкое" },
                new InfluenceModel { ID = 2, Name = "Ниже среднего" },
                new InfluenceModel { ID = 3, Name = "Среднее" },
                new InfluenceModel { ID = 4, Name = "Выше среднего" },
                new InfluenceModel { ID = 5, Name = "Высокое" },

            });
            modelBuilder.Entity<ReactionModel>().HasData(new ReactionModel[] {
                new ReactionModel { ID = 1, Name = "Уклонение" },
                new ReactionModel { ID = 2, Name = "Передача" },
                new ReactionModel { ID = 3, Name = "Снижение" },
                new ReactionModel { ID = 4, Name = "Принятие пассивное" },
                new ReactionModel { ID = 5, Name = "Принятие активное" },
            });

            modelBuilder.Entity<RiskTypeModel>().HasData(new RiskTypeModel[] {
                new RiskTypeModel { ID = 1, Name = "Экономический" },
                new RiskTypeModel { ID = 2, Name = "Политический" },
            });

            // ------------------------------------------------------------------------




            base.OnModelCreating(modelBuilder);
        }
    }
}
