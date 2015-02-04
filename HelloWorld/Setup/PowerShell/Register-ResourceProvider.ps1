# PowerShell script to register Windows Azure Pack resource provider.
# Copyright (c) Microsoft Corporation. All rights reserved.

# NOTE: This script is designed to run on a machine where MgmtSvc-AdminAPI is installed.
# The *-MgmtSvcResourceProviderConfiguration cmdlets resolve the connection string and encryption key parameters from the web.config of the MgmtSvc-AdminAPI web site.

$rpName = 'helloworld'

Write-Host -ForegroundColor Green "Get existing resource provider '$rpName'..."
$rp = Get-MgmtSvcResourceProviderConfiguration -Name $rpName
if ($rp -ne $null)
{
    Write-Host -ForegroundColor Green "Remove existing resource provider '$rpName' $($rp.InstanceId)..."
    $rp = Remove-MgmtSvcResourceProviderConfiguration -Name $rpName -InstanceId $rp.InstanceId
}
else
{
    Write-Host -ForegroundColor Green "Resource provider '$rpName' not found."
}

$hostName = "$env:ComputerName" + ":30030"
$userName = "admin"
$password = "!!123abc"

Write-Host -ForegroundColor Green "Create new resource provider '$rpName'..."
$rpSettings = @{
    'Name' = $rpName;
    'DisplayName' = 'Hello OpenStack';
    'InstanceDisplayName' = 'Hello World';
    'AdminForwardingAddress' = "http://$hostName/admin";
    'AdminAuthenticationMode' = 'Basic';
    'AdminAuthenticationUserName' = $userName;
    'AdminAuthenticationPassword' = $password;        
    'TenantForwardingAddress' = "http://$hostName/";
    'TenantAuthenticationMode' = 'Basic';
    'TenantAuthenticationUserName' = $userName;
    'TenantAuthenticationPassword' = $password;
    'TenantSourceUriTemplate' = '{subid}/services/helloworld/{*path}';
    'TenantTargetUriTemplate' = 'subscriptions/{subid}/{*path}';
    'NotificationForwardingAddress' = "http://$hostName/admin";
    'NotificationAuthenticationMode' = 'Basic';
    'NotificationAuthenticationUserName' = $userName;
    'NotificationAuthenticationPassword' = $password;
}
$rp = New-MgmtSvcResourceProviderConfiguration @rpSettings
$rp.MaxQuotaUpdateBatchSize = 5
Write-Host -ForegroundColor Green "Created new resource provider '$rpName'."

Write-Host -ForegroundColor Green "Add new resource provider '$rpName'..."
$rp = Add-MgmtSvcResourceProviderConfiguration -ResourceProvider $rp
Write-Host -ForegroundColor Green "Added new resource provider '$rpName'."

Write-Host -ForegroundColor Green "Get existing resource provider '$rpName' as Xml..."
Get-MgmtSvcResourceProviderConfiguration -Name $rpName -as XmlString
