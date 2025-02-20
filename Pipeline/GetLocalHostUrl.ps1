param (
    [Parameter(mandatory=$false)][int]$StartPort = 5000,
    [Parameter(mandatory=$false)][int]$EndPort = 60000
)

$usedPorts = (Get-NetTCPConnection).LocalPort + (Get-NetUDPEndpoint).LocalPort
$nextPort = $StartPort..$EndPort | where { $usedPorts -notcontains $_ } | select -first 1

if ($nextPort -is [int]) {
    Write-Host "Using port $nextPort"
    $url = "https://localhost:$nextPort/"
    Write-Host "Url: $url"
    Write-Host "##vso[task.setvariable variable=Urls.0]$url"
} else {
    throw "Can't find an available port"
}
