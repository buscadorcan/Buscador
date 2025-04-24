CREATE TABLE [dbo].[LogScript] (
    [IdLogScript]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [StateLog]     NVARCHAR (10)  CONSTRAINT [DF_LogScript_StateLog] DEFAULT ('ERROR') NOT NULL,
    [TextLog]      NVARCHAR (900) CONSTRAINT [DF_LogScript_TextLog] DEFAULT ('') NOT NULL,
    [TimeRun]      NVARCHAR (10)  CONSTRAINT [DF_LogScript_TimeRun] DEFAULT ('00:00:00') NOT NULL,
    [NameScript]   NVARCHAR (100) CONSTRAINT [DF_LogScript_NameScript] DEFAULT ('') NOT NULL,
    [TimeLog]      DATETIME       CONSTRAINT [DC_LogScript_DateUpdate] DEFAULT (getdate()) NOT NULL,
    [HostName]     NVARCHAR (100) CONSTRAINT [DF_LogScript_HostName] DEFAULT (host_name()) NOT NULL,
    [LoggedInUser] NVARCHAR (100) CONSTRAINT [DF_LogScript_LoggedInUser] DEFAULT (suser_name()) NOT NULL,
    [IdLogTrace]   BIGINT         CONSTRAINT [DF_LogScript_IdLogTrace] DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([IdLogScript] ASC),
    CONSTRAINT [KC_LogScript_StateLog] CHECK ([StateLog]='@script' OR [StateLog]='ERROR' OR [StateLog]='OK')
);

