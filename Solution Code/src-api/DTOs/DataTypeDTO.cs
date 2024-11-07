using System.Linq;

namespace DTOs;

 public class Response_Data_Type_DTO
{
    public int dataTypeId { get; set; }
    public string dataTypeDesc { get; set; }
    public bool? usePrecision { get; set; }
    public bool? userDefined { get; set; }
}

 public class Response_Phrase_DTO
{
    public int phraseId { get; set; }
    public int dataTypeId { get; set; }
    public string? oldPhrase { get; set; }
    public int phraseOrder { get; set; }
    public string phraseValue { get; set; }
}

 public class Trash_Data_Type_DTO
{
    public List<Response_Phrase_DTO> phrases { get; set; }
}

 public class Create_Update_Data_Type_DTO
{
    public int dataTypeId { get; set; }
    public string dataTypeDesc { get; set; }
    public List<Response_Phrase_DTO> phrases { get; set; }
    public Trash_Data_Type_DTO trash { get; set; }
}