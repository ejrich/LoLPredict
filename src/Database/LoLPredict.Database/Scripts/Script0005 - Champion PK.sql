declare @champs table (
    Id INT PRIMARY KEY,
    Name VARCHAR(100),
    Image VARCHAR(100),
    Major INT,
    Minor INT,
    Version INT
);

IF OBJECT_ID (N'Game..Champion', N'U') IS NOT NULL
BEGIN
    INSERT INTO @champs
    SELECT Id, Name, Image, LEFT(Patch, 1), SUBSTRING(Patch, 3, LEN(Patch) - 4), 1
    FROM Game..Champion
    WHERE Patch = '9.8.1';

    INSERT INTO @champs
    SELECT TOP 1 Id, Name, Image, LEFT(Patch, 1), SUBSTRING(Patch, 3, LEN(Patch) - 4), 1
    FROM Game..Champion
    WHERE Id NOT IN (SELECT Id FROM @champs);

    DROP TABLE Game..Champion;
END

CREATE TABLE Game..Champion(
    Id INT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Image VARCHAR(100) NOT NULL,
    Major INT NOT NULL,
    Minor INT NOT NULL,
    Version INT NOT NULL
);

ALTER TABLE Game..Champion ADD CONSTRAINT Patch_FK
    FOREIGN KEY (Major, Minor, Version) REFERENCES Game..Patch (Major, Minor, Version)
GO

INSERT INTO Game..Champion
SELECT * FROM @champs
