<#
.SYNOPSIS
Test Blueprint assignment cmdlets. Get a single Blueprint and assignment it to a subscription and delete.
#>
function Test-BlueprintAssignment
{
	
	$mgId = "AzBlueprint";
	$blueprintName = "Filiz_Powershell_Test";
	$subscriptionId = "4ce8c9fe-cadc-47d6-9c76-335812fd59df";
	$assignmentName = "PS-Test-Assignment"
	$location = "East US";
	$parameters = @{P1="v1"; P2="v2"};

    $blueprint = Get-AzureRmBlueprint -ManagementGroupId $mgId -Name $blueprintName
	Assert-NotNull $blueprint
	Assert-AreEqual $blueprintName $blueprint.Name
	
	$assignment = New-AzureRMBlueprintAssignment -Name $assignmentName -Blueprint $blueprint -SubscriptionId $subscriptionId -Location $location -Parameters $parameters

	$expectedProvisioningState = "Creating"

	Assert-NotNull $assignment
	Assert-AreEqual $assignment.ProvisioningState $expectedProvisioningState

	# $removeAssignment = RemoveAzureRmBlueprintAssignment -Name $assignmentName -SubscriptionId $subscriptionId
}
