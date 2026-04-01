// Models/DepositRequest.cs
public class DepositRequest
{
    public int     Id               { get; set; }
    public int     UserId           { get; set; }
    public decimal Amount           { get; set; }
    public string  PaymentMethod    { get; set; } = "upi";
    public string  TransactionId    { get; set; } = "";
    public string? Notes            { get; set; }
    public string? ScreenshotPath   { get; set; }
    public string  Status           { get; set; } = "pending";
    public string? RejectionReason  { get; set; }
    public int?    ReviewedBy       { get; set; }
    public DateTime SubmittedAt     { get; set; }
    public DateTime? ReviewedAt     { get; set; }

    public string UserName { get; set; } = ""; // From JOIN with users table
    public string MobileNumber { get; set; } = ""; // From JOIN with users table
}

// DTOs
public class SubmitDepositDto
{
    public decimal Amount        { get; set; }
    public string  TransactionId { get; set; } = "";
    public string  PaymentMethod { get; set; } = "upi";
    public string? Notes         { get; set; }
    public IFormFile ReceiptFile { get; set; } = null!;

    public int UserId { get; set; } // This will be set in the controller based on the authenticated user
}

public class ReviewDepositDto
{
    public string  Action          { get; set; } = "";  // "approve" | "reject"
    public string? RejectionReason { get; set; }
}

public class BulkApproveDto
{
    public List<int> Ids { get; set; } = new();
}