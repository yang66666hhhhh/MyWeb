param(
    [string]$ScriptPath = (Join-Path $PSScriptRoot 'verify-mvp.ps1')
)

$ErrorActionPreference = 'Stop'

function Assert-True {
    param(
        [bool]$Condition,
        [string]$Message
    )

    if (-not $Condition) {
        throw $Message
    }
}

Assert-True (Test-Path -LiteralPath $ScriptPath) "Expected verification script to exist at $ScriptPath."

$staticOutput = & $ScriptPath -SkipRuntime 6>&1 2>&1 | Out-String
$staticResult = [PSCustomObject]@{
    ExitCode = $LASTEXITCODE
    Output = $staticOutput
}
Assert-True ($staticResult.ExitCode -eq 0) "Expected static MVP verification to pass. Output:`n$($staticResult.Output)"
Assert-True ($staticResult.Output -match 'MVP verification passed') "Expected success summary in static verification output."
Assert-True ($staticResult.Output -match 'docker-compose.yml') "Expected Docker Compose file check in output."
Assert-True ($staticResult.Output -match 'manifest.webmanifest') "Expected PWA manifest check in output."

$strictOutput = & $ScriptPath -BaseUrl 'http://127.0.0.1:1' -RequireRuntime 6>&1 2>&1 | Out-String
$strictResult = [PSCustomObject]@{
    ExitCode = $LASTEXITCODE
    Output = $strictOutput
}
Assert-True ($strictResult.ExitCode -ne 0) "Expected strict runtime verification to fail for an unreachable URL."
Assert-True ($strictResult.Output -match 'Runtime check failed') "Expected strict runtime failure to be reported."

Write-Host 'verify-mvp tests passed'
