using System.Linq;
using Models;

namespace DTOs;

public class Create_Variable_DTO{
    public Variable variable {get;set;}
    public AlarmTemplate alarmTemplate {get;set;}
    public Sheet alarmDisplay {get;set;}
    public Sheet qAlarmDisplay {get;set;}
    
    public AutologDisplay autologDisplay {get;set;}
    public UDP[] udpList {get;set;}
    
    public Variable[]? subGroupSizeVariable {get;set;}

    public CalculationInputData[] calculationInputs {get;set;}
    
}