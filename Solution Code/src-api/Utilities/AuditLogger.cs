using Contexts;
using Models;

namespace Utilities;

public class AuditLogger {
    private readonly CentralDBContext _centralContext;
    
    public AuditLogger(CentralDBContext centralContext)
    {
        _centralContext = centralContext;
    }

    public async void log(string transactionType, int serverId, string username, string? transactionDetails, bool hasErrors = false) {
        var transactionLog = await _centralContext.AddAsync(new Transaction_Log{
            TransactionLogId = null,
            ServerId = serverId, 
            TransactionType = transactionType,
            TransactionDate = DateTime.UtcNow,
            UserName = username,
            TransactionDetails = transactionDetails,
            HasErrors = hasErrors
        });
        
        await _centralContext.SaveChangesAsync();
    }
}