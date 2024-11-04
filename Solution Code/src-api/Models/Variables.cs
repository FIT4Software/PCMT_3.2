using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Variable
    {
        public int Var_Id { get; set; }
        public string Var_Desc { get; set; }
        public string? Var_Desc_Global { get; set; }
        public int PU_Id { get; set; }
        public int PUG_Id { get; set; }
        public int PUG_Order { get; set; }
        public int Data_Type_Id { get; set; }
        public byte? Var_Precision { get; set; }
        public int? Spec_Id { get; set; }
        public string? Spec_Desc { get; set; }
        public string? Prop_Desc { get; set; }
        public Int16? Sampling_Interval { get; set; }
        public Int16? Sampling_Offset { get; set; }
        public int? Sampling_Type { get; set; }
        public int? Sampling_Window { get; set; }
        public int? Sampling_Window_Type { get; set; }
        public byte Event_Type { get; set; }
        public string Event_Type_Desc { get; set; }
        public bool Var_Reject { get; set; }
        public bool Unit_Reject { get; set; }
        public Int16? Rank { get; set; }
        public string? Input_Tag { get; set; }
        public string? Input_Tag2 { get; set; }
        public string? Output_Tag { get; set; }
        public string? DQ_Tag { get; set; }
        public string? UEL_Tag { get; set; }
        public string? URL_Tag { get; set; }
        public string? UWL_Tag { get; set; }
        public string? UUL_Tag { get; set; }
        public string? Target_Tag { get; set; }
        public string? LUL_Tag { get; set; }
        public string? LWL_Tag { get; set; }
        public string? LRL_Tag { get; set; }
        public string? LEL_Tag { get; set; }
        public Single? Tot_Factor { get; set; }
        public byte? TF_Reset { get; set; }
        public byte SA_Id { get; set; }
        public int? Comparison_Operator_Id { get; set; }
        public string? Comparison_Value { get; set; }
        public byte? Repeating { get; set; }
        public int? Repeat_Back_Time { get; set; }
        public bool Should_Archive { get; set; } //tentative
        public string? Extended_Info { get; set; }
        public string? User_Defined1 { get; set; }
        public string? User_Defined2 { get; set; }
        public string? User_Defined3 { get; set; }
        public bool Unit_Summarize { get; set; }
        public byte? Force_Sign_Entry { get; set; }
        public string? Test_Name { get; set; }
        public int? Extended_Test_Freq { get; set; }
        public double? Max_RPM { get; set; }
        public double? Reset_Value { get; set; }
        public bool Is_Conformance_Variable { get; set; }
        public int? E_Signature_Level { get; set; }
        public int? Event_Subtype_Id { get; set; }
        public string? Event_Subtype_Desc { get; set; }
        public byte? Event_Dimension { get; set; }
        public int? PEI_Id { get; set; }
        public int? SPC_Calculation_Type_Id { get; set; }
        public int? SPC_Group_Variable_Type_Id { get; set; }
        public int? SPC_Variable_Type_Id { get; set; }
        public int? Sampling_Reference_Var_Id { get; set; }
        public byte? String_Specification_Setting { get; set; }
        public int? Write_Group_DS_Id { get; set; }
        public int? CPK_SubGroup_Size { get; set; }
        public byte? Reload_Flag { get; set; }
        public int? ReadLagTime { get; set; }
        public byte? Event_Lookup { get; set; }
        public string? Eng_Units { get; set; }
        public int DS_Id { get; set; }
        public string DS_Desc { get; set; }
        public int? Group_Id { get; set; }
        public byte? Ignore_Event_Status { get; set; }
        public int Dept_Id { get; set; }
        public string Dept_Desc { get; set; }
        public string PU_Desc { get; set; }
        public int PL_Id { get; set; }
        public string PL_Desc { get; set; }
        public int Master_Unit { get; set; }
        public string PUG_Desc { get; set; }
        public int? Calculation_Id { get; set; }
        public string External_Link { get; set; }
        public int? PVar_Id { get; set; }
        public int? ET_Id { get; set; }
        public string? ET_Desc { get; set; }
        public bool? Is_Active { get; set; }

    }
}
