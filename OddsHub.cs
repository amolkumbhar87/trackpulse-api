using Microsoft.AspNetCore.SignalR;

public class OddsHub  : Hub
{
    public async Task JoinRace(string raceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"race-{raceId}");
    }

    public async Task LeaveRace(string raceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"race-{raceId}");
    }
}