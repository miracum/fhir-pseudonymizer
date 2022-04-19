-- version from 2020-09-11 RS

USE gras;

-- config
SET @domainName  = 'ths';
SET @projectName = 'gpas';
SET @displayName = 'gPAS';

-- domain
-- createDomain(<domainName>, <description>)
CALL createDomain(@domainName, 'parent domain');

-- project
-- createProject(<projectName>, <description>)
CALL createProject(@projectName, @displayName);

-- group_
-- createGroup(<projectName>, <groupName>, <description>)
CALL createGroup(@projectName, CONCAT(@displayName, '-users'), 'this group is for users with basic right');
CALL createGroup(@projectName, CONCAT(@displayName, '-admins'), 'this group is for users with extended right');

-- role
-- createRole(<projectName>, <roleName>, <description>)
CALL createRole(@projectName, CONCAT('role.',@projectName,'.user'), CONCAT(@displayName, ' userspace'));
CALL createRole(@projectName, CONCAT('role.',@projectName,'.admin'), CONCAT(@displayName, ' adminspace' ));

-- group_role_mapping
-- createGroupRoleMapping(<projectName>, <groupName>, <roleName>)
CALL createGroupRoleMapping(@projectName, CONCAT(@displayName, '-users'), CONCAT('role.',@projectName,'.user'));
CALL createGroupRoleMapping(@projectName, CONCAT(@displayName, '-admins'), CONCAT('role.',@projectName,'.user'));
CALL createGroupRoleMapping(@projectName, CONCAT(@displayName, '-admins'), CONCAT('role.',@projectName,'.admin'));

-- default user 
-- createUser(<userName>, <password>, <description>)
call createUser('admin', 'ttp-tools', 'user for admin privileges');
call createUser('user', 'ttp-tools', 'user for standard privileges');

-- grant privileges for project
-- grantAdminRights(<domainName>, <projectName>, <userName>)
call grantAdminRights(@domainName, @projectName, 'admin');

-- grantStandardRights(<domainName>, <projectName>, <userName>)
call grantStandardRights(@domainName, @projectName,'user');
