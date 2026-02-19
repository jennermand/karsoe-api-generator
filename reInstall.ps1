# PowerShell script to uninstall, repack, and reinstall the .NET tool for testing

# Set the package ID and version
$packageId = "Karsoe.ApiGenerator"
$version = "1.0.0"

# Step 1: Uninstall the tool globally
Write-Host "Uninstalling $packageId globally..."
dotnet tool uninstall $packageId -g

# Step 2: Clean and pack the project
Write-Host "Cleaning and packing the project..."
dotnet clean
dotnet pack -c Release

# Step 3: Install the tool from the local package
Write-Host "Installing $packageId from local package..."
dotnet tool install $packageId --add-source ./bin/Release -g

# Verify installation
Write-Host "Verifying installation..."
dotnet tool list -g | Select-String $packageId

Write-Host "Done! You can now test the tool with 'karsoe-api-gen --help'"