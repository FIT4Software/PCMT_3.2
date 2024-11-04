namespace query;

public class Queries {
    public static readonly string UpdateVariableQuery = @"
        SET NOCOUNT ON

        DECLARE 
        @intOrder 					int,
        @intOldSpecId				int,
        @dtmTimestamp				datetime,
        @FieldName					varchar(50),
        @intPU_Id					int,
        @intALTERUnit				int,
        @TableId						int,
        @FieldId						int,
        @intTableId					INTEGER,
        @intQAVarId					INTEGER,
        @intPropID					INTEGER,
        @intQASpecID				INTEGER,
        @intMaxOrder				INTEGER,
        @intMaxVarId				INTEGER,
        @intDifference				INTEGER,
        @intOffset					INTEGER,
        @vcrIndividualIDs 		VARCHAR(8000),
        @vcrIndividualValues		VARCHAR(8000),
        @vcrIndividualVarDesc	VARCHAR(255),
        @intIndividualVarId		INTEGER,
        @intCounter					INTEGER,

        --> Added by PDD
        @vcrVarNameTemp			VARCHAR(50),
        @intChildVarId 			INTEGER,
        @vcrSSNumber				VARCHAR(2),
        @vcrOldVarDesc				VARCHAR(50),
            
        @vcrChildGlobalDesc		VARCHAR(255),
        @vcrChildLocalDesc		VARCHAR(255),
        @vcrChildName				VARCHAR(255),

        @DoInsert						BIT,
        @Old_AT_Id						INT,
        @Old_Sheet_Id					INT,
        @intAlarm_Display_Order		INT,
        @Down								INT,
        @NextVarOrder					INT,
        @OldVarOrder					INT,
        @intPerformEventLookup		INT = 1

        DECLARE	--SPC Calcs
        --@vcrVarNameTemp			VARCHAR(50),
        --@intChildVarId 			INTEGER,
        @intCalcId 				INTEGER,
        @intRangeVarId 			INTEGER,
        @intStdDevVarId 		INTEGER,
        @intMRVarId 			INTEGER,
        --@vcrSSNumber			VARCHAR(2),
        @vcrParentCalcName		VARCHAR(255),
        --@vcrChildName			VARCHAR(255),
        --@vcrChildGlobalDesc		VARCHAR(255),
        --@vcrChildLocalDesc		VARCHAR(255),
        @vcrOldGlobalDesc		VARCHAR(255),
        @vcrUD1					VARCHAR(255),
        @vcrUD2					VARCHAR(255),
        @vcrUD3					VARCHAR(255),
        @vcrEI					VARCHAR(255),
        @bitESign				BIT,
        @vcrEngUnits			VARCHAR(255),
        @vcrExtLink				VARCHAR(255),
        @intSamplingInterval	INTEGER,
        @intSamplingOffset		INTEGER,
        @IsFirst				INT,
        @IsLast					INT,
        @intMaxCounter			INT,
        @intTempVarId			INT,
        @intTempVarOrder		INT,
        @intTableFieldId		INT,
        @vchTableFieldValue		VARCHAR(8000)

        DECLARE -- Variables for PutVarSheetData
            @DataType		INT,
            @Precision		INT,
            @SInterval		INT,
            @SOffset		INT,
            @SType			TINYINT,
            @EngUnits		INT,
            @DSId			INT,
            @EventType		INT,
            @UnitReject		INT,
            @USummarize		BIT,
            @VReject		BIT,
            @Rank			INT,
            @InputTag		VARCHAR(255),
            @OutTag			VARCHAR(255),
            @DQTag			VARCHAR(255),
            @UELTag			VARCHAR(255),
            @URLTag			VARCHAR(255),
            @UWLTag			VARCHAR(255),
            @UULTag			VARCHAR(255),
            @TargetTag		VARCHAR(255),
            @LULTag			VARCHAR(255),
            @LWLTag			VARCHAR(255),
            @LRLTag			VARCHAR(255),
            @LELTag			VARCHAR(255),
            @TotFactor		REAL,
            @SGroupId		INT,
            @SpecId			INT,
            @SAId			INT,
            @Repeat			BIT,
            @RBackTime		INT,
            @SWindow		INT,
            @SArchive		BIT,
            @ExtInfo		VARCHAR(255),
            @UDefined1		VARCHAR(255),
            @UDefined2		VARCHAR(255),
            @UDefined3		VARCHAR(255),
            @TFReset		TINYINT,
            @ForceSign		TINYINT,
            @TestName		VARCHAR(50),
            @ExtTF			INT,
            @ArStatOnly		BIT,
            @CTID			INT,
            @CV				VARCHAR(50),
            @MaxRPM			FLOAT,
            @ResetValue		FLOAT,
            @IsConfVar		INT,
            @ESignLevel		INT,
            @ESubtypeId		INT,
            @EDimension		TINYINT,
            @PEIId			INT,
            @SPCCalcTypeId	INT,
            @SPCVarTypeId	INT,
            @InputTag2		INT,
            @SampRefVarId	INT,
            @StrSpecSet		TINYINT,
            @WGroupDSId		INT,
            @CPKSGroupSize	INT,
            @RLagTime		INT,
            @ReloadFlag		INT,
            @ELookup		INT,
            @Debug			INT,
            @IgnoreEStatus	INT,
            @VarId			INT


        CREATE TABLE #TempOrder(
            New_Order	INT IDENTITY,
            Old_Order	INT,
            Var_Id		INT
        )
            
        CREATE TABLE #SheetVariables  (
            RcdIdx			INT IDENTITY,
            SheetId			INT,
            VarId			INT,
            VarOrder		INT)
            
        CREATE TABLE #SheetVariables1  (
            RcdIdx			INT IDENTITY,
            SheetId			INT,
            VarId			INT,
            VarOrder		INT)

        -- contains all line descriptions
        DECLARE @PlDescs TABLE (
            LineDesc		varchar(50))
            
        CREATE TABLE #TableFieldIds(
        ItemId				INTEGER IDENTITY(1, 1) NOT NULL,
        TableFieldId			INTEGER
        )

        CREATE TABLE #TableFieldValues(
        ItemId				INTEGER IDENTITY(1, 1) NOT NULL,
        TableFieldValue		VARCHAR(8000)
        )

        CREATE TABLE #TableFieldValuesTemp(
        RcdIdx				INT IDENTITY(1, 1) NOT NULL,
        KeyId					INT,
        TableFieldId			INT, 
        TableId				INT, 
        TableFieldValue		VARCHAR(8000)
        )

        CREATE TABLE #Extended_Infos(
            Item_Id				INTEGER,
            Var_Name				VARCHAR(255),
            EI						VARCHAR(255),
            UD1					VARCHAR(255),
            UD2					VARCHAR(255),
            UD3					VARCHAR(255))

        CREATE TABLE #AllUDPs(
            Item_Id				INTEGER,
            Var_Name				VARCHAR(255),
            UDPs					VARCHAR(7750))



        IF @intPVar_Id = 0 BEGIN
            SET @intPVar_Id = NULL
        END

        SET @intALTERUnit = 0

        -- retreive the pu_id from unit description
        SET @intPU_Id = (SELECT pu_id FROM dbo.Prod_Units_Base (NOLOCK) WHERE pu_desc = @vcrPUDesc AND pl_id = @intPLId)

        IF @intPU_Id IS NULL
        BEGIN
            SET @intALTERUnit = 1	
        END
        ELSE
        BEGIN
            -- verify if unit already exists on the line
            IF NOT EXISTS(SELECT pl_id FROM dbo.Prod_Units_Base (NOLOCK) WHERE pu_id = @intPU_Id and pl_id = @intPLId)
            BEGIN
                SET @intALTERUnit = 1	
            END
        END
        --SELECT 'STEP 1'

        IF @intALTERUnit = 1
        BEGIN
            -- get all line descriptions
            INSERT INTO @PlDescs (LineDesc)
                SELECT pl_desc 
                FROM dbo.Prod_Lines_Base (NOLOCK) 

            -- verify if the unit desc contains a line desc, if so replaces the line desc in the unit desc
            IF (SELECT COUNT(LineDesc) 
                FROM @PlDescs 
                WHERE LineDesc LIKE '%' + SUBSTRING(@vcrPUDesc, 1, CHARINDEX(' ', @vcrPUDesc, 1) - 1) + '%') > 0
            BEGIN
                SET @vcrPUDesc = (SELECT pl_desc FROM dbo.Prod_Lines_Base (NOLOCK) WHERE pl_id = @intPLId) + ' ' + 
                                    SUBSTRING(@vcrPUDesc, CHARINDEX(' ', @vcrPUDesc, 1) + 1,
                                                                    LEN(@vcrPUDesc) - CHARINDEX(' ', @vcrPUDesc, 1) + 1)

            END

            -- verify if the unit already exists on the line
            IF NOT EXISTS(SELECT pl_id FROM dbo.Prod_Units_Base (NOLOCK) WHERE pu_desc = @vcrPUDesc AND pl_id = @intPLId)
            BEGIN
                -- ALTER the production unit
                SELECT @intOrder = ISNULL(MAX(pu_order),0) + 1
                FROM dbo.Prod_Units_Base (NOLOCK)
                WHERE pl_id = @intPLId

                
                ---- create the production unit
                EXEC dbo.spEM_CreateProdUnit @vcrPUDesc,@intPLId,@intUser_Id,@intPU_Id

                -- Get the Id of the new prod unit ALTER
            SELECT @intPU_Id = pu_id
            FROM dbo.Prod_Units_Base (NOLOCK)
            WHERE pu_desc = @vcrPUDesc
            END
            ELSE
            BEGIN
                -- Get the Id of the existing unit
            SELECT @intPU_Id = pu_id
            FROM dbo.Prod_Units_Base (NOLOCK)
            WHERE pu_desc = @vcrPUDesc
            END
        END
        --SELECT 'STEP 2'


        -- retreive pug_id on the unit
        SET @intPUG_Id = (SELECT pug_id FROM dbo.pu_groups (NOLOCK) WHERE pug_desc = @vcrPug_Desc AND pu_id = @intPU_Id)

        --Manage the production group
        IF (@intPUG_Id IS NULL) AND (@vcrPug_Desc IS NOT NULL)
        BEGIN
            SELECT @intOrder = ISNULL(MAX(pug_order),0) + 1
            FROM dbo.PU_Groups (NOLOCK)
            WHERE pu_id = @intPu_Id

            EXEC dbo.spEM_CreatePUG
                @vcrPug_Desc		--@Description 
                ,@intPu_Id			--@PU_Id		
                ,@intOrder			--@PUG_Order	
                ,@intUser_Id		--@User_Id 		
                ,@intPug_Id			--@PUG_Id	
        
            
            SELECT @intPug_Id = PUG_Id
            FROM dbo.PU_Groups (NOLOCK)
            WHERE (Pu_Id = @intPu_Id) AND (PUG_Desc = @vcrPug_Desc)

        END

        --SELECT 'STEP 3'
        --Get the old spec_id
        SET @intOldSpecId = (SELECT Spec_Id FROM dbo.Variables_Base (NOLOCK) WHERE Var_Id = @intVar_id)

        -- Check if is a Historian variable 
        IF (@vcrInput_Tag IS NOT NULL) 
        BEGIN
            SET @intPerformEventLookup = 0
        END

        --Update the variable
        SELECT	@VarId				= @intVar_id						
                ,@DataType			= @intData_Type_Id					
                ,@Precision			= @tintVar_Precision				
                ,@SInterval			= @sintSampling_Interval			
                ,@SOffset			= @sintSampling_Offset				
                ,@SType				= @intST_Id							
                ,@EngUnits			= @vcrEng_Units						
                ,@DSId				= @intDS_Id							
                ,@EventType			= @tintEvent_Type					
                ,@UnitReject		= 0								
                ,@USummarize		= 0								
                ,@VReject			= 0									
                ,@Rank				= 0									
                ,@InputTag			= @vcrInput_Tag						
                ,@OutTag			= @vcrOutput_Tag					
                ,@DQTag				= @vcrDQ_Tag						
                ,@UELTag			= NULL								
                ,@URLTag			= NULL								
                ,@UWLTag			= NULL								
                ,@UULTag			= NULL								
                ,@TargetTag			= NULL								
                ,@LULTag			= NULL								
                ,@LWLTag			= NULL								
                ,@LRLTag			= NULL								
                ,@LELTag			= NULL								
                ,@TotFactor			= 1									
                ,@SGroupId			= NULL								
                ,@SpecId			= @intSpec_Id						
                ,@SAId				= @tintSA_Id						
                ,@Repeat			= @tintRepeating					
                ,@RBackTime			= @intRepeat_Backtime				
                ,@SWindow			= NULL								
                ,@SArchive			= 1									
                ,@ExtInfo			= @vcrExtendedInfo					
                ,@UDefined1			= @vcrUserDefined1					
                ,@UDefined2			= @vcrUserDefined2					
                ,@UDefined3			= @vcrUserDefined3					
                ,@TFReset			= 0									
                ,@ForceSign			= @bitForceSignEntry				
                ,@TestName			= @vcrTestName						
                ,@ExtTF				= 1									
                ,@ArStatOnly		= 0									
                ,@CTId				= @vcrComparison_Operator_Id		
                ,@CV				= @vcrComparison_Value				
                ,@MaxRPM			= NULL								
                ,@ResetValue		= NULL								
                ,@IsConfVar			= 1									
                ,@ESignLevel		= NULL								
                ,@ESubtypeId		= NULL								
                ,@EDimension		= NULL								
                ,@PEIId				= NULL								
                ,@SPCCalcTypeId		= NULL								
                ,@SPCVarTypeId		= NULL								
                ,@InputTag2			= NULL								
                ,@SampRefVarId		= NULL								
                ,@StrSpecSet		= NULL								
                ,@WGroupDSId		= NULL								
                ,@CPKSGroupSize		= NULL								
                ,@RLagTime			= NULL								
                ,@ReloadFlag		= NULL								
                ,@ELookup			= @intPerformEventLookup			
                ,@Debug				= NULL								
                ,@IgnoreEStatus		= NULL								
                                                                        
        EXEC spEM_PutVarSheetData
                @VarId,
                @DataType,
                @Precision,
                @SInterval,
                @SOffset,
                @SType,
                @EngUnits,
                @DSId,
                @EventType,
                @UnitReject,
                @USummarize,
                @VReject,
                @Rank,
                @InputTag,
                @OutTag,
                @DQTag,
                @UELTag,
                @URLTag,
                @UWLTag,
                @UULTag,
                @TargetTag,
                @LULTag,
                @LWLTag,
                @LRLTag,
                @LELTag,
                @TotFactor,
                @SGroupId,
                @SpecId,
                @SAId,
                @Repeat,
                @RBackTime,
                @SWindow,
                @SArchive,
                @ExtInfo,
                @UDefined1,
                @UDefined2,
                @UDefined3,
                @TFReset,
                @ForceSign,
                @TestName,
                @ExtTF,
                @ArStatOnly,
                @CTId,
                @CV,
                @MaxRPM,
                @ResetValue,
                @IsConfVar,
                @ESignLevel,
                @ESubtypeId,
                @EDimension,
                @PEIId,
                @SPCCalcTypeId,
                @SPCVarTypeId,
                @InputTag2,
                @SampRefVarId,
                @StrSpecSet,
                @WGroupDSId,
                @CPKSGroupSize,
                @RLagTime,
                @ReloadFlag,
                @ELookup,
                @Debug,
                @IgnoreEStatus,
                @intUser_Id

        /*********************************************************************************/


        exec dbo.spEM_PutExtLink @VarId, 'ag', @vcrExternal_Link, NULL,NULL, @intUser_Id


        --> Added by PDD
        IF @intSPCTypeId = -1 AND @intSGSize <> 0 BEGIN

            --Downsizing subgroup size if needed
            IF @intSGSize < 0 BEGIN

                SET @intCounter = 0

                --For each variable we want to get rid off
                WHILE @intCounter > @intSGSize BEGIN

                    --Get the variable ID
                    IF @intCounter < 10 BEGIN
                        SET @vcrSSNumber = '0' + CAST(@intCounter AS VARCHAR(2)) END
                    ELSE BEGIN
                        SET @vcrSSNumber = CAST(@intCounter AS VARCHAR(2))
                    END
                    SET @vcrVarNameTemp = @vcrVar_Desc + '-' + @vcrSSNumber	
                    SET @intChildVarId = NULL
                    SELECT @intChildVarId = MAX(var_id) FROM dbo.Variables_Base (NOLOCK) WHERE PVar_Id = @intVar_id		

                    --Dropping variable
                    IF @intChildVarId IS NOT NULL BEGIN
                        EXECUTE spEM_DropVariable @intChildVarId, @intUser_Id		--Variable itself
                        EXECUTE spEM_DropVariableSlave @intChildVarId	--Sheet and calculation instances

                    END

                    SET @intCounter = @intCounter - 1
                END
            END
        END

        --Upsizing subgroup size if needed
            SET @intCounter = 1
            SET @intChildVarId = 0
            --For each variable in the subgroup
            WHILE @intCounter <= @intSGSize BEGIN

                --Get the variable name...
                IF @intCounter < 10 BEGIN
                    SET @vcrSSNumber = '0' + CAST(@intCounter AS VARCHAR(2)) END
                ELSE BEGIN
                    SET @vcrSSNumber = CAST(@intCounter AS VARCHAR(2))
                END

                SET @intChildVarId = (SELECT MIN(Var_Id) FROM dbo.Variables_Base (NOLOCK) WHERE PVar_Id = @intVar_id AND Var_Id > @intChildVarId AND Calculation_Id IS NULL)
                
                IF @intChildVarId IS NOT NULL BEGIN
                    SET @vcrChildName = (SELECT Var_Desc FROM dbo.Variables_Base (NOLOCK) WHERE Var_Id  = @intChildVarId )

                    IF @vcrChildName <> @vcrOldVarDesc + '-' + @vcrSSNumber 
                    BEGIN
                        SELECT @vcrChildLocalDesc = Var_Desc, 
                            @vcrChildGlobalDesc = Var_Desc_Global 
                            FROM dbo.Variables_Base (NOLOCK) 
                            WHERE Var_Id = @intChildVarId 
                    END		
                    ELSE 
                    BEGIN
                        SET @vcrChildGlobalDesc = @vcrGlobalDesc + '-' + @vcrSSNumber
                        SET @vcrChildLocalDesc = @vcrVar_Desc + '-' + @vcrSSNumber
                    END 
                END
                ELSE 
                BEGIN
                    SET @vcrChildGlobalDesc = @vcrGlobalDesc + '-' + @vcrSSNumber
                    SET @vcrChildLocalDesc = @vcrVar_Desc + '-' + @vcrSSNumber	
                END

                --If variable does not exist then ALTER
                IF @intChildVarId IS NULL 
                BEGIN
                    EXECUTE spEM_ALTERChildVariable 
                            @vcrChildLocalDesc, @intVar_id, -1, @intDS_Id, @intData_Type_Id, @tintEvent_Type, 
                            @tintVar_Precision, 1, @intSpec_Id, @sintSampling_Interval, @intUser_Id, @intChildVarId OUTPUT
                END

                EXEC spEM_PutVarSheetData	
                    @intChildVarId									--@VarId
                    ,2												--@DataType,			
                    ,@tintVar_Precision								--@Precision,			
                    ,@intSamplingInterval							--@SInterval,			
                    ,@intSamplingOffset								--@SOffset,			
                    ,NULL											--@SType,
                    ,@vcrEngUnits									--@EngUnits,			
                    ,16												--@DSId,				
                    ,@tintEvent_Type								--@EventType,			
                    ,0											--@UnitReject,
                    ,0												--@USummarize,		
                    ,0											--@VReject,			
                    ,0											--@Rank,				
                    ,NULL											--@InputTag,			
                    ,NULL											--@OutTag,			
                    ,NULL											--@DQTag,				
                    ,NULL											--@UELTag,
                    ,NULL											--@URLTag,
                    ,NULL											--@UWLTag,
                    ,NULL											--@UULTag,
                    ,NULL											--@TargetTag,
                    ,NULL											--@LULTag,
                    ,NULL											--@LWLTag,
                    ,NULL											--@LRLTag,
                    ,NULL											--@LELTag,
                    ,1											--@TotFactor,			
                    ,NULL											--@SGroupId,			
                    ,NULL											--@SpecId,			
                    ,1											--@SAId,				
                    ,NULL											--@Repeat,			
                    ,NULL											--@RBackTime,			
                    ,NULL											--@SWindow,
                    ,1											--@SArchive,			
                    ,@vcrEI											--@ExtInfo,			
                    ,@vcrUD1										--@UDefined1,			
                    ,@vcrUD2										--@UDefined2,			
                    ,@vcrUD3										--@UDefined3,			
                    ,0											--@TFReset,			
                    ,NULL											--@ForceSign,			
                    ,@vcrTestName									--@TestName,			
                    ,1											--@ExtTF,				
                    ,0											--@ArStatOnly,		
                    ,NULL											--@CTId,
                    ,NULL											--@CV,
                    ,NULL											--@MaxRPM,			
                    ,NULL											--@ResetValue,
                    ,1											--@IsConfVar,			
                    ,NULL											--@ESignLevel,
                    ,NULL											--@ESubtypeId,		
                    ,NULL											--@EDimension,
                    ,NULL											--@PEIId,
                    ,NULL											--@SPCCalcTypeId,		
                    ,2												--@SPCVarTypeId,
                    ,NULL											--@InputTag2,
                    ,NULL											--@SampRefVarId,
                    ,NULL											--@StrSpecSet,
                    ,NULL											--@WGroupDSId,
                    ,NULL											--@CPKSGroupSize,
                    ,NULL											--@RLagTime,
                    ,NULL											--@ReloadFlag,
                    ,@intPerformEventLookup											--@ELookup,
                    ,NULL											--@Debug,
                    ,NULL											--@IgnoreEStatus,
                    ,@intUser_Id									--@UserId

                SET @intCounter = @intCounter + 1

            END
        --END
        /*********************************************************************************/


        IF @intSPCTypeId <> -1 AND @intPVar_Id IS NULL
        BEGIN
                SELECT	@vcrTestName = Test_Name, 
                            @vcrUD1 = User_Defined1, 
                            @vcrUD2 = User_Defined2, 
                            @vcrUD3 = User_Defined3, 
                            @vcrEI = Extended_Info,
                            @bitESign = Force_Sign_Entry, 
                            @vcrEngUnits = Eng_Units,
                            @vcrExtLink = External_Link,
                            @intSamplingInterval = Sampling_Interval,
                            @intSamplingOffset = Sampling_Offset
                FROM 
                            dbo.Variables_Base (NOLOCK) 
                WHERE 
                            Var_Id = @intVar_id

                --Downsizing subgroup size if needed
                IF @intSGSize < 0 BEGIN

                    SET @intCounter = 0

                    --For each variable we want to get rid off
                    WHILE @intCounter > @intSGSize BEGIN

                        --Get the variable ID
                        IF @intCounter < 10 BEGIN
                            SET @vcrSSNumber = '0' + CAST(@intCounter AS VARCHAR(2)) END
                        ELSE BEGIN
                            SET @vcrSSNumber = CAST(@intCounter AS VARCHAR(2))
                        END
                        SET @vcrVarNameTemp = @vcrVar_Desc + '-' + @vcrSSNumber	
                        SET @intChildVarId = NULL

                        SELECT @intChildVarId = MAX(var_id) FROM dbo.variables_Base (NOLOCK) WHERE PVar_Id = @intVar_id		
            
                        --Dropping variable
                        IF @intChildVarId IS NOT NULL BEGIN
                            EXECUTE spEM_DropVariable @intChildVarId, @intUser_Id		--Variable itself
                            EXECUTE spEM_DropVariableSlave @intChildVarId				--Sheet and calculation instances

                        END

                        SET @intCounter = @intCounter - 1

                    END

                END
                
                SELECT @vcrParentCalcName = 
                    CASE 
                        WHEN @intSPCTypeId = 2 THEN 'MSI_UchartTotal'
                        WHEN @intSPCTypeId = 3 THEN 'MSI_PchartTotal'
                        ELSE 'MSI_Calc_Average'
                    END

                --Setting average calculation on the parent variable
                EXECUTE spEMCC_FindCalcByName @vcrParentCalcName, @intCalcId OUTPUT
                EXECUTE spEMCC_BuildDataSetUpdate 94, @intCalcId, @intVar_id, @intSPCTypeId, NULL, '', '', @intUser_Id

                IF @intSPCTypeId NOT IN (1, 2, 3, 7) BEGIN

                    --Creating variable for the range calculation (if needed)
                    SET @vcrVarNameTemp = @vcrVar_Desc + '-Range'
                    SELECT @intRangeVarId = var_id FROM dbo.variables_Base (NOLOCK) WHERE pvar_id = @intVar_id AND var_desc LIKE '%-Range' + '%'

                    IF @intRangeVarId IS NULL 
                    BEGIN
                        --PRINT 'Call spEM_ALTERChildVariable with ' +  @vcrVarNameTemp
                        -- Range
                        EXECUTE spEM_ALTERChildVariable 
                            @vcrVarNameTemp,				
                            @intVar_id, 
                            -1, 
                            16, 
                            2, 
                            @tintEvent_Type, 
                            @tintVar_Precision, 
                            2, 
                            NULL, 
                            1, 
                            @intUser_Id, 
                            @intRangeVarId OUTPUT
                    END

                    EXEC spEM_PutVarSheetData	
                        @intRangeVarId									--@VarId
                        ,2												--@DataType,			
                        ,@tintVar_Precision								--@Precision,			
                        ,@intSamplingInterval							--@SInterval,			
                        ,@intSamplingOffset								--@SOffset,			
                        ,NULL											--@SType,
                        ,@vcrEngUnits									--@EngUnits,			
                        ,16												--@DSId,				
                        ,@tintEvent_Type								--@EventType,			
                        ,0											--@UnitReject,
                        ,0												--@USummarize,		
                        ,0											--@VReject,			
                        ,0											--@Rank,				
                        ,NULL											--@InputTag,			
                        ,NULL											--@OutTag,			
                        ,NULL											--@DQTag,				
                        ,NULL											--@UELTag,
                        ,NULL											--@URLTag,
                        ,NULL											--@UWLTag,
                        ,NULL											--@UULTag,
                        ,NULL											--@TargetTag,
                        ,NULL											--@LULTag,
                        ,NULL											--@LWLTag,
                        ,NULL											--@LRLTag,
                        ,NULL											--@LELTag,
                        ,1											--@TotFactor,			
                        ,NULL											--@SGroupId,			
                        ,NULL											--@SpecId,			
                        ,1											--@SAId,				
                        ,NULL											--@Repeat,			
                        ,NULL											--@RBackTime,			
                        ,NULL											--@SWindow,
                        ,1											--@SArchive,			
                        ,@vcrEI											--@ExtInfo,			
                        ,@vcrUD1										--@UDefined1,			
                        ,@vcrUD2										--@UDefined2,			
                        ,@vcrUD3										--@UDefined3,			
                        ,0											--@TFReset,			
                        ,NULL											--@ForceSign,			
                        ,@vcrTestName									--@TestName,			
                        ,1											--@ExtTF,				
                        ,0											--@ArStatOnly,		
                        ,NULL											--@CTId,
                        ,NULL											--@CV,
                        ,NULL											--@MaxRPM,			
                        ,NULL											--@ResetValue,
                        ,1											--@IsConfVar,			
                        ,NULL											--@ESignLevel,
                        ,NULL											--@ESubtypeId,		
                        ,NULL											--@EDimension,
                        ,NULL											--@PEIId,
                        ,NULL											--@SPCCalcTypeId,		
                        ,2												--@SPCVarTypeId,
                        ,NULL											--@InputTag2,
                        ,NULL											--@SampRefVarId,
                        ,NULL											--@StrSpecSet,
                        ,NULL											--@WGroupDSId,
                        ,NULL											--@CPKSGroupSize,
                        ,NULL											--@RLagTime,
                        ,NULL											--@ReloadFlag,
                        ,@intPerformEventLookup											--@ELookup,
                        ,NULL											--@Debug,
                        ,NULL											--@IgnoreEStatus,
                        ,@intUser_Id									--@UserId


                    --Setting calculation on the range variable
                    EXECUTE spEMCC_FindCalcByName 'MSI_Calc_Range', @intCalcId OUTPUT
                    EXECUTE spEMCC_BuildDataSetUpdate 94, @intCalcId, @intRangeVarId, NULL, NULL, '', '', @intUser_Id
            
                    --Creating variable for the standard deviation calculation (if needed)
                    SET @vcrVarNameTemp = @vcrVar_Desc + '-StdDev'
                    SELECT @intStdDevVarId = var_id FROM dbo.variables_Base (NOLOCK) WHERE pvar_id = @intVar_id AND var_desc LIKE '%-StdDev' + '%' --var_desc = @vcrVarNameTemp AND pu_id = @intPUId

                    IF @intStdDevVarId IS NULL 
                    BEGIN
                        -- StdDev
                        EXECUTE spEM_ALTERChildVariable 
                            @vcrVarNameTemp, @intVar_id, -1, 16, 2, @tintEvent_Type, 
                            @tintVar_Precision, 3, NULL, 1, @intUser_Id, @intStdDevVarId OUTPUT
                    END

                    
                    EXEC spEM_PutVarSheetData	
                        @intStdDevVarId									--@VarId
                        ,2												--@DataType,			
                        ,@tintVar_Precision								--@Precision,			
                        ,@intSamplingInterval							--@SInterval,			
                        ,@intSamplingOffset								--@SOffset,			
                        ,NULL											--@SType,
                        ,@vcrEngUnits									--@EngUnits,			
                        ,16												--@DSId,				
                        ,@tintEvent_Type								--@EventType,			
                        ,0											--@UnitReject,
                        ,0											--@USummarize,		
                        ,0											--@VReject,			
                        ,0											--@Rank,				
                        ,NULL											--@InputTag,			
                        ,NULL											--@OutTag,			
                        ,NULL											--@DQTag,				
                        ,NULL											--@UELTag,
                        ,NULL											--@URLTag,
                        ,NULL											--@UWLTag,
                        ,NULL											--@UULTag,
                        ,NULL											--@TargetTag,
                        ,NULL											--@LULTag,
                        ,NULL											--@LWLTag,
                        ,NULL											--@LRLTag,
                        ,NULL											--@LELTag,
                        ,1											--@TotFactor,			
                        ,NULL											--@SGroupId,			
                        ,NULL											--@SpecId,			
                        ,1											--@SAId,				
                        ,NULL											--@Repeat,			
                        ,NULL											--@RBackTime,			
                        ,NULL											--@SWindow,
                        ,1											--@SArchive,			
                        ,@vcrEI											--@ExtInfo,			
                        ,@vcrUD1										--@UDefined1,			
                        ,@vcrUD2										--@UDefined2,			
                        ,@vcrUD3										--@UDefined3,			
                        ,0											--@TFReset,			
                        ,NULL											--@ForceSign,			
                        ,@vcrTestName									--@TestName,			
                        ,1											--@ExtTF,				
                        ,0											--@ArStatOnly,		
                        ,NULL											--@CTId,
                        ,NULL											--@CV,
                        ,NULL											--@MaxRPM,			
                        ,NULL											--@ResetValue,
                        ,1											--@IsConfVar,			
                        ,NULL											--@ESignLevel,
                        ,NULL											--@ESubtypeId,		
                        ,NULL											--@EDimension,
                        ,NULL											--@PEIId,
                        ,NULL											--@SPCCalcTypeId,		
                        ,2												--@SPCVarTypeId,
                        ,NULL											--@InputTag2,
                        ,NULL											--@SampRefVarId,
                        ,NULL											--@StrSpecSet,
                        ,NULL											--@WGroupDSId,
                        ,NULL											--@CPKSGroupSize,
                        ,NULL											--@RLagTime,
                        ,NULL											--@ReloadFlag,
                        ,@intPerformEventLookup											--@ELookup,
                        ,NULL											--@Debug,
                        ,NULL											--@IgnoreEStatus,
                        ,@intUser_Id									--@UserId

                    --Setting calculation on the standard deviation variable
                    EXECUTE spEMCC_FindCalcByName 'MSI_Calc_StDev', @intCalcId OUTPUT
                    EXECUTE spEMCC_BuildDataSetUpdate 94, @intCalcId, @intStdDevVarId, NULL, NULL, '', '', @intUser_Id

                
                    --Creating variable for the moving range calculation (if needed)
                    SET @vcrVarNameTemp = @vcrVar_Desc + '-MR'
                    SELECT @intMRVarId = var_id FROM dbo.Variables_Base (NOLOCK) WHERE pvar_id = @intVar_id AND var_desc LIKE '%-MR' + '%' --var_desc = @vcrVarNameTemp AND pu_id = @intPUId

                    IF @intMRVarId IS NULL 
                    BEGIN
                        --PRINT 'Call spEM_ALTERChildVariable with ' +  @vcrVarNameTemp
                        -- MR
                        EXECUTE spEM_ALTERChildVariable 
                            @vcrVarNameTemp, @intVar_id, -1, 16, 2, @tintEvent_Type, 
                            @tintVar_Precision, 4, NULL, 1, @intUser_Id, @intMRVarId OUTPUT
                    END
                    
                    EXEC spEM_PutVarSheetData	
                        @intMRVarId									--@VarId
                        ,2												--@DataType,			
                        ,@tintVar_Precision								--@Precision,			
                        ,@intSamplingInterval							--@SInterval,			
                        ,@intSamplingOffset								--@SOffset,			
                        ,NULL											--@SType,
                        ,@vcrEngUnits									--@EngUnits,			
                        ,16												--@DSId,				
                        ,@tintEvent_Type								--@EventType,			
                        ,0											--@UnitReject,
                        ,0											--@USummarize,		
                        ,0											--@VReject,			
                        ,0											--@Rank,				
                        ,NULL											--@InputTag,			
                        ,NULL											--@OutTag,			
                        ,NULL											--@DQTag,				
                        ,NULL											--@UELTag,
                        ,NULL											--@URLTag,
                        ,NULL											--@UWLTag,
                        ,NULL											--@UULTag,
                        ,NULL											--@TargetTag,
                        ,NULL											--@LULTag,
                        ,NULL											--@LWLTag,
                        ,NULL											--@LRLTag,
                        ,NULL											--@LELTag,
                        ,1											--@TotFactor,			
                        ,NULL											--@SGroupId,			
                        ,NULL											--@SpecId,			
                        ,1											--@SAId,				
                        ,NULL											--@Repeat,			
                        ,NULL											--@RBackTime,			
                        ,NULL											--@SWindow,
                        ,1											--@SArchive,			
                        ,@vcrEI											--@ExtInfo,			
                        ,@vcrUD1										--@UDefined1,			
                        ,@vcrUD2										--@UDefined2,			
                        ,@vcrUD3										--@UDefined3,			
                        ,0											--@TFReset,			
                        ,NULL											--@ForceSign,			
                        ,@vcrTestName									--@TestName,			
                        ,1											--@ExtTF,				
                        ,0											--@ArStatOnly,		
                        ,NULL											--@CTId,
                        ,NULL											--@CV,
                        ,NULL											--@MaxRPM,			
                        ,NULL											--@ResetValue,
                        ,1											--@IsConfVar,			
                        ,NULL											--@ESignLevel,
                        ,NULL											--@ESubtypeId,		
                        ,NULL											--@EDimension,
                        ,NULL											--@PEIId,
                        ,NULL											--@SPCCalcTypeId,		
                        ,2												--@SPCVarTypeId,
                        ,NULL											--@InputTag2,
                        ,NULL											--@SampRefVarId,
                        ,NULL											--@StrSpecSet,
                        ,NULL											--@WGroupDSId,
                        ,NULL											--@CPKSGroupSize,
                        ,NULL											--@RLagTime,
                        ,NULL											--@ReloadFlag,
                        ,@intPerformEventLookup											--@ELookup,
                        ,NULL											--@Debug,
                        ,NULL											--@IgnoreEStatus,
                        ,@intUser_Id									--@UserId

                    --Setting calculation on the moving range variable
                    EXECUTE spEMCC_FindCalcByName 'MSI_MovingRange', @intCalcId OUTPUT
                    EXECUTE spEMCC_BuildDataSet 99, @intCalcId, NULL, NULL, NULL, NULL, NULL, @intUser_Id

                    --Add inputs to the moving range calculation
                    EXECUTE spEMCC_BuildDataSetUpdate 35, @intCalcId, 19, @intMRVarId, @intRangeVarId, '', NULL, @intUser_Id
                    EXECUTE spEMCC_BuildDataSetUpdate 35, @intCalcId, 20, @intMRVarId, @intRangeVarId, '', NULL, @intUser_Id
                    EXECUTE spEMCC_BuildDataSetUpdate 94, @intCalcId, @intMRVarId, NULL, NULL, '', '', @intUser_Id

                END

                -- Before
                -- SET @intCounter = 1
                SET @intCounter = 1
                SET @intChildVarId = 0
                --For each variable in the subgroup
                WHILE @intCounter <= @intSGSize BEGIN

                    --Get the variable name...
                    IF @intCounter < 10 BEGIN
                        SET @vcrSSNumber = '0' + CAST(@intCounter AS VARCHAR(2)) END
                    ELSE BEGIN
                        SET @vcrSSNumber = CAST(@intCounter AS VARCHAR(2))
                    END

                    --... then the variable ID
                    SET @intChildVarId = NULL
                    SELECT @intChildVarId = var_id FROM dbo.variables_Base (NOLOCK) WHERE pvar_id = @intVar_id AND var_desc LIKE @vcrVar_Desc + '-' + @vcrSSNumber + '%' 

                    IF @intChildVarId IS NOT NULL BEGIN
                        SET @vcrChildName = (SELECT Var_Desc FROM dbo.Variables_Base (NOLOCK) WHERE Var_Id  = @intChildVarId )
                        IF @vcrChildName <> @vcrOldGlobalDesc + '-' + @vcrSSNumber 
                        BEGIN
                            SELECT	@vcrChildLocalDesc = Var_Desc, 
                                    @vcrChildGlobalDesc = Var_Desc_Global 
                                FROM dbo.Variables_Base (NOLOCK)
                                WHERE Var_Id = @intChildVarId END
                        ELSE 
                        BEGIN
                            SET @vcrChildGlobalDesc = @vcrGlobalDesc + '-' + @vcrSSNumber
                            SET @vcrChildLocalDesc = @vcrVar_Desc + '-' + @vcrSSNumber
                        END END
                    ELSE BEGIN
                        SET @vcrChildGlobalDesc = @vcrGlobalDesc + '-' + @vcrSSNumber
                        SET @vcrChildLocalDesc = @vcrVar_Desc + '-' + @vcrSSNumber	
                    END

                    --If variable does not exist then ALTER
                    IF @intChildVarId IS NULL 
                    BEGIN
                        -- Var Child
                        EXECUTE spEM_ALTERChildVariable 
                            @vcrChildLocalDesc, @intVar_id, -1, @intDS_Id, @intData_Type_Id, 
                            @tintEvent_Type, @tintVar_Precision, 1, @intSpec_Id, 1, @intUser_Id, @intChildVarId OUTPUT
                    END				
                    
                    EXEC spEM_PutVarSheetData	
                        @intChildVarId									--@VarId
                        ,2												--@DataType,			
                        ,@tintVar_Precision								--@Precision,			
                        ,@intSamplingInterval							--@SInterval,			
                        ,@intSamplingOffset								--@SOffset,			
                        ,NULL											--@SType,
                        ,@vcrEngUnits									--@EngUnits,			
                        ,16												--@DSId,				
                        ,@tintEvent_Type								--@EventType,			
                        ,0											--@UnitReject,
                        ,0											--@USummarize,		
                        ,0											--@VReject,			
                        ,0											--@Rank,				
                        ,NULL											--@InputTag,			
                        ,NULL											--@OutTag,			
                        ,NULL											--@DQTag,				
                        ,NULL											--@UELTag,
                        ,NULL											--@URLTag,
                        ,NULL											--@UWLTag,
                        ,NULL											--@UULTag,
                        ,NULL											--@TargetTag,
                        ,NULL											--@LULTag,
                        ,NULL											--@LWLTag,
                        ,NULL											--@LRLTag,
                        ,NULL											--@LELTag,
                        ,1											--@TotFactor,			
                        ,NULL											--@SGroupId,			
                        ,NULL											--@SpecId,			
                        ,1											--@SAId,				
                        ,NULL											--@Repeat,			
                        ,NULL											--@RBackTime,			
                        ,NULL											--@SWindow,
                        ,1											--@SArchive,			
                        ,@vcrEI											--@ExtInfo,			
                        ,@vcrUD1										--@UDefined1,			
                        ,@vcrUD2										--@UDefined2,			
                        ,@vcrUD3										--@UDefined3,			
                        ,0											--@TFReset,			
                        ,NULL											--@ForceSign,			
                        ,@vcrTestName									--@TestName,			
                        ,1											--@ExtTF,				
                        ,0											--@ArStatOnly,		
                        ,NULL											--@CTId,
                        ,NULL											--@CV,
                        ,NULL											--@MaxRPM,			
                        ,NULL											--@ResetValue,
                        ,1											--@IsConfVar,			
                        ,NULL											--@ESignLevel,
                        ,NULL											--@ESubtypeId,		
                        ,NULL											--@EDimension,
                        ,NULL											--@PEIId,
                        ,NULL											--@SPCCalcTypeId,		
                        ,2												--@SPCVarTypeId,
                        ,NULL											--@InputTag2,
                        ,NULL											--@SampRefVarId,
                        ,NULL											--@StrSpecSet,
                        ,NULL											--@WGroupDSId,
                        ,NULL											--@CPKSGroupSize,
                        ,NULL											--@RLagTime,
                        ,NULL											--@ReloadFlag,
                        ,@intPerformEventLookup											--@ELookup,
                        ,NULL											--@Debug,
                        ,NULL											--@IgnoreEStatus,
                        ,@intUser_Id									--@UserId

                    --Add input to average, range & standard deviation calculations
                    EXECUTE spEMCC_BuildDataByID 84, @intVar_id, @intChildVarId, 0, NULL, @intUser_Id
                    IF @intSPCTypeId NOT IN (1, 2, 3, 7) BEGIN
                        EXECUTE spEMCC_BuildDataByID 84, @intRangeVarId, @intChildVarId, 0, NULL, @intUser_Id
                        EXECUTE spEMCC_BuildDataByID 84, @intStdDevVarId, @intChildVarId, 0, NULL, @intUser_Id
                    END

                    SET @intCounter = @intCounter + 1

                END

                IF @intSPCTypeId = 3 BEGIN
                    EXECUTE spEMCC_BuildDataSet 99, @intCalcId, NULL, NULL, NULL, NULL, NULL, @intUser_Id
                    EXECUTE spEMCC_BuildDataSetUpdate 35, @intCalcId, 25, @intVar_id, 0, @vcrSPCFailure, NULL, @intUser_Id

                END
        END
            

        --Updating _AL variable
        SET @intQAVarId = (	SELECT var_id 
                                    FROM dbo.Variables_Base (NOLOCK) v LEFT JOIN dbo.pu_groups (NOLOCK) pug ON (v.pug_id = pug.pug_id) 
                                    WHERE pug.pug_desc like 'Q%Alarms' and v.pu_id = @intPU_Id AND v.extended_info = CAST(@intVar_id AS VARCHAR(10))
                                )
        IF @intQAVarId IS NOT NULL BEGIN

            SET @intPropID = (SELECT prop_id FROM dbo.specifications (NOLOCK) WHERE spec_id = @intSpec_Id)
            SET @intQASpecID = (SELECT spec_id FROM dbo.specifications (NOLOCK) WHERE prop_id = @intPropID AND spec_desc = 'FireAlarm')

            IF @intQASpecID IS NOT NULL 
            BEGIN
            
                    EXEC spEM_PutVarSheetData
                            @intQAVarId
                            ,2											--@DataType,			
                            ,2											--@Precision,			
                            ,0											--@SInterval,			
                            ,NULL										--@SOffset,			
                            ,NULL										--@SType,
                            ,NULL										--@EngUnits,			
                            ,4											--@DSId,				
                            ,0											--@EventType,			
                            ,0										--@UnitReject,
                            ,0										--@USummarize,		
                            ,0										--@VReject,			
                            ,0										--@Rank,				
                            ,NULL										--@InputTag,			
                            ,NULL										--@OutTag,			
                            ,NULL										--@DQTag,				
                            ,NULL										--@UELTag,
                            ,NULL										--@URLTag,
                            ,NULL										--@UWLTag,
                            ,NULL										--@UULTag,
                            ,NULL										--@TargetTag,
                            ,NULL										--@LULTag,
                            ,NULL										--@LWLTag,
                            ,NULL										--@LRLTag,
                            ,NULL										--@LELTag,
                            ,1										--@TotFactor,			
                            ,NULL										--@SGroupId,			
                            ,@intQASpecID								--@SpecId,			
                            ,1										--@SAId,				
                            ,NULL										--@Repeat,			
                            ,NULL										--@RBackTime,			
                            ,NULL										--@SWindow,
                            ,1										--@SArchive,			
                            ,@intVar_id							--@ExtInfo,			
                            ,NULL										--@UDefined1,			
                            ,NULL										--@UDefined2,			
                            ,NULL										--@UDefined3,			
                            ,0										--@TFReset,			
                            ,NULL										--@ForceSign,			
                            ,NULL										--@TestName,			
                            ,1										--@ExtTF,				
                            ,0										--@ArStatOnly,		
                            ,NULL										--@CTId,
                            ,NULL										--@CV,
                            ,NULL										--@MaxRPM,			
                            ,NULL										--@ResetValue,
                            ,1										--@IsConfVar,			
                            ,NULL										--@ESignLevel,
                            ,NULL										--@ESubtypeId,		
                            ,NULL										--@EDimension,
                            ,NULL										--@PEIId,
                            ,NULL										--@SPCCalcTypeId,		
                            ,NULL										--@SPCVarTypeId,
                            ,NULL										--@InputTag2,
                            ,NULL										--@SampRefVarId,
                            ,NULL										--@StrSpecSet,
                            ,NULL										--@WGroupDSId,
                            ,NULL										--@CPKSGroupSize,
                            ,NULL										--@RLagTime,
                            ,NULL										--@ReloadFlag,
                            ,@intPerformEventLookup										--@ELookup,
                            ,NULL										--@Debug,
                            ,NULL										--@IgnoreEStatus,
                            ,@intUser_Id								--@UserId
            
            END
            ELSE 
            BEGIN
                SET @intQAVarId = NULL
            END
                    
        END


            -------------------------------------------------------------------------------------------------------------------
            -------------------------------------------------------------------------------------------------------------------
            --Add variable to the alarm template
            -------------------------------------------------------------------------------------------------------------------
            -------------------------------------------------------------------------------------------------------------------
            SET @DoInsert = 0
            SET @Old_AT_Id = NULL
            SET @Old_AT_Id = (SELECT TOP 1 AT_Id FROM dbo.Alarm_Template_Var_Data (NOLOCK) WHERE Var_Id = @intVar_id)

            --We have a template where we need to insert
            IF @intAlarm_Template_Id IS NOT NULL BEGIN
            
                --We were having template data before
                IF @Old_AT_Id IS NOT NULL BEGIN

                    --We did not change template
                    IF @Old_AT_Id = @intAlarm_Template_Id BEGIN

                        --We are downsizing
                        IF @intSGSize < 0 BEGIN

                            SET @DoInsert = 1
                            
                        END

                        --We are upsizing
                        IF @intSGSize > 0 BEGIN

                            SET @DoInsert = 1

                        END END

                    --We changed template
                    ELSE BEGIN

                        SET @DoInsert = 1

                        --Removing parent and children from old template
                        EXEC dbo.spEMAC_DeleteAttachedVariables 
                            @Old_AT_Id		--@AT_Id int,
                            ,@intVar_Id					--@Var_Id int,
                            ,@intUser_Id				--@User_Id int,

                    END END

                --We were not having template data before
                ELSE BEGIN

                    SET @DoInsert = 1				
                
                END END	

            --We have no template where we need to insert
            ELSE BEGIN

                --We were having template data before
                IF @Old_AT_Id IS NOT NULL BEGIN

                    --Removing parent and children from old template			
                        EXEC dbo.spEMAC_DeleteAttachedVariables 
                            @Old_AT_Id		--@AT_Id int,
                            ,@intVar_Id					--@Var_Id int,
                            ,@intUser_Id				--@User_Id int,
                            
                END

            END 

            --We detected the need to insert template data
            IF @DoInsert = 1 
            BEGIN	
                    EXEC dbo.spEMAC_AddAttachedVariables 
                        @intAlarm_Template_Id		--@AT_Id int,
                        ,@intVar_id					--@Var_Id int,
                        ,NULL						--@EG_Id int,
                        ,@intUser_Id				--@User_Id int,
                        ,@intSGSize					--@SamplingSize Int = 0
                        
            END


        -------------------------------------------------------------------------------------------------------------------
        -------------------------------------------------------------------------------------------------------------------
        --Add variable to the alarm sheet
        -------------------------------------------------------------------------------------------------------------------
        -------------------------------------------------------------------------------------------------------------------
        SET @DoInsert = 0
        SET @Old_Sheet_Id = NULL
        SET @Old_Sheet_Id = (	SELECT TOP 1 SV.Sheet_Id 
                                        FROM 
                                            dbo.Sheet_Variables SV (NOLOCK)
                                            LEFT JOIN dbo.Sheets S (NOLOCK) ON (S.Sheet_Id = SV.Sheet_Id)
                                        WHERE 
                                            Var_Id = @intVar_id
                                            AND Sheet_Type = 11)

        SET @intAlarm_Display_Order = (SELECT MAX(Var_Order) + 1 FROM dbo.Sheet_Variables (NOLOCK) WHERE Sheet_Id = @intAlarm_Display_Id)

        --We have a sheet where we need to insert
        IF @intAlarm_Display_Id IS NOT NULL 
        BEGIN
            
            --We were having sheet data before
            IF @Old_Sheet_Id IS NOT NULL 
            BEGIN

                --We did not change sheet
                IF @Old_Sheet_Id = @intAlarm_Display_Id 
                BEGIN

                    --We are downsizing
                    IF @intSGSize < 0
                    BEGIN

                        SET @DoInsert = 1

                    END

                    --We are upsizing
                    IF @intSGSize > 0
                    BEGIN

                        SET @DoInsert = 1

                    END 
                END

                --We changed sheet
                ELSE 
                BEGIN

                    SET @DoInsert = 1
                    
                    TRUNCATE TABLE  #SheetVariables

                    INSERT INTO #SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK)
                    WHERE (Var_Id <> @intVar_Id AND Var_Id NOT IN (SELECT Var_Id FROM dbo.Variables_Base (NOLOCK) WHERE PVar_Id = @intVar_Id))
                        AND Sheet_Id = @Old_Sheet_Id
                    ORDER BY Var_Order
                    
                    SELECT @intCounter = 1
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),1) FROM #SheetVariables

                    WHILE @intCounter <= @intMaxCounter
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = @intCounter
                                ,@IsFirst = CASE WHEN @intCounter = 1 THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM #SheetVariables) THEN 1 ELSE 0 END
                        FROM #SheetVariables
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @Old_Sheet_Id			--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END
            
                END 
            END

            --We were not having sheet data before
            ELSE 
            BEGIN

                SET @DoInsert = 1				
                
            END END		

        --We have no sheet where we need to insert
        ELSE 
        BEGIN

            --We were having sheet data before
            IF @Old_Sheet_Id IS NOT NULL 
            BEGIN		
                    
                    TRUNCATE TABLE  #SheetVariables

                    INSERT INTO #SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK)
                    WHERE (Var_Id <> @intVar_Id AND Var_Id NOT IN (SELECT Var_Id FROM dbo.Variables_Base (NOLOCK) WHERE PVar_Id = @intVar_Id))
                        AND Sheet_Id = @Old_Sheet_Id
                    ORDER BY Var_Order
                    
                    SELECT @intCounter = 1
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),1) FROM #SheetVariables

                    WHILE @intCounter <= @intMaxCounter
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = @intCounter
                                ,@IsFirst = CASE WHEN @intCounter = 1 THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM #SheetVariables) THEN 1 ELSE 0 END
                        FROM #SheetVariables
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @Old_Sheet_Id			--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END

            END

        END 

        --We detected the need to insert sheet data
        IF @DoInsert = 1 
        BEGIN
            -------------------------------------------------------------------------------------------------------------------
            --We have a sheet where we need to insert
            
            -- Cambio alarmas
            -------------------------------------------------------------------------------------------------------------------
            -------------------------------------------------------------------------------------------------------------------
            -------------------------------------------------------------------------------------------------------------------
            --Add variable to the alarm display
            IF @intAlarm_Display_Id IS NOT NULL 
            BEGIN
                IF  @intSPCTypeId = -1
                --Not SPC
                BEGIN
                    TRUNCATE TABLE  #SheetVariables
                    TRUNCATE TABLE  #SheetVariables1

                    INSERT INTO #SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK)
                    WHERE sheet_id = @intAlarm_Display_Id
                    
                    SELECT @intMaxOrder = ISNULL(MAX(VarOrder),0) + 1 FROM #SheetVariables

                    INSERT INTO #SheetVariables(SheetId,VarId,VarOrder) VALUES (@intAlarm_Display_Id,@intVar_Id,@intMaxOrder)

                    SELECT @intCounter = ISNULL(MIN(RcdIdx),0) FROM #SheetVariables
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),0) FROM #SheetVariables

                    
                    WHILE @intCounter <= @intMaxCounter AND @intCounter <> 0 AND @intMaxCounter <> 0
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = VarOrder
                                ,@IsFirst = CASE WHEN @intCounter = (SELECT ISNULL(MIN(RcdIdx),0) FROM #SheetVariables) THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM #SheetVariables) THEN 1 ELSE 0 END
                        FROM #SheetVariables
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @intAlarm_Display_Id	--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END
                END
                --SPC
                ELSE BEGIN
                
                    TRUNCATE TABLE  #SheetVariables

                    INSERT INTO #SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK) 
                    WHERE sheet_id = @intAlarm_Display_Id
                    
                SET @intMaxOrder = (SELECT ISNULL(MAX(var_order),0) 
                                            FROM dbo.Sheet_Variables (NOLOCK)
                                            WHERE sheet_id = @intAlarm_Display_Id)
                    
                    SET @intDifference = @intVar_Id - @intMaxOrder
                
                INSERT #SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		@intAlarm_Display_Id
                                ,Var_Id
                                ,Var_Id - @intDifference + 1
                    FROM dbo.Variables_Base (NOLOCK)
                    WHERE
                        Var_Id = @intVar_Id OR PVar_Id = @intVar_Id
                    ORDER BY
                        Var_Desc

                    SELECT @intCounter = ISNULL(MIN(RcdIdx),0) FROM #SheetVariables
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),0) FROM #SheetVariables

                    
                    WHILE @intCounter <= @intMaxCounter AND @intCounter <> 0 AND @intMaxCounter <> 0
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = VarOrder
                                ,@IsFirst = CASE WHEN @intCounter = (SELECT ISNULL(MIN(RcdIdx),0) FROM #SheetVariables) THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM #SheetVariables) THEN 1 ELSE 0 END
                        FROM #SheetVariables
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @intAlarm_Display_Id	--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END
                END
            END
            -- Fin Cambio alarmas
            -------------------------------------------------------------------------------------------------------------------
            -------------------------------------------------------------------------------------------------------------------
            -------------------------------------------------------------------------------------------------------------------
        END

            -------------------------------------------------------------------------------------------------------------------
            -------------------------------------------------------------------------------------------------------------------
            --Add variable to the autolog sheet
            -------------------------------------------------------------------------------------------------------------------
            -------------------------------------------------------------------------------------------------------------------
            SET @DoInsert = 0
            SET @Old_Sheet_Id = NULL
            SET @Old_Sheet_Id = (	SELECT TOP 1 SV.Sheet_Id 
                                            FROM 
                                                dbo.Sheet_Variables SV (NOLOCK)
                                                LEFT JOIN dbo.Sheets S (NOLOCK) ON (S.Sheet_Id = SV.Sheet_Id)
                                            WHERE 
                                                Var_Id = @intVar_id
                                                AND Sheet_Type <> 11)

            --We have a sheet where we need to insert
            IF @intAutolog_Display_Id IS NOT NULL BEGIN

                --We were having sheet data before
                IF @Old_Sheet_Id IS NOT NULL BEGIN

                    --We did not change sheet
                    IF @Old_Sheet_Id = @intAutolog_Display_Id BEGIN

                        --We are downsizing
                        IF @intSGSize < 0 BEGIN

                            SET @DoInsert = 1

                        END

                        SELECT @OldVarOrder = Var_Order FROM dbo.Sheet_Variables (NOLOCK) WHERE Var_Id = @intVar_id AND Sheet_Id = @intAutolog_Display_Id

                        --We are upsizing
                        IF @intSGSize > 0 OR @OldVarOrder <> @intAutolog_Display_Order BEGIN

                            SET @DoInsert = 1

                        END END

                    --We changed sheet
                    ELSE BEGIN

                        SET @DoInsert = 1				
                    
                        TRUNCATE TABLE  #SheetVariables

                        INSERT INTO #SheetVariables (
                                    SheetId
                                    ,VarId
                                    ,VarOrder)
                        SELECT		Sheet_Id
                                    ,Var_Id
                                    ,Var_Order
                        FROM dbo.Sheet_Variables (NOLOCK)
                        WHERE (Var_Id <> @intVar_Id AND Var_Id NOT IN (SELECT Var_Id FROM dbo.Variables_Base (NOLOCK) WHERE PVar_Id = @intVar_Id))
                            AND Sheet_Id = @Old_Sheet_Id
                        ORDER BY Var_Order
                    
                        SELECT @intCounter = 1
                        SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),1) FROM #SheetVariables

                        WHILE @intCounter <= @intMaxCounter
                        BEGIN
                            SELECT   @intTempVarId = VarId
                                    ,@intTempVarOrder = @intCounter
                                    ,@IsFirst = CASE WHEN @intCounter = 1 THEN 1 ELSE 0 END
                                    ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM #SheetVariables) THEN 1 ELSE 0 END
                            FROM #SheetVariables
                            WHERE RcdIdx = @intCounter

                            EXEC dbo.spEM_PutSheetVariables
                                @Old_Sheet_Id			--@Sheet_Id 
                                ,@intTempVarOrder		--@Order	
                                ,@intTempVarId			--@Id		
                                ,NULL					--@Title	
                                ,@IsLast				--@IsLast	
                                ,@IsFirst				--@IsFirst
                                ,@intUser_Id				--@User_Id
                            
                            SET @intCounter = @intCounter + 1
                        END
            
                    END END

                --We were not having sheet data before
                ELSE BEGIN

                    SET @DoInsert = 1				
                
                END END	

            --We have no sheet where we need to insert
            ELSE BEGIN

                --We were having sheet data before
                IF @Old_Sheet_Id IS NOT NULL 
                BEGIN					
                    
                    TRUNCATE TABLE  #SheetVariables

                    INSERT INTO #SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK)
                    WHERE (Var_Id <> @intVar_Id AND Var_Id NOT IN (SELECT Var_Id FROM dbo.Variables_Base (NOLOCK) WHERE PVar_Id = @intVar_Id))
                        AND Sheet_Id = @Old_Sheet_Id
                    ORDER BY Var_Order
                    
                    SELECT @intCounter = 1
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),1) FROM #SheetVariables

                    WHILE @intCounter <= @intMaxCounter
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = @intCounter
                                ,@IsFirst = CASE WHEN @intCounter = 1 THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM #SheetVariables) THEN 1 ELSE 0 END
                        FROM #SheetVariables
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @Old_Sheet_Id			--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END

                END

            END 

            --We detected the need to insert sheet data
            IF @DoInsert = 1 BEGIN

                BEGIN
                    TRUNCATE TABLE #SheetVariables
                    TRUNCATE TABLE #SheetVariables1

                    INSERT INTO #SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK)
                    WHERE sheet_id = @intAutolog_Display_Id

                    
                    IF EXISTS (SELECT Var_Id
                                FROM dbo.Sheet_Variables (NOLOCK)
                                WHERE sheet_id = @intAutolog_Display_Id and
                                    var_order = @intAutolog_Display_Order) 
                    BEGIN
            
                        SET @intOffset = (SELECT COUNT(Var_Id) FROM dbo.Variables_Base (NOLOCK) WHERE Var_Id = @intVar_id OR pvar_id = @intVar_id)	
                        
                        UPDATE #SheetVariables
                            SET VarOrder = VarOrder + @intOffset
                        WHERE 
                            SheetId = @intAutolog_Display_Id and
                            VarOrder >= @intAutolog_Display_Order		
                            
                    END
                    
                    SET @intDifference = @intVar_id - @intAutolog_Display_Order
                        
                    INSERT #SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		@intAutolog_Display_Id
                                ,Var_Id
                                ,Var_Id - @intDifference			
                    FROM dbo.Variables_Base (NOLOCK)
                    WHERE var_id = @intVar_id OR pvar_id = @intVar_id
                    ORDER BY var_desc
                                
                    INSERT INTO #SheetVariables1(SheetId,VarId,VarOrder) SELECT SheetId,VarId,VarOrder FROM #SheetVariables ORDER BY VarOrder
                    
                    SELECT @intCounter = ISNULL(MIN(RcdIdx),0) FROM #SheetVariables1
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),0) FROM #SheetVariables1

                    WHILE @intCounter <= @intMaxCounter AND @intCounter <> 0 AND @intMaxCounter <> 0
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = VarOrder
                                ,@IsFirst = CASE WHEN @intCounter = (SELECT ISNULL(MIN(RcdIdx),0) FROM #SheetVariables1) THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM #SheetVariables1) THEN 1 ELSE 0 END
                        FROM #SheetVariables1
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @intAutolog_Display_Id	--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END
                END	

            END

            DROP TABLE #TempOrder




            --Update the var_specs table if there is var_specs 
            IF @intSpec_Id IS NOT NULL
            BEGIN 
                EXEC dbo.spEM_PutSpecVariableData
                    @intVar_id			--@Var_Id            int,
                    ,@intSpec_Id			--@Spec_Id           int,
                    ,@intUser_Id			--@User_Id int
            END
            
            
            --Add Table_Field Values
            SET @intTableId = (SELECT TableId FROM dbo.Tables (NOLOCK) WHERE TableName = 'Variables')

            INSERT #TableFieldIds(TableFieldId)
            SELECT value FROM STRING_SPLIT(REPLACE(@vcrTableFieldIDs,'[REC]',','),',')

            INSERT #TableFieldValues(TableFieldValue)
            SELECT value FROM STRING_SPLIT(REPLACE(@vcrTableFieldValues,'[REC]',','),',')

            INSERT #TableFieldValuesTemp (KeyId, TableFieldId, TableId, TableFieldValue)
            SELECT Var_Id, TableFieldId, @intTableId, TableFieldValue
                FROM #TableFieldIds tfi 
                JOIN #TableFieldValues tfv ON (tfi.ItemId = tfv.ItemId),
                    dbo.Variables_Base (NOLOCK)
                    WHERE Var_Id = @intVar_id OR PVar_Id = @intVar_id
                        
            SET @intCounter = 1     

            WHILE @intCounter <= (SELECT COUNT(*) FROM #TableFieldValuesTemp)
            BEGIN
                SELECT	@intTableFieldId = TableFieldId
                        ,@vchTableFieldValue = TableFieldValue
                FROM #TableFieldValuesTemp
                WHERE RcdIdx = @intCounter

                EXEC dbo.spEMTFV_PutFieldValues @intVar_id,@intTableId,@intTableFieldId,NULL,@intUser_Id,@vchTableFieldValue
            
                SET @intCounter = @intCounter + 1
            END
            
                
            --Update all extended_infos and user_defined values for parent and child variables (if needed)
            IF @vcrEIs <> '' 
            BEGIN		
            
                TRUNCATE TABLE #TableFieldIds
                INSERT #TableFieldIds(TableFieldId)
                SELECT value FROM STRING_SPLIT(REPLACE(@vcrEIs,'[REC]',','),',')

                
                SET @intCounter = 1
                WHILE @intCounter <= (SELECT COUNT(*) FROM #TableFieldIds) 
                BEGIN

                    SELECT @vcrEIs = ISNULL(TableFieldId,'') FROM #TableFieldIds WHERE ItemId = @intCounter

                    IF LEN(@vcrEIs) > 0 AND CHARINDEX('[FLD]',@vcrEIs) > 0
                    BEGIN

                        INSERT INTO #ExtendedInfos (VarName) SELECT SUBSTRING(@vcrEIs,1,CHARINDEX('[FLD]',@vcrEIs)- 1) 
                        SELECT @vcrEIs = SUBSTRING(@vcrEIs,CHARINDEX('[FLD]',@vcrEIs) +5,LEN(@vcrEIs)) 
                        
                        UPDATE #ExtendedInfos SET ExtInfo = SUBSTRING(@vcrEIs,1,CHARINDEX('[FLD]',@vcrEIs) -1) 
                        SELECT @vcrEIs = SUBSTRING(@vcrEIs,CHARINDEX('[FLD]',@vcrEIs) +5,LEN(@vcrEIs))		
                        UPDATE #ExtendedInfos SET UDefined1 = SUBSTRING(@vcrEIs,1,CHARINDEX('[FLD]',@vcrEIs) -1) 
                        SELECT @vcrEIs = SUBSTRING(@vcrEIs,CHARINDEX('[FLD]',@vcrEIs) +5,LEN(@vcrEIs))
                        UPDATE #ExtendedInfos SET UDefined2 = SUBSTRING(@vcrEIs,1,CHARINDEX('[FLD]',@vcrEIs) -1) 
                        SELECT @vcrEIs = SUBSTRING(@vcrEIs,CHARINDEX('[FLD]',@vcrEIs) +5,LEN(@vcrEIs))
                        UPDATE #ExtendedInfos SET UDefined3 = @vcrEIs

                    ENd

                    SET @intCounter = @intCounter + 1
                ENd

                INSERT INTO #Variables(
                        VarId
                        ,VarName)
                SELECT	 
                        Var_Id
                        ,Var_Desc
                    FROM dbo.Variables_Base (NOLOCK)
                    WHERE Var_Id = @intVar_id OR PVar_Id = @intVar_id
                    
                SET @intCounter = 1
                WHILE @intCounter <= (SELECT COUNT(*) FROM #Variables) 
                BEGIN

                    SELECT	@VarId			= v.VarId,
                            @DataType		= Data_Type_Id,
                            @Precision		= Var_Precision,
                            @SInterval		= Sampling_Interval,
                            @SOffset		= Sampling_Offset,
                            @SType			= Sampling_Type,
                            @EngUnits		= Eng_Units,
                            @DSId			= DS_Id,
                            @EventType		= Event_Type,
                            @UnitReject		= Unit_Reject,
                            @USummarize		= Unit_Summarize,
                            @VReject		= Var_Reject,
                            @Rank			= Rank,
                            @InputTag		= Input_Tag,
                            @OutTag			= Output_Tag,
                            @DQTag			= DQ_Tag,
                            @UELTag			= UEL_Tag,
                            @URLTag			= URL_Tag,
                            @UWLTag			= UWL_Tag,
                            @UULTag			= UUL_Tag,
                            @TargetTag		= Target_Tag,
                            @LULTag			= LUL_Tag,
                            @LWLTag			= LWL_Tag,
                            @LRLTag			= LRL_Tag,
                            @LELTag			= LEL_Tag,
                            @TotFactor		= Tot_Factor,
                            @SGroupId		= Group_Id,
                            @SpecId			= Spec_Id,
                            @SAId			= SA_Id,
                            @Repeat			= Repeating,
                            @RBackTime		= Repeat_BackTime,
                            @SWindow		= Sampling_Window,
                            @SArchive		= ShouldArchive,
                            @ExtInfo		= ei.ExtInfo,
                            @UDefined1		= ei.UDefined1,
                            @UDefined2		= ei.UDefined2,
                            @UDefined3		= ei.UDefined3,
                            @TFReset		= TF_Reset,
                            @ForceSign		= Force_Sign_Entry,
                            @TestName		= Test_Name,
                            @ExtTF			= Extended_Test_Freq,
                            @ArStatOnly		= ArrayStatOnly,
                            @CTId			= Comparison_Operator_Id,
                            @CV				= Comparison_Value,
                            @MaxRPM			= Max_RPM,
                            @ResetValue		= Reset_Value,
                            @IsConfVar		= Is_Conformance_Variable,
                            @ESignLevel		= Esignature_Level,
                            @ESubtypeId		= Event_Subtype_Id,
                            @EDimension		= Event_Dimension,
                            @PEIId			= PEI_Id,
                            @SPCCalcTypeId	= SPC_Calculation_Type_Id,
                            @SPCVarTypeId	= SPC_Group_Variable_Type_Id,
                            @InputTag2		= Input_Tag2,
                            @SampRefVarId	= Sampling_Reference_Var_Id,
                            @StrSpecSet		= String_Specification_Setting,
                            @WGroupDSId		= Write_Group_DS_Id,
                            @CPKSGroupSize	= CPK_SubGroup_Size,
                            @RLagTime		= ReadLagTime,
                            @ReloadFlag		= Reload_Flag,
                            @ELookup		= Perform_Event_Lookup,
                            @Debug			= Debug,
                            @IgnoreEStatus	= Ignore_Event_Status
                    FROM #Variables v
                    JOIN dbo.Variables_Base vb (NOLOCK) ON vb.Var_Id = v.VarId
                    JOIN #ExtendedInfos ei ON ei.VarName = v.VarName
                    WHERE v.ItemId = @intCounter

                    EXEC spEM_PutVarSheetData
                            @VarId,
                            @DataType,
                            @Precision,
                            @SInterval,
                            @SOffset,
                            @SType,
                            @EngUnits,
                            @DSId,
                            @EventType,
                            @UnitReject,
                            @USummarize,
                            @VReject,
                            @Rank,
                            @InputTag,
                            @OutTag,
                            @DQTag,
                            @UELTag,
                            @URLTag,
                            @UWLTag,
                            @UULTag,
                            @TargetTag,
                            @LULTag,
                            @LWLTag,
                            @LRLTag,
                            @LELTag,
                            @TotFactor,
                            @SGroupId,
                            @SpecId,
                            @SAId,
                            @Repeat,
                            @RBackTime,
                            @SWindow,
                            @SArchive,
                            @ExtInfo,
                            @UDefined1,
                            @UDefined2,
                            @UDefined3,
                            @TFReset,
                            @ForceSign,
                            @TestName,
                            @ExtTF,
                            @ArStatOnly,
                            @CTId,
                            @CV,
                            @MaxRPM,
                            @ResetValue,
                            @IsConfVar,
                            @ESignLevel,
                            @ESubtypeId,
                            @EDimension,
                            @PEIId,
                            @SPCCalcTypeId,
                            @SPCVarTypeId,
                            @InputTag2,
                            @SampRefVarId,
                            @StrSpecSet,
                            @WGroupDSId,
                            @CPKSGroupSize,
                            @RLagTime,
                            @ReloadFlag,
                            @ELookup,
                            @Debug,
                            @IgnoreEStatus,
                            @intUser_Id

                    SET @intCounter = @intCounter + 1

                END
            END

        SELECT @intVar_id as 'Var_Id'

        DROP TABLE #TableFieldIds
        DROP TABLE #TableFieldValues
        DROP TABLE #TableFieldValuesTemp
        DROP TABLE #Extended_Infos
        DROP TABLE #AllUDPs
        DROP TABLE #SheetVariables
        DROP TABLE #SheetVariables1";

        
    public readonly static string AddVariableQuery = @"


        SET NOCOUNT ON

        DECLARE
        @IsFirst			INT,
        @IsLast				INT,
        @intVarId			INT,
        @intMaxOrder		INT,
        @intMaxOrderVar		INT,
        @intOrder 			INT,
        @dtmTimestamp		DATETIME,
        @FieldName			VARCHAR(50),
        @linedesc			VARCHAR(50),
        @SQLCommand			nVARCHAR(4000),
        @FieldId				INT,
        @TableId				INT,
        @intPUId				INT,
        @intCreateUnit	INT,
        @intTableId			INT,
        @intQAVarId			INT,
        @intPropID			INT,
        @intQAPUGID			INT,
        @intQASpecID		INT,
        @intDifference		INT,
        @intOffset			INT,
        @vcrIndividualIDs 		VARCHAR(8000),
        @vcrIndividualValues		VARCHAR(8000),
        @vcrIndividualVarDesc	VARCHAR(8000),
        @intIndividualVarId		INT,
        @intCounter			INT,
        @intMaxCounter		INT,
        @intTempVarId		INT,
        @intTempVarOrder	INT,
        @QTemplate 			INT, 
        @exist 				INT,
        @intPerformEventLookup	INT = 1,
        @intData_TypeId		INT,
        @vcrVar_DescAL		NVARCHAR(255),
        @intTableFieldId	INT,
        @vchTableFieldValue VARCHAR(8000)

        DECLARE	--SPC Calcs
        @vcrVarNameTemp			VARCHAR(50),
        @intChildVarId 			INTEGER,
        @intCalcId 				INTEGER,
        @intRangeVarId 			INTEGER,
        @intStdDevVarId 		INTEGER,
        @intMRVarId 			INTEGER,
        @vcrSSNumber			VARCHAR(2),
        @vcrParentCalcName		VARCHAR(255),
        @vcrChildName			VARCHAR(255),
        @vcrChildGlobalDesc		VARCHAR(255),
        @vcrChildLocalDesc		VARCHAR(255),
        @vcrOldGlobalDesc		VARCHAR(255),
        @vcrUD1					VARCHAR(255),
        @vcrUD2					VARCHAR(255),
        @vcrUD3					VARCHAR(255),
        @vcrEI					VARCHAR(255),
        @bitESign				BIT,
        @vcrEngUnits			VARCHAR(255),
        @vcrExtLink				VARCHAR(255),
        @intSamplingInterval	INTEGER,
        @intSamplingOffset		INTEGER

        DECLARE -- Variables for PutVarSheetData
            @DataType		INT,
            @Precision		INT,
            @SInterval		INT,
            @SOffset		INT,
            @SType			TINYINT,
            @EngUnits		INT,
            @DSId			INT,
            @EventType		INT,
            @UnitReject		INT,
            @USummarize		BIT,
            @VReject		BIT,
            @Rank			INT,
            @InputTag		VARCHAR(255),
            @OutTag			VARCHAR(255),
            @DQTag			VARCHAR(255),
            @UELTag			VARCHAR(255),
            @URLTag			VARCHAR(255),
            @UWLTag			VARCHAR(255),
            @UULTag			VARCHAR(255),
            @TargetTag		VARCHAR(255),
            @LULTag			VARCHAR(255),
            @LWLTag			VARCHAR(255),
            @LRLTag			VARCHAR(255),
            @LELTag			VARCHAR(255),
            @TotFactor		REAL,
            @SGroupId		INT,
            @SpecId			INT,
            @SAId			INT,
            @Repeat			BIT,
            @RBackTime		INT,
            @SWindow		INT,
            @SArchive		BIT,
            @ExtInfo		VARCHAR(255),
            @UDefined1		VARCHAR(255),
            @UDefined2		VARCHAR(255),
            @UDefined3		VARCHAR(255),
            @TFReset		TINYINT,
            @ForceSign		TINYINT,
            @TestName		VARCHAR(50),
            @ExtTF			INT,
            @ArStatOnly		BIT,
            @CTID			INT,
            @CV				VARCHAR(50),
            @MaxRPM			FLOAT,
            @ResetValue		FLOAT,
            @IsConfVar		INT,
            @ESignLevel		INT,
            @ESubtypeId		INT,
            @EDimension		TINYINT,
            @PEIId			INT,
            @SPCCalcTypeId	INT,
            @SPCVarTypeId	INT,
            @InputTag2		INT,
            @SampRefVarId	INT,
            @StrSpecSet		TINYINT,
            @WGroupDSId		INT,
            @CPKSGroupSize	INT,
            @RLagTime		INT,
            @ReloadFlag		INT,
            @ELookup		INT,
            @Debug			INT,
            @IgnoreEStatus	INT,
            @VarId			INT


        -- contains all line descriptions
        DECLARE @PlDescs TABLE (
            LineDesc		VARCHAR(50))
            
        DECLARE @SheetVariables TABLE (
            RcdIdx			INT IDENTITY,
            SheetId			INT,
            VarId			INT,
            VarOrder		INT)
            
        DECLARE @SheetVariables1 TABLE (
            RcdIdx			INT IDENTITY,
            SheetId			INT,
            VarId			INT,
            VarOrder		INT)
                
        CREATE TABLE #TableFieldIds(
        ItemId				INTEGER IDENTITY(1, 1) NOT NULL,
        TableFieldId			INTEGER
        )

        CREATE TABLE #TableFieldValues(
        ItemId				INTEGER IDENTITY(1, 1) NOT NULL,
        TableFieldValue		VARCHAR(8000)
        )

        CREATE TABLE #TableFieldValuesTemp(
        RcdIdx				INT IDENTITY(1, 1) NOT NULL,
        KeyId					INT,
        TableFieldId			INT, 
        TableId				INT, 
        TableFieldValue		VARCHAR(8000)
        )

        CREATE TABLE #VariablesLocal(
        ItemId				INTEGER IDENTITY(1, 1) NOT NULL,
        VarId					INTEGER,
        VarName				NVARCHAR(255)
        )


        CREATE TABLE #ExtendedInfos(
        ItemId				INTEGER IDENTITY(1, 1) NOT NULL,
        VarName				NVARCHAR(255),
        ExtInfo				NVARCHAR(255),
        UDefined1				NVARCHAR(255),
        UDefined2				NVARCHAR(255),
        UDefined3				NVARCHAR(255))


        CREATE TABLE #AllUDPs(
            Item_Id				INT,
            Var_Name				VARCHAR(255),
            UDPs					VARCHAR(7750))


        SET @intMaxOrderVar = 0
        SET @exist = 1

        IF @intPVar_Id = 0 
        BEGIN
            SET @intPVar_Id = NULL
        END

        SET @intCreateUnit = 0

        -- retreive the pu_id from unit description
        SET @intPUId = (SELECT pu_id FROM dbo.Prod_Units_Base (NOLOCK) WHERE pu_desc = @vcrPUDesc and pl_id = @intPLId)

        -- retreive line description
        SET @linedesc = (SELECT pl_desc FROM dbo.Prod_Lines_Base (NOLOCK) WHERE pl_id = @intPLId)

        IF @intPUId IS NULL
        BEGIN
            SET @intCreateUnit = 1	
        END
        ELSE
        BEGIN
            -- verify if unit already exists on the line
            IF NOT EXISTS(SELECT pl_id FROM dbo.Prod_Units_Base (NOLOCK) WHERE pu_id = @intPUId and pl_id = @intPLId)
            BEGIN
                SET @intCreateUnit = 1	
            END
        END

        IF @intCreateUnit = 1
        BEGIN
            -- get all line descriptions
            INSERT INTO @PlDescs (LineDesc)
                SELECT pl_desc 
                FROM dbo.Prod_Lines_Base (NOLOCK) 

            -- verify if the unit desc contains a line desc, if so replaces the line desc in the unit desc
            IF (SELECT COUNT(LineDesc) 
                FROM @PlDescs 
                WHERE LineDesc LIKE '%' + SUBSTRING(@vcrPUDesc, 1, CHARINDEX(' ', @vcrPUDesc, 1) - 1) + '%') > 0
            BEGIN
                SET @vcrPUDesc = (SELECT pl_desc FROM dbo.Prod_Lines_Base (NOLOCK) WHERE pl_id = @intPLId) + ' ' + 
                                    SUBSTRING(@vcrPUDesc, CHARINDEX(' ', @vcrPUDesc, 1) + 1,
                                                                    LEN(@vcrPUDesc) - CHARINDEX(' ', @vcrPUDesc, 1) + 1)
            END

            -- verify if the unit already exists on the line
            IF NOT EXISTS(SELECT pl_id FROM dbo.Prod_Units_Base (NOLOCK) WHERE pu_desc = @vcrPUDesc and pl_id = @intPLId)
            BEGIN
                ---- create the production unit
                EXEC dbo.spEM_CreateProdUnit @vcrPUDesc,@intPLId,@intUser_Id,@intPUId

                -- Get the Id of the new prod unit created

            SET @intPUId = (SELECT pu_id FROM dbo.Prod_Units_Base (NOLOCK) WHERE pu_desc = @vcrPUDesc)
            END
            ELSE
            BEGIN
                -- Get the Id of the prod unit containing the other line desc
            SET @intPUId = (SELECT pu_id FROM dbo.Prod_Units_Base (NOLOCK) WHERE pu_desc = @vcrPUDesc)
            END
        END

        -- retreive pug_id on the unit
        SET @intPUG_Id = (SELECT pug_id FROM dbo.pu_groups (NOLOCK) WHERE pug_desc = @vcrPug_Desc and pu_id = @intPUId)

        --Manage the production group
        IF (@intPUG_Id IS NULL) AND (@vcrPug_Desc IS NOT NULL)
        BEGIN      
            SET @intMaxOrder = (SELECT ISNULL(MAX(pug_order),0) + 1 FROM dbo.PU_Groups (NOLOCK) WHERE pu_id = @intPUId)
                
            EXEC dbo.spEM_CreatePUG
                    @vcrPug_Desc		--@Description 
                    ,@intPUId			--@PU_Id		
                    ,@intMaxOrder		--@PUG_Order	
                    ,@intUser_Id		--@User_Id 		
                    ,@intPug_Id			--@PUG_Id		
            
            -- Get the Id of the new PU_Group created
        SET @intPug_Id = (SELECT pug_id FROM dbo.PU_Groups (NOLOCK) WHERE pu_id = @intPUId and pug_desc = @vcrPug_Desc)
        END


        -- Check if variable already exists (in case of SPC, it could have been created by a previous call)
        IF @intSPCTypeId <> -1
        BEGIN
            SET @intVarId = (SELECT var_id FROM dbo.variables_Base (NOLOCK) WHERE var_desc = @vcrVar_Desc and pu_id = @intPUId)
        END

        -- Search Pug_Order
        IF @intPVar_Id IS NULL 
        BEGIN
            -- Is var parent
            SELECT @intMaxOrderVar = MAX(pug_order) + 1 FROM dbo.variables_Base (NOLOCK) WHERE Pug_id = @intPUG_Id
            IF @intMaxOrderVar IS NULL
            BEGIN
                SET @intMaxOrderVar = 1
            END 
        END
        ELSE
        BEGIN
            -- Is var child
            SET @intMaxOrderVar = 1
        END


        -- Check if is a Historian variable 
        IF (@vcrInput_Tag IS NOT NULL) 
        BEGIN
            SET @intPerformEventLookup = 0
        END

        IF @intVarId IS NULL
        BEGIN
            
            SET @exist = 0
                
            EXEC spEM_CreateVariable
                    @vcrVar_Desc		----@VarDesc,
                    ,@intPUId			----@UnitId,
                    ,@intPUG_Id			----@GroupId,
                    ,@intMaxOrderVar		----@VarOrder,
                    ,@intUser_Id		----@UserId,
                    ,@intVarId			OUTPUT
                        
            IF @intVarId IS NOT NULL
            BEGIN
                SELECT @intData_TypeId = CASE WHEN @intSPCTypeId <> -1 THEN 2 ELSE @intData_Type_Id END

                EXEC spEM_PutVarSheetData
                        @intVarId
                        ,@intData_TypeId													--@DataType,			
                        ,@tintVar_Precision													--@Precision,			
                        ,@sintSampling_Interval												--@SInterval,			
                        ,@sintSampling_Offset												--@SOffset,			
                        ,NULL																--@SType,
                        ,@vcrEng_Units														--@EngUnits,			
                        ,@intDS_Id															--@DSId,				
                        ,@tintEvent_Type														--@EventType,			
                        ,0																--@UnitReject,
                        ,0																	--@USummarize,		
                        ,0																--@VReject,			
                        ,0																	--@Rank,				
                        ,@vcrInput_Tag														--@InputTag,			
                        ,@vcrOutput_Tag														--@OutTag,			
                        ,@vcrDQ_Tag															--@DQTag,				
                        ,NULL																--@UELTag,
                        ,NULL																--@URLTag,
                        ,NULL																--@UWLTag,
                        ,NULL																--@UULTag,
                        ,NULL																--@TargetTag,
                        ,NULL																--@LULTag,
                        ,NULL																--@LWLTag,
                        ,NULL																--@LRLTag,
                        ,NULL																--@LELTag,
                        ,1																	--@TotFactor,			
                        ,NULL																--@SGroupId,			
                        ,@intSpec_Id														--@SpecId,			
                        ,@tintSA_Id															--@SAId,				
                        ,@tintRepeating														--@Repeat,			
                        ,@intRepeat_Backtime												--@RBackTime,			
                        ,NULL																--@SWindow,
                        ,1																	--@SArchive,			
                        ,@vcrExtendedInfo													--@ExtInfo,			
                        ,@vcrUserDefined1													--@UDefined1,			
                        ,@vcrUserDefined2													--@UDefined2,			
                        ,@vcrUserDefined3													--@UDefined3,			
                        ,0																	--@TFReset,			
                        ,@bitForceSignEntry													--@ForceSign,			
                        ,@vcrTestName														--@TestName,			
                        ,1																	--@ExtTF,				
                        ,0																	--@ArStatOnly,		
                        ,NULL																--@CTId,
                        ,NULL																--@CV,
                        ,NULL																--@MaxRPM,			
                        ,NULL																--@ResetValue,
                        ,1																	--@IsConfVar,			
                        ,NULL																--@ESignLevel,
                        ,@tintEventSubtype													--@ESubtypeId,		
                        ,NULL																--@EDimension,
                        ,NULL																--@PEIId,
                        ,NULL														--@SPCCalcTypeId,		
                        ,NULL																--@SPCVarTypeId,
                        ,NULL																--@InputTag2,
                        ,NULL																--@SampRefVarId,
                        ,NULL																--@StrSpecSet,
                        ,NULL																--@WGroupDSId,
                        ,NULL																--@CPKSGroupSize,
                        ,NULL																--@RLagTime,
                        ,NULL																--@ReloadFlag,
                        ,@intPerformEventLookup																--@ELookup,
                        ,NULL																--@Debug,
                        ,NULL																--@IgnoreEStatus,
                        ,@intUser_Id														--@UserId

                        EXEC dbo.spEM_PutExtLink @intVarId, 'ag', @vcrExternal_Link, NULL,NULL, @intUser_Id
            END
        END

        IF @intSPCTypeId <> -1 AND @exist = 0 AND @intPVar_Id IS NULL
        BEGIN
                SELECT	@vcrTestName = Test_Name, 
                            @vcrUD1 = User_Defined1, 
                            @vcrUD2 = User_Defined2, 
                            @vcrUD3 = User_Defined3, 
                            @vcrEI = Extended_Info,
                            @bitESign = Force_Sign_Entry, 
                            @vcrEngUnits = Eng_Units,
                            @vcrExtLink = External_Link,
                            @intSamplingInterval = Sampling_Interval,
                            @intSamplingOffset = Sampling_Offset
                FROM 
                            dbo.Variables_Base (NOLOCK) 
                WHERE 
                            Var_Id = @intVarId

                --Downsizing subgroup size if needed
                IF @intSGSize < 0 BEGIN

                    SET @intCounter = 0

                    --For each variable we want to get rid off
                    WHILE @intCounter > @intSGSize BEGIN

                        --Get the variable ID
                        IF @intCounter < 10 BEGIN
                            SET @vcrSSNumber = '0' + CAST(@intCounter AS VARCHAR(2)) END
                        ELSE BEGIN
                            SET @vcrSSNumber = CAST(@intCounter AS VARCHAR(2))
                        END
                        SET @vcrVarNameTemp = @vcrVar_Desc + '-' + @vcrSSNumber	
                        SET @intChildVarId = NULL

                        SELECT @intChildVarId = MAX(var_id) FROM dbo.variables_Base (NOLOCK) WHERE PVar_Id = @intVarId		
            
                        --Dropping variable
                        IF @intChildVarId IS NOT NULL BEGIN
                            EXECUTE spEM_DropVariable @intChildVarId, @intUser_Id		--Variable itself
                            EXECUTE spEM_DropVariableSlave @intChildVarId				--Sheet and calculation instances

                        END

                        SET @intCounter = @intCounter - 1

                    END

                END
                
                SELECT @vcrParentCalcName = 
                    CASE 
                        WHEN @intSPCTypeId = 2 THEN 'MSI_UchartTotal'
                        WHEN @intSPCTypeId = 3 THEN 'MSI_PchartTotal'
                        ELSE 'MSI_Calc_Average'
                    END

                --Setting average calculation on the parent variable
                EXECUTE spEMCC_FindCalcByName @vcrParentCalcName, @intCalcId OUTPUT
                EXECUTE spEMCC_BuildDataSetUpdate 94, @intCalcId, @intVarId, @intSPCTypeId, NULL, '', '', @intUser_Id

                IF @intSPCTypeId NOT IN (1, 2, 3, 7) BEGIN

                    --Creating variable for the range calculation (if needed)
                    SET @vcrVarNameTemp = @vcrVar_Desc + '-Range'
                    SELECT @intRangeVarId = var_id FROM dbo.variables_Base (NOLOCK) WHERE pvar_id = @intVarId AND var_desc LIKE '%-Range' + '%' --var_desc = @vcrVarNameTemp AND pu_id = @intPUId

                    IF @intRangeVarId IS NULL 
                    BEGIN
                        --PRINT 'Call spEM_ALTERChildVariable with ' +  @vcrVarNameTemp
                        -- Range
                        EXECUTE spEM_ALTERChildVariable 
                            @vcrVarNameTemp,				
                            @intVarId, 
                            -1, 
                            16, 
                            2, 
                            @tintEvent_Type, 
                            @tintVar_Precision, 
                            2, 
                            NULL, 
                            1, 
                            @intUser_Id, 
                            @intRangeVarId OUTPUT
                    END

                    EXEC spEM_PutVarSheetData	
                        @intRangeVarId									--@VarId
                        ,2												--@DataType,			
                        ,@tintVar_Precision								--@Precision,			
                        ,@intSamplingInterval							--@SInterval,			
                        ,@intSamplingOffset								--@SOffset,			
                        ,NULL											--@SType,
                        ,@vcrEngUnits									--@EngUnits,			
                        ,16												--@DSId,				
                        ,@tintEvent_Type								--@EventType,			
                        ,0											--@UnitReject,
                        ,0												--@USummarize,		
                        ,0											--@VReject,			
                        ,0											--@Rank,				
                        ,NULL											--@InputTag,			
                        ,NULL											--@OutTag,			
                        ,NULL											--@DQTag,				
                        ,NULL											--@UELTag,
                        ,NULL											--@URLTag,
                        ,NULL											--@UWLTag,
                        ,NULL											--@UULTag,
                        ,NULL											--@TargetTag,
                        ,NULL											--@LULTag,
                        ,NULL											--@LWLTag,
                        ,NULL											--@LRLTag,
                        ,NULL											--@LELTag,
                        ,1											--@TotFactor,			
                        ,NULL											--@SGroupId,			
                        ,NULL											--@SpecId,			
                        ,1											--@SAId,				
                        ,NULL											--@Repeat,			
                        ,NULL											--@RBackTime,			
                        ,NULL											--@SWindow,
                        ,1											--@SArchive,			
                        ,@vcrEI											--@ExtInfo,			
                        ,@vcrUD1										--@UDefined1,			
                        ,@vcrUD2										--@UDefined2,			
                        ,@vcrUD3										--@UDefined3,			
                        ,0											--@TFReset,			
                        ,NULL											--@ForceSign,			
                        ,@vcrTestName									--@TestName,			
                        ,1											--@ExtTF,				
                        ,0											--@ArStatOnly,		
                        ,NULL											--@CTId,
                        ,NULL											--@CV,
                        ,NULL											--@MaxRPM,			
                        ,NULL											--@ResetValue,
                        ,1											--@IsConfVar,			
                        ,NULL											--@ESignLevel,
                        ,NULL											--@ESubtypeId,		
                        ,NULL											--@EDimension,
                        ,NULL											--@PEIId,
                        ,NULL											--@SPCCalcTypeId,		
                        ,2												--@SPCVarTypeId,
                        ,NULL											--@InputTag2,
                        ,NULL											--@SampRefVarId,
                        ,NULL											--@StrSpecSet,
                        ,NULL											--@WGroupDSId,
                        ,NULL											--@CPKSGroupSize,
                        ,NULL											--@RLagTime,
                        ,NULL											--@ReloadFlag,
                        ,@intPerformEventLookup											--@ELookup,
                        ,NULL											--@Debug,
                        ,NULL											--@IgnoreEStatus,
                        ,@intUser_Id									--@UserId


                    --Setting calculation on the range variable
                    EXECUTE spEMCC_FindCalcByName 'MSI_Calc_Range', @intCalcId OUTPUT
                    EXECUTE spEMCC_BuildDataSetUpdate 94, @intCalcId, @intRangeVarId, NULL, NULL, '', '', @intUser_Id
            
                    --Creating variable for the standard deviation calculation (if needed)
                    SET @vcrVarNameTemp = @vcrVar_Desc + '-StdDev'
                    SELECT @intStdDevVarId = var_id FROM dbo.variables_Base (NOLOCK) WHERE pvar_id = @intVarId AND var_desc LIKE '%-StdDev' + '%' --var_desc = @vcrVarNameTemp AND pu_id = @intPUId

                    IF @intStdDevVarId IS NULL 
                    BEGIN
                        --PRINT 'Call spEM_ALTERChildVariable with ' +  @vcrVarNameTemp
                        -- StdDev
                        EXECUTE spEM_ALTERChildVariable 
                            @vcrVarNameTemp, @intVarId, -1, 16, 2, @tintEvent_Type, 
                            @tintVar_Precision, 3, NULL, 1, @intUser_Id, @intStdDevVarId OUTPUT
                    END

                    
                    EXEC spEM_PutVarSheetData	
                        @intStdDevVarId									--@VarId
                        ,2												--@DataType,			
                        ,@tintVar_Precision								--@Precision,			
                        ,@intSamplingInterval							--@SInterval,			
                        ,@intSamplingOffset								--@SOffset,			
                        ,NULL											--@SType,
                        ,@vcrEngUnits									--@EngUnits,			
                        ,16												--@DSId,				
                        ,@tintEvent_Type								--@EventType,			
                        ,0											--@UnitReject,
                        ,0											--@USummarize,		
                        ,0											--@VReject,			
                        ,0											--@Rank,				
                        ,NULL											--@InputTag,			
                        ,NULL											--@OutTag,			
                        ,NULL											--@DQTag,				
                        ,NULL											--@UELTag,
                        ,NULL											--@URLTag,
                        ,NULL											--@UWLTag,
                        ,NULL											--@UULTag,
                        ,NULL											--@TargetTag,
                        ,NULL											--@LULTag,
                        ,NULL											--@LWLTag,
                        ,NULL											--@LRLTag,
                        ,NULL											--@LELTag,
                        ,1											--@TotFactor,			
                        ,NULL											--@SGroupId,			
                        ,NULL											--@SpecId,			
                        ,1											--@SAId,				
                        ,NULL											--@Repeat,			
                        ,NULL											--@RBackTime,			
                        ,NULL											--@SWindow,
                        ,1											--@SArchive,			
                        ,@vcrEI											--@ExtInfo,			
                        ,@vcrUD1										--@UDefined1,			
                        ,@vcrUD2										--@UDefined2,			
                        ,@vcrUD3										--@UDefined3,			
                        ,0											--@TFReset,			
                        ,NULL											--@ForceSign,			
                        ,@vcrTestName									--@TestName,			
                        ,1											--@ExtTF,				
                        ,0											--@ArStatOnly,		
                        ,NULL											--@CTId,
                        ,NULL											--@CV,
                        ,NULL											--@MaxRPM,			
                        ,NULL											--@ResetValue,
                        ,1											--@IsConfVar,			
                        ,NULL											--@ESignLevel,
                        ,NULL											--@ESubtypeId,		
                        ,NULL											--@EDimension,
                        ,NULL											--@PEIId,
                        ,NULL											--@SPCCalcTypeId,		
                        ,2												--@SPCVarTypeId,
                        ,NULL											--@InputTag2,
                        ,NULL											--@SampRefVarId,
                        ,NULL											--@StrSpecSet,
                        ,NULL											--@WGroupDSId,
                        ,NULL											--@CPKSGroupSize,
                        ,NULL											--@RLagTime,
                        ,NULL											--@ReloadFlag,
                        ,@intPerformEventLookup											--@ELookup,
                        ,NULL											--@Debug,
                        ,NULL											--@IgnoreEStatus,
                        ,@intUser_Id									--@UserId

                    --Setting calculation on the standard deviation variable
                    EXECUTE spEMCC_FindCalcByName 'MSI_Calc_StDev', @intCalcId OUTPUT
                    EXECUTE spEMCC_BuildDataSetUpdate 94, @intCalcId, @intStdDevVarId, NULL, NULL, '', '', @intUser_Id

                
                    --Creating variable for the moving range calculation (if needed)
                    SET @vcrVarNameTemp = @vcrVar_Desc + '-MR'
                    SELECT @intMRVarId = var_id FROM dbo.Variables_Base (NOLOCK) WHERE pvar_id = @intVarId AND var_desc LIKE '%-MR' + '%' --var_desc = @vcrVarNameTemp AND pu_id = @intPUId

                    IF @intMRVarId IS NULL 
                    BEGIN
                        --PRINT 'Call spEM_ALTERChildVariable with ' +  @vcrVarNameTemp
                        -- MR
                        EXECUTE spEM_ALTERChildVariable 
                            @vcrVarNameTemp, @intVarId, -1, 16, 2, @tintEvent_Type, 
                            @tintVar_Precision, 4, NULL, 1, @intUser_Id, @intMRVarId OUTPUT
                    END
                    
                    EXEC spEM_PutVarSheetData	
                        @intMRVarId									--@VarId
                        ,2												--@DataType,			
                        ,@tintVar_Precision								--@Precision,			
                        ,@intSamplingInterval							--@SInterval,			
                        ,@intSamplingOffset								--@SOffset,			
                        ,NULL											--@SType,
                        ,@vcrEngUnits									--@EngUnits,			
                        ,16												--@DSId,				
                        ,@tintEvent_Type								--@EventType,			
                        ,0											--@UnitReject,
                        ,0											--@USummarize,		
                        ,0											--@VReject,			
                        ,0											--@Rank,				
                        ,NULL											--@InputTag,			
                        ,NULL											--@OutTag,			
                        ,NULL											--@DQTag,				
                        ,NULL											--@UELTag,
                        ,NULL											--@URLTag,
                        ,NULL											--@UWLTag,
                        ,NULL											--@UULTag,
                        ,NULL											--@TargetTag,
                        ,NULL											--@LULTag,
                        ,NULL											--@LWLTag,
                        ,NULL											--@LRLTag,
                        ,NULL											--@LELTag,
                        ,1											--@TotFactor,			
                        ,NULL											--@SGroupId,			
                        ,NULL											--@SpecId,			
                        ,1											--@SAId,				
                        ,NULL											--@Repeat,			
                        ,NULL											--@RBackTime,			
                        ,NULL											--@SWindow,
                        ,1											--@SArchive,			
                        ,@vcrEI											--@ExtInfo,			
                        ,@vcrUD1										--@UDefined1,			
                        ,@vcrUD2										--@UDefined2,			
                        ,@vcrUD3										--@UDefined3,			
                        ,0											--@TFReset,			
                        ,NULL											--@ForceSign,			
                        ,@vcrTestName									--@TestName,			
                        ,1											--@ExtTF,				
                        ,0											--@ArStatOnly,		
                        ,NULL											--@CTId,
                        ,NULL											--@CV,
                        ,NULL											--@MaxRPM,			
                        ,NULL											--@ResetValue,
                        ,1											--@IsConfVar,			
                        ,NULL											--@ESignLevel,
                        ,NULL											--@ESubtypeId,		
                        ,NULL											--@EDimension,
                        ,NULL											--@PEIId,
                        ,NULL											--@SPCCalcTypeId,		
                        ,2												--@SPCVarTypeId,
                        ,NULL											--@InputTag2,
                        ,NULL											--@SampRefVarId,
                        ,NULL											--@StrSpecSet,
                        ,NULL											--@WGroupDSId,
                        ,NULL											--@CPKSGroupSize,
                        ,NULL											--@RLagTime,
                        ,NULL											--@ReloadFlag,
                        ,@intPerformEventLookup											--@ELookup,
                        ,NULL											--@Debug,
                        ,NULL											--@IgnoreEStatus,
                        ,@intUser_Id									--@UserId

                    --Setting calculation on the moving range variable
                    EXECUTE spEMCC_FindCalcByName 'MSI_MovingRange', @intCalcId OUTPUT
                    EXECUTE spEMCC_BuildDataSet 99, @intCalcId, NULL, NULL, NULL, NULL, NULL, @intUser_Id

                    --Add inputs to the moving range calculation
                    EXECUTE spEMCC_BuildDataSetUpdate 35, @intCalcId, 19, @intMRVarId, @intRangeVarId, '', NULL, @intUser_Id
                    EXECUTE spEMCC_BuildDataSetUpdate 35, @intCalcId, 20, @intMRVarId, @intRangeVarId, '', NULL, @intUser_Id
                    EXECUTE spEMCC_BuildDataSetUpdate 94, @intCalcId, @intMRVarId, NULL, NULL, '', '', @intUser_Id

                END

                -- Before
                -- SET @intCounter = 1
                SET @intCounter = 1
                SET @intChildVarId = 0
                --For each variable in the subgroup
                WHILE @intCounter <= @intSGSize BEGIN

                    --Get the variable name...
                    IF @intCounter < 10 BEGIN
                        SET @vcrSSNumber = '0' + CAST(@intCounter AS VARCHAR(2)) END
                    ELSE BEGIN
                        SET @vcrSSNumber = CAST(@intCounter AS VARCHAR(2))
                    END
                    --SET @vcrVarNameTemp = @vcrVar_Desc + '-' + @vcrSSNumber

                    --... then the variable ID
                    SET @intChildVarId = NULL
                    SELECT @intChildVarId = var_id FROM dbo.variables_Base (NOLOCK) WHERE pvar_id = @intVarId AND var_desc LIKE @vcrVar_Desc + '-' + @vcrSSNumber + '%' 


                    IF @intChildVarId IS NOT NULL BEGIN
                        SET @vcrChildName = (SELECT Var_Desc FROM dbo.Variables_Base (NOLOCK) WHERE Var_Id  = @intChildVarId )
                        IF @vcrChildName <> @vcrOldGlobalDesc + '-' + @vcrSSNumber 
                        BEGIN
                            SELECT	@vcrChildLocalDesc = Var_Desc, 
                                    @vcrChildGlobalDesc = Var_Desc_Global 
                                FROM dbo.Variables_Base (NOLOCK)
                                WHERE Var_Id = @intChildVarId END
                        ELSE 
                        BEGIN
                            SET @vcrChildGlobalDesc = @vcrGlobalDesc + '-' + @vcrSSNumber
                            SET @vcrChildLocalDesc = @vcrVar_Desc + '-' + @vcrSSNumber
                        END END
                    ELSE BEGIN
                        SET @vcrChildGlobalDesc = @vcrGlobalDesc + '-' + @vcrSSNumber
                        SET @vcrChildLocalDesc = @vcrVar_Desc + '-' + @vcrSSNumber	
                    END

                    --If variable does not exist then ALTER
                    IF @intChildVarId IS NULL 
                    BEGIN
                        -- Var Child
                        EXECUTE spEM_ALTERChildVariable 
                            @vcrChildLocalDesc, @intVarId, -1, @intDS_Id, @intData_Type_Id, 
                            @tintEvent_Type, @tintVar_Precision, 1, @intSpec_Id, 1, @intUser_Id, @intChildVarId OUTPUT
                    END				
                    
                    EXEC spEM_PutVarSheetData	
                        @intChildVarId									--@VarId
                        ,2												--@DataType,			
                        ,@tintVar_Precision								--@Precision,			
                        ,@intSamplingInterval							--@SInterval,			
                        ,@intSamplingOffset								--@SOffset,			
                        ,NULL											--@SType,
                        ,@vcrEngUnits									--@EngUnits,			
                        ,16												--@DSId,				
                        ,@tintEvent_Type								--@EventType,			
                        ,0											--@UnitReject,
                        ,0											--@USummarize,		
                        ,0											--@VReject,			
                        ,0											--@Rank,				
                        ,NULL											--@InputTag,			
                        ,NULL											--@OutTag,			
                        ,NULL											--@DQTag,				
                        ,NULL											--@UELTag,
                        ,NULL											--@URLTag,
                        ,NULL											--@UWLTag,
                        ,NULL											--@UULTag,
                        ,NULL											--@TargetTag,
                        ,NULL											--@LULTag,
                        ,NULL											--@LWLTag,
                        ,NULL											--@LRLTag,
                        ,NULL											--@LELTag,
                        ,1											--@TotFactor,			
                        ,NULL											--@SGroupId,			
                        ,NULL											--@SpecId,			
                        ,1											--@SAId,				
                        ,NULL											--@Repeat,			
                        ,NULL											--@RBackTime,			
                        ,NULL											--@SWindow,
                        ,1											--@SArchive,			
                        ,@vcrEI											--@ExtInfo,			
                        ,@vcrUD1										--@UDefined1,			
                        ,@vcrUD2										--@UDefined2,			
                        ,@vcrUD3										--@UDefined3,			
                        ,0											--@TFReset,			
                        ,NULL											--@ForceSign,			
                        ,@vcrTestName									--@TestName,			
                        ,1											--@ExtTF,				
                        ,0											--@ArStatOnly,		
                        ,NULL											--@CTId,
                        ,NULL											--@CV,
                        ,NULL											--@MaxRPM,			
                        ,NULL											--@ResetValue,
                        ,1											--@IsConfVar,			
                        ,NULL											--@ESignLevel,
                        ,NULL											--@ESubtypeId,		
                        ,NULL											--@EDimension,
                        ,NULL											--@PEIId,
                        ,NULL											--@SPCCalcTypeId,		
                        ,2												--@SPCVarTypeId,
                        ,NULL											--@InputTag2,
                        ,NULL											--@SampRefVarId,
                        ,NULL											--@StrSpecSet,
                        ,NULL											--@WGroupDSId,
                        ,NULL											--@CPKSGroupSize,
                        ,NULL											--@RLagTime,
                        ,NULL											--@ReloadFlag,
                        ,@intPerformEventLookup											--@ELookup,
                        ,NULL											--@Debug,
                        ,NULL											--@IgnoreEStatus,
                        ,@intUser_Id									--@UserId

                    --Add input to average, range & standard deviation calculations
                    EXECUTE spEMCC_BuildDataByID 84, @intVarId, @intChildVarId, 0, NULL, @intUser_Id
                    IF @intSPCTypeId NOT IN (1, 2, 3, 7) BEGIN
                        EXECUTE spEMCC_BuildDataByID 84, @intRangeVarId, @intChildVarId, 0, NULL, @intUser_Id
                        EXECUTE spEMCC_BuildDataByID 84, @intStdDevVarId, @intChildVarId, 0, NULL, @intUser_Id
                    END

                    SET @intCounter = @intCounter + 1

                END

                IF @intSPCTypeId = 3 BEGIN
                    EXECUTE spEMCC_BuildDataSet 99, @intCalcId, NULL, NULL, NULL, NULL, NULL, @intUser_Id
                    EXECUTE spEMCC_BuildDataSetUpdate 35, @intCalcId, 25, @intVarId, 0, @vcrSPCFailure, NULL, @intUser_Id

                    --UPDATE dbo.calculation_input_data 
                    --	SET default_value = @vcrSPCFailure 
                    --	WHERE result_var_id = @intVarId 
                    --	AND calc_input_id = 25
                END
        END

        --Creating _AL variable
        IF @intQAlarm_Display_Id IS NOT NULL
        BEGIN

            IF @intData_Type_Id < 3
            BEGIN
                SET @intQAPUGID = (SELECT pug_id FROM dbo.pu_groups (NOLOCK) WHERE pu_id = @intPUId AND pug_desc = 'QV Alarms')
                SET @QTemplate = (SELECT at_id FROM dbo.alarm_templates (NOLOCK) WHERE at_desc = LTRIM(RTRIM(@linedesc)) + ' Quality Variable Alarms')
            END
            IF @intQAPUGID IS NULL
            BEGIN
                SET @intQAPUGID = (SELECT pug_id FROM dbo.pu_groups (NOLOCK) WHERE pu_id = @intPUId AND pug_desc = 'QA Alarms')
            END
            IF @QTemplate IS NULL
            BEGIN
                SET @QTemplate = (SELECT at_id FROM dbo.alarm_templates (NOLOCK) WHERE at_desc = LTRIM(RTRIM(@linedesc)) + ' Quality Attribute Alarms')
            END

            SET @intPropID = (SELECT prop_id FROM dbo.specifications (NOLOCK) WHERE spec_id = @intSpec_Id)
            SET @intQASpecID = (SELECT spec_id FROM dbo.specifications (NOLOCK) WHERE prop_id = @intPropID AND spec_desc = 'FireAlarm')


            IF @intQAPUGID IS NOT NULL AND @intQASpecID IS NOT NULL 
            BEGIN
            
                --EXEC sp_ExecuteSQL @SQLCommand
                SELECT @intMaxOrderVar = MAX(pug_order) + 1 FROM dbo.Variables_Base (NOLOCK) WHERE Pug_id = @intQAPUGID
                SELECT @vcrVar_DescAL = @vcrVar_Desc + '_AL'
                EXEC spEM_CreateVariable
                        @vcrVar_DescAL				----@VarDesc,
                        ,@intQAPUGID					----@UnitId,
                        ,@intQAPUGID					----@GroupId,
                        ,@intMaxOrderVar				----@VarOrder,
                        ,@intUser_Id					----@UserId,
                        ,@intQAVarId					----@VarId	OUTPUT
            
                IF @intVarId IS NOT NULL
                BEGIN
                    EXEC spEM_PutVarSheetData
                            @intQAVarId
                            ,2											--@DataType,			
                            ,2											--@Precision,			
                            ,0											--@SInterval,			
                            ,NULL										--@SOffset,			
                            ,NULL										--@SType,
                            ,NULL										--@EngUnits,			
                            ,4											--@DSId,				
                            ,0											--@EventType,			
                            ,0										--@UnitReject,
                            ,0										--@USummarize,		
                            ,0										--@VReject,			
                            ,0										--@Rank,				
                            ,NULL										--@InputTag,			
                            ,NULL										--@OutTag,			
                            ,NULL										--@DQTag,				
                            ,NULL										--@UELTag,
                            ,NULL										--@URLTag,
                            ,NULL										--@UWLTag,
                            ,NULL										--@UULTag,
                            ,NULL										--@TargetTag,
                            ,NULL										--@LULTag,
                            ,NULL										--@LWLTag,
                            ,NULL										--@LRLTag,
                            ,NULL										--@LELTag,
                            ,1										--@TotFactor,			
                            ,NULL										--@SGroupId,			
                            ,@intQASpecID								--@SpecId,			
                            ,1										--@SAId,				
                            ,NULL										--@Repeat,			
                            ,NULL										--@RBackTime,			
                            ,NULL										--@SWindow,
                            ,1										--@SArchive,			
                            ,@intVarId							--@ExtInfo,			
                            ,NULL										--@UDefined1,			
                            ,NULL										--@UDefined2,			
                            ,NULL										--@UDefined3,			
                            ,0										--@TFReset,			
                            ,NULL										--@ForceSign,			
                            ,NULL										--@TestName,			
                            ,1										--@ExtTF,				
                            ,0										--@ArStatOnly,		
                            ,NULL										--@CTId,
                            ,NULL										--@CV,
                            ,NULL										--@MaxRPM,			
                            ,NULL										--@ResetValue,
                            ,1										--@IsConfVar,			
                            ,NULL										--@ESignLevel,
                            ,NULL										--@ESubtypeId,		
                            ,NULL										--@EDimension,
                            ,NULL										--@PEIId,
                            ,NULL										--@SPCCalcTypeId,		
                            ,NULL										--@SPCVarTypeId,
                            ,NULL										--@InputTag2,
                            ,NULL										--@SampRefVarId,
                            ,NULL										--@StrSpecSet,
                            ,NULL										--@WGroupDSId,
                            ,NULL										--@CPKSGroupSize,
                            ,NULL										--@RLagTime,
                            ,NULL										--@ReloadFlag,
                            ,@intPerformEventLookup										--@ELookup,
                            ,NULL										--@Debug,
                            ,NULL										--@IgnoreEStatus,
                            ,@intUser_Id								--@UserId
            
                    --Get the new id for alarm variable
                    SET @intQAVarId = (SELECT var_id FROM dbo.Variables_Base (NOLOCK) WHERE var_desc = @vcrVar_Desc + '_AL' and pu_id = @intPUId)
                
                
                    --Add alarm variable to the alarm display
                    IF @intQAlarm_Display_Id IS NOT NULL
                    BEGIN
                        INSERT INTO @SheetVariables (
                                    SheetId
                                    ,VarId
                                    ,VarOrder)
                        SELECT		Sheet_Id
                                    ,Var_Id
                                    ,Var_Order
                        FROM dbo.Sheet_Variables  (NOLOCK)
                        WHERE sheet_id = @intQAlarm_Display_Id

                        SELECT @intCounter = ISNULL(MIN(RcdIdx),0) FROM @SheetVariables
                        SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),0) FROM @SheetVariables

                        WHILE @intCounter <= @intMaxCounter AND @intCounter <> 0 AND @intMaxCounter <> 0
                        BEGIN
                            SELECT   @intTempVarId = VarId
                                    ,@intTempVarOrder = VarOrder
                            FROM @SheetVariables
                            WHERE RcdIdx = @intCounter

                            EXEC dbo.spEM_PutSheetVariables
                                @intQAlarm_Display_Id	--@Sheet_Id 
                                ,@intTempVarOrder		--@Order	
                                ,@intTempVarId			--@Id		
                                ,NULL					--@Title	
                                ,0						--@IsLast	
                                ,0						--@IsFirst
                                ,@intUser_Id				--@User_Id
                            
                            SET @intCounter = @intCounter + 1
                        END
                                
                    SET @intMaxOrder = (SELECT ISNULL(MAX(var_order),0) 
                                                FROM dbo.Sheet_Variables (NOLOCK)
                                                WHERE sheet_id = @intQAlarm_Display_Id) + 1
                    
                        EXEC dbo.spEM_PutSheetVariables
                            @intQAlarm_Display_Id		--@Sheet_Id 
                            ,@intMaxOrder				--@Order	
                            ,@intQAVarId					--@Id		
                            ,NULL						--@Title	
                            ,1							--@IsLast	
                            ,0							--@IsFirst
                            ,@intUser_Id					--@User_Id

                    END
                    ----------------------------------------------------------------------------------------------------
                END
            END
        END

        --Log this transaction in the variable log
        IF @exist = 0
        BEGIN	
            --Add variable to the alarm template
            IF @intAlarm_Template_Id IS NOT NULL BEGIN
            
                --Not SPC
                IF @intSPCTypeId = -1 
                BEGIN  

                    EXEC dbo.spEMAC_AddAttachedVariables 
                        @intAlarm_Template_Id		--@AT_Id int,
                        ,@intVarId					--@Var_Id int,
                        ,NULL						--@EG_Id int,
                        ,@intUser_Id					--@User_Id int,
                        ,NULL						--@SamplingSize Int = 0
                    
                END
                
                --SPC
                ELSE BEGIN

                    EXEC dbo.spEMAC_AddAttachedVariables 
                        @intAlarm_Template_Id		--@AT_Id int,
                        ,@intVarId					--@Var_Id int,
                        ,NULL						--@EG_Id int,
                        ,@intUser_Id				--@User_Id int,
                        ,@intSGSize					--@SamplingSize Int = 0
            
                END
            END 
            
                
            --Add variable to the alarm display
            IF @intAlarm_Display_Id IS NOT NULL 
            BEGIN
                IF  @intSPCTypeId = -1
                --Not SPC
                BEGIN
                    DELETE FROM @SheetVariables
                    DELETE FROM @SheetVariables1

                    INSERT INTO @SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK)
                    WHERE sheet_id = @intAlarm_Display_Id
                    
                    SELECT @intCounter = ISNULL(MIN(RcdIdx),0) FROM @SheetVariables
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),0) + 1 FROM @SheetVariables

                    INSERT INTO @SheetVariables(SheetId,VarId,VarOrder) VALUES (@intAlarm_Display_Id,@intVarId,@intMaxCounter)
                    
                    WHILE @intCounter <= @intMaxCounter AND @intCounter <> 0 AND @intMaxCounter <> 0
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = VarOrder
                                ,@IsFirst = CASE WHEN @intCounter = (SELECT ISNULL(MIN(RcdIdx),0) FROM @SheetVariables) THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM @SheetVariables) THEN 1 ELSE 0 END
                        FROM @SheetVariables
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @intAlarm_Display_Id	--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END
                END
                --SPC
                ELSE BEGIN
                
                    DELETE FROM @SheetVariables

                    INSERT INTO @SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK) 
                    WHERE sheet_id = @intAlarm_Display_Id
                    
                SET @intMaxOrder = (SELECT ISNULL(MAX(var_order),0) 
                                            FROM dbo.Sheet_Variables (NOLOCK)
                                            WHERE sheet_id = @intAlarm_Display_Id)
                    
                    SET @intDifference = @intVarId - @intMaxOrder
                
                INSERT @SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		@intAlarm_Display_Id
                                ,Var_Id
                                ,Var_Id - @intDifference + 1
                    FROM dbo.Variables_Base (NOLOCK)
                    WHERE
                        Var_Id = @intVarId OR PVar_Id = @intVarId
                    ORDER BY
                        Var_Desc

                    SELECT @intCounter = ISNULL(MIN(RcdIdx),0) FROM @SheetVariables
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),0) FROM @SheetVariables

                    
                    WHILE @intCounter <= @intMaxCounter AND @intCounter <> 0 AND @intMaxCounter <> 0
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = VarOrder
                                ,@IsFirst = CASE WHEN @intCounter = (SELECT ISNULL(MIN(RcdIdx),0) FROM @SheetVariables) THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM @SheetVariables) THEN 1 ELSE 0 END
                        FROM @SheetVariables
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @intAlarm_Display_Id	--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END
                END

            END

            --Add variable to the autolog display
            IF @intAutolog_Display_Id IS NOT NULL 
            BEGIN	
                --Not SPC
                IF  @intSPCTypeId = -1 
                BEGIN
                    DELETE FROM @SheetVariables
                    DELETE FROM @SheetVariables1

                    INSERT INTO @SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK)
                    WHERE sheet_id = @intAutolog_Display_Id

                    IF EXISTS (SELECT Var_Id
                            FROM dbo.Sheet_Variables (NOLOCK)
                            WHERE sheet_id = @intAutolog_Display_Id and
                                    var_order = @intAutolog_Display_Order) 
                    BEGIN
                        UPDATE @SheetVariables
                            SET VarOrder = VarOrder + 1
                        WHERE 
                            SheetId = @intAutolog_Display_Id and
                            VarOrder >= @intAutolog_Display_Order		
                    END

                    INSERT INTO @SheetVariables(SheetId,VarId,VarOrder) VALUES (@intAutolog_Display_Id,@intVarId,@intAutolog_Display_Order)
                    
                    INSERT INTO @SheetVariables1(SheetId,VarId,VarOrder) SELECT SheetId,VarId,VarOrder FROM @SheetVariables ORDER BY VarOrder

                    SELECT @intCounter = ISNULL(MIN(RcdIdx),0) FROM @SheetVariables1
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),0) FROM @SheetVariables1

                    WHILE @intCounter <= @intMaxCounter AND @intCounter <> 0 AND @intMaxCounter <> 0
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = VarOrder
                                ,@IsFirst = CASE WHEN @intCounter = (SELECT ISNULL(MIN(RcdIdx),0) FROM @SheetVariables1) THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM @SheetVariables1) THEN 1 ELSE 0 END
                        FROM @SheetVariables1
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @intAutolog_Display_Id	--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END
                END
                --SPC
                ELSE 
                BEGIN
                    DELETE FROM @SheetVariables
                    DELETE FROM @SheetVariables1

                    INSERT INTO @SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		Sheet_Id
                                ,Var_Id
                                ,Var_Order
                    FROM dbo.Sheet_Variables (NOLOCK)
                    WHERE sheet_id = @intAutolog_Display_Id

                    
                    IF EXISTS (SELECT Var_Id
                                FROM dbo.Sheet_Variables (NOLOCK)
                                WHERE sheet_id = @intAutolog_Display_Id and
                                    var_order = @intAutolog_Display_Order) 
                    BEGIN
            
                        SET @intOffset = (SELECT COUNT(Var_Id) FROM dbo.Variables_Base (NOLOCK) WHERE Var_Id = @intVarId OR pvar_id = @intVarId)	
                        
                        UPDATE @SheetVariables
                            SET VarOrder = VarOrder + @intOffset
                        WHERE 
                            SheetId = @intAutolog_Display_Id and
                            VarOrder >= @intAutolog_Display_Order		
                            
                    END
                    
                    SET @intDifference = @intVarId - @intAutolog_Display_Order
                        
                    INSERT @SheetVariables (
                                SheetId
                                ,VarId
                                ,VarOrder)
                    SELECT		@intAutolog_Display_Id
                                ,Var_Id
                                ,Var_Id - @intDifference			
                    FROM dbo.Variables_Base (NOLOCK)
                    WHERE var_id = @intVarId OR pvar_id = @intVarId
                    ORDER BY var_desc
                                
                    INSERT INTO @SheetVariables1(SheetId,VarId,VarOrder) SELECT SheetId,VarId,VarOrder FROM @SheetVariables ORDER BY VarOrder
                    
                    SELECT @intCounter = ISNULL(MIN(RcdIdx),0) FROM @SheetVariables1
                    SELECT @intMaxCounter = ISNULL(MAX(RcdIdx),0) FROM @SheetVariables1

                    WHILE @intCounter <= @intMaxCounter AND @intCounter <> 0 AND @intMaxCounter <> 0
                    BEGIN
                        SELECT   @intTempVarId = VarId
                                ,@intTempVarOrder = VarOrder
                                ,@IsFirst = CASE WHEN @intCounter = (SELECT ISNULL(MIN(RcdIdx),0) FROM @SheetVariables1) THEN 1 ELSE 0 END
                                ,@IsLast = CASE WHEN @intCounter = (SELECT ISNULL(MAX(RcdIdx),0) FROM @SheetVariables1) THEN 1 ELSE 0 END
                        FROM @SheetVariables1
                        WHERE RcdIdx = @intCounter

                        EXEC dbo.spEM_PutSheetVariables
                            @intAutolog_Display_Id	--@Sheet_Id 
                            ,@intTempVarOrder		--@Order	
                            ,@intTempVarId			--@Id		
                            ,NULL					--@Title	
                            ,@IsLast				--@IsLast	
                            ,@IsFirst				--@IsFirst
                            ,@intUser_Id				--@User_Id
                            
                        SET @intCounter = @intCounter + 1
                    END
                END			
            END
        END
            
            
            --Update the var_specs table if there is var_specs 
            IF @intSpec_Id IS NOT NULL
            BEGIN 
                EXEC dbo.spEM_PutSpecVariableData
                    @intVarId			--@Var_Id            int,
                    ,@intSpec_Id			--@Spec_Id           int,
                    ,@intUser_Id			--@User_Id int

            END
            
            
            --Add Table_Field Values
            SET @intTableId = (SELECT TableId FROM dbo.Tables (NOLOCK) WHERE TableName = 'Variables')

            INSERT #TableFieldIds(TableFieldId)
            SELECT value FROM STRING_SPLIT(REPLACE(@vcrTableFieldIDs,'[REC]',','),',')

            INSERT #TableFieldValues(TableFieldValue)
            SELECT value FROM STRING_SPLIT(REPLACE(@vcrTableFieldValues,'[REC]',','),',')

            INSERT #TableFieldValuesTemp (KeyId, TableFieldId, TableId, TableFieldValue)
            SELECT Var_Id, TableFieldId, @intTableId, TableFieldValue
                FROM #TableFieldIds tfi 
                JOIN #TableFieldValues tfv ON (tfi.ItemId = tfv.ItemId),
                    dbo.Variables_Base (NOLOCK)
                    WHERE Var_Id = @intVarId OR PVar_Id = @intVarId
                        
            SET @intCounter = 1     

            WHILE @intCounter <= (SELECT COUNT(*) FROM #TableFieldValuesTemp)
            BEGIN
                SELECT	@intTableFieldId = TableFieldId
                        ,@vchTableFieldValue = TableFieldValue
                FROM #TableFieldValuesTemp
                WHERE RcdIdx = @intCounter

                EXEC dbo.spEMTFV_PutFieldValues @intVarId,@intTableId,@intTableFieldId,NULL,@intUser_Id,@vchTableFieldValue
            
                SET @intCounter = @intCounter + 1
            END
            
                
            --Update all extended_infos and user_defined values for parent and child variables (if needed)
            IF @vcrEIs <> '' 
            BEGIN		
            
                TRUNCATE TABLE #TableFieldIds
                INSERT #TableFieldIds(TableFieldId)
                SELECT value FROM STRING_SPLIT(REPLACE(@vcrEIs,'[REC]',','),',')

                
                SET @intCounter = 1
                WHILE @intCounter <= (SELECT COUNT(*) FROM #TableFieldIds) 
                BEGIN

                    SELECT @vcrEIs = ISNULL(TableFieldId,'') FROM #TableFieldIds WHERE ItemId = @intCounter

                    IF LEN(@vcrEIs) > 0 AND CHARINDEX('[FLD]',@vcrEIs) > 0
                    BEGIN

                        INSERT INTO #ExtendedInfos (VarName) SELECT SUBSTRING(@vcrEIs,1,CHARINDEX('[FLD]',@vcrEIs)- 1) 
                        SELECT @vcrEIs = SUBSTRING(@vcrEIs,CHARINDEX('[FLD]',@vcrEIs) +5,LEN(@vcrEIs)) 
                        
                        UPDATE #ExtendedInfos SET ExtInfo = SUBSTRING(@vcrEIs,1,CHARINDEX('[FLD]',@vcrEIs) -1) 
                        SELECT @vcrEIs = SUBSTRING(@vcrEIs,CHARINDEX('[FLD]',@vcrEIs) +5,LEN(@vcrEIs))		
                        UPDATE #ExtendedInfos SET UDefined1 = SUBSTRING(@vcrEIs,1,CHARINDEX('[FLD]',@vcrEIs) -1) 
                        SELECT @vcrEIs = SUBSTRING(@vcrEIs,CHARINDEX('[FLD]',@vcrEIs) +5,LEN(@vcrEIs))
                        UPDATE #ExtendedInfos SET UDefined2 = SUBSTRING(@vcrEIs,1,CHARINDEX('[FLD]',@vcrEIs) -1) 
                        SELECT @vcrEIs = SUBSTRING(@vcrEIs,CHARINDEX('[FLD]',@vcrEIs) +5,LEN(@vcrEIs))
                        UPDATE #ExtendedInfos SET UDefined3 = @vcrEIs

                    ENd

                    SET @intCounter = @intCounter + 1
                ENd

                INSERT INTO #VariablesLocal(
                        VarId
                        ,VarName)
                SELECT	 
                        Var_Id
                        ,Var_Desc
                    FROM dbo.Variables_Base (NOLOCK)
                    WHERE Var_Id = @intVarId OR PVar_Id = @intVarId
                    
                SET @intCounter = 1
                WHILE @intCounter <= (SELECT COUNT(*) FROM #VariablesLocal) 
                BEGIN

                    SELECT	@VarId			= v.VarId,
                            @DataType		= Data_Type_Id,
                            @Precision		= Var_Precision,
                            @SInterval		= Sampling_Interval,
                            @SOffset		= Sampling_Offset,
                            @SType			= Sampling_Type,
                            @EngUnits		= Eng_Units,
                            @DSId			= DS_Id,
                            @EventType		= Event_Type,
                            @UnitReject		= Unit_Reject,
                            @USummarize		= Unit_Summarize,
                            @VReject		= Var_Reject,
                            @Rank			= Rank,
                            @InputTag		= Input_Tag,
                            @OutTag			= Output_Tag,
                            @DQTag			= DQ_Tag,
                            @UELTag			= UEL_Tag,
                            @URLTag			= URL_Tag,
                            @UWLTag			= UWL_Tag,
                            @UULTag			= UUL_Tag,
                            @TargetTag		= Target_Tag,
                            @LULTag			= LUL_Tag,
                            @LWLTag			= LWL_Tag,
                            @LRLTag			= LRL_Tag,
                            @LELTag			= LEL_Tag,
                            @TotFactor		= Tot_Factor,
                            @SGroupId		= Group_Id,
                            @SpecId			= Spec_Id,
                            @SAId			= SA_Id,
                            @Repeat			= Repeating,
                            @RBackTime		= Repeat_BackTime,
                            @SWindow		= Sampling_Window,
                            @SArchive		= ShouldArchive,
                            @ExtInfo		= ei.ExtInfo,
                            @UDefined1		= ei.UDefined1,
                            @UDefined2		= ei.UDefined2,
                            @UDefined3		= ei.UDefined3,
                            @TFReset		= TF_Reset,
                            @ForceSign		= Force_Sign_Entry,
                            @TestName		= Test_Name,
                            @ExtTF			= Extended_Test_Freq,
                            @ArStatOnly		= ArrayStatOnly,
                            @CTId			= Comparison_Operator_Id,
                            @CV				= Comparison_Value,
                            @MaxRPM			= Max_RPM,
                            @ResetValue		= Reset_Value,
                            @IsConfVar		= Is_Conformance_Variable,
                            @ESignLevel		= Esignature_Level,
                            @ESubtypeId		= Event_Subtype_Id,
                            @EDimension		= Event_Dimension,
                            @PEIId			= PEI_Id,
                            @SPCCalcTypeId	= SPC_Calculation_Type_Id,
                            @SPCVarTypeId	= SPC_Group_Variable_Type_Id,
                            @InputTag2		= Input_Tag2,
                            @SampRefVarId	= Sampling_Reference_Var_Id,
                            @StrSpecSet		= String_Specification_Setting,
                            @WGroupDSId		= Write_Group_DS_Id,
                            @CPKSGroupSize	= CPK_SubGroup_Size,
                            @RLagTime		= ReadLagTime,
                            @ReloadFlag		= Reload_Flag,
                            @ELookup		= Perform_Event_Lookup,
                            @Debug			= Debug,
                            @IgnoreEStatus	= Ignore_Event_Status
                    FROM #VariablesLocal v
                    JOIN dbo.Variables_Base vb (NOLOCK) ON vb.Var_Id = v.VarId
                    JOIN #ExtendedInfos ei ON ei.VarName = v.VarName
                    WHERE v.ItemId = @intCounter

                    EXEC spEM_PutVarSheetData
                            @VarId,
                            @DataType,
                            @Precision,
                            @SInterval,
                            @SOffset,
                            @SType,
                            @EngUnits,
                            @DSId,
                            @EventType,
                            @UnitReject,
                            @USummarize,
                            @VReject,
                            @Rank,
                            @InputTag,
                            @OutTag,
                            @DQTag,
                            @UELTag,
                            @URLTag,
                            @UWLTag,
                            @UULTag,
                            @TargetTag,
                            @LULTag,
                            @LWLTag,
                            @LRLTag,
                            @LELTag,
                            @TotFactor,
                            @SGroupId,
                            @SpecId,
                            @SAId,
                            @Repeat,
                            @RBackTime,
                            @SWindow,
                            @SArchive,
                            @ExtInfo,
                            @UDefined1,
                            @UDefined2,
                            @UDefined3,
                            @TFReset,
                            @ForceSign,
                            @TestName,
                            @ExtTF,
                            @ArStatOnly,
                            @CTId,
                            @CV,
                            @MaxRPM,
                            @ResetValue,
                            @IsConfVar,
                            @ESignLevel,
                            @ESubtypeId,
                            @EDimension,
                            @PEIId,
                            @SPCCalcTypeId,
                            @SPCVarTypeId,
                            @InputTag2,
                            @SampRefVarId,
                            @StrSpecSet,
                            @WGroupDSId,
                            @CPKSGroupSize,
                            @RLagTime,
                            @ReloadFlag,
                            @ELookup,
                            @Debug,
                            @IgnoreEStatus,
                            @intUser_Id

                    SET @intCounter = @intCounter + 1

                END
            END


        SELECT @intVarId as Var_Id

        DROP TABLE #TableFieldIds
        DROP TABLE #TableFieldValues
        DROP TABLE #TableFieldValuesTemp
        DROP TABLE #ExtendedInfos
        DROP TABLE #VariablesLocal
        DROP TABLE #AllUDPs



        ";


        
}
