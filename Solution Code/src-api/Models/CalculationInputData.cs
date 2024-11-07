using System.ComponentModel.DataAnnotations;

namespace Models;
public class CalculationInputData
{
    public int Calc_Input_Id { get; set; }
    public string? Input_Name { get; set; }
    public string? Entity_Name { get; set; }
    public int? Member_Var_Id { get; set; }
    public string? Default_Value { get; set; }
    public string? Var_Desc { get; set; }
}
