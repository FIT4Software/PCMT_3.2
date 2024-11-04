using System.ComponentModel.DataAnnotations;

namespace Models;

public class Server_Data
{
    [Key]
    public int ServerId { get; set; }
    public string ServerName { get; set; }
    public string ServerURL { get; set; }
    public string ServerDBName { get; set; }
    public string? Business { get; set; }
    public string? TimeZone { get; set; }
    public int Active { get; set; }
    public string ServerSSOGroups { get; set; }
}


public class Server_Data_Trimmed
{
    [Key]
    public int ServerId { get; set; }
    public string ServerName { get; set; }
    public string ServerURL { get; set; }
    public string ServerDBName { get; set; }
    public string? Business { get; set; }
    public string? TimeZone { get; set; }
}
