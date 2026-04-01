using Dapper;

public class DepositRepository : IDepositRepository
{
    private readonly DapperContext _ctx;
    public DepositRepository(DapperContext ctx) => _ctx = ctx;

    // User submits
    public async Task<int> CreateAsync(DepositRequest req)
    {
        const string sql = @"
            INSERT INTO trackpulse.deposit_requests
                (user_id, amount, payment_method, transaction_id, notes, screenshot_path)
            VALUES
                (@UserId, @Amount, @PaymentMethod, @TransactionId, @Notes, @ScreenshotPath)
            RETURNING id
            ";
        using var conn = _ctx.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, req);
    }

    // Admin list — filterable by status + date + search
    public async Task<IEnumerable<DepositRequest>> GetAllAsync(
        string? status, string? search, string? dateRange)
    {
        var where = new List<string>();
        if (!string.IsNullOrEmpty(status) && status != "all")
            where.Add("dr.status = @Status");
        if (!string.IsNullOrEmpty(search))
            where.Add("(u.username ILIKE @Search OR u.mobile ILIKE @Search OR dr.transaction_id ILIKE @Search)");
        if (dateRange == "today")
            where.Add("dr.submitted_at::date = CURRENT_DATE");
        else if (dateRange == "yesterday")
            where.Add("dr.submitted_at::date = CURRENT_DATE - 1");
        else if (dateRange == "week")
            where.Add("dr.submitted_at >= NOW() - INTERVAL '7 days'");

        var sql = $"""
            SELECT dr.*, dr.screenshot_path AS screenshotpath,  u.user_name AS name, u.mobile_number
            FROM trackpulse.deposit_requests dr
            JOIN trackpulse.users u ON u.user_id = dr.user_id
            {(where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "")}
            ORDER BY dr.submitted_at DESC
            """;
        using var conn = _ctx.CreateConnection();
        return await conn.QueryAsync<DepositRequest>(sql, new {
            Status = status, Search = $"%{search}%"
        });
    }

    // Admin approve / reject single
    public async Task ReviewAsync(int id, string action, string? reason, int adminId)
    {
        const string sql = """
            UPDATE trackpulse.deposit_requests
            SET status           = @Status,
                rejection_reason = @Reason,
                reviewed_by      = @AdminId,
                reviewed_at      = NOW()
            WHERE id = @Id
            """;
        using var conn = _ctx.CreateConnection();
        await conn.ExecuteAsync(sql, new {
            Id = id, Status = action == "approve" ? "approved" : "rejected",
            Reason = reason, AdminId = adminId
        });
    }

    // Admin bulk approve
    public async Task BulkApproveAsync(List<int> ids, int adminId)
    {
        const string sql = """
            UPDATE deposit_requests
            SET status      = 'approved',
                reviewed_by = @AdminId,
                reviewed_at = NOW()
            WHERE id = ANY(@Ids) AND status = 'pending'
            """;
        using var conn = _ctx.CreateConnection();
        await conn.ExecuteAsync(sql, new { Ids = ids.ToArray(), AdminId = adminId });
    }
}