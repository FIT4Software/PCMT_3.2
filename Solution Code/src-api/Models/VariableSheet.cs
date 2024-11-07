using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class VariableSheet
    {
        
        public int? Spec_Id { get; set; } //correct
        public string? Spec_Desc { get; set; } // correct
        public string? Prop_Desc { get; set; } // correct
        public Int16? Sampling_Interval { get; set; } // correct
        public Int16? Sampling_Offset { get; set; } // correct
        public byte? Sampling_Type { get; set; } // correct
        public byte Event_Type { get; set; } // correct
        public int? Sampling_Window { get; set; } // correct
        public int? Sampling_Window_Type { get; set; } // correct
        public bool Var_Reject { get; set; } // correct - bit
        public bool Unit_Reject { get; set; } // correct - bit
        public Int16 Rank { get; set; } // correct - short
        public string? Input_Tag { get; set; } // correct
        public string? Input_Tag2 { get; set; } // correct
        public string? Output_Tag { get; set; } // correct
        public string? DQ_Tag { get; set; } // correct
        public string? UEL_Tag { get; set; } // correct
        public string? URL_Tag { get; set; } // correct
        public string? UWL_Tag { get; set; } // correct
        public string? UUL_Tag { get; set; } // correct
        public string? Target_Tag { get; set; } // correct
        public string? LUL_Tag { get; set; } // correct
        public string? LWL_Tag { get; set; } // correct
        public string? LRL_Tag { get; set; } // correct
        public string? LEL_Tag { get; set; } // correct
        public Single? Tot_Factor { get; set; } // correct
        public byte? TF_Reset { get; set; } // correct
        public byte SA_Id { get; set; } // correct
        public int? Comparison_Operator_Id { get; set; } // correct
        public string? Comparison_Value { get; set; } // correct
        public int? Repeating { get; set; } // correct
        public int? Repeat_Backtime { get; set; } // correct
        public string? Extended_Info { get; set; } // correct
        public string? User_Defined1 { get; set; } // correct
        public string? User_Defined2 { get; set; } // correct
        public string? User_Defined3 { get; set; } // correct
        public bool Unit_Summarize { get; set; } // correct
        public byte? Force_Sign_Entry { get; set; } // correct
        public string? Test_Name { get; set; }  // correct
        public int? Extended_Test_Freq { get; set; } // correct
        public byte? ArrayStatOnly { get; set; } // correct
        public double? Max_RPM { get; set; } // correct
        public double? Reset_Value { get; set; } // correct
        public bool Is_Conformance_Variable { get; set; } // correct
        public int? Esignature_Level { get; set; } // correct
        public int? Event_Subtype_Id { get; set; } // correct
        public byte? Event_Dimension { get; set; } // correct
        public int? PEI_Id { get; set; } // correct
        public int? SPC_Calculation_Type_Id { get; set; } // correct
        public int? SPC_Group_Variable_Type_Id { get; set; } // correct
        public int? Sampling_Reference_Var_Id { get; set; } // correct
        public byte? String_Specification_Setting { get; set; } // correct
        public int? Write_Group_DS_Id { get; set; } // correct
        public int? CPK_SubGroup_Size { get; set; } // correct
        public byte? Reload_Flag { get; set; } // correct
        public int? ReadLagTime { get; set; } // correct
        public int? Should_Archive { get; set; } // correct
        public byte? EventLookup { get; set; }
        public bool? Debug { get; set; }
        public byte? Ignore_Event_Status { get; set; }
      }
}
