param(
    [string]$BaseUrl = 'http://localhost:5666',
    [switch]$RequireRuntime,
    [switch]$SkipRuntime
)

$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '..')
$failures = New-Object System.Collections.Generic.List[string]
$warnings = New-Object System.Collections.Generic.List[string]

function Write-Check {
    param(
        [string]$Status,
        [string]$Message
    )

    Write-Information ("[{0}] {1}" -f $Status, $Message) -InformationAction Continue
}

function Add-Failure {
    param([string]$Message)

    $failures.Add($Message)
    Write-Check 'FAIL' $Message
}

function Add-Warning {
    param([string]$Message)

    $warnings.Add($Message)
    Write-Check 'WARN' $Message
}

function Add-Pass {
    param([string]$Message)

    Write-Check 'PASS' $Message
}

function Test-FileExists {
    param(
        [string]$RelativePath,
        [string]$Label
    )

    $path = Join-Path $root $RelativePath
    if (Test-Path -LiteralPath $path) {
        Add-Pass "$Label found: $RelativePath"
        return $true
    }

    Add-Failure "$Label missing: $RelativePath"
    return $false
}

function Get-TextFile {
    param([string]$RelativePath)

    $path = Join-Path $root $RelativePath
    if (-not (Test-Path -LiteralPath $path)) {
        return ''
    }

    return Get-Content -Raw -LiteralPath $path
}

function Assert-Contains {
    param(
        [string]$Text,
        [string]$Pattern,
        [string]$Message
    )

    if ($Text -match $Pattern) {
        Add-Pass $Message
        return
    }

    Add-Failure $Message
}

function Test-Url {
    param(
        [string]$Url,
        [string]$Label
    )

    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 8
        if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 400) {
            Add-Pass "$Label reachable: $Url"
            return
        }

        Add-Failure "$Label returned HTTP $($response.StatusCode): $Url"
    }
    catch {
        $message = "$Label unreachable: $Url. $($_.Exception.Message)"
        if ($RequireRuntime) {
            Add-Failure "Runtime check failed. $message"
        }
        else {
            Add-Warning $message
        }
    }
}

Write-Information 'MVP verification started' -InformationAction Continue
Write-Information "Repository: $root" -InformationAction Continue
Write-Information "Base URL: $BaseUrl" -InformationAction Continue
Write-Information '' -InformationAction Continue

$hasCompose = Test-FileExists 'docker-compose.yml' 'Docker Compose file'
$hasEnvExample = Test-FileExists '.env.example' 'Root environment example'
Test-FileExists 'docs/MVP_ACCEPTANCE_CHECKLIST.md' 'MVP acceptance checklist' | Out-Null
Test-FileExists 'vue-vben-admin/apps/web-antd/.env.production' 'Frontend production env' | Out-Null
Test-FileExists 'vue-vben-admin/apps/web-antd/dist/manifest.webmanifest' 'PWA manifest.webmanifest' | Out-Null
Test-FileExists 'vue-vben-admin/apps/web-antd/dist/sw.js' 'PWA sw.js' | Out-Null

if ($hasCompose) {
    $compose = Get-TextFile 'docker-compose.yml'
    Assert-Contains $compose 'api:' 'docker-compose.yml defines api service'
    Assert-Contains $compose 'web:' 'docker-compose.yml defines web service'
    Assert-Contains $compose 'mysql:' 'docker-compose.yml defines mysql service'
    Assert-Contains $compose '/healthz/live' 'api healthcheck uses /healthz/live'
    Assert-Contains $compose '/health' 'web healthcheck uses /health'
    Assert-Contains $compose 'Database__AutoMigrate' 'api supports Database__AutoMigrate'
    Assert-Contains $compose 'Database__SeedOnStartup' 'api supports Database__SeedOnStartup'
}

if ($hasEnvExample) {
    $envExample = Get-TextFile '.env.example'
    foreach ($name in @(
            'MYSQL_ROOT_PASSWORD',
            'DB_CONNECTION',
            'JWT_SECRET_KEY',
            'SECURITY_ENCRYPTION_KEY',
            'OPENAI_API_KEY',
            'DATABASE_AUTO_MIGRATE=true',
            'DATABASE_SEED_ON_STARTUP=true'
        )) {
        Assert-Contains $envExample ([regex]::Escape($name)) ".env.example contains $name"
    }
}

$frontendProductionEnv = Get-TextFile 'vue-vben-admin/apps/web-antd/.env.production'
Assert-Contains $frontendProductionEnv 'VITE_GLOB_API_URL=/api' 'frontend production API URL points to /api'

$checklist = Get-TextFile 'docs/MVP_ACCEPTANCE_CHECKLIST.md'
foreach ($account in @('vben', 'admin', 'jack', 'lisa')) {
    Assert-Contains $checklist "\| ``$account`` \|" "acceptance checklist documents $account demo account"
}

if (-not $SkipRuntime) {
    $docker = Get-Command docker -ErrorAction SilentlyContinue
    if ($docker) {
        try {
            $composeOutput = & docker compose ps --format json 2>$null
            if ($LASTEXITCODE -eq 0 -and $composeOutput) {
                Add-Pass 'docker compose ps is available'
                Write-Information $composeOutput -InformationAction Continue
            }
            else {
                Add-Warning 'docker compose ps did not return running services. Start with: rtk docker compose --env-file .env up -d --build'
            }
        }
        catch {
            Add-Warning "docker compose ps failed: $($_.Exception.Message)"
        }
    }
    else {
        Add-Warning 'Docker CLI is not available on this machine. Runtime container checks were skipped.'
    }

    Test-Url $BaseUrl 'web'
    Test-Url "$BaseUrl/api/healthz/live" 'api live health'
    Test-Url "$BaseUrl/api/healthz/ready" 'api ready health'
}
else {
    Add-Warning 'Runtime checks skipped by -SkipRuntime.'
}

Write-Information '' -InformationAction Continue
Write-Information ("Summary: {0} failure(s), {1} warning(s)" -f $failures.Count, $warnings.Count) -InformationAction Continue

if ($failures.Count -gt 0) {
    Write-Information 'MVP verification failed' -InformationAction Continue
    exit 1
}

Write-Information 'MVP verification passed' -InformationAction Continue
exit 0
