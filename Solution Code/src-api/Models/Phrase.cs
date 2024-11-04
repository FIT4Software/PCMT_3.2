using System.ComponentModel.DataAnnotations;

namespace Models;

public class Phrase
{
    [Key]
    public int Phrase_Id { get; set; }
    public bool Active { get; set; }
    public DateTime? Changed_Date { get; set; }
    public bool Comment_Required { get; set; }
    public int Data_Type_Id { get; set; }
    public string? Old_Phrase { get; set; }
    public Int16 Phrase_Order { get; set; }
    public string Phrase_Value { get; set; }
}