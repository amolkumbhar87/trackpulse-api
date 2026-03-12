public interface IHorseRepository
{
    Task<Horse> GetOrCreateAsync(string horseName, int? age, string? color, string? gender);
}