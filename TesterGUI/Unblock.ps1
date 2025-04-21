# Path to the folder containing the .resx files
$folderPath = ".\CSharpExample"

# Get all .resx files in the folder and subfolders
$resxFiles = Get-ChildItem -Path $folderPath -Recurse -Filter "*.resx"

foreach ($file in $resxFiles) {
    # Unblock the file
    Unblock-File -Path $file.FullName
}