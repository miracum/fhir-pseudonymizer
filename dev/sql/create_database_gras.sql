-- version from 2020-09-11 RS

DROP SCHEMA IF EXISTS `gras` ;
/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

CREATE DATABASE gras DEFAULT CHARACTER SET utf8;

USE gras;

-- Exportiere Struktur von Tabelle gras.domain
CREATE TABLE IF NOT EXISTS `domain` (
  `name` varchar(255) NOT NULL DEFAULT '',
  `description` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von View gras.domainuser_password
-- Erstelle temporäre Tabelle um View Abhängigkeiten zuvorzukommen
CREATE TABLE `domainuser_password` (
	`domainuser` VARCHAR(511) NOT NULL COLLATE 'latin1_swedish_ci',
	`password` VARCHAR(255) NULL COLLATE 'latin1_swedish_ci'
) ENGINE=MyISAM;

-- Exportiere Struktur von View gras.domainuser_role
-- Erstelle temporäre Tabelle um View Abhängigkeiten zuvorzukommen
CREATE TABLE `domainuser_role` (
	`domainuser` VARCHAR(511) NOT NULL COLLATE 'latin1_swedish_ci',
	`role` VARCHAR(255) NOT NULL COLLATE 'latin1_swedish_ci'
) ENGINE=MyISAM;

-- Exportiere Struktur von Tabelle gras.group_
CREATE TABLE IF NOT EXISTS `group_` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL DEFAULT '',
  `project_name` varchar(255) NOT NULL DEFAULT '',
  `description` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `UNIQUE_NAME_PROJECT` (`name`,`project_name`),
  KEY `FK_PROJECT_GROUP` (`project_name`),
  CONSTRAINT `FK_PROJECT_GROUP` FOREIGN KEY (`project_name`) REFERENCES `project` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.group_role_mapping
CREATE TABLE IF NOT EXISTS `group_role_mapping` (
  `group_id` int(11) NOT NULL,
  `role_id` int(11) NOT NULL,
  PRIMARY KEY (`group_id`,`role_id`),
  KEY `FK_GROUP` (`group_id`),
  KEY `FK_ROLE` (`role_id`),
  CONSTRAINT `FK_GROUP` FOREIGN KEY (`group_id`) REFERENCES `group_` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ROLE` FOREIGN KEY (`role_id`) REFERENCES `role` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.hist_domain
CREATE TABLE IF NOT EXISTS `hist_domain` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `timestamp` datetime NOT NULL,
  `action` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.hist_group_
CREATE TABLE IF NOT EXISTS `hist_group_` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `group_id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `project_name` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `timestamp` datetime NOT NULL,
  `action` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.hist_group_role_mapping
CREATE TABLE IF NOT EXISTS `hist_group_role_mapping` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `group_id` varchar(255) NOT NULL,
  `role_id` varchar(255) NOT NULL,
  `timestamp` datetime NOT NULL,
  `action` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.hist_permission
CREATE TABLE IF NOT EXISTS `hist_permission` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `group_id` int(11) NOT NULL,
  `user_name` varchar(255) NOT NULL,
  `domain_name` varchar(255) NOT NULL,
  `timestamp` datetime NOT NULL,
  `action` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.hist_project
CREATE TABLE IF NOT EXISTS `hist_project` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `timestamp` datetime NOT NULL,
  `action` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.hist_role
CREATE TABLE IF NOT EXISTS `hist_role` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `role_id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `project_name` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `timestamp` datetime NOT NULL,
  `action` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.hist_user
CREATE TABLE IF NOT EXISTS `hist_user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL DEFAULT '',
  `password` varchar(255) DEFAULT NULL,
  `active` tinyint(1) DEFAULT '1',
  `description` varchar(255) NOT NULL DEFAULT '',
  `email` varchar(255) NOT NULL DEFAULT '',
  `timestamp` datetime NOT NULL,
  `action` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.permission
CREATE TABLE IF NOT EXISTS `permission` (
  `group_id` int(11) NOT NULL,
  `user_name` varchar(255) NOT NULL DEFAULT '',
  `domain_name` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`group_id`,`user_name`,`domain_name`),
  KEY `FK_PERMISSION_DOMAIN` (`domain_name`),
  KEY `FK_PERMISSION_USER` (`user_name`),
  KEY `FK_PERMISSION_GROUP` (`group_id`),
  CONSTRAINT `FK_PERMISSION_DOMAIN` FOREIGN KEY (`domain_name`) REFERENCES `domain` (`name`),
  CONSTRAINT `FK_PERMISSION_GROUP` FOREIGN KEY (`group_id`) REFERENCES `group_` (`id`),
  CONSTRAINT `FK_PERMISSION_USER` FOREIGN KEY (`user_name`) REFERENCES `user` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.project
CREATE TABLE IF NOT EXISTS `project` (
  `name` varchar(255) NOT NULL DEFAULT '',
  `description` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.role
CREATE TABLE IF NOT EXISTS `role` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL DEFAULT '',
  `project_name` varchar(255) NOT NULL DEFAULT '',
  `description` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `UNIQUE_NAME_PROJECT` (`name`),
  KEY `FK_PROJECT_ROLE` (`project_name`),
  CONSTRAINT `FK_PROJECT_ROLE` FOREIGN KEY (`project_name`) REFERENCES `project` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;

-- Exportiere Struktur von Tabelle gras.user
CREATE TABLE IF NOT EXISTS `user` (
  `name` varchar(255) NOT NULL DEFAULT '',
  `password` varchar(255) DEFAULT NULL,
  `active` tinyint(1) DEFAULT '1',
  `description` varchar(255) NOT NULL DEFAULT '',
  `email` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportiere Struktur von View gras.validate
-- Erstelle temporäre Tabelle um View Abhängigkeiten zuvorzukommen
CREATE TABLE `validate` (
	`domainuser` VARCHAR(511) NOT NULL COLLATE 'latin1_swedish_ci',
	`password` VARCHAR(255) NULL COLLATE 'latin1_swedish_ci',
	`role` VARCHAR(255) NOT NULL COLLATE 'latin1_swedish_ci'
) ENGINE=MyISAM;

-- Exportiere Struktur von Trigger gras.deleteDomain
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `deleteDomain` BEFORE DELETE ON `domain` FOR EACH ROW BEGIN
    INSERT INTO hist_domain(name,description,timestamp,action) 
        VALUES(OLD.name,OLD.description, now(),'delete');
    DELETE FROM gras.permission WHERE domain_name = OLD.name;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.deleteGroup
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `deleteGroup` BEFORE DELETE ON `group_` FOR EACH ROW BEGIN
    INSERT INTO hist_group_(group_id,name,project_name,description,timestamp,action)
        VALUES(OLD.id,OLD.name,OLD.project_name,OLD.description, now(),
			'delete');
    DELETE FROM gras.group_role_mapping WHERE group_id = OLD.id;
    DELETE FROM gras.permission WHERE group_id = OLD.id;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.deleteGroupRoleMapping
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `deleteGroupRoleMapping` BEFORE DELETE ON `group_role_mapping` FOR EACH ROW INSERT INTO hist_group_role_mapping(group_id,role_id,timestamp,action)
        VALUES(OLD.group_id, OLD.role_id, now() ,'delete' )//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.deletePermission
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `deletePermission` BEFORE DELETE ON `permission` FOR EACH ROW INSERT INTO hist_permission(group_id,user_name,domain_name,timestamp,action)
        VALUES(OLD.group_id, OLD.user_name, OLD.domain_name, now(), 'delete' )//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.deleteProject
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `deleteProject` BEFORE DELETE ON `project` FOR EACH ROW BEGIN
    INSERT INTO hist_project(name,description,timestamp,action) 
        VALUES(OLD.name,OLD.description, now(),'delete');
    DELETE FROM gras.group_ WHERE project_name = OLD.name;
    DELETE FROM gras.role WHERE project_name = OLD.name;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.deleteRole
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `deleteRole` BEFORE DELETE ON `role` FOR EACH ROW BEGIN
    INSERT INTO hist_role(role_id,name,project_name,description,timestamp,action)
        VALUES(OLD.id,OLD.name, OLD.project_name, OLD.description, now(),
			'delete');
    DELETE FROM gras.group_role_mapping WHERE role_id = OLD.id;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.deleteUser
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `deleteUser` BEFORE DELETE ON `user` FOR EACH ROW BEGIN
    INSERT INTO hist_user(name,password,active,description,email,timestamp,action)
        VALUES(OLD.name, OLD.password, OLD.active,OLD.description,OLD.email, now(), 'delete' );
    DELETE FROM gras.permission WHERE user_name = OLD.name;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.insertGroupRoleMapping
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `insertGroupRoleMapping` BEFORE INSERT ON `group_role_mapping` FOR EACH ROW BEGIN
    DECLARE msg VARCHAR(255);
    IF NOT EXISTS (SELECT * FROM (SELECT r.id AS role_id , g.id AS group_id FROM role r, group_ g WHERE r.id = NEW.role_id AND g.id = NEW.group_id AND g.project_name = r.project_name) AS joined) THEN
        set msg = "Role and Group have to belong to the same Project";
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = msg;
    END IF;        
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.updateDomain
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `updateDomain` BEFORE UPDATE ON `domain` FOR EACH ROW INSERT INTO hist_domain(name,description,timestamp,action) 
		VALUES(OLD.name,OLD.description, now(),'update')//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.updateGroup
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `updateGroup` BEFORE UPDATE ON `group_` FOR EACH ROW INSERT INTO hist_group_(group_id,name,project_name,description,timestamp,action)
        VALUES(OLD.id,OLD.name,OLD.project_name,OLD.description, now(),
			'update')//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.updateGroupRoleMapping
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `updateGroupRoleMapping` BEFORE UPDATE ON `group_role_mapping` FOR EACH ROW INSERT INTO hist_group_role_mapping(group_id,role_id,timestamp,action)
        VALUES(OLD.group_id, OLD.role_id, now() ,'update' )//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.updatePermission
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `updatePermission` BEFORE UPDATE ON `permission` FOR EACH ROW INSERT INTO hist_permission(group_id,user_name,domain_name,timestamp,action)
        VALUES(OLD.group_id, OLD.user_name, OLD.domain_name, now(), 'update' )//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.updateProject
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `updateProject` BEFORE UPDATE ON `project` FOR EACH ROW INSERT INTO hist_project(name,description,timestamp,action) 
        VALUES(OLD.name,OLD.description, now(),'update')//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.updateRole
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `updateRole` BEFORE UPDATE ON `role` FOR EACH ROW INSERT INTO hist_role(role_id,name,project_name,description,timestamp,action)
        VALUES(OLD.id,OLD.name, OLD.project_name,OLD.description, now(),
			'update')//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger gras.updateUser
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE TRIGGER `updateUser` BEFORE UPDATE ON `user` FOR EACH ROW INSERT INTO hist_user(name,password,active,description,email,timestamp,action)
        VALUES(OLD.name, OLD.password, OLD.active,OLD.description,OLD.email, now(), 'update' )//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von View gras.domainuser_password
-- Entferne temporäre Tabelle und erstelle die eigentliche View
DROP TABLE IF EXISTS `domainuser_password`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `domainuser_password` AS select distinct concat_ws('@',`p`.`user_name`,`p`.`domain_name`) AS `domainuser`,`u`.`password` AS `password` from (`permission` `p` join `user` `u`) where ((`u`.`name` = `p`.`user_name`) and (`u`.`active` = 1));

-- Exportiere Struktur von View gras.domainuser_role
-- Entferne temporäre Tabelle und erstelle die eigentliche View
DROP TABLE IF EXISTS `domainuser_role`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `domainuser_role` AS select 

distinct concat_ws('@',`p`.`user_name`,`p`.`domain_name`) AS `domainuser`,
`r`.`name` AS `role` 
from (((`permission` `p` join `group_role_mapping` `m`) join `role` `r`) join `user` `u`)

where (
(`p`.`group_id` = `m`.`group_id`) and 
(`m`.`role_id` = `r`.`id`) and 
(`u`.`name`=`p`.`user_name`) and
(`u`.`active`= 1)
);

-- Exportiere Struktur von View gras.validate
-- Entferne temporäre Tabelle und erstelle die eigentliche View
DROP TABLE IF EXISTS `validate`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `validate` AS select distinct `a`.`domainuser` AS `domainuser`,`a`.`password` AS `password`,`b`.`role` AS `role` from (`domainuser_password` `a` join `domainuser_role` `b`);

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

-- *** comfort procedures ***

-- create domain
DELIMITER $$
DROP PROCEDURE IF EXISTS `createDomain`$$
CREATE PROCEDURE `createDomain`(
	IN domainName VARCHAR(255),
	IN description VARCHAR(255)
)
BEGIN
	INSERT IGNORE INTO `gras`.`domain` SET `name`=domainName, `description`=description;
END$$


-- create project
DROP PROCEDURE IF EXISTS `createProject`$$
CREATE PROCEDURE `createProject`(
	IN projectName VARCHAR(255),
	IN description VARCHAR(255)
)
BEGIN
	INSERT IGNORE INTO `gras`.`project` SET `name`=projectName, `description`=description;
END$$


-- disable given user
DROP PROCEDURE IF EXISTS `disableUser`$$
CREATE PROCEDURE `disableUser`(
	IN userName VARCHAR(255)
)
BEGIN
	UPDATE `gras`.`user` SET `active`=0 WHERE `name`=userName;
END$$


-- enable given userName
DROP procedure IF EXISTS `enableUser`$$
CREATE PROCEDURE `enableUser`(
	IN userName VARCHAR(255)
)
BEGIN
	UPDATE `gras`.`user` SET `active`=1 WHERE `name`=userName;
END$$


-- change user-password
DROP procedure IF EXISTS `changePassword`$$
CREATE PROCEDURE `changePassword`(
	IN userName VARCHAR(255),
    IN newPassword VARCHAR(255)
)
BEGIN
	UPDATE `gras`.`user` SET `password`=SHA2(newPassword, 256) WHERE `name`=userName;
END$$


-- user and privileges
DROP procedure IF EXISTS `createUser`$$
CREATE PROCEDURE `createUser`(	
	IN userName VARCHAR(255),
    IN password VARCHAR(255),
    IN description VARCHAR(255)
)
BEGIN
	INSERT IGNORE INTO `user` SET `name`=userName, `password`=SHA2(password, 256), `active`=1, `description`=description;    
END$$


DROP procedure IF EXISTS `createGroup`$$
CREATE PROCEDURE `createGroup`(
	IN projectName VARCHAR(255),
	IN groupName VARCHAR(255),
    IN description VARCHAR(255)
)
BEGIN
    INSERT INTO `group_` SET `name`=groupName, `project_name`=projectName, `description`=description;
END$$


DROP procedure IF EXISTS `createRole`$$
CREATE PROCEDURE `createRole`(
	IN projectName VARCHAR(255),
	IN roleName VARCHAR(255),
	IN description VARCHAR(255)   
)
BEGIN
    INSERT INTO `role` SET `name`=roleName, `project_name`=projectName, `description`=description;
END$$


DROP procedure IF EXISTS `createGroupRoleMapping`$$
CREATE PROCEDURE `createGroupRoleMapping`(
	IN projectName VARCHAR(255),
	IN groupName VARCHAR(255),
	IN roleName VARCHAR(255)   
)
BEGIN
	INSERT INTO `group_role_mapping` (`group_id`, `role_id`) VALUES (
		(SELECT id from `group_` WHERE `name`=groupName AND `project_name`=projectName), 
		(SELECT id FROM `role` WHERE `name`=roleName)
	);
END$$


DROP procedure IF EXISTS `grantAdminRights`$$
CREATE PROCEDURE `grantAdminRights`(
	IN domainName VARCHAR(255),
	IN projectName VARCHAR(255),
	IN userName VARCHAR(255)   
)
BEGIN
    INSERT INTO `permission` (`group_id`, `user_name`, `domain_name`)
		SELECT DISTINCT `id`, userName, domainName FROM `group_` WHERE `project_name`=projectName;
END$$


DROP procedure IF EXISTS `grantStandardRights`$$
CREATE PROCEDURE `grantStandardRights`(
	IN domainName VARCHAR(255),
	IN projectName VARCHAR(255),
	IN userName VARCHAR(255)
)
BEGIN
    INSERT INTO `permission` (`group_id`, `user_name`, `domain_name`)
		SELECT DISTINCT `id`, userName, domainName FROM `group_` WHERE `project_name`=projectName AND `name` NOT RLIKE '[aA]dmin';
END$$

DELIMITER ;


-- for remote and wildfly access
create user 'gras_user'@'%' identified by 'gras_password';
GRANT ALL PRIVILEGES ON gras.* TO 'gras_user'@'%';

-- for docker exec commands
create user 'gras_user'@'localhost' identified by 'gras_password';
GRANT ALL PRIVILEGES ON gras.* TO 'gras_user'@'localhost';


-- for remote and wildfly access
GRANT ALL PRIVILEGES ON gras.* TO 'root'@'%';

-- for docker exec commands
GRANT ALL PRIVILEGES ON gras.* TO 'root'@'localhost';
