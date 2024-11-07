using System.Linq;

namespace DTOs;

public class Response_Auth_Server_Data_DTO
{
    public int serverId { get; set; }
    public string serverName { get; set; }
    public string server { get; set; }
    public string serverDB { get; set; }
    public string groups { get; set; }

}

public class Authenticate_DTO
{
    public string? serverId { get; set; }
}
public class User
{
    public string token { get; set; }
    public string[] sites { get; set; }
    public string thumbnailphoto { get; set; }
    public string message { get; set; }

}