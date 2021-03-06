/* Migration Script */

CREATE TABLE [dbo].[Badges](
	[Id] int PRIMARY KEY NOT NULL IDENTITY(1,1),
	[Title] nvarchar(128) NOT NULL,
	[Description] nvarchar(512) NOT NULL,
);

CREATE TABLE [dbo].[UsersBadges](
	[UserId] int NOT NULL,
	[BadgeId] int NOT NULL,
	
	CONSTRAINT [PK_UsersBadges] PRIMARY KEY CLUSTERED ([UserId] ASC, [BadgeId] ASC),
	CONSTRAINT [FK_UsersBadges_AspNetUsers] FOREIGN KEY(UserId) REFERENCES [dbo].AspNetUsers(Id),
	CONSTRAINT [FK_UsersBadges_Badges] FOREIGN KEY(BadgeId) REFERENCES [dbo].Badges(Id)
);

GO
INSERT INTO [dbo].[Badges]
           ([Title], [Description])
     VALUES
		(N'Podłączony', N'Założyłeś konto, cieszymy się, że jesteś!'),
		(N'Pierwsze piwo', N'Wypiłeś z TapFinder-em swoje pierwsze piwo.'),
		(N'Pierwsza promocja', N'Dodałeś do aplikacji pierwszą promocję. Gratulacje!'),
		(N'Aktywny', N'Dodałeś do aplikacji dziesięć promocji! Oby tak dalej!'),
		(N'Piwosz', N'Wypiłeś z naszą aplikacją pierwsze dziesięć piw. Gratulacje!')
;