# Database Restoration PowerShell Script for Swimming Academy
# This script automates the restoration of the database from the backup file

param(
    [string]$BackupPath = ".\SwimmingAcademy.bak",
    [string]$ServerInstance = "localhost",
    [string]$DatabaseName = "db_abab0e_omarsafyna"
)

Write-Host "Starting database restoration process..." -ForegroundColor Green

# Check if backup file exists
if (-not (Test-Path $BackupPath)) {
    Write-Host "Error: Backup file not found at $BackupPath" -ForegroundColor Red
    exit 1
}

Write-Host "Backup file found: $BackupPath" -ForegroundColor Yellow

# SQL Server connection string
$connectionString = "Server=$ServerInstance;Integrated Security=true;"

try {
    # Create SQL Server connection
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    $connection.Open()
    
    Write-Host "Connected to SQL Server successfully" -ForegroundColor Green
    
    # Check if database exists
    $checkDbQuery = "SELECT name FROM sys.databases WHERE name = '$DatabaseName'"
    $command = New-Object System.Data.SqlClient.SqlCommand($checkDbQuery, $connection)
    $result = $command.ExecuteScalar()
    
    if ($result) {
        Write-Host "Database '$DatabaseName' exists. Dropping it..." -ForegroundColor Yellow
        
        # Drop existing database
        $dropQuery = @"
        ALTER DATABASE [$DatabaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE [$DatabaseName];
"@
        $command.CommandText = $dropQuery
        $command.ExecuteNonQuery()
        Write-Host "Existing database dropped successfully" -ForegroundColor Green
    }
    
    # Restore database
    Write-Host "Restoring database from backup..." -ForegroundColor Yellow
    
    $restoreQuery = @"
    RESTORE DATABASE [$DatabaseName] 
    FROM DISK = '$BackupPath'
    WITH 
        MOVE 'SwimmingAcademy' TO 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\${DatabaseName}.mdf',
        MOVE 'SwimmingAcademy_Log' TO 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\${DatabaseName}_Log.ldf',
        REPLACE,
        RECOVERY;
"@
    
    $command.CommandText = $restoreQuery
    $command.ExecuteNonQuery()
    
    Write-Host "Database restored successfully!" -ForegroundColor Green
    
    # Verify restoration
    Write-Host "Verifying restoration..." -ForegroundColor Yellow
    $verifyQuery = "USE [$DatabaseName]; SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';"
    $command.CommandText = $verifyQuery
    $tableCount = $command.ExecuteScalar()
    
    Write-Host "Database verification complete. Found $tableCount tables." -ForegroundColor Green
    
} catch {
    Write-Host "Error during database restoration: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host "Database restoration process completed successfully!" -ForegroundColor Green 