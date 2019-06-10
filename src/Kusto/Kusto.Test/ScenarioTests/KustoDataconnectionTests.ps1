<#
.SYNOPSIS
Tests Kusto cluster lifecycle (Create, Update, Get, List, Delete).
#>
function Test-KustoDataconnectionLifecycle
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
		$databaseFullName = "$clusterName/$databaseName"
		$expectedException = Get-Database-Not-Exist-Message -DatabaseName $databaseName
		$storageAccountName = Get-Storage-Account-Name
		
		#to change
		$dataConnectionName = "dcname"
		$dataconnectionType = "eventhub"
		$consumerGroupName = Get-Event-Hub-Consumer-Group-Name
		$eventhubNamespaceName = Get-Event-Hub-Namespace-Name
		$eventhubname = Get-Event-Hub-Name

		
		

		#create cluster for the databases
		New-AzResourceGroup -Name $resourceGroupName -Location $RGlocation
		$clusterCreated = New-AzKustoCluster -ResourceGroupName $resourceGroupName -Name $clusterName -Location $location -Sku $sku

		#create database for data connection
		$databaseCreated = New-AzKustoDatabase -ResourceGroupName $resourceGroupName -ClusterName $clusterName -Name $databaseName -SoftDeletePeriod $softDeletePeriod -HotCachePeriod $hotCachePeriod


		#creating event hub for the data connection
		New-AzEventHubNamespace -ResourceGroupName $resourceGroupName  -Name $eventhubNamespaceName -Location  $location
		$eventHub = New-AzEventHub -ResourceGroupName $resourceGroupName -Namespace $eventhubNamespaceName -Name $eventhubname
		New-AzEventHubConsumerGroup -ResourceGroupName $resourceGroupName -Namespace $eventhubNamespaceName -EventHub $eventhubname -Name $consumerGroupName
		$eventhubResourceId = $eventHub.Id


		$dataConnection = New-AzKustoDataConnection -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -Name $dataConnectionName -DataConnectionType $dataconnectionType -EventHubResourceId $eventhubResourceId -ConsumerGroup $consumerGroup
		#verify data connection

		$dataConnection = Update-AzKustoDataConnection -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -Name $dataConnectionName -DataFormat "csv"
		#verify data connection

		$dataConnection = Get-AzKustoDataConnection -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -Name $dataConnectionName
		#verify data connection

		Remove-AzKustoDataConnection  -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -Name $dataConnectionName
		#verify data connection deleted

		# creating using resourceId
		$dataConnection = New-AzKustoDataConnection -ResourceId $databaseCreated.Id   -DataConnectionType $dataconnectionType -EventHubResourceId $eventhubResourceId -ConsumerGroup $consumerGroup
		#verify data connection

		$dataConnection = Update-AzKustoDataConnection -ResourceId $dataConnection.Id -DataFormat "csv"
		#verify data connection

		$dataConnection = Get-AzKustoDataConnection -ResourceId $dataConnection.Id
		#verify data connection

		Remove-AzKustoDataConnection  -ResourceId $dataConnection.Id
		#verify data connection deleted

		#Using inputObject
		$dataConnection = New-AzKustoDataConnection -InputObject $databaseCreated -Name $dataConnectionName -DataConnectionType $dataconnectionType -EventHubResourceId $eventhubResourceId -ConsumerGroup $consumerGroup
		#verify data connection

		$dataConnection = Update-AzKustoDataConnection -InputObject $dataConnection -DataFormat "csv"
		#verify data connection

		$dataConnection = Get-AzKustoDataConnection -InputObject $dataConnection
		#verify data connection

		Remove-AzKustoDataConnection -InputObject $dataConnection
		#verify data connection deleted

		

		Remove-AzKustoDatabase -ResourceGroupName $resourceGroupName -ClusterName $clusterName -Name $databaseName

		Remove-AzKustoCluster -ResourceGroupName $resourceGroupName -Name $clusterName
	}
	finally
	{
		# cleanup the resource group that was used in case it still exists. This is a best effort task, we ignore failures here.
		Invoke-HandledCmdlet -Command {Remove-AzKustoCluster -ResourceGroupName $resourceGroupName -Name $clusterName -ErrorAction SilentlyContinue} -IgnoreFailures
		Invoke-HandledCmdlet -Command {Remove-AzResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue} -IgnoreFailures
	}
}

<#
.SYNOPSIS
Tests Kusto Add Get and Remove using resourceId and InputObject
This Test requireds a cluster with a database and table (not available from the clients).
Do not record this test
#>
function Test-KustoEventGridDataConnection {
	
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
		$dataConnectionName = Get-DataConnection-Name
		$dataConnectionFullName = "$clusterName/$databaseName/$dataConnectionName"
		
		$storageAccountName = Get-Storage-Account-Name
		$dataconnectionType = "EventGrid"
		$consumerGroupName = Get-Event-Hub-Consumer-Group-Name
		$eventhubNamespaceName = Get-Event-Hub-Namespace-Name
		$eventhubname = Get-Event-Hub-Name

		
		#creating Storage account
		$storageAccount = New-AzStorageAccount -ResourceGroupName $resourceGroupName -Name $storageAccountName -SkuName Standard_LRS -Location $location 


		#creating event hub for the data connection
		New-AzEventHubNamespace -ResourceGroupName $resourceGroupName  -Name $eventhubNamespaceName -Location  $location
		$eventHub = New-AzEventHub -ResourceGroupName $resourceGroupName -Namespace $eventhubNamespaceName -Name $eventhubname
		New-AzEventHubConsumerGroup -ResourceGroupName $resourceGroupName -Namespace $eventhubNamespaceName -EventHub $eventhubname -Name $consumerGroupName
		$eventhubResourceId = $eventHub.Id
		
		$dataConnection = New-AzKustoDataConnection -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -Name $dataConnectionName -DataConnectionType $dataconnectionType -EventHubResourceId $eventhubResourceId -ConsumerGroup $consumerGroup
		#verify data connection

		$dataConnection = Update-AzKustoDataConnection -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -Name $dataConnectionName -DataFormat "csv"
		#verify data connection

		$dataConnection = Get-AzKustoDataConnection -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -Name $dataConnectionName
		#verify data connection

		Remove-AzKustoDataConnection  -ResourceGroupName $resourceGroupName -ClusterName $clusterName -DatabaseName $databaseName -Name $dataConnectionName
		#verify data connection deleted

		# creating using resourceId
		$dataConnection = New-AzKustoDataConnection -ResourceId $databaseCreated.Id   -DataConnectionType $dataconnectionType -EventHubResourceId $eventhubResourceId -ConsumerGroup $consumerGroup
		#verify data connection

		$dataConnection = Update-AzKustoDataConnection -ResourceId $dataConnection.Id -DataFormat "csv"
		#verify data connection

		$dataConnection = Get-AzKustoDataConnection -ResourceId $dataConnection.Id
		#verify data connection

		Remove-AzKustoDataConnection  -ResourceId $dataConnection.Id
		#verify data connection deleted

		#Using inputObject
		$dataConnection = New-AzKustoDataConnection -InputObject $databaseCreated -Name $dataConnectionName -DataConnectionType $dataconnectionType -EventHubResourceId $eventhubResourceId -ConsumerGroup $consumerGroup
		#verify data connection

		$dataConnection = Update-AzKustoDataConnection -InputObject $dataConnection -DataFormat "csv"
		#verify data connection

		$dataConnection = Get-AzKustoDataConnection -InputObject $dataConnection
		#verify data connection

		Remove-AzKustoDataConnection -InputObject $dataConnection
		#verify data connection deleted
	}
	finally
	{
		# no cleanup  is reuired
	}
}

function Validate_Database {
	Param ([Object]$Database,
		[string]$DatabaseFullName,
		[string]$Location,
		[string]$Type,
		[TimeSpan]$SoftDeletePeriod,
		[TimeSpan]$HotCachePeriod)
		Assert-AreEqual $DatabaseFullName $Database.Name
		Assert-AreEqual $Location $Database.Location
		Assert-AreEqual $ResourceType $Database.Type
		Assert-AreEqual $SoftDeletePeriod $Database.SoftDeletePeriod 
		Assert-AreEqual $HotCachePeriod $Database.HotCachePeriod 
}

function Ensure_DataConnection_Not_Exist {
	Param ([String]$ResourceGroupName,
			[String]$ClusterName,
			[string]$DatabaseName,
		[string]$ExpectedErrorMessage)
		$expectedException = $false;
		try
        {
			$databaseGetItemDeleted = Get-AzKustoDatabase -ResourceGroupName $ResourceGroupName -ClusterName $ClusterName -Name $DatabaseName
        }
        catch
        {
            if ($_ -Match $ExpectedErrorMessage)
            {
                $expectedException = $true;
            }
        }
        if (-not $expectedException)
        {
            throw "Expected exception from calling Get-AzKustoDatabase was not caught: '$expectedErrorMessage'.";
        }
}