Function ParameterEncryption($string) {
    Add-Type -AssemblyName System.Security
    $bytes = [System.Text.Encoding]::Unicode.GetBytes($string)
    $SecureStr = [Security.Cryptography.ProtectedData]::Protect($bytes, $null, [Security.Cryptography.DataProtectionScope]::LocalMachine)
    $SecureStrBase64 = [System.Convert]::ToBase64String($SecureStr)
    return $SecureStrBase64
}

$parameter = Read-Host "Please input the parameter you want to encrypt"

$encryptedParam = ParameterEncryption($parameter)

Write-Host "Encrypted parameter: $encryptedParam"

Read-Host "Press Enter to Exit"