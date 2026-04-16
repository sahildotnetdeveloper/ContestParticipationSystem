CREATE TABLE IF NOT EXISTS "Users" (
    "Id" uuid PRIMARY KEY,
    "FullName" varchar(100) NOT NULL,
    "Email" varchar(256) NOT NULL UNIQUE,
    "PasswordHash" varchar(1000) NOT NULL,
    "Role" integer NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL
);

CREATE TABLE IF NOT EXISTS "Contests" (
    "Id" uuid PRIMARY KEY,
    "Name" varchar(150) NOT NULL,
    "Description" varchar(2000) NOT NULL,
    "StartTimeUtc" timestamp with time zone NOT NULL,
    "EndTimeUtc" timestamp with time zone NOT NULL,
    "ContestType" integer NOT NULL,
    "Prize" varchar(250) NOT NULL,
    "CreatedByUserId" uuid NOT NULL REFERENCES "Users"("Id") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "IX_Contests_StartTimeUtc" ON "Contests" ("StartTimeUtc");
CREATE INDEX IF NOT EXISTS "IX_Contests_EndTimeUtc" ON "Contests" ("EndTimeUtc");

CREATE TABLE IF NOT EXISTS "Questions" (
    "Id" uuid PRIMARY KEY,
    "ContestId" uuid NOT NULL REFERENCES "Contests"("Id") ON DELETE CASCADE,
    "Text" varchar(1000) NOT NULL,
    "QuestionType" integer NOT NULL,
    "Order" integer NOT NULL,
    CONSTRAINT "UX_Questions_ContestId_Order" UNIQUE ("ContestId", "Order")
);

CREATE TABLE IF NOT EXISTS "QuestionOptions" (
    "Id" uuid PRIMARY KEY,
    "QuestionId" uuid NOT NULL REFERENCES "Questions"("Id") ON DELETE CASCADE,
    "Text" varchar(500) NOT NULL,
    "IsCorrect" boolean NOT NULL,
    "SortOrder" integer NOT NULL,
    CONSTRAINT "UX_QuestionOptions_QuestionId_SortOrder" UNIQUE ("QuestionId", "SortOrder")
);

CREATE TABLE IF NOT EXISTS "Participations" (
    "Id" uuid PRIMARY KEY,
    "UserId" uuid NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "ContestId" uuid NOT NULL REFERENCES "Contests"("Id") ON DELETE CASCADE,
    "StartedAtUtc" timestamp with time zone NOT NULL,
    "SubmittedAtUtc" timestamp with time zone NULL,
    "Score" integer NOT NULL,
    "Status" integer NOT NULL,
    CONSTRAINT "UX_Participations_UserId_ContestId" UNIQUE ("UserId", "ContestId")
);

CREATE INDEX IF NOT EXISTS "IX_Participations_UserId" ON "Participations" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_Participations_ContestId" ON "Participations" ("ContestId");

CREATE TABLE IF NOT EXISTS "ParticipationAnswers" (
    "Id" uuid PRIMARY KEY,
    "ParticipationId" uuid NOT NULL REFERENCES "Participations"("Id") ON DELETE CASCADE,
    "QuestionId" uuid NOT NULL REFERENCES "Questions"("Id") ON DELETE RESTRICT,
    "AnswerOptionIdsJson" jsonb NOT NULL,
    "IsCorrect" boolean NOT NULL,
    "AwardedPoints" integer NOT NULL,
    "SubmittedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "UX_ParticipationAnswers_ParticipationId_QuestionId" UNIQUE ("ParticipationId", "QuestionId")
);
