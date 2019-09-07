IF OBJECT_ID (N'Game..Patch', N'U') IS NOT NULL
	DROP TABLE Game..Patch;

CREATE TABLE Game..Patch(
	Major INT NOT NULL,
	Minor INT NOT NULL,
	Version INT NOT NULL,
	Live BIT NOT NULL,
	Tournament BIT NOT NULL,
	CONSTRAINT PK_Patch PRIMARY KEY CLUSTERED (Major, Minor, Version)
);
