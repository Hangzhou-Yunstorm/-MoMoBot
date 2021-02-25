using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.Models.KnowledgeMapModel;

namespace MoMoBot.Infrastructure.Database
{
    public class MoMoDbContext : IdentityDbContext<MoMoBotUser>
    {
        public DbSet<QandA> Answers { get; set; }
        public DbSet<QueryParameter> QueryParameters { get; set; }
        public DbSet<AnswerQueries> AnswerQueries { get; set; }
        public DbSet<Unknown> Unknowns { get; set; }
        public DbSet<FeedbackInfo> FeedbackInfos { get; set; }
        public DbSet<ServiceRecord> ServiceRecords { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<QAPermission> QAPermissions { get; set; }
        public DbSet<Modular> Modulars { get; set; }
        public DbSet<ModularPermission> ModularPermissions { get; set; }
        public DbSet<HotIntent> HotIntents { get; set; }
        public DbSet<Voice> Voices { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatRecord> ChatRecords { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Metadata> MetadataSet { get; set; }

        public DbSet<KnowledgeMap> KnowledgeMaps { get; set; }
        //public DbSet<MapBranch> Branches { get; set; }
        public DbSet<MapStep> Steps { get; set; }

        public MoMoDbContext(DbContextOptions<MoMoDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Department>()
                .HasOne(o => o.Parent)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AnswerQueries>()
                .HasKey(q => new { q.AnswerId, q.ParameterId });

            builder.Entity<QAPermission>()
                .HasKey(p => new { p.DepartmentId, p.QAId });

            builder.Entity<ModularPermission>()
                .HasKey(p => new { p.DepartmentId, p.ModularId });

        }
    }
}
