// Services/IBetService.cs


// Services/BetService.cs
public class BetService : IBetService
{
    private readonly IBetRepository _betRepo;
    private readonly IUserRepository _userRepo;

    public BetService(IBetRepository betRepo, IUserRepository userRepo
    )
    {
        _betRepo = betRepo;
        _userRepo = userRepo;
    }

    public async Task PlaceBetAsync(BetDto dto)
    {
        // var validation = await _validator.ValidateAsync(dto);
        // if (!validation.IsValid)
        //     throw new ValidationException(validation.Errors);

        // var balance = await _userRepo.GetWalletBalanceAsync(dto.UserId);
        // if (balance < dto.Stake)
        //     throw new InvalidOperationException("Insufficient balance");

        // await _userRepo.DeductAsync(dto.UserId, dto.Stake);
        // await _betRepo.PlaceBetsAsync(dto);

        // return new BetResult { Success = true, Message = "Bet placed successfully" };
    }
}