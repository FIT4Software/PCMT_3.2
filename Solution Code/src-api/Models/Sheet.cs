using System.ComponentModel.DataAnnotations;

namespace Models;

public class Sheet
{
    public int Sheet_Id { get; set; }
    public string Sheet_Desc { get; set; }
    public byte? Sheet_Type { get; set; }
    public string? Sheet_Type_Desc { get; set; }
    public int Var_Order { get; set; }

}