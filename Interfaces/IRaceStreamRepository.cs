public interface IRaceStreamRepository
{
    Task<bool> UpdateStreamUrlAsync(int raceId, string streamUrl);
    Task<string> GetStreamUrlAsync(int raceId);
    Task<List<RaceStream>> GetAllActiveStreamsAsync();
    Task<bool> DeactivateStreamAsync(int raceId);
    Task<RaceStream> GetStreamByIdAsync(int id);
    Task<bool> DeleteStreamAsync(int raceId);


    //Task<IEnumerable<RaceStreamDto>> GetStreamsByRaceIdsAsync(IEnumerable<int> raceIds);
}