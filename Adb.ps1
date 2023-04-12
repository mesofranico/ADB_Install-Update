# Verifica se o script está sendo executado como administrador
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator))
{
    # Se não for, solicita a execução como administrador e reinicia o script
    Write-Host "Este script precisa ser executado como administrador." -ForegroundColor Yellow
    Start-Sleep -Seconds 2
    Start-Process powershell.exe -Verb RunAs -ArgumentList "-File",$MyInvocation.MyCommand.Path
    exit
}

# Define a política de execução para RemoteSigned
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser -Force

$installPath = "C:\platform-tools"

# Apagar a pasta de destino, se ela existir
if (Test-Path $installPath) {
    Write-Host "Removendo pasta existente em $installPath..."
    Remove-Item $installPath -Recurse
}

# Fazer o download da página de destino
$url = "https://androidsdkmanager.azurewebsites.net/Platformtools"
$pageContent = Invoke-WebRequest -Uri $url

# Extrair o link para a versão mais recente do Windows do Platform Tools
$linkPattern = "https://dl.google.com/android/repository/platform-tools_r\d+.\d+.\d+-windows.zip"
$platformToolsLink = [regex]::Matches($pageContent.Content, $linkPattern).Value

# Fazer o download do arquivo ZIP
$downloadPath = "C:\platform-tools.zip"
Invoke-WebRequest -Uri $platformToolsLink[0] -OutFile $downloadPath

# Extrair o arquivo ZIP para a pasta de destino
$extractPath = "C:\"
Expand-Archive -Path $downloadPath -DestinationPath $extractPath

# Limpar o arquivo ZIP
Remove-Item $downloadPath

Write-Host "Platform Tools instalado em $installPath"

Write-Host "Pressione qualquer tecla para continuar..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") | Out-Null
