---
external help file: Microsoft.Azure.Commands.ResourceManager.Blueprint.dll-Help.xml
Module Name: AzureRM.Blueprint
online version:
schema: 2.0.0
---

# Get-AzureRmBlueprintAssignment

## SYNOPSIS
Gets a single or a list of Blueprint assignments.

## SYNTAX

### BlueprintAssignmentByName
```
Get-AzureRmBlueprintAssignment [-SubscriptionId] <String> [-Name] <String>
 [-DefaultProfile <IAzureContextContainer>] [<CommonParameters>]
```

### BlueprintAssignmentListBySubscriptionScope
```
Get-AzureRmBlueprintAssignment [[-SubscriptionId] <String>] [-DefaultProfile <IAzureContextContainer>]
 [<CommonParameters>]
```

## DESCRIPTION
Gets a single or a list of Blueprint assignments. Blueprint assignments can be queried at Subscription scope.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-AzureRmBlueprintAssignment -SubscriptionId 00000000-1111-0000-1111-000000000000
```

Gets a list of Blueprint assignments within the specified subscription.

### Example 2
```powershell
PS C:\> et-AzureRmBlueprintAssignment -SubscriptionId 00000000-1111-0000-1111-000000000000 -Name "myAssignmentName"
```

Gets the specified Blueprint assignment.

## PARAMETERS

### -DefaultProfile
The credentials, account, tenant, and subscription used for communication with Azure.

```yaml
Type: IAzureContextContainer
Parameter Sets: (All)
Aliases: AzureRmContext, AzureCredential

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Blueprint assignment name.

```yaml
Type: String
Parameter Sets: BlueprintAssignmentByName
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -SubscriptionId
SubscriptionId.

```yaml
Type: String
Parameter Sets: BlueprintAssignmentByName
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

```yaml
Type: String
Parameter Sets: BlueprintAssignmentListBySubscriptionScope
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
