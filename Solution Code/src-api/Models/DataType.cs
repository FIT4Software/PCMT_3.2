using System.ComponentModel.DataAnnotations;

namespace Models;

public class Data_Type
{
    [Key]
    public int Data_Type_Id { get; set; }
    public string Data_Type_Desc { get; set; }
    public bool? Use_Precision { get; set; }
    public bool? User_Defined { get; set; }
}