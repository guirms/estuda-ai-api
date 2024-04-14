CREATE DATABASE `plasson_farm`;

USE `plasson_farm`;

CREATE TABLE `__EFMigrationsHistory` (
          `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
          `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
          CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
      ) CHARACTER SET=utf8mb4;
	  
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `User` (
          `Id` int NOT NULL AUTO_INCREMENT,
          `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
          `Age` int NOT NULL,
          CONSTRAINT `PK_User` PRIMARY KEY (`Id`)
      ) CHARACTER SET=utf8mb4;
	  
INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
      VALUES ('20230529115753_Initial_Migration', '7.0.5');