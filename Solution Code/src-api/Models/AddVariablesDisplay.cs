using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class AddVariablesDisplay{
        public VarSheetData[] variables {get;set;}

        public AddVariablesDisplay(List<DTOs.Response_Sheet_Variable_DTO> sheets) {
            this.variables = sheets.Select(r => new VarSheetData(r)).ToArray();
        }
    }
}