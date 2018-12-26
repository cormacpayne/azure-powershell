---
external help file: Microsoft.Azure.Commands.ResourceManager.Blueprint.dll-Help.xml
Module Name: AzureRM.Blueprint
online version:
schema: 2.0.0
---

# New-AzureRmBlueprintAssignment

## SYNOPSIS
{{Fill in the Synopsis}}

## SYNTAX

```
New-AzureRmBlueprintAssignment -Name <String> -Blueprint <PSBlueprintBase> [-SubscriptionId <String[]>]
 -Location <String> [-Parameters <Hashtable>] [-Lock <PSLockMode>] [-DefaultProfile <IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Assign a blueprint you created to one of the subscriptions in your management group hierarchy.

## EXAMPLES

### Example 1
```powershell
PS C:\> New-AzureRMBlueprintAssignment -Name "myAssignment" -Blueprint $blueprintObject -SubscriptionId 00000000-1111-0000-1111-000000000000 -Location "location" -Parameters @{P1="v1"; P2="v2"}
```

Create a new assignment.

### Example 2
```powershell
PS C:\> New-AzureRmBlueprintAssignment -Name "myAssignment" -Blueprint $blueprintObject -SubscriptionId 00000000-1111-0000-1111-000000000000 -Location "location" -Parameters @{P1="v1"; P2="v2"} -Lock AllResources
```

Create a new assignment with resource locks.

## PARAMETERS

### -Blueprint
Blueprint object.

```yaml
Type: PSBlueprintBase
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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

### -Location
Region for managed identity to be created in.
Learn more at aka.ms/blueprintmsi

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Lock
Lock resources.
Learn more at aka.ms/blueprintlocks

```yaml
Type: PSLockMode
Parameter Sets: (All)
Aliases:
Accepted values: Unknown, None, AllResources

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
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Parameters
Artifact parameters.

```yaml
Type: Hashtable
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -SubscriptionId
SubscriptionId to assign the Blueprint.
Can be a comma delimited list of subscriptionId strings.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

### Microsoft.Azure.Commands.Blueprint.Models.PSBlueprintBase

### System.String[]

### System.Collections.Hashtable

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
