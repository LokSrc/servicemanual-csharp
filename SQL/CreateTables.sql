CREATE TABLE "FactoryDevice" (
	"Id"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"Name"	TEXT,
	"Year"	INTEGER,
	"Type"	TEXT
);

CREATE TABLE "ServiceTask" (
	"TaskId"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"TargetId"	INTEGER NOT NULL,
	"Criticality"	INTEGER,
	"DateIssued"	TEXT,
	"Description"	TEXT,
	"Closed"	INTEGER,
	FOREIGN KEY("TargetId") REFERENCES "FactoryDevice"("Id")
);