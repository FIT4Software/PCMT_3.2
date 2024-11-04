using System.ComponentModel.DataAnnotations;

namespace Models;

public class Service{
    public string Service_Desc{ get; set; }
    public string Service_Display { get; set; }
    public int Service_Id{ get; set; }

    public Service(int serviceId, string service_Desc, string service_Display){
        Service_Id = serviceId;
        Service_Desc = service_Desc;
        Service_Display = service_Display;
    }
}