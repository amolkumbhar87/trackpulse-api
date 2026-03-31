public interface IVenueRepository
{
    Task<IEnumerable<Venue>> GetAllAsync();
}