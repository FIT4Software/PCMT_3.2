namespace Models;

public class Transaction_Log
{
    public int? TransactionLogId { get; set; }
    public int? ServerId { get; set; }
    public string TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public string UserName { get; set; }
    public bool HasErrors { get; set; }
    public string? TransactionDetails { get; set; }
}



    

    
