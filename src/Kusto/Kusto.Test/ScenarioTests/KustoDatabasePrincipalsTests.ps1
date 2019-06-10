<#
.SYNOPSIS
Tests Kusto cluster lifecycle (Create, Update, Get, List, Delete).
#>
function Test-KustoDatabasePrincipalsLifecycle
{
	try
	{  
		$RGlocation = Get-RG-Location
		$location = Get-Location
		$resourceGroupName = Get-RG-Name
		$clusterName = Get-Cluster-Name
		$sku = Get-Sku
		$databaseName = Get-Database-Name
		$resourceType =  Get-Database-Type
		$softDeletePeriod =  Get-Soft-Delete-Period
		$hotCachePeriod =  Get-Hot-Cache-Period
		$principalsList = Get-Principals-List-To-Add
		
		$softDeletePeriodUpdated = Get-Updated-Soft-Delete-Period
		$hotCachePeriodUpdated = Get-Updated-Hot-Cache-Period
		

		#create cluster for the databases
		New-AzResourceGroup -Name $resourceGroupName -Location $RGlocation
		$clusterCreated = New-AzKustoCluster -ResourceGroupName $resourceGroupName -Name $clusterName -Location $location -Sku $sku

		#create database for database principals
		$databaseCreated = New-AzKustoDatabase -ResourceGroupName $resourceGroupName -ClusterName $clusterName -Name $databaseName -SoftDeletePeriod $softDeletePeriod -HotCachePeriod $hotCachePeriod


		$principal = $principalsList[0]
		[array]$returnedPrincipalsList = Add-AzKustoDatabasePrincipals -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -DatabasePrincipal $principalsList
		Validate_DatabasePrincipal $returnedPrincipalsList[1] $principal.Name $principal.Role $principal.Type $principal.Email

		[array]$returnedPrincipalsList = Get-AzKustoDatabasePrincipals -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName 
		Validate_DatabasePrincipal $returnedPrincipalsList[1] $principal.Name $principal.Role $principal.Type $principal.Email

		[array]$returnedPrincipalsList = Remove-AzKustoDatabasePrincipals -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -DatabasePrincipal $principalsList
		Ensure_DatabasePrincipals_Length $returnedPrincipalsList


		#input Object
		[array]$returnedPrincipalsList= Add-AzKustoDatabasePrincipals -InputObject $databaseCreated -DatabasePrincipal $principalsList
		Validate_DatabasePrincipal $returnedPrincipalsList[1] $principal.Name $principal.Role $principal.Type $principal.Email

		[array]$returnedPrincipalsList = Get-AzKustoDatabasePrincipals -InputObject $databaseCreated
		Validate_DatabasePrincipal $returnedPrincipalsList[1] $principal.Name $principal.Role $principal.Type $principal.Email

		[array]$returnedPrincipalsList = Remove-AzKustoDatabasePrincipals -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -DatabasePrincipal $principalsList
		Ensure_DatabasePrincipals_Length $returnedPrincipalsList



		#resource ID
		[array]$returnedPrincipalsList= Add-AzKustoDatabasePrincipals -ResourceId $databaseCreated.Id -DatabasePrincipal $principalsList
		Validate_DatabasePrincipal $returnedPrincipalsList[1] $principal.Name $principal.Role $principal.Type $principal.Email

		[array]$returnedPrincipalsList = Get-AzKustoDatabasePrincipals -ResourceId $databaseCreated.Id 
		Validate_DatabasePrincipal $returnedPrincipalsList[1] $principal.Name $principal.Role $principal.Type $principal.Email

		[array]$returnedPrincipalsList = Remove-AzKustoDatabasePrincipals -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -DatabasePrincipal $principalsList
		Ensure_DatabasePrincipals_Length $returnedPrincipalsList


	}
	finally
	{
		# cleanup the resource group that was used in case it still exists. This is a best effort task, we ignore failures here.
		Invoke-HandledCmdlet -Command {Remove-AzKustoCluster -ResourceGroupName $resourceGroupName -Name $clusterName -ErrorAction SilentlyContinue} -IgnoreFailures
		Invoke-HandledCmdlet -Command {Remove-AzResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue} -IgnoreFailures
	}
}


function Validate_DatabasePrincipal {
	Param ([Object]$DatabasePrincipal,
		[string]$Name,
		[string]$Role,
		[string]$Type,
		[string]$Email)

		Assert-AreEqual $Role $DatabasePrincipal.Role
		Assert-AreEqual $Name $DatabasePrincipal.Name
		Assert-AreEqual $Type $DatabasePrincipal.Type
		Assert-AreEqual $Email $DatabasePrincipal.Email 
}

function Ensure_DatabasePrincipals_Length {
	Param ([Object[]]$DatabasePrincipalList)
			
			if (-not $DatabasePrincipalList.Length -eq 1)
			{
				throw "DatabasePrincipalList.Length is larger then 1";
			}
}
