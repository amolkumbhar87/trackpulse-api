// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User>           Users           { get; set; }
    public DbSet<City>           Cities          { get; set; }
    public DbSet<Venue>          Venues          { get; set; }
    public DbSet<Jockey>         Jockeys         { get; set; }
    public DbSet<Horse>          Horses          { get; set; }
    public DbSet<RaceDay>        RaceDays        { get; set; }
    public DbSet<Race>           Races           { get; set; }
    public DbSet<RaceHorse>      RaceHorses      { get; set; }
    public DbSet<Odds>           Odds            { get; set; }
    public DbSet<Bet>            Bets            { get; set; }
    public DbSet<BetTransaction> BetTransactions { get; set; }
    public DbSet<RaceResult>     RaceResults     { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique constraints
        modelBuilder.Entity<RaceDay>()
            .HasIndex(rd => new { rd.VenueId, rd.RaceDate }).IsUnique();

        modelBuilder.Entity<Race>()
            .HasIndex(r => new { r.RaceDayId, r.RaceNumber }).IsUnique();

        modelBuilder.Entity<RaceHorse>()
            .HasIndex(rh => new { rh.RaceId, rh.HorseId }).IsUnique();

        modelBuilder.Entity<RaceResult>()
            .HasIndex(rr => new { rr.RaceId, rr.RaceHorseId }).IsUnique();

        modelBuilder.Entity<RaceResult>()
            .HasIndex(rr => new { rr.RaceId, rr.FinishPosition }).IsUnique();

        // Decimal precision
        modelBuilder.Entity<Odds>()
            .Property(o => o.WinOdds).HasPrecision(6, 2);
        modelBuilder.Entity<Odds>()
            .Property(o => o.PlaceOdds).HasPrecision(6, 2);
        modelBuilder.Entity<Bet>()
            .Property(b => b.Amount).HasPrecision(10, 2);
        modelBuilder.Entity<Bet>()
            .Property(b => b.OddsAtBet).HasPrecision(6, 2);
        modelBuilder.Entity<BetTransaction>()
            .Property(bt => bt.Amount).HasPrecision(10, 2);
    }
}