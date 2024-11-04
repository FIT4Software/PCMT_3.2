namespace DTOs;

 public class Response_Server_Data_DTO
{
    public int serverId { get; set; }
    public string server { get; set; }
    public string serverName { get; set; }
    public string serverDB { get; set; }
    public string serverSSO { get; set; }
    public bool? active { get; set; }
}

 public class Create_Server_Data_DTO
{
    public string server { get; set; }
    public string serverName { get; set; }
    public string serverSSO { get; set; }
    public bool? active { get; set; }
}

 public class Update_Server_Data_DTO
{
    public int serverId { get; set; }
    public string server { get; set; }
    public string serverName { get; set; }
    public string serverDB { get; set; }
    public string serverSSO { get; set; }
    public bool? active { get; set; }
}