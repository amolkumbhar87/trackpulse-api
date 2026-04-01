public interface IDepositRepository
{
   Task<int> CreateAsync(DepositRequest req);
    Task<IEnumerable<DepositRequest>> GetAllAsync(        string? status, string? search, string? dateRange);
    Task ReviewAsync(int id, string action, string? reason, int adminId);
    Task BulkApproveAsync(List<int> ids, int adminId);
}