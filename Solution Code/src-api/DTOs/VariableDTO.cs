using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DTOs;

public class Response_Data_Source_DTO{
    public int dsId {get;set;}
    public required string dsDesc {get;set;}

}

public class Response_Sampling_Type_DTO{
    public byte stId {get;set;}
    public string stDesc {get; set;}
}

public class Response_Eng_Units_DTO{
    public string? engUnits {get;set;}
}

public class Response_SPC_Type_DTO{
    public int? spcCalculationTypeId {get;set;}
    public string? spcCalculationTypeDesc {get;set;}
}

public class Response_Extended_Info_DTO{
    public string extendedInfo {get;set;}
}

public class Response_Event_Types_DTO{
    public byte etId {get;set;}
    public string etDesc {get;set;}
}

public class Response_Event_SubTypes_DTO{
    public int? eventSubType {get;set;}
    public string? eventSubTypeDesc {get;set;}
}

public class Response_Sampling_Interval_DTO{
    public Int16? samplingInterval {get;set;}  
}

public class Response_Sampling_Offset_DTO {
    public Int16? samplingOffset {get;set;}
}

public class Response_Sub_Group_Size_DTO{
    public int sizeId {get;set;}
}

public class Response_Comparison_Operator_DTO{
    public int comparisonOperatorId {get;set;}
    public string comparisonOperatorValue{get;set;}
}

public class Response_Calculation_DTO{
    public int calculationId {get;set;}
    public string calculationName {get;set;}
}

public class Response_Calculation_Input_Data_DTO{
    public int calculationInputId {get;set;}
    public string calculationInputName{get;set;}  
    public string entityName{get;set;}
    public int? memberVarId{get;set;}
    public string defaultValue {get;set;}  
    public string varDesc {get;set;}
}

public class Response_Calculation_Input_DTO{
    public int calculationInputId {get;set;}
    public string calculationInputName{get;set;}  
    public string entityName{get;set;}
}

public class Response_Variable_All_DTO{
    public int varId { get; set; }
    public string varDesc { get; set; }
    public string? varDescGlobal { get; set; }
    public int pugId { get; set; }
    public string pugDesc { get; set; }
    public int puId { get; set; }
    public string puDesc { get; set; }
    public int pugOrder { get; set; }
    public int dsId { get; set; }
    public int dataTypeId { get; set; }
    public byte? varPrecision { get; set; }
    public string? engUnits { get; set; }
    public int? groupId { get; set; }
    public int plId { get; set; }
    public string plDesc { get; set; }
    public int? eventSubTypeId { get; set; }
    public string? eventSubTypeDesc { get; set; }
    public byte etId { get; set; }
    public string etDesc { get; set; }
    public string dsDesc { get; set; }
}

public class Response_Search_DTO{
    public int varId { get; set; }
    public string varDesc { get; set; }
    public string? varDescGlobal { get; set; }
    public int puId { get; set; }
    public int pugId { get; set; }
    public int pugOrder { get; set; }
    public int dsId {get; set; }
    public int dataTypeId {get; set; }

    public byte? varPrecision {get; set; }
    public string? engUnits {get; set; }

    public int? groupId {get; set; }
    public int plId{ get; set; }
    public string plDesc{get; set; }

    public string puDesc{ get; set; }

    public string pugDesc{get; set;}

}
public class Response_Lookup_DTO{
    public int varId { get; set; }
    public string varDesc { get; set; }
    public string? varDescGlobal { get; set; }
    public int puId { get; set; }
    public int pugId { get; set; }
    public int pugOrder { get; set; }
    public int dsId {get; set; }
    public int dataTypeId {get; set; }

    public byte? varPrecision {get; set; }
    public string? engUnits {get; set; }

    public int? groupId {get; set; }
    public int plId{ get; set; }
    public string plDesc{get; set; }

    public string puDesc{ get; set; }

    public string pugDesc{get; set;}

}

public class Response_Find_DTO{
    public int varId { get; set; }
    public string varDesc { get; set; }
    public string varDescGlobal { get; set; }
    public int puId { get; set; }
    public int pugId { get; set; }
    public int pugOrder { get; set; }
    public int dsId {get; set; }
    public int dataTypeId {get; set; }

    public byte? varPrecision {get; set; }
    public string? engUnits {get; set; }

    public int? groupId {get; set; }
    public int plId{ get; set; }
    public string plDesc{get; set; }

    public string puDesc{ get; set; }

    public string pugDesc{get; set;}

    public int? calculationId {get; set; }
    public string externalLink {get; set; }

    public int? specId { get; set; }
    public string? specDescs { get; set; }
    public string? propDesc { get; set; }
    public Int16? samplingInterval { get; set; }
    public Int16? samplingOffset { get; set; }
    public byte? samplingType { get; set; }
    public int? samplingWindowType { get; set; }
    public int? samplingWindow { get; set; }
    public byte eventType { get; set; }
    public bool varReject { get; set; }
    public bool unitReject { get; set; }
    public Int16 rank { get; set; }
    public string? inputTag { get; set; }
    public string? inputTag2 { get; set; }
    public string? outputTag { get; set; }
    public string? dqTag { get; set; }
    public string? uelTag { get; set; }
    public string? urlTag { get; set; }
    public string? uwlTag { get; set; }
    public string? uulTag { get; set; }
    public string? targetTag { get; set; }
    public string? lulTag { get; set; }
    public string? lwlTag { get; set; }
    public string? lrlTag { get; set; }
    public string? lelTag { get; set; }
    public Single? totFactor { get; set; }
    public byte? tfReset { get; set; }
    public byte saId { get; set; }
    public int? comparisonOperatorId { get; set; }
    public string? comparisonValue { get; set; }
    public int? repeating { get; set; }
    public int? repeatBackTime { get; set; }
    public int? shouldArchive { get; set; }
    public string extendedInfo { get; set; }
    public string userDefined1 { get; set; }
    public string userDefined2 { get; set; }
    public string userDefined3 { get; set; }
    public bool? unitSummarize { get; set; }
    public byte? forceSignEntry { get; set; }
    public string? testName { get; set; }
    public int? extendedTestFreq { get; set; }
    public double? maxRPM { get; set; }
    public double? resetValue { get; set; }
    public bool isConformanceVariable { get; set; }
    public int? eSignatureLevel { get; set; }
    public int? eventSubTypeId { get; set; }
    public byte? eventDimension { get; set; }
    public int? peiId { get; set; }
    public int? spcCalculationTypeId { get; set; }
    public int? spcGroupVariableTypeId { get; set; }
    public int? samplingReferenceVarId { get; set; }
    public int? stringSpecificationSetting { get; set; }
    public int? writeGroupDSId { get; set; }
    public byte? reloadFlag { get; set; }
    public int? readLagTime { get; set; }
    public byte? eventLookup { get; set; }
    public byte? ignoreEventStatus { get; set; }

}

public class Response_UDP_DTO{
    public int tableFieldId {get;set;}
    public int tableId {get;set;}
    public string value {get;set;}
    public int keyId {get;set;}
    public string tableFieldDesc {get;set;}
    public string fieldTypeDesc {get;set;}

}

public class Response_Find_Child_DTO{
    public int varId { get; set; }
    public string varDesc { get; set; }
    public string? varDescGlobal { get; set; }
    public int puId { get; set; }
    public int pugId { get; set; }
    public int pugOrder { get; set; }
    public int dsId {get; set; }
    public int dataTypeId {get; set; }

    public byte? varPrecision {get; set; }
    public string? engUnits {get; set; }

    public int? groupId {get; set; }

    public string puDesc{ get; set; }
    public string? externalLink {get; set; }

}

public class Response_Variable_Sheet_DTO{
    public int sheetId { get; set; }
    public string sheetDesc { get; set; }
    public byte? sheetTypeId { get; set; }
    public string? sheetTypeDesc { get; set; }
    
    public int varOrder { get; set; }
}

public class Response_Variable_Template_DTO{
    public int atId { get; set; }
    public string atDesc { get; set; }
}


public class Response_Display_Template_DTO{
    public int sheetId { get; set; }
    public string sheetDesc { get; set; }
    public byte? sheetTypeId { get; set; }
    public string? sheetTypeDesc { get; set; }
    
    public int varOrder { get; set; }
    public int atId { get; set; }
    public string atDesc { get; set; }
}
public class AlterVariableDto{
    public int varId { get; set; } 
    public string varDesc { get; set; }
    public string varDescGlobal { get; set; }
}

public class Response_Sheet_Variable_DTO{
    public int varId { get; set; }
    public string varDesc { get; set; }
    public string plDesc { get; set; }
    public int plId { get; set; }
    public string puDesc { get; set; }
    public int puId { get; set; }
    public string pugDesc { get; set; }
    public int pugId { get; set; }
    public int sheetId { get; set; }
    public string title { get; set; }
    public string sheetDesc { get; set; }

    public int varOrder { get; set; }

}
