using Microsoft.AspNetCore.SignalR;

public class OddsHub  : Hub
{
    public async Task JoinRace(int raceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"race-{raceId}");
    }

    public async Task LeaveRace(int raceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"race-{raceId}");
    }
}