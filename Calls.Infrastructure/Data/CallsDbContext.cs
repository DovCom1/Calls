using Microsoft.EntityFrameworkCore;
using Calls.Domain.Rooms;

namespace Calls.Infrastructure.Data;

public class CallsDbContext : DbContext
{
    public CallsDbContext(DbContextOptions<CallsDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.RoomId);
            entity.Property(r => r.RoomId).ValueGeneratedNever();
            entity.Property(r => r.Name).IsRequired().HasMaxLength(500);
            
            entity.OwnsMany(r => r.Participants, participant =>
            {
                participant.ToTable("RoomParticipants");
                participant.HasKey("RoomId", "UserId");
                participant.Property(p => p.UserId).IsRequired();
                
                participant.OwnsOne(p => p.Settings);
                
                participant.OwnsOne(p => p.SmallUserInfo);
            });
            
            entity.Navigation(r => r.Participants)
                .HasField("_participants")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}

