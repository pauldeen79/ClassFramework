# Set the directory path and file extension
$directoryPath = Get-Location
$fileExtension = "*.cs"

# Get all files with the specified extension in the directory
$fileList = Get-ChildItem -Path $directoryPath -Filter $fileExtension -Recurse | Where-Object { $_.FullName -notlike '*\bin\*' -and $_.FullName -notlike '*\obj\*' -and $_.FullName -notlike '*.generated.cs' }

# Loop through each file and read its contents line by line
foreach ($file in $fileList) {
    $fileContent = Get-Content $file.FullName

    foreach ($line in $fileContent) {
        if ($line -like "namespace *") {
            # stop searching for using.
            # if it's found after the namespace, then it's probably in a string literal
            break
        }

        if ($line -like "using *") {
            Write-Host "$($file.FullName):$line"
            break
        }
    }
}
