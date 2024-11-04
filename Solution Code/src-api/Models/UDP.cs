using System.ComponentModel.DataAnnotations;

namespace Models;

public class UDP
{
    public int Table_Field_Id { get; set; }
    public int TableId { get; set; }
    public string Value { get; set; }
    public int KeyId { get; set; }
    public string Table_Field_Desc { get; set; }
    public string Field_Type_Desc { get; set; }
}
