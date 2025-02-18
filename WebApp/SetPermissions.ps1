$folderPath = "C:\inetpub\wwwroot\Webapp\wwwroot\Files"
$logPath = "C:\Buscador\WebApp\SetPermissionsLog.txt"

# Registrar log
Start-Transcript -Path $logPath -Append

Write-Output "Ejecutando como usuario: $env:USERNAME"
Write-Output "Iniciando configuración de permisos en $folderPath..."

# 1️⃣ Esperar para evitar conflictos si Visual Studio eliminó la carpeta temporalmente
Start-Sleep -Seconds 10  
Write-Output "Verificando si la carpeta Files existe después de la espera..."

# 2️⃣ Capturar errores en Test-Path
$folderExists = $false
try {
    $folderExists = Test-Path $folderPath -PathType Container
} catch {
    Write-Output "Error verificando la existencia de la carpeta Files: $_"
}

if (-Not $folderExists) {
    Write-Output "⚠️ La carpeta Files NO EXISTE después de la espera. Creándola..."
    New-Item -Path $folderPath -ItemType Directory -Force
} else {
    Write-Output "✅ La carpeta Files ya existe después de la espera."
}

# 3️⃣ Verificar si se está ejecutando como Administrador
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-Not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Output "El script no tiene privilegios de Administrador. Reiniciando con permisos elevados..."
    
    # Reejecutar con permisos de Administrador
    Start-Process powershell -ArgumentList "-ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

try {
    # 4️⃣ Verificar si "Everyone" ya tiene permisos y eliminarlos antes de aplicar nuevos
    Write-Output "Verificando permisos actuales..."
    $acl = Get-Acl $folderPath
    $existingRule = $acl.Access | Where-Object { $_.IdentityReference -eq "Everyone" }

    if ($existingRule) {
        Write-Output "Eliminando permisos previos de Everyone..."
        icacls $folderPath /remove Everyone
    }

    # 5️⃣ Asignar permisos "Everyone: FullControl"
    Write-Output "Asignando permisos 'Everyone: FullControl' en $folderPath..."
    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule("Everyone", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.AddAccessRule($rule)
    Set-Acl -Path $folderPath -AclObject $acl
    Write-Output "Permisos aplicados correctamente con Set-Acl."

    # 6️⃣ Aplicar permisos con icacls como respaldo
    Write-Output "Aplicando permisos con icacls..."
    icacls $folderPath /grant Everyone:F /T /C
    Write-Output "Permisos aplicados correctamente con icacls."

} catch {
    Write-Output "Error al aplicar permisos: $_"
}

Write-Output "Finalizando configuración de permisos..."
Stop-Transcript
