using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DepositController : ControllerBase
{
    private readonly IDepositRepository _repo;
    private readonly IWebHostEnvironment _env;

    public DepositController(IDepositRepository repo, IWebHostEnvironment env)
    {
        _repo = repo;
        _env  = env;
    }

    // ── User: submit deposit request ─────────────────
    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> Submit([FromForm] SubmitDepositDto dto)
    {
        // Save screenshot file
        string? screenshotPath = null;
        if (dto.ReceiptFile != null)
        {
            var uploads = Path.Combine(_env.ContentRootPath, "uploads", "deposits");
            Directory.CreateDirectory(uploads);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ReceiptFile.FileName)}";
            var fullPath = Path.Combine(uploads, fileName);
            using var stream = System.IO.File.Create(fullPath);
            await dto.ReceiptFile.CopyToAsync(stream);
            screenshotPath = $"/uploads/deposits/{fileName}";
        }

        //var userId = int.Parse(User.FindFirst("sub")!.Value);
        var req = new DepositRequest
        {
            UserId        = dto.UserId,
            Amount        = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            TransactionId = dto.TransactionId,
            Notes         = dto.Notes,
            ScreenshotPath = screenshotPath,
        };
        var id = await _repo.CreateAsync(req);
        return Ok(new { id });
    }

    // ── Admin: get all deposits ───────────────────────
    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? search,
        [FromQuery] string? date)
    {
        var deposits = await _repo.GetAllAsync(status, search, date);
var result = deposits.Select(d => new DepositRequest
{
    Id = d.Id,
    UserId = d.UserId,
    UserName = d.UserName,
    MobileNumber = d.MobileNumber,
    Amount = d.Amount,
    PaymentMethod = d.PaymentMethod,
    TransactionId = d.TransactionId,
    Notes = d.Notes,
    Status = d.Status,
    RejectionReason = d.RejectionReason,
    ScreenshotPath = !string.IsNullOrEmpty(d.ScreenshotPath) 
                    ? $"{Request.Scheme}://{Request.Host}{d.ScreenshotPath}" 
                    : null
});

        return Ok(result);
    }

    // ── Admin: approve or reject single ──────────────
    [HttpPatch("{id}/review")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Review(int id, [FromBody] ReviewDepositDto dto)
    {
        //var adminId = int.Parse(User.FindFirst("sub")!.Value);
        var adminId = 2; // TODO: replace with actual admin ID from auth
        await _repo.ReviewAsync(id, dto.Action, dto.RejectionReason, adminId);

        // Credit wallet if approved
        if (dto.Action == "approve")
            await CreditWalletAsync(id);

        return Ok();
    }

    // ── Admin: bulk approve ───────────────────────────
    [HttpPost("bulk-approve")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkApprove([FromBody] BulkApproveDto dto)
    {
        var adminId = int.Parse(User.FindFirst("sub")!.Value);
        await _repo.BulkApproveAsync(dto.Ids, adminId);
        // Credit each wallet
        foreach (var id in dto.Ids)
            await CreditWalletAsync(id);
        return Ok();
    }

    // ── User: own deposit history ─────────────────────
    [HttpGet("my")]
    //[Authorize]
    public async Task<IActionResult> MyDeposits()
    {
        var userId = int.Parse(User.FindFirst("sub")!.Value);
        var deposits = await _repo.GetAllAsync(null, null, null);
        return Ok(deposits.Where(d => d.UserId == userId));
    }

    private async Task CreditWalletAsync(int depositId)
    {
        // TODO: call WalletRepository.CreditAsync(userId, amount)
        // Get deposit → credit user wallet → done
    }
}