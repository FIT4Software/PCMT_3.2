using System.Linq;
namespace DTOs;

public class Response_Transaction_Types_DTO
{
    public string name { get; set; }
    public string[] columns { get; set; }
}

public class Response_Transaction_Log_DTO
{
    public int? transactionLogId { get; set; }
    public int? serverId { get; set; }
    public string transactionType { get; set; }
    public DateTime transactionDate { get; set; }
    public string username { get; set; }
    public bool hasErrors { get; set; }
    public string? transactionDetails { get; set; }
}

    
