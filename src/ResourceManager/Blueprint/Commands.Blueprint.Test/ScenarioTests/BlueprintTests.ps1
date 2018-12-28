<#
.SYNOPSIS
Test Blueprint cmdlets. Get a single Blueprint and list of Blueprints.
#>
function Test-GetBlueprint
{
    $blueprints = Get-AzureRmBlueprint

    Assert-True {$blueprints.Count -ge 1}
	Assert-NotNull $blueprints[0].Name
	Assert-NotNull $blueprints[0].Id
	Assert-NotNull $blueprints[0].ManagementGroupName
	Assert-NotNull $blueprints[0].TargetScope
	Assert-NotNull $blueprints[0].Description

	$mgId = $blueprints[0].ManagementGroupName
	Assert-NotNull $mgId

    $mgBlueprints = Get-AzureRmBlueprint -ManagementGroupId $mgId

	Assert-True {$mgBlueprints.Count -ge 1}
	Assert-NotNull $mgBlueprints[0].Name
	Assert-NotNull $mgBlueprints[0].Id
	Assert-NotNull $mgBlueprints[0].ManagementGroupName
	Assert-NotNull $mgBlueprints[0].TargetScope
	Assert-NotNull $mgBlueprints[0].Description

	$blueprintName = $blueprints[0].Name
	Assert-NotNull $blueprintName

    $blueprint = Get-AzureRmBlueprint -ManagementGroupId $mgId -Name $blueprintName

	Assert-AreEqual $blueprintName $blueprint.Name
	Assert-NotNull $blueprint.Name
	Assert-NotNull $blueprint.Id
	Assert-NotNull $blueprint.ManagementGroupName
	Assert-NotNull $blueprint.TargetScope
	Assert-NotNull $blueprint.Description
}
