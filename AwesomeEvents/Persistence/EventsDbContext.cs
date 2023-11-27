using AwesomeEvents.Entities;
using Microsoft.EntityFrameworkCore;

namespace AwesomeEvents.Persistence;

public class EventsDbContext : DbContext
{
    public DbSet<Event> Events { get; set; }
    public DbSet<EventSpeaker> EventSpeakers { get; set; }

    public EventsDbContext(DbContextOptions<EventsDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Event>(entity =>
        {
            entity.HasKey(currentEvent => currentEvent.Id);

            entity.Property(currentEvent => currentEvent.Title).IsRequired(false);

            entity.Property(currentEvent => currentEvent.Description).HasMaxLength(200).HasColumnType("varchar(200)");

            entity.Property(currentEvent => currentEvent.StartDate).HasColumnName("Start_Date");
            
            entity.Property(currentEvent => currentEvent.EndDate).HasColumnName("End_Date");

            entity.HasMany(currentEvent => currentEvent.Speakers).WithOne().HasForeignKey(speaker => speaker.EventId);
        });

        builder.Entity<EventSpeaker>(entity =>
        {
            entity.HasKey(currentEvent => currentEvent.Id);
        });
    }
}
