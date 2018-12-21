---
external help file: Microsoft.Azure.Commands.ResourceManager.Blueprint.dll-Help.xml
Module Name: AzureRM.Blueprint
online version:
schema: 2.0.0
---

# Get-AzureRmBlueprint

## SYNOPSIS
{{Fill in the Synopsis}}

## SYNTAX

### BlueprintByDefaultSet (Default)
```
Get-AzureRmBlueprint [[-ManagementGroupName] <String>] [[-Name] <String>]
 [-DefaultProfile <IAzureContextContainer>] [<CommonParameters>]
```

### BlueprintByVersion
```
Get-AzureRmBlueprint [-ManagementGroupName] <String> [-Name] <String> [[-Version] <String>]
 [-DefaultProfile <IAzureContextContainer>] [<CommonParameters>]
```

### BlueprintByLatestPublished
```
Get-AzureRmBlueprint [-ManagementGroupName] <String> [-Name] <String> [-LatestPublished]
 [-DefaultProfile <IAzureContextContainer>] [<CommonParameters>]
```

## DESCRIPTION
{{Fill in the Description}}

## EXAMPLES

### Example 1: Get Blueprints in high level Management Group scope.
```powershell
PS C:\> Get-AzureRmBlueprint
```

Gets Blueprints definitions within the high level Management groups that the user is part of.

### Example 1: Get Blueprints in high level Management Group scope.
```powershell
PS C:\> Get-AzureRmBlueprint
```

Gets Blueprints definitions within the high level Management groups that the user is part of.

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

### -LatestPublished
The latest published Blueprint flag.
When set, execution returns the latest published version of Blueprint.
Default to false.

```yaml
Type: SwitchParameter
Parameter Sets: BlueprintByLatestPublished
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ManagementGroupName
Management group name.

```yaml
Type: String
Parameter Sets: BlueprintByDefaultSet
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

```yaml
Type: String
Parameter Sets: BlueprintByVersion, BlueprintByLatestPublished
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Name
Blueprint definition name.

```yaml
Type: String
Parameter Sets: BlueprintByDefaultSet
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

```yaml
Type: String
Parameter Sets: BlueprintByVersion, BlueprintByLatestPublished
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Version
Blueprint definition version.

```yaml
Type: String
Parameter Sets: BlueprintByVersion
Aliases:

Required: False
Position: 2
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
