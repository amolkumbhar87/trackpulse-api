using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RaceController : ControllerBase
{


    private readonly IRaceRepository _raceRepository;
    private readonly IHorseRepository _horseRepository; 
    private readonly IRaceHorseRepository _raceHorseRepository;

    public RaceController(IRaceRepository raceRepository, IHorseRepository horseRepository, IRaceHorseRepository raceHorseRepository)
    {
        _raceRepository = raceRepository;
        _horseRepository = horseRepository;
        _raceHorseRepository = raceHorseRepository;
    }

    // [HttpGet("races/{cityName}/{raceDate}")]
    [HttpGet("races")]
    public async Task<IActionResult> GetRaceByCityAndDateAsync(string cityName, string raceDate)
    {
        var races = await _raceRepository.GetRaceByCityAndDateAsync(cityName, raceDate);
        return Ok(races);
    }

    [HttpPost("parse-html")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ParseAndImport(IFormFile file, [FromQuery] int raceDayId)
    {
        using var stream = new StreamReader(file.OpenReadStream());
        var html = await stream.ReadToEndAsync();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var raceTitles = doc.DocumentNode.SelectNodes("//h1");
        if (raceTitles == null) return BadRequest("No races found in file.");

        var importedRaces = new List<RaceImportResult>();
        int raceNumber = 1;

        foreach (var titleNode in raceTitles)
        {
            // --- Parse race info ---
            var raceName = titleNode.InnerText.Trim();
            var distanceNode = titleNode.SelectSingleNode("following-sibling::h4[2]");
            var distanceText = distanceNode?.InnerText.Trim(); // "2400 M"
            int.TryParse(distanceText?.Replace("M", "").Trim(), out int distance);

            // --- Create Race if not exists ---
            var race = await _raceRepository.GetByRaceDayAndNameAsync(raceDayId, raceName);
            if (race == null)
            {
                race = await _raceRepository.CreateAsync(new Race
                {
                    RaceDayId = raceDayId,
                    RaceNumber = raceNumber++,
                    RaceName = raceName,
                    DistanceMeters = distance,
                    Status = "Upcoming"
                });
            }

            // --- Parse horse rows ---
            var table = titleNode.SelectSingleNode("following-sibling::table[1]");
            var rows = table?.SelectNodes(".//tr[position()>1]");
            if (rows == null) continue;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null || cells.Count < 5) continue;

                var drawNumber = cells[0].InnerText.Trim();
                var horseName = cells[1].InnerText.Trim().ToUpper();
                var desc = cells[2].InnerText.Trim(); // "6y b g"
                var trainerName = cells[4].InnerText.Trim();
                var weight = cells[5].InnerText.Trim();
                var rating = cells[6].InnerText.Trim();

                // Parse desc → age, color, gender
                var (age, color, gender) = ParseDesc(desc);

                // --- UPSERT Horse ---
                var horse = await _horseRepository.GetOrCreateAsync(horseName.ToString(), age, color, gender);
                // if (horse == null)
                // {
                //     horse = await _horseRepository.GetOrCreateAsync(new Horse
                //     {
                //         HorseName = horseName,
                //         Age = age,
                //         Color = color,
                //         Gender = gender,
                //         IsActive = true
                //     });
                // }

                // --- UPSERT Jockey (from jockey column if available) ---
                // Note: indiarace handicap page doesn't show jockey — 
                // jockey is on the racecard page. Set null for now.
                // Admin can update later from racecard page.
                int? jockeyId = null;

                // --- Create RaceHorse entry if not exists ---
                var exists = await _raceHorseRepository.ExistsAsync(race.RaceId, horse.HorseId);
                if (!exists)
                {
                    await _raceHorseRepository.CreateAsync(new RaceHorse
                    {
                        RaceId = race.RaceId,
                        HorseId = horse.HorseId,
                        JockeyId = jockeyId,
                        DrawNumber = int.TryParse(drawNumber, out int dn) ? dn : 0,
                        Rating = int.TryParse(rating, out int rt) ? rt : null,
                        Weight = decimal.TryParse(weight, out decimal wt) ? wt : null
                    });
                }
            }

            importedRaces.Add(new RaceImportResult
            {
                RaceName = raceName,
                Distance = distance,
                HorseCount = rows.Count
            });
        }

        return Ok(importedRaces);
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