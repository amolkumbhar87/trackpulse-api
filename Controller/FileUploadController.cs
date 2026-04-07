using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IRaceRepository _raceRepository;
    private readonly IHorseRepository _horseRepository;
    private readonly IJockeyRepository _jockeyRepository;

    private readonly IRaceHorseRepository _raceHorseRepository;

    private readonly IRaceDayRepository _raceDayRepository;

    private readonly IVenueRepository _venueRepository;

    private readonly ITrainerRepository _trainerRepository;
    public FileUploadController(IRaceRepository raceRepository, IHorseRepository horseRepository, 
    IJockeyRepository jockeyRepository, IRaceHorseRepository raceHorseRepository,
    IRaceDayRepository raceDayRepository,
    IVenueRepository venueRepository, ITrainerRepository trainerRepository)
    {
        _raceRepository = raceRepository;
        _horseRepository = horseRepository;
        _jockeyRepository = jockeyRepository;
        _raceHorseRepository = raceHorseRepository;
        _raceDayRepository = raceDayRepository;
        _venueRepository = venueRepository;
        _trainerRepository = trainerRepository;
    }

    [HttpPost("parse-html")]
    public async Task<IActionResult> ParseAndImport(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = new StreamReader(file.OpenReadStream());
        var html = await stream.ReadToEndAsync();

        var parser = new HtmlParser();
        var result=  await parser.ParseRaceCardHtml(html);
var parsedRaces = result.Item1;
var raceDate = result.Item2;
var cityName = result.Item3;
        var importedRaces = new List<RaceImportResult>();
var venueList  = await _venueRepository.GetAllAsync();
        foreach (var parsedRace in parsedRaces)
        {
            //Create or get RaceDay (you'll need to determine the race day from context)
            var venue = venueList.FirstOrDefault(v => v.City.CityName.ToLower() == cityName.ToLower());
            int raceDayId = await GetOrCreateRaceDayId(cityName, venue.VenueId, raceDate);

            // Check if race exists
            var race = await _raceRepository.GetByRaceDayAndNameAsync(raceDayId, parsedRace.RaceName);
            if (race == null)
            {
                race = await _raceRepository.CreateAsync(new Race
                {
                    RaceDayId = raceDayId,
                    RaceNumber = parsedRace.RaceNumber,
                    RaceName = parsedRace.RaceName,
                    DistanceMeters = parsedRace.DistanceMeters,
                    StartTime = parsedRace.StartTime,
                    Status = parsedRace.Status
                });
            }

            int horseCount = 0;
            foreach (var parsedHorse in parsedRace.Horses)
            {
                // Get or create Horse
              var horse=  await _horseRepository.GetOrCreateAsync(parsedHorse.HorseName, parsedHorse.Age, parsedHorse.Color, parsedHorse.Gender);

               
                // Get or create Trainer
                var trainer = await _trainerRepository.GetOrCreateByNameAsync(parsedHorse.Trainer);

                // Get or create Jockey
                int? jockeyId = null;
                if (!string.IsNullOrEmpty(parsedHorse.Jockey))
                {
                    var jockey = await _jockeyRepository.GetOrCreateAsync(parsedHorse.Jockey);
                    jockeyId = jockey?.JockeyId;
                }

                // Check if RaceHorse already exists
                var exists = await _raceHorseRepository.ExistsAsync(race.RaceId, horse.HorseId);
                if (!exists)
                {
                    await _raceHorseRepository.CreateAsync(new RaceHorse
                    {
                        RaceId = race.RaceId,
                        HorseId = horse.HorseId,
                        //TrainerId = trainer?.TrainerId,
                        Position = parsedHorse.Position, // Position will be
                        JockeyId = jockeyId,
                        DrawNumber = parsedHorse.DrawNumber,
                        Rating = parsedHorse.Rating > 0 ? parsedHorse.Rating : null,
                        Weight = parsedHorse.Weight > 0 ? parsedHorse.Weight : null,
                        //Equipment = parsedHorse.Equipment
                    });
                }

                horseCount++;
            }

            importedRaces.Add(new RaceImportResult
            {
                RaceId = race.RaceId,
                RaceName = parsedRace.RaceName,
                Distance = parsedRace.DistanceMeters,
                HorseCount = horseCount
            });
        }

        return Ok(new { Message = $"Successfully imported {importedRaces.Count} races", ImportedRaces = importedRaces });
    }

    // Helper method to get or create RaceDay (implement based on your logic)
    private async Task<int> GetOrCreateRaceDayId(string cityName, int venueId, DateTime? raceDate=null)
    {

       
          var   raceDay = await _raceDayRepository.CreateAsync(new RaceDay
            {
                RaceDate = raceDate,
                CityName = cityName, // Set appropriate city name
                VenueId = venueId,
                Status = "Upcoming"
            });
                return raceDay.RaceDayId;
    }

    // Parses "6y b g" → (6, "Bay", "Gelding")
    private (int? age, string color, string gender) ParseDesc(string desc)
    {
        // e.g. "6y b g" or "4y ch f" or "5y dkb h"
        var parts = desc.ToLower().Split(' ');

        int? age = null;
        if (parts[0].EndsWith("y"))
            int.TryParse(parts[0].Replace("y", ""), out int a);
        age = int.TryParse(parts[0].Replace("y", ""), out int parsed) ? parsed : null;

        var colorMap = new Dictionary<string, string>
    {
        { "b", "Bay" }, { "ch", "Chestnut" }, { "dkb", "Dark Bay" },
        { "gr", "Grey" }, { "br", "Brown" }, { "bl", "Black" }
    };

        var genderMap = new Dictionary<string, string>
    {
        { "g", "Gelding" }, { "c", "Colt" }, { "f", "Filly" },
        { "h", "Horse" }, { "m", "Mare" }
    };

        string color = parts.Length > 1 && colorMap.ContainsKey(parts[1]) ? colorMap[parts[1]] : null;
        string gender = parts.Length > 2 && genderMap.ContainsKey(parts[2]) ? genderMap[parts[2]] : null;

        return (age, color, gender);
    }
}