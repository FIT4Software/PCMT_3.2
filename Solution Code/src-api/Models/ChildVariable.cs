using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Child_Variable{
        public int Var_Id { get; set; }
        public string Var_Desc { get; set; }
        public string? Var_Desc_Global { get; set; }

        public int PUG_Id { get; set; }
        public int PUG_Order { get; set; }
        public int PU_Id { get; set; }
        public string PU_Desc { get; set; } 
        public int DS_Id { get; set; }

        public int Data_Type_Id { get; set; }
        public byte? Var_Precision { get; set; }
        public string? Eng_Units { get; set; }
        public int? Group_Id { get; set; }

        public string? External_Link { get; set; }

    }
}