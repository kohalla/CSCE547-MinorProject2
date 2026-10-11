/* =====================================================================
   Schedule Optimizer - Database Schema
   File 1 of 3: creates the database and all tables.
   Run order: 01_CreateSchema.sql -> 02_SeedData.sql -> 03_Security.sql

   Tables
     Roles            - lookup: Admin / Standard
     Users            - app accounts (RoleID decides admin rights)
     Categories       - lookup: goal types, daily tasks, chores, fun activities, appointment types
     Appointments     - dated events with start/end times
     Goals            - school / professional / hobby goals with progress tracking
     Tasks            - recurring daily tasks, chores and fun activities
     TaskCompletions  - log of each time a task was done
     AuditLog         - ADMIN ONLY: security and change history
   ===================================================================== */

IF DB_ID(N'ScheduleOptimizerDB') IS NULL
    CREATE DATABASE ScheduleOptimizerDB;
GO

USE ScheduleOptimizerDB;
GO

/* Drop in reverse dependency order so the script can be re-run */
DROP PROCEDURE IF EXISTS dbo.usp_GetAuditLog;
DROP TABLE IF EXISTS dbo.AuditLog;
DROP TABLE IF EXISTS dbo.TaskCompletions;
DROP TABLE IF EXISTS dbo.Tasks;
DROP TABLE IF EXISTS dbo.Goals;
DROP TABLE IF EXISTS dbo.Appointments;
DROP TABLE IF EXISTS dbo.Categories;
DROP TABLE IF EXISTS dbo.Users;
DROP TABLE IF EXISTS dbo.Roles;
GO

/* ---------------------------------------------------------------------
   Roles
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Roles
(
    RoleID       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    RoleName     NVARCHAR(50)      NOT NULL CONSTRAINT UQ_Roles_RoleName UNIQUE,
    Description  NVARCHAR(200)     NULL
);
GO

/* ---------------------------------------------------------------------
   Users
   Users are deactivated (IsActive = 0) rather than deleted, so their
   history in AuditLog stays intact.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Users
(
    UserID        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    RoleID        INT               NOT NULL CONSTRAINT FK_Users_Roles REFERENCES dbo.Roles(RoleID),
    Username      NVARCHAR(50)      NOT NULL CONSTRAINT UQ_Users_Username UNIQUE,
    Email         NVARCHAR(255)     NOT NULL CONSTRAINT UQ_Users_Email UNIQUE,
    PasswordHash  NVARCHAR(128)     NOT NULL,
    FirstName     NVARCHAR(50)      NOT NULL,
    LastName      NVARCHAR(50)      NOT NULL,
    IsActive      BIT               NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedAt     DATETIME2(0)      NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSDATETIME()),
    LastLoginAt   DATETIME2(0)      NULL
);
GO

/* ---------------------------------------------------------------------
   Categories
   CategoryGroup tells the app which screen/list the category belongs to.
   ColorHex can be bound directly to a WPF Brush for color-coding.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Categories
(
    CategoryID     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
    CategoryName   NVARCHAR(50)      NOT NULL CONSTRAINT UQ_Categories_Name UNIQUE,
    CategoryGroup  NVARCHAR(20)      NOT NULL
        CONSTRAINT CK_Categories_Group CHECK (CategoryGroup IN
            (N'Goal', N'DailyTask', N'Chore', N'FunActivity', N'Appointment')),
    Description    NVARCHAR(200)     NULL,
    ColorHex       CHAR(7)           NOT NULL CONSTRAINT DF_Categories_Color DEFAULT ('#808080')
        CONSTRAINT CK_Categories_Color CHECK (ColorHex LIKE '#[0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f]')
);
GO

/* ---------------------------------------------------------------------
   Appointments
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Appointments
(
    AppointmentID          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Appointments PRIMARY KEY,
    UserID                 INT               NOT NULL CONSTRAINT FK_Appointments_Users REFERENCES dbo.Users(UserID),
    CategoryID             INT               NOT NULL CONSTRAINT FK_Appointments_Categories REFERENCES dbo.Categories(CategoryID),
    Title                  NVARCHAR(100)     NOT NULL,
    Location               NVARCHAR(150)     NULL,
    StartDateTime          DATETIME2(0)      NOT NULL,
    EndDateTime            DATETIME2(0)      NOT NULL,
    Notes                  NVARCHAR(500)     NULL,
    ReminderMinutesBefore  INT               NOT NULL CONSTRAINT DF_Appointments_Reminder DEFAULT (30)
        CONSTRAINT CK_Appointments_Reminder CHECK (ReminderMinutesBefore >= 0),
    CONSTRAINT CK_Appointments_Times CHECK (EndDateTime > StartDateTime)
);
GO

CREATE INDEX IX_Appointments_User_Start ON dbo.Appointments (UserID, StartDateTime);
GO

/* ---------------------------------------------------------------------
   Goals
   ProgressPercent is computed (0-100) so a WPF ProgressBar can bind to it.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Goals
(
    GoalID           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Goals PRIMARY KEY,
    UserID           INT               NOT NULL CONSTRAINT FK_Goals_Users REFERENCES dbo.Users(UserID),
    CategoryID       INT               NOT NULL CONSTRAINT FK_Goals_Categories REFERENCES dbo.Categories(CategoryID),
    Title            NVARCHAR(100)     NOT NULL,
    Description      NVARCHAR(500)     NULL,
    StartDate        DATE              NOT NULL,
    TargetDate       DATE              NULL,
    TargetValue      DECIMAL(10,2)     NOT NULL CONSTRAINT CK_Goals_Target CHECK (TargetValue > 0),
    CurrentValue     DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Goals_Current DEFAULT (0)
        CONSTRAINT CK_Goals_Current CHECK (CurrentValue >= 0),
    Unit             NVARCHAR(30)      NOT NULL,
    Status           NVARCHAR(20)      NOT NULL CONSTRAINT DF_Goals_Status DEFAULT (N'Not Started')
        CONSTRAINT CK_Goals_Status CHECK (Status IN (N'Not Started', N'In Progress', N'Completed', N'On Hold')),
    ProgressPercent  AS CAST(CASE WHEN CurrentValue >= TargetValue THEN 100
                                  ELSE CurrentValue * 100.0 / TargetValue END AS DECIMAL(5,2)),
    CONSTRAINT CK_Goals_Dates CHECK (TargetDate IS NULL OR TargetDate >= StartDate)
);
GO

CREATE INDEX IX_Goals_User ON dbo.Goals (UserID);
GO

/* ---------------------------------------------------------------------
   Tasks
   One table covers daily tasks, chores and fun activities; the
   Category's CategoryGroup says which is which.
   GoalID optionally links a task to the goal it moves forward.
   IsFlexible = 1 means the scheduler may move it; 0 means fixed time (e.g. meds).
   Priority: 1 = highest, 5 = lowest.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Tasks
(
    TaskID           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tasks PRIMARY KEY,
    UserID           INT               NOT NULL CONSTRAINT FK_Tasks_Users REFERENCES dbo.Users(UserID),
    CategoryID       INT               NOT NULL CONSTRAINT FK_Tasks_Categories REFERENCES dbo.Categories(CategoryID),
    GoalID           INT               NULL     CONSTRAINT FK_Tasks_Goals REFERENCES dbo.Goals(GoalID),
    Title            NVARCHAR(100)     NOT NULL,
    Description      NVARCHAR(500)     NULL,
    Frequency        NVARCHAR(10)      NOT NULL
        CONSTRAINT CK_Tasks_Frequency CHECK (Frequency IN
            (N'Once', N'Daily', N'Weekdays', N'Weekly', N'Biweekly', N'Monthly')),
    PreferredDay     NVARCHAR(9)       NULL
        CONSTRAINT CK_Tasks_Day CHECK (PreferredDay IS NULL OR PreferredDay IN
            (N'Monday', N'Tuesday', N'Wednesday', N'Thursday', N'Friday', N'Saturday', N'Sunday')),
    PreferredTime    TIME(0)           NULL,
    DurationMinutes  INT               NOT NULL CONSTRAINT CK_Tasks_Duration CHECK (DurationMinutes > 0),
    Priority         TINYINT           NOT NULL CONSTRAINT DF_Tasks_Priority DEFAULT (3)
        CONSTRAINT CK_Tasks_Priority CHECK (Priority BETWEEN 1 AND 5),
    IsFlexible       BIT               NOT NULL CONSTRAINT DF_Tasks_IsFlexible DEFAULT (1),
    IsActive         BIT               NOT NULL CONSTRAINT DF_Tasks_IsActive DEFAULT (1)
);
GO

CREATE INDEX IX_Tasks_User ON dbo.Tasks (UserID);
GO

/* ---------------------------------------------------------------------
   TaskCompletions
   Deleting a task removes its completion history with it.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.TaskCompletions
(
    CompletionID   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TaskCompletions PRIMARY KEY,
    TaskID         INT               NOT NULL
        CONSTRAINT FK_TaskCompletions_Tasks REFERENCES dbo.Tasks(TaskID) ON DELETE CASCADE,
    CompletedAt    DATETIME2(0)      NOT NULL CONSTRAINT DF_TaskCompletions_At DEFAULT (SYSDATETIME()),
    ActualMinutes  INT               NULL CONSTRAINT CK_TaskCompletions_Minutes CHECK (ActualMinutes IS NULL OR ActualMinutes > 0),
    Notes          NVARCHAR(300)     NULL
);
GO

CREATE INDEX IX_TaskCompletions_Task_At ON dbo.TaskCompletions (TaskID, CompletedAt);
GO

/* ---------------------------------------------------------------------
   AuditLog  (ADMIN ONLY - permissions are set in 03_Security.sql)
   --------------------------------------------------------------------- */
CREATE TABLE dbo.AuditLog
(
    AuditID        BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLog PRIMARY KEY,
    UserID         INT               NULL CONSTRAINT FK_AuditLog_Users REFERENCES dbo.Users(UserID),
    ActionType     NVARCHAR(20)      NOT NULL
        CONSTRAINT CK_AuditLog_Action CHECK (ActionType IN
            (N'Login', N'Logout', N'FailedLogin', N'Create', N'Update', N'Delete', N'RoleChange', N'PasswordReset')),
    TableAffected  NVARCHAR(50)      NULL,
    RecordID       INT               NULL,
    Details        NVARCHAR(500)     NULL,
    IPAddress      NVARCHAR(45)      NULL,
    LoggedAt       DATETIME2(0)      NOT NULL CONSTRAINT DF_AuditLog_LoggedAt DEFAULT (SYSDATETIME())
);
GO

CREATE INDEX IX_AuditLog_LoggedAt ON dbo.AuditLog (LoggedAt DESC);
GO

PRINT N'ScheduleOptimizerDB schema created.';
GO
