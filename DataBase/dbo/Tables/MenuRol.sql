CREATE TABLE [dbo].[MenuRol] (
    [IdMenuRol]     INT          IDENTITY (1, 1) NOT NULL,
    [IdHRol]        INT          DEFAULT (NULL) NULL,
    [IdHMenu]       INT          DEFAULT (NULL) NULL,
    [Estado]        NVARCHAR (1) DEFAULT ('A') NOT NULL,
    [FechaCreacion] DATETIME     DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_MR_IdMenuRol] PRIMARY KEY CLUSTERED ([IdMenuRol] ASC),
    CONSTRAINT [CK_MR_Estado] CHECK ([Estado]='X' OR [Estado]='A'),
    CONSTRAINT [FK_MR_IdHMenu] FOREIGN KEY ([IdHMenu]) REFERENCES [dbo].[Homologacion] ([IdHomologacion]),
    CONSTRAINT [FK_MR_IdHRol] FOREIGN KEY ([IdHRol]) REFERENCES [dbo].[Homologacion] ([IdHomologacion])
);

