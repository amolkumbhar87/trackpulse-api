using System.Text.RegularExpressions;
using HtmlAgilityPack;

public class HtmlParser
{
    
public async Task<(List<ParsedRaceInfo>, DateTime?, string)> ParseRaceCardHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var raceCardText = doc.DocumentNode
    .SelectSingleNode("//div[contains(@class, 'home headline_home')]//h3")
    ?.InnerText.Trim() ?? "";

var parts = raceCardText.Split('-');

string cityName = parts.Length > 1 ? parts[1].Trim() : "Unknown City";
string raceDateText = parts.Length > 2 ? parts[2].Trim() : "";

        // Try to parse the race date
        DateTime? raceDate = null;
        if (!string.IsNullOrEmpty(raceDateText))
        {
            if (DateTime.TryParse(raceDateText, out DateTime parsedDate))
            {
                raceDate = parsedDate;
            }
        }

        var races = new List<ParsedRaceInfo>();
        var raceDivs = doc.DocumentNode.SelectNodes("//div[starts-with(@id, 'race-')]");

        if (raceDivs == null) return (races, raceDate, cityName);

        int raceNumber = 1;
        foreach (var raceDiv in raceDivs)
        {
            var raceInfo = ParseSingleRace(raceDiv, raceNumber++);
            if (raceInfo != null)
            {
                races.Add(raceInfo);
            }
        }

        return (races, raceDate, cityName);
    }

    private ParsedRaceInfo ParseSingleRace(HtmlNode raceDiv, int raceNumber)
    {
        try
        {
            var raceInfo = new ParsedRaceInfo
            {
                RaceNumber = raceNumber,
                Status = "Upcoming"
            };

            // Extract Race ID from div id (race-1, race-2, etc.)
            var divId = raceDiv.GetAttributeValue("id", "");
            var raceIdMatch = Regex.Match(divId, @"race-(\d+)");
            if (raceIdMatch.Success)
            {
                raceInfo.RaceId = int.Parse(raceIdMatch.Groups[1].Value);
            }

            // Extract Race Name and Distance/Time from heading_div
            var headingDiv = raceDiv.SelectSingleNode(".//div[contains(@class, 'heading_div')]");
            if (headingDiv != null)
            {
                // Race Name
                var raceNameNode = headingDiv.SelectSingleNode(".//div[contains(@class, 'center_heading')]//h2");
                if (raceNameNode != null)
                {
                    raceInfo.RaceName = raceNameNode.InnerText.Trim();
                }

                // Distance and Time from archive_time
                var archiveTimeDiv = headingDiv.SelectSingleNode(".//div[contains(@class, 'archive_time')]");
                if (archiveTimeDiv != null)
                {
                    var h4Nodes = archiveTimeDiv.SelectNodes(".//h4");
                    if (h4Nodes != null && h4Nodes.Count >= 2)
                    {
                        // Distance (e.g., "1600 M")
                        var distanceText = h4Nodes[0].InnerText.Trim();
                        var distanceMatch = Regex.Match(distanceText, @"(\d+)");
                        if (distanceMatch.Success)
                        {
                            raceInfo.DistanceMeters = int.Parse(distanceMatch.Groups[1].Value);
                        }

                        // Time (e.g., "04:30 PM")
                        var timeText = h4Nodes[1].InnerText.Trim();
                        if (!string.IsNullOrEmpty(timeText))
                        {
                            // You might need to combine with date from somewhere else
                            // For now, just parse the time
                            if (DateTime.TryParse(timeText, out DateTime parsedTime))
                            {
                                raceInfo.StartTime = parsedTime;
                            }
                        }
                    }
                }
            }

            // Parse Horse Table
            var table = raceDiv.SelectSingleNode(".//table[contains(@class, 'race_card_tab')]");
            if (table != null)
            {
                var tbody = table.SelectSingleNode(".//tbody");
                var rows = tbody?.SelectNodes(".//tr[contains(@class, 'dividend_tr')]") ?? table.SelectNodes(".//tr[contains(@class, 'dividend_tr')]");

                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var horseInfo = ParseHorseRow(row);
                        if (horseInfo != null)
                        {
                            raceInfo.Horses.Add(horseInfo);
                        }
                    }
                }
            }

            return raceInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing race {raceNumber}: {ex.Message}");
            return null;
        }
    }

    private ParsedHorseInfo ParseHorseRow(HtmlNode row)
    {
        try
        {
            var horseInfo = new ParsedHorseInfo();
            var cells = row.SelectNodes(".//td");
            if (cells == null || cells.Count < 12) return null;

            // Column 0: Position and Draw Number
            // Format: "1<br><span>(4)</span>"
            var positionCell = cells[0];
            var positionText = positionCell.SelectSingleNode(".//text()[1]")?.InnerText.Trim() ?? "";
            var drawSpan = positionCell.SelectSingleNode(".//span");
            var drawText = drawSpan?.InnerText.Trim().Trim('(', ')') ?? "";

            horseInfo.Position = int.TryParse(positionText, out int pos) ? pos : 0;
            horseInfo.DrawNumber = int.TryParse(drawText, out int draw) ? draw : 0;

            // Column 2: Horse Name and Pedigree (index might vary based on Silk column)
            // The horse name is in the race_card_td div
            int horseNameColumnIndex = 2; // Adjust if Silk column is present
            if (cells.Count > horseNameColumnIndex)
            {
                var horseTd = cells[horseNameColumnIndex];
                
                // Horse Name
                var horseNameNode = horseTd.SelectSingleNode(".//h5//a");
                if (horseNameNode != null)
                {
                    horseInfo.HorseName = horseNameNode.InnerText.Trim();
                }

                // Pedigree (Sire-Dam)
                var pedigreeNode = horseTd.SelectSingleNode(".//h6");
                if (pedigreeNode != null)
                {
                    var sireLink = pedigreeNode.SelectSingleNode(".//a[1]");
                    var damLink = pedigreeNode.SelectSingleNode(".//a[2]");
                    if (sireLink != null && damLink != null)
                    {
                        horseInfo.Pedigree = $"{sireLink.InnerText.Trim()}-{damLink.InnerText.Trim()}";
                    }

                    // Last 5 runs
                    var lastFiveSpan = pedigreeNode.SelectSingleNode(".//span[contains(@class, 'last-five-runs-lable')]");
                    if (lastFiveSpan != null)
                    {
                        var runs = lastFiveSpan.InnerText.Trim().Split('-');
                        foreach (var run in runs)
                        {
                            if (int.TryParse(run.Trim().Trim('.'), out int runValue))
                            {
                                horseInfo.LastFiveRuns.Add(runValue);
                            }
                        }
                    }
                }
            }

            // Find Description column (usually "Desc" - 4y ch g)
            int descColumnIndex = 3;
            if (cells.Count > descColumnIndex)
            {
                horseInfo.Description = cells[descColumnIndex].InnerText.Trim();
                ParseDescription(horseInfo.Description, horseInfo);
            }

            // Owner column
            int ownerColumnIndex = 4;
            if (cells.Count > ownerColumnIndex)
            {
                horseInfo.Owner = cells[ownerColumnIndex].InnerText.Trim();
            }

            // Trainer column
            int trainerColumnIndex = 5;
            if (cells.Count > trainerColumnIndex)
            {
                var trainerNode = cells[trainerColumnIndex].SelectSingleNode(".//a");
                horseInfo.Trainer = trainerNode?.InnerText.Trim() ?? cells[trainerColumnIndex].InnerText.Trim();
            }

            // Jockey column
            int jockeyColumnIndex = 6;
            if (cells.Count > jockeyColumnIndex)
            {
                var jockeyNode = cells[jockeyColumnIndex].SelectSingleNode(".//a");
                horseInfo.Jockey = jockeyNode?.InnerText.Trim() ?? cells[jockeyColumnIndex].InnerText.Trim();
            }

            // Weight column
            int weightColumnIndex = 7;
            if (cells.Count > weightColumnIndex)
            {
                var weightText = cells[weightColumnIndex].InnerText.Trim();
                if (decimal.TryParse(weightText, out decimal weight))
                {
                    horseInfo.Weight = weight;
                }
            }

            // Allowance (Al)
            int allowanceColumnIndex = 8;
            if (cells.Count > allowanceColumnIndex)
            {
                horseInfo.Allowance = cells[allowanceColumnIndex].InnerText.Trim();
            }

            // Shoe (Sh)
            int shoeColumnIndex = 9;
            if (cells.Count > shoeColumnIndex)
            {
                horseInfo.Shoe = cells[shoeColumnIndex].InnerText.Trim();
            }

            // Equipment (Eq)
            int equipmentColumnIndex = 10;
            if (cells.Count > equipmentColumnIndex)
            {
                horseInfo.Equipment = cells[equipmentColumnIndex].InnerText.Trim();
            }

            // Rating (Rtg)
            int ratingColumnIndex = 11;
            if (cells.Count > ratingColumnIndex)
            {
                var ratingText = cells[ratingColumnIndex].InnerText.Trim();
                // Format: "27" or "27"
                if (int.TryParse(ratingText, out int rating))
                {
                    horseInfo.Rating = rating;
                }
                else
                {
                    // Try to extract from HTML like <sup><small>27</small></sup>25
                    var supNode = cells[ratingColumnIndex].SelectSingleNode(".//sup");
                    if (supNode != null)
                    {
                        var ratingMatch = Regex.Match(supNode.InnerText, @"(\d+)");
                        if (ratingMatch.Success)
                        {
                            horseInfo.Rating = int.Parse(ratingMatch.Groups[1].Value);
                        }
                    }
                }
            }

            return horseInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing horse row: {ex.Message}");
            return null;
        }
    }

    private void ParseDescription(string description, ParsedHorseInfo horseInfo)
    {
        // Description format examples:
        // "4y ch g" (4 years, chestnut, gelding)
        // "5y dkb g" (5 years, dark bay, gelding)
        // "4y b f" (4 years, bay, filly)
        // "6y b m" (6 years, bay, mare)
        
        if (string.IsNullOrEmpty(description)) return;

        var parts = description.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3)
        {
            // Age (e.g., "4y")
            if (parts[0].EndsWith("y") && int.TryParse(parts[0].TrimEnd('y'), out int age))
            {
                horseInfo.Age = age;
            }

            // Color
            horseInfo.Color = ParseColorCode(parts[1]);

            // Gender
            horseInfo.Gender = ParseGenderCode(parts[2]);
        }
    }

    private string ParseColorCode(string code)
    {
        return code.ToLower() switch
        {
            "ch" => "Chestnut",
            "b" => "Bay",
            "dkb" => "Dark Bay",
            "gr" => "Grey",
            "ro" => "Roan",
            "bl" => "Black",
            "w" => "White",
            _ => code
        };
    }

    private string ParseGenderCode(string code)
    {
        return code.ToLower() switch
        {
            "g" => "Gelding",
            "f" => "Filly",
            "m" => "Mare",
            "c" => "Colt",
            "h" => "Horse",
            _ => code
        };
    }
}