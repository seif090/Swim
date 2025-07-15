-- Database Restoration Script for Swimming Academy
-- This script restores the database from the SwimmingAcademy.bak file

-- Step 1: Check if the database exists and drop it if it does
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'db_abab0e_omarsafyna')
BEGIN
    ALTER DATABASE [db_abab0e_omarsafyna] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [db_abab0e_omarsafyna];
    PRINT 'Existing database dropped successfully.';
END
GO

-- Step 2: Restore the database from backup
RESTORE DATABASE [db_abab0e_omarsafyna] 
FROM DISK = 'C:\SwimmingAcademy.bak'  -- Adjust path as needed
WITH 
    MOVE 'SwimmingAcademy' TO 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\db_abab0e_omarsafyna.mdf',
    MOVE 'SwimmingAcademy_Log' TO 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\db_abab0e_omarsafyna_Log.ldf',
    REPLACE,
    RECOVERY;
GO

-- Step 3: Verify the restoration
USE [db_abab0e_omarsafyna];
GO

-- Check if tables exist
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';
GO

PRINT 'Database restoration completed successfully!';
GO 