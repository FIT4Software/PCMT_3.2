using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Variable_All{
        public int Var_Id { get; set; }
        public string Var_Desc { get; set; }
        public string? Var_Desc_Global { get; set; }
        public int PUG_Id { get; set; }
        public int PUG_Order { get; set; }
        public string PUG_Desc { get; set; }
        public int PU_Id { get; set; }
        public string PU_Desc { get; set; } 
        public int DS_Id { get; set; }
        public string DS_Desc { get; set; }
        public int Data_Type_Id { get; set; }
        public byte? Var_Precision { get; set; }
        public string? Eng_Units { get; set; }
        public int? Group_Id { get; set; }
        public int PL_Id { get; set; }
        public string PL_Desc { get; set; }
        public int? Event_Subtype_Id { get; set; }
        public string? Event_Subtype_Desc { get; set; }
        public byte ET_Id { get; set; }
        public string ET_Desc { get; set; }
    }
}