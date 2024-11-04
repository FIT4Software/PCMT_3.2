using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class VarSheetData{

        public int Var_Id { get; set; }
        public string Var_Desc { get; set; }

        public int Var_Order { get; set; }
        public int PL_Id { get; set; }
        
        public string PL_Desc { get; set; }
        public int PU_Id { get; set; }
        public string PU_Desc { get; set; }
        public int PUG_Id { get; set; }
        public string PUG_Desc { get; set; }
        public int Sheet_Id { get; set; }
        public string Title { get; set; }
        public string Sheet_Desc { get; set; }
    
        public VarSheetData(DTOs.Response_Sheet_Variable_DTO dto) {
            this.Var_Id = dto.varId;
            this.Var_Desc = dto.varDesc;
            this.Var_Order = dto.varOrder;
            this.PL_Id = dto.plId;
            this.PL_Desc = dto.plDesc;
            this.PU_Id = dto.puId;
            this.PU_Desc = dto.puDesc;
            this.PUG_Id = dto.pugId;
            this.PUG_Desc = dto.pugDesc;
            this.Sheet_Id = dto.sheetId;
            this.Title = dto.title;
            this.Sheet_Desc = dto.sheetDesc;
        }
    }
}