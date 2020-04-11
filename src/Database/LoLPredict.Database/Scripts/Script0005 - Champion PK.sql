ALTER TABLE Game..Champion
ADD Major INT,
Minor INT,
Version INT;

GO

declare @champions table (
    Id INT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Image VARCHAR(100) NOT NULL,
    Patch VARCHAR(10) NOT NULL,
    Major INT NOT NULL,
    Minor INT NOT NULL,
    Version INT NOT NULL
);

INSERT INTO @champions
SELECT Id, Max(Name), Max(Image), '', Min(Major), Min(Minor), 1
FROM (SELECT Id, Name, Image, Major = CONVERT(int, LEFT(Patch, 1)), Minor = CONVERT(int, SUBSTRING(Patch, 3, LEN(Patch) - 4))
FROM Game..Champion) champions
GROUP BY Id;

DELETE FROM Game..Champion;

INSERT INTO Game..Champion
SELECT * FROM @champions;

ALTER TABLE Game..Champion
DROP COLUMN Patch;

ALTER TABLE Game..Champion
ADD PRIMARY KEY (Id);

ALTER TABLE Game..Champion ADD CONSTRAINT Patch_FK
    FOREIGN KEY (Major, Minor, Version) REFERENCES Game..Patch (Major, Minor, Version);
GO
