$folderPath = "C:\inetpub\wwwroot\Webapp\wwwroot\Files"

# Verifica si la carpeta existe
if (Test-Path $folderPath) {
    Write-Output "🔹 La carpeta Files existe. Aplicando permisos..."

    # Otorgar permisos de escritura a "Everyone"
    icacls $folderPath /grant Everyone:F /T

    Write-Output "✅ Permisos aplicados correctamente."
} else {
    Write-Output "⚠️ La carpeta Files no existe. Creándola..."
    
    # Crear la carpeta si no existe
    New-Item -Path $folderPath -ItemType Directory

    # Asignar permisos
    icacls $folderPath /grant Everyone:F /T

    Write-Output "✅ Carpeta creada y permisos aplicados correctamente."
}
