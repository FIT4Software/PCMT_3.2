using System.Linq;

namespace DTOs;

public class Response_Service_DTO{
    public int serviceId { get; set; }
    public string serviceDesc { get; set; }
    public string serviceDisplay { get; set; }
}

public class Response_Reload_Service_DTO{
    public int serviceId {get; set;}
}

public class Request_Reload_Service_Ids_DTO {
    public int[] serviceIds {get; set;}
}