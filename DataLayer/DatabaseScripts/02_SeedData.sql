/* =====================================================================
   Schedule Optimizer - Sample Data
   File 2 of 3: fills every table with test data (92 rows total).
   Safe to re-run: existing rows are cleared first.

   Row counts
     Roles 2 | Users 5 | Categories 17 | Goals 9 | Appointments 11
     Tasks 20 | TaskCompletions 16 | AuditLog 12   => 92 rows

   Demo logins (plain-text passwords, FOR TESTING ONLY):
     admin     / Admin#2026     (Admin)
     jmartinez / Student#2026   (Standard)
     pkim      / Career#2026    (Standard)
     tbrooks   / Reader#2026    (Standard)
     rpatel    / Painter#2026   (Admin)

   Password hashes are SHA-256 of the NVARCHAR password, as uppercase hex.
   To match in C#:
       var bytes = SHA256.HashData(Encoding.Unicode.GetBytes(password));
       string hash = Convert.ToHexString(bytes);   // compare to Users.PasswordHash
   (For a real app, use a salted hash such as PBKDF2 or BCrypt.)
   ===================================================================== */

USE ScheduleOptimizerDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

/* Clear existing data (children first) */
DELETE FROM dbo.AuditLog;
DELETE FROM dbo.TaskCompletions;
DELETE FROM dbo.Tasks;
DELETE FROM dbo.Goals;
DELETE FROM dbo.Appointments;
DELETE FROM dbo.Categories;
DELETE FROM dbo.Users;
DELETE FROM dbo.Roles;

/* ---------------------------------------------------------------------
   Roles (2)
   --------------------------------------------------------------------- */
SET IDENTITY_INSERT dbo.Roles ON;
INSERT INTO dbo.Roles (RoleID, RoleName, Description) VALUES
(1, N'Admin',    N'Full access, including user management and the audit log'),
(2, N'Standard', N'Manages own appointments, goals, tasks and activities');
SET IDENTITY_INSERT dbo.Roles OFF;

/* ---------------------------------------------------------------------
   Users (5)
   --------------------------------------------------------------------- */
SET IDENTITY_INSERT dbo.Users ON;
INSERT INTO dbo.Users (UserID, RoleID, Username, Email, PasswordHash, FirstName, LastName, IsActive, CreatedAt, LastLoginAt) VALUES
(1, 1, N'admin',     N'admin@scheduleoptimizer.local',
    CONVERT(NVARCHAR(128), HASHBYTES('SHA2_256', N'Admin#2026'), 2),   N'System', N'Administrator', 1, '2026-08-15 09:00', '2026-09-24 08:00'),
(2, 2, N'jmartinez', N'jordan.martinez@example.com',
    CONVERT(NVARCHAR(128), HASHBYTES('SHA2_256', N'Student#2026'), 2), N'Jordan', N'Martinez',      1, '2026-08-20 14:12', '2026-09-24 07:55'),
(3, 2, N'pkim',      N'priya.kim@example.com',
    CONVERT(NVARCHAR(128), HASHBYTES('SHA2_256', N'Career#2026'), 2),  N'Priya',  N'Kim',           1, '2026-08-21 10:30', '2026-09-24 06:25'),
(4, 2, N'tbrooks',   N'taylor.brooks@example.com',
    CONVERT(NVARCHAR(128), HASHBYTES('SHA2_256', N'Reader#2026'), 2),  N'Taylor', N'Brooks',        1, '2026-08-25 19:45', '2026-09-24 06:40'),
(5, 1, N'rpatel',    N'riley.patel@example.com',
    CONVERT(NVARCHAR(128), HASHBYTES('SHA2_256', N'Painter#2026'), 2), N'Riley',  N'Patel',         1, '2026-09-01 11:05', '2026-09-23 16:20');
SET IDENTITY_INSERT dbo.Users OFF;

/* ---------------------------------------------------------------------
   Categories (17)
   --------------------------------------------------------------------- */
SET IDENTITY_INSERT dbo.Categories ON;
INSERT INTO dbo.Categories (CategoryID, CategoryName, CategoryGroup, Description, ColorHex) VALUES
-- Goal types
( 1, N'School',       N'Goal',        N'Coursework, grades and study goals',           '#4A90D9'),
( 2, N'Professional', N'Goal',        N'Career, certification and job goals',          '#2E7D5B'),
( 3, N'Hobby',        N'Goal',        N'Personal projects and creative goals',         '#8E6CC1'),
-- Daily tasks
( 4, N'Medication',   N'DailyTask',   N'Morning and evening medications',              '#D9534F'),
( 5, N'Cooking',      N'DailyTask',   N'Meal planning, prep and cooking',              '#F0A030'),
( 6, N'Exercise',     N'DailyTask',   N'Walking, gym, yoga and other workouts',        '#3BAFDA'),
-- Chores
( 7, N'Laundry',      N'Chore',       N'Washing, drying and folding clothes',          '#7A8B99'),
( 8, N'Cleaning',     N'Chore',       N'Cleaning the apartment',                       '#5D9C8A'),
( 9, N'Groceries',    N'Chore',       N'Grocery shopping and restocking',              '#A0785A'),
-- Fun activities
(10, N'Knitting',     N'FunActivity', N'Knitting projects',                            '#E57399'),
(11, N'Crocheting',   N'FunActivity', N'Crochet projects',                             '#C75B9B'),
(12, N'Sewing',       N'FunActivity', N'Sewing and quilting',                          '#9B59B6'),
(13, N'Painting',     N'FunActivity', N'Watercolor, acrylic and other painting',       '#F39C12'),
-- Appointment types
(14, N'Medical',      N'Appointment', N'Doctor, dentist and pharmacy visits',          '#C0392B'),
(15, N'Academic',     N'Appointment', N'Classes, exams, advising and office hours',    '#2980B9'),
(16, N'Work',         N'Appointment', N'Meetings, reviews and work events',            '#16A085'),
(17, N'Social',       N'Appointment', N'Friends, family and community events',         '#D35400');
SET IDENTITY_INSERT dbo.Categories OFF;

/* ---------------------------------------------------------------------
   Goals (9)
   --------------------------------------------------------------------- */
SET IDENTITY_INSERT dbo.Goals ON;
INSERT INTO dbo.Goals (GoalID, UserID, CategoryID, Title, Description, StartDate, TargetDate, TargetValue, CurrentValue, Unit, Status) VALUES
(1, 2, 1, N'Finish WPF scheduling app project', N'Complete and present the final class project',          '2026-08-24', '2026-12-04', 100,  35,   N'percent',   N'In Progress'),
(2, 2, 1, N'Study 120 hours this semester',     N'Log focused study time across all classes',             '2026-08-24', '2026-12-11', 120,  41.5, N'hours',     N'In Progress'),
(3, 2, 3, N'Knit a winter scarf',               N'Chunky rib-stitch scarf in charcoal wool',              '2026-09-01', '2026-11-30',  60,  18,   N'inches',    N'In Progress'),
(4, 3, 2, N'Earn CompTIA A+ certification',     N'Pass both Core 1 and Core 2 exams',                     '2026-07-01', '2026-12-31',   2,   0,   N'exams',     N'In Progress'),
(5, 3, 2, N'Complete 10 online courses',        N'Short courses on networking and help-desk skills',      '2026-06-01', '2026-12-31',  10,   4,   N'courses',   N'In Progress'),
(6, 4, 1, N'Read 12 books for Lit seminar',     N'Required and elective reading for the seminar',         '2026-08-24', '2026-12-11',  12,   5,   N'books',     N'In Progress'),
(7, 4, 3, N'Sew a quilt for my sister',         N'Twin-size patchwork quilt, 20 blocks',                  '2026-10-01', '2027-02-14',  20,   0,   N'blocks',    N'Not Started'),
(8, 5, 3, N'Finish watercolor series',          N'Six landscape paintings for the fall art fair',         '2026-06-15', '2026-09-30',   6,   6,   N'paintings', N'Completed'),
(9, 3, 3, N'Crochet 5 amigurumi animals',       N'Small stuffed animals as holiday gifts',                '2026-08-15', '2026-12-15',   5,   2,   N'animals',   N'In Progress');
SET IDENTITY_INSERT dbo.Goals OFF;

/* ---------------------------------------------------------------------
   Appointments (11)
   --------------------------------------------------------------------- */
SET IDENTITY_INSERT dbo.Appointments ON;
INSERT INTO dbo.Appointments (AppointmentID, UserID, CategoryID, Title, Location, StartDateTime, EndDateTime, Notes, ReminderMinutesBefore) VALUES
( 1, 2, 14, N'Annual physical',               N'Riverside Family Clinic',          '2026-09-29 09:30', '2026-09-29 10:30', N'Bring current medication list',     60),
( 2, 2, 15, N'Advisor meeting',               N'Academic Success Center, Rm 204',  '2026-10-30 14:00', '2026-10-30 14:30', N'Discuss spring registration',       30),
( 3, 2, 15, N'Programming midterm exam',      N'Science Hall 110',                 '2026-11-08 10:00', '2026-11-08 11:15', N'Covers chapters 1-6',             1440),
( 4, 3, 16, N'Quarterly review with manager', N'Office - Conference Room B',       '2026-11-02 13:00', '2026-11-02 14:00', N'Bring certification progress',      30),
( 5, 3, 14, N'Dental cleaning',               N'Bright Smile Dental',              '2026-11-06 08:00', '2026-11-06 09:00', NULL,                                 60),
( 6, 3, 16, N'CompTIA A+ Core 1 exam',        N'Campus Testing Center',            '2026-11-15 09:00', '2026-11-15 10:30', N'Bring two forms of ID',           1440),
( 7, 4, 17, N'Knitting circle',               N'Public library community room',    '2026-11-26 10:00', '2026-11-26 12:00', N'Bring spare needles to share',      60),
( 8, 4, 15, N'Lit seminar office hours',      N'Humanities Bldg, Rm 318',          '2026-11-28 15:00', '2026-11-28 15:30', N'Ask about essay topic',             30),
( 9, 5, 14, N'Pharmacy refill pickup',        N'Main Street Pharmacy',             '2026-11-25 17:00', '2026-11-25 17:15', NULL,                                 30),
(10, 5, 17, N'Art fair booth setup',          N'Downtown Arts Pavilion',           '2026-10-15 07:00', '2026-10-15 09:00', N'Load paintings the night before',  120),
(11, 4, 17, N'Dinner with Sam',               N'Home',                             '2026-10-20 18:30', '2026-10-20 20:30', N'Make lasagna',                      60);
SET IDENTITY_INSERT dbo.Appointments OFF;

/* ---------------------------------------------------------------------
   Tasks (20) - daily tasks, chores and fun activities
   --------------------------------------------------------------------- */
SET IDENTITY_INSERT dbo.Tasks ON;
INSERT INTO dbo.Tasks (TaskID, UserID, CategoryID, GoalID, Title, Description, Frequency, PreferredDay, PreferredTime, DurationMinutes, Priority, IsFlexible, IsActive) VALUES
-- Jordan (UserID 2)
( 1, 2,  4, NULL, N'Take morning meds',        N'Take with breakfast',                  N'Daily',    NULL,        '08:00',   5, 1, 0, 1),
( 2, 2,  4, NULL, N'Take evening meds',        N'Take before bed',                      N'Daily',    NULL,        '21:00',   5, 1, 0, 1),
( 3, 2,  5, NULL, N'Cook dinner',              NULL,                                    N'Daily',    NULL,        '18:00',  45, 2, 1, 1),
( 4, 2,  6, NULL, N'30-minute walk',           N'Loop around the park',                 N'Weekdays', NULL,        '07:00',  30, 2, 1, 1),
( 5, 2,  1,    2, N'Study session',            N'Focused study, phone in another room', N'Weekdays', NULL,        '19:00',  90, 1, 1, 1),
( 6, 2,  7, NULL, N'Laundry',                  N'Wash, dry and fold',                   N'Weekly',   N'Sunday',   '10:00',  90, 3, 1, 1),
( 7, 2, 10,    3, N'Knit scarf rows',          N'Relaxing wind-down activity',          N'Daily',    NULL,        '20:00',  30, 4, 1, 1),
-- Priya (UserID 3)
( 8, 3,  4, NULL, N'Take morning meds',        NULL,                                    N'Daily',    NULL,        '07:30',   5, 1, 0, 1),
( 9, 3,  5, NULL, N'Meal prep lunches',        N'Prep five lunches for the work week',  N'Weekly',   N'Sunday',   '13:00', 120, 2, 1, 1),
(10, 3,  6, NULL, N'Gym - strength training',  NULL,                                    N'Weekly',   N'Wednesday','17:30',  60, 3, 1, 1),
(11, 3,  9, NULL, N'Buy groceries',            N'Use the shared shopping list',         N'Weekly',   N'Saturday', '11:00',  60, 2, 1, 1),
(12, 3, 11,    9, N'Crochet amigurumi',        NULL,                                    N'Weekly',   N'Friday',   '20:00',  60, 4, 1, 1),
(13, 3,  2,    4, N'A+ practice questions',    N'50 questions per session',             N'Daily',    NULL,        '06:30',  45, 1, 1, 1),
-- Taylor (UserID 4)
(14, 4,  8, NULL, N'Clean apartment',          N'Vacuum, bathroom, kitchen surfaces',   N'Weekly',   N'Saturday', '09:00', 120, 3, 1, 1),
(15, 4, 12,    7, N'Sew quilt blocks',         NULL,                                    N'Weekly',   N'Sunday',   '14:00',  90, 4, 1, 1),
(16, 4,  6, NULL, N'Morning yoga',             NULL,                                    N'Daily',    NULL,        '06:45',  20, 3, 1, 1),
(17, 4,  1,    6, N'Seminar reading',          NULL,                                    N'Daily',    NULL,        '21:00',  45, 2, 1, 1),
-- Riley (UserID 5)
(18, 5, 13, NULL, N'Painting session',         N'Open studio time',                     N'Weekly',   N'Saturday', '15:00', 120, 4, 1, 1),
(19, 5,  7, NULL, N'Laundry',                  NULL,                                    N'Biweekly', N'Thursday', '18:00',  90, 3, 1, 1),
(20, 5,  4, NULL, N'Take evening meds',        NULL,                                    N'Daily',    NULL,        '22:00',   5, 1, 0, 1);
SET IDENTITY_INSERT dbo.Tasks OFF;

/* ---------------------------------------------------------------------
   TaskCompletions (16)
   --------------------------------------------------------------------- */
SET IDENTITY_INSERT dbo.TaskCompletions ON;
INSERT INTO dbo.TaskCompletions (CompletionID, TaskID, CompletedAt, ActualMinutes, Notes) VALUES
( 1,  1, '2026-09-22 08:05',   5, NULL),
( 2,  2, '2026-09-22 21:10',   5, NULL),
( 3,  1, '2026-09-23 08:02',   5, NULL),
( 4,  2, '2026-09-23 21:30',   5, N'Late - took after the reminder went off'),
( 5,  1, '2026-09-24 07:58',   5, NULL),
( 6,  5, '2026-09-22 19:00',  95, NULL),
( 7,  5, '2026-09-23 19:15',  80, N'Finished lab 4'),
( 8,  7, '2026-09-23 20:10',  35, N'Added 2 inches'),
( 9, 13, '2026-09-23 06:35',  50, N'Scored 82%'),
(10, 13, '2026-09-24 06:30',  45, N'Scored 88%'),
(11, 12, '2026-09-18 20:00',  70, N'Finished the frog body'),
(12, 14, '2026-09-19 09:00', 135, NULL),
(13, 17, '2026-09-23 21:05',  40, NULL),
(14, 16, '2026-09-24 06:50',  20, NULL),
(15, 18, '2026-09-19 15:00', 150, N'Final painting in the series'),
(16,  6, '2026-09-20 10:15',  85, NULL);
SET IDENTITY_INSERT dbo.TaskCompletions OFF;

/* ---------------------------------------------------------------------
   AuditLog (12) - ADMIN ONLY
   --------------------------------------------------------------------- */
SET IDENTITY_INSERT dbo.AuditLog ON;
INSERT INTO dbo.AuditLog (AuditID, UserID, ActionType, TableAffected, RecordID, Details, IPAddress, LoggedAt) VALUES
( 1, 1, N'Login',         NULL,            NULL, N'Administrator signed in',                  N'10.0.0.5',  '2026-09-01 10:58'),
( 2, 1, N'Create',        N'Users',        5,    N'Created account rpatel',                   N'10.0.0.5',  '2026-09-01 11:05'),
( 3, 1, N'RoleChange',    N'Users',        5,    N'Role changed Standard -> Admin',           N'10.0.0.5',  '2026-09-01 11:07'),
( 4, 2, N'Login',         NULL,            NULL, N'User signed in',                           N'10.0.0.21', '2026-09-15 18:40'),
( 5, 2, N'Create',        N'Appointments', 3,    N'Added "Programming midterm exam"',         N'10.0.0.21', '2026-09-15 18:44'),
( 6, 3, N'FailedLogin',   NULL,            NULL, N'Incorrect password (attempt 1)',           N'10.0.0.34', '2026-09-18 06:20'),
( 7, 3, N'Login',         NULL,            NULL, N'User signed in',                           N'10.0.0.34', '2026-09-18 06:21'),
( 8, 3, N'Update',        N'Goals',        5,    N'CurrentValue 3 -> 4',                      N'10.0.0.34', '2026-09-18 06:30'),
( 9, 5, N'Update',        N'Goals',        8,    N'Status In Progress -> Completed',          N'10.0.0.47', '2026-09-19 17:35'),
(10, 1, N'PasswordReset', N'Users',        4,    N'Password reset at user''s request',        N'10.0.0.5',  '2026-09-21 09:12'),
(11, 4, N'Delete',        N'Tasks',        NULL, N'Deleted duplicate task "Clean bathroom"',  N'10.0.0.52', '2026-09-21 20:03'),
(12, 2, N'Logout',        NULL,            NULL, N'User signed out',                          N'10.0.0.21', '2026-09-23 21:45');
SET IDENTITY_INSERT dbo.AuditLog OFF;

COMMIT TRANSACTION;
GO

/* Verify row counts */
SELECT 'Roles' AS TableName, COUNT(*) AS RowsInserted FROM dbo.Roles
UNION ALL SELECT 'Users',           COUNT(*) FROM dbo.Users
UNION ALL SELECT 'Categories',      COUNT(*) FROM dbo.Categories
UNION ALL SELECT 'Goals',           COUNT(*) FROM dbo.Goals
UNION ALL SELECT 'Appointments',    COUNT(*) FROM dbo.Appointments
UNION ALL SELECT 'Tasks',           COUNT(*) FROM dbo.Tasks
UNION ALL SELECT 'TaskCompletions', COUNT(*) FROM dbo.TaskCompletions
UNION ALL SELECT 'AuditLog',        COUNT(*) FROM dbo.AuditLog;
GO
