DROP TABLE MemberRoles;
DROP TABLE Members;
DROP TABLE Messages;
DROP TABLE RoleOverwrites;
DROP TABLE Channels;
DROP TABLE Categories;
DROP TABLE Roles;
DROP TABLE Guilds;

DELETE FROM Members;
DELETE FROM MemberRoles;
DELETE FROM Messages;
DELETE FROM RoleOverwrites;
DELETE FROM Channels;
DELETE FROM Categories;
DELETE FROM Roles;
DELETE FROM Guilds;

SELECT * FROM Guilds;
SELECT * FROM Roles;
SELECT * FROM Categories;
SELECT * FROM Channels;
SELECT * FROM RoleOverwrites;
SELECT * FROM Messages;
SELECT * FROM MemberRoles;
SELECT * FROM Members;