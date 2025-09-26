param(
  [string]$Api = "https://localhost:7165/api",
  [Parameter(Mandatory=$true)][string]$Email,
  [Parameter(Mandatory=$true)][string]$Password,
  [switch]$Insecure
)

if ($Insecure) {
  try { [System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true } } catch {}
}

$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

function Get-CookieValue {
  param(
    [Microsoft.PowerShell.Commands.WebRequestSession]$Session,
    [string]$Uri,
    [string]$Name
  )
  try {
    $u = [System.Uri]$Uri
    $cookies = $Session.Cookies.GetCookies($u)
    foreach ($c in $cookies) { if ($c.Name -eq $Name) { return $c.Value } }
    return $null
  } catch { return $null }
}

Write-Host "1) Login"
$loginBody = @{ email = $Email; password = $Password } | ConvertTo-Json
$loginRes = Invoke-WebRequest -Method POST -Uri ($Api.TrimEnd('/') + "/auth/login") -Body $loginBody -ContentType "application/json" -WebSession $session -UseBasicParsing -ErrorAction Stop

# Workaround: en Windows PowerShell clásico, CookieContainer puede no manejar SameSite=None.
# Extraemos Set-Cookie manualmente y armamos un header Cookie para siguientes requests.
$setCookies = @()
if ($loginRes.Headers.ContainsKey('Set-Cookie')) {
  $v = $loginRes.Headers['Set-Cookie']
  if ($v -is [System.Array]) { $setCookies += $v } else { $setCookies += ,$v }
}

$cookieMap = @{}
foreach ($line in $setCookies) {
  if (-not $line) { continue }
  $first = $line.Split(';')[0]
  $parts = $first.Split('=',2)
  if ($parts.Count -eq 2) { $cookieMap[$parts[0]] = $parts[1] }
}

$xsrf1 = (Get-CookieValue -Session $session -Uri $Api -Name "XSRF-TOKEN")
if (-not $xsrf1 -and $cookieMap.ContainsKey('XSRF-TOKEN')) { $xsrf1 = $cookieMap['XSRF-TOKEN'] }
$at1 = (Get-CookieValue -Session $session -Uri $Api -Name "access_token")
if (-not $at1 -and $cookieMap.ContainsKey('access_token')) { $at1 = $cookieMap['access_token'] }
$rt1 = (Get-CookieValue -Session $session -Uri $Api -Name "refresh_token")
if (-not $rt1 -and $cookieMap.ContainsKey('refresh_token')) { $rt1 = $cookieMap['refresh_token'] }

$cookieHeader = $null
if ($cookieMap.Keys.Count -gt 0) {
  $cookieHeader = ($cookieMap.GetEnumerator() | ForEach-Object { "{0}={1}" -f $_.Key,$_.Value }) -join '; '
}

Write-Host "2) /auth/me"
$headersMe = @{}
if ($cookieHeader) { $headersMe['Cookie'] = $cookieHeader }
$me1 = Invoke-WebRequest -Method GET -Uri ($Api.TrimEnd('/') + "/auth/me") -Headers $headersMe -WebSession $session -UseBasicParsing -ErrorAction SilentlyContinue
$me1Status = if ($me1) { $me1.StatusCode } else { 0 }

Write-Host "3) /auth/refresh"
$headers = @{}
if ($xsrf1) { $headers['X-XSRF-TOKEN'] = $xsrf1 }
if ($cookieHeader) { $headers['Cookie'] = $cookieHeader }
$refreshRes = Invoke-WebRequest -Method POST -Uri ($Api.TrimEnd('/') + "/auth/refresh") -Headers $headers -WebSession $session -UseBasicParsing -ErrorAction SilentlyContinue
$refreshStatus = if ($refreshRes) { $refreshRes.StatusCode } else { 0 }

# Actualizar cookies a partir de respuesta de refresh si llegaron
$setCookies2 = @()
if ($refreshRes -and $refreshRes.Headers.ContainsKey('Set-Cookie')) {
  $v2 = $refreshRes.Headers['Set-Cookie']
  if ($v2 -is [System.Array]) { $setCookies2 += $v2 } else { $setCookies2 += ,$v2 }
  foreach ($line in $setCookies2) {
    if (-not $line) { continue }
    $first = $line.Split(';')[0]
    $parts = $first.Split('=',2)
    if ($parts.Count -eq 2) { $cookieMap[$parts[0]] = $parts[1] }
  }
  $cookieHeader = ($cookieMap.GetEnumerator() | ForEach-Object { "{0}={1}" -f $_.Key,$_.Value }) -join '; '
}

$xsrf2 = (Get-CookieValue -Session $session -Uri $Api -Name "XSRF-TOKEN")
if (-not $xsrf2 -and $cookieMap.ContainsKey('XSRF-TOKEN')) { $xsrf2 = $cookieMap['XSRF-TOKEN'] }
$at2 = (Get-CookieValue -Session $session -Uri $Api -Name "access_token")
if (-not $at2 -and $cookieMap.ContainsKey('access_token')) { $at2 = $cookieMap['access_token'] }
$rt2 = (Get-CookieValue -Session $session -Uri $Api -Name "refresh_token")
if (-not $rt2 -and $cookieMap.ContainsKey('refresh_token')) { $rt2 = $cookieMap['refresh_token'] }

Write-Host "4) /auth/me (after)"
$headersMe2 = @{}
if ($cookieHeader) { $headersMe2['Cookie'] = $cookieHeader }
$me2 = Invoke-WebRequest -Method GET -Uri ($Api.TrimEnd('/') + "/auth/me") -Headers $headersMe2 -WebSession $session -UseBasicParsing -ErrorAction SilentlyContinue
$me2Status = if ($me2) { $me2.StatusCode } else { 0 }

$rotated = ($rt1 -ne $rt2) -and ($null -ne $rt2)

Write-Host ("Login:        {0}" -f $loginRes.StatusCode)
Write-Host ("Me (before):  {0}" -f $me1Status)
Write-Host ("Refresh:      {0}" -f $refreshStatus)
Write-Host ("Me (after):   {0}" -f $me2Status)
Write-Host ("Rotated RT?:  {0}" -f $rotated)
if (-not $xsrf1) { Write-Host "Warning: XSRF-TOKEN cookie no encontrada tras login; se intentó extraer de Set-Cookie." }
