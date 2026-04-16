public class HorsePositionDto
{
    public int RaceHorseId { get; set; }
    public int Position { get; set; }
}

public class RaceResultDto
{
    public int RaceId { get; set; }
    public List<HorsePositionDto> Positions { get; set; } = new();
}
