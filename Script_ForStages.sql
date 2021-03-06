/****** Object:  Table [dbo].[GroupCheckList]    Script Date: 7/16/2018 1:55:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupCheckList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Points] [int] NOT NULL,
 CONSTRAINT [PK_GroupCheckList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InterviewCheckListMapping]    Script Date: 7/16/2018 1:55:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InterviewCheckListMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InterviewId] [int] NOT NULL,
	[CheckListId] [int] NOT NULL,
 CONSTRAINT [PK_InterviewCheckListMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StageGroups]    Script Date: 7/16/2018 1:55:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StageGroups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StageId] [int] NOT NULL,
	[GroupName] [nvarchar](max) NOT NULL,
	[Sequence] [int] NOT NULL,
 CONSTRAINT [PK_StageGroups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Stages]    Script Date: 7/16/2018 1:55:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InterviewTypeId] [int] NOT NULL,
	[StageName] [nvarchar](max) NOT NULL,
	[Sequence] [int] NOT NULL,
 CONSTRAINT [PK_Stages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[GroupCheckList] ON 

INSERT [dbo].[GroupCheckList] ([Id], [GroupId], [Name], [Points]) VALUES (1, 5, N'Resume Copies', 10)
INSERT [dbo].[GroupCheckList] ([Id], [GroupId], [Name], [Points]) VALUES (2, 5, N'Iron Shirt', 5)
INSERT [dbo].[GroupCheckList] ([Id], [GroupId], [Name], [Points]) VALUES (3, 5, N'Shine Shoes', 10)
INSERT [dbo].[GroupCheckList] ([Id], [GroupId], [Name], [Points]) VALUES (4, 6, N'Proper On Time Travel Arrangement', 10)
INSERT [dbo].[GroupCheckList] ([Id], [GroupId], [Name], [Points]) VALUES (5, 7, N'Study about the company', 10)
INSERT [dbo].[GroupCheckList] ([Id], [GroupId], [Name], [Points]) VALUES (6, 8, N'Give Interview with confidence and proper communication skills', 10)
INSERT [dbo].[GroupCheckList] ([Id], [GroupId], [Name], [Points]) VALUES (7, 9, N'Check all the documents provided by company', 10)
SET IDENTITY_INSERT [dbo].[GroupCheckList] OFF
SET IDENTITY_INSERT [dbo].[StageGroups] ON 

INSERT [dbo].[StageGroups] ([Id], [StageId], [GroupName], [Sequence]) VALUES (5, 4, N'Dress And Accessories', 1)
INSERT [dbo].[StageGroups] ([Id], [StageId], [GroupName], [Sequence]) VALUES (6, 4, N'Travel', 2)
INSERT [dbo].[StageGroups] ([Id], [StageId], [GroupName], [Sequence]) VALUES (7, 4, N'Company', 3)
INSERT [dbo].[StageGroups] ([Id], [StageId], [GroupName], [Sequence]) VALUES (8, 5, N'Proper Communication', 1)
INSERT [dbo].[StageGroups] ([Id], [StageId], [GroupName], [Sequence]) VALUES (9, 6, N'Check Documents', 1)
SET IDENTITY_INSERT [dbo].[StageGroups] OFF
SET IDENTITY_INSERT [dbo].[Stages] ON 

INSERT [dbo].[Stages] ([Id], [InterviewTypeId], [StageName], [Sequence]) VALUES (4, 1, N'Prepare', 1)
INSERT [dbo].[Stages] ([Id], [InterviewTypeId], [StageName], [Sequence]) VALUES (5, 1, N'Perform', 2)
INSERT [dbo].[Stages] ([Id], [InterviewTypeId], [StageName], [Sequence]) VALUES (6, 1, N'Win', 3)
SET IDENTITY_INSERT [dbo].[Stages] OFF
ALTER TABLE [dbo].[GroupCheckList]  WITH CHECK ADD  CONSTRAINT [FK_GroupCheckList_StageGroups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[StageGroups] ([Id])
GO
ALTER TABLE [dbo].[GroupCheckList] CHECK CONSTRAINT [FK_GroupCheckList_StageGroups]
GO
ALTER TABLE [dbo].[InterviewCheckListMapping]  WITH CHECK ADD  CONSTRAINT [FK_InterviewCheckListMapping_GroupCheckList] FOREIGN KEY([CheckListId])
REFERENCES [dbo].[GroupCheckList] ([Id])
GO
ALTER TABLE [dbo].[InterviewCheckListMapping] CHECK CONSTRAINT [FK_InterviewCheckListMapping_GroupCheckList]
GO
ALTER TABLE [dbo].[InterviewCheckListMapping]  WITH CHECK ADD  CONSTRAINT [FK_InterviewCheckListMapping_InterviewDetails] FOREIGN KEY([InterviewId])
REFERENCES [dbo].[InterviewDetails] ([InterviewDetailId])
GO
ALTER TABLE [dbo].[InterviewCheckListMapping] CHECK CONSTRAINT [FK_InterviewCheckListMapping_InterviewDetails]
GO
ALTER TABLE [dbo].[StageGroups]  WITH CHECK ADD  CONSTRAINT [FK_StageGroups_Stages] FOREIGN KEY([StageId])
REFERENCES [dbo].[Stages] ([Id])
GO
ALTER TABLE [dbo].[StageGroups] CHECK CONSTRAINT [FK_StageGroups_Stages]
GO
ALTER TABLE [dbo].[Stages]  WITH CHECK ADD  CONSTRAINT [FK_Stages_InterviewTypes] FOREIGN KEY([InterviewTypeId])
REFERENCES [dbo].[InterviewTypes] ([InterviewTypeId])
GO
ALTER TABLE [dbo].[Stages] CHECK CONSTRAINT [FK_Stages_InterviewTypes]
GO
/****** Object:  StoredProcedure [dbo].[GetInterviewStage]    Script Date: 7/16/2018 1:55:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetInterviewStage] 
	@InterviewId int
AS
BEGIN
	DECLARE @topStage INT
	DECLARE @countIntersection INT

	select Distinct  s.StageName, s.Id, s.Sequence INTO #temp FROM stages s
	INNER JOIN StageGroups sg ON sg.StageId = s.id 
	INNER JOIN GroupCheckList gcl ON gcl.GroupId = sg.Id
	INNER JOIN InterviewCheckListMapping iclm ON iclm.CheckListId = gcl.Id
	where iclm.InterviewId = @InterviewId

	SET @topStage  = (SELECT TOP 1 Id FROM #temp ORDER BY SEQUENCE DESC)

	if(@topStage > 0)
	BEGIN
		SET @countIntersection = (SELECT COUNT(*) FROM STAGES S
		INNER JOIN StageGroups SG ON SG.StageId = S.Id
		INNER JOIN GroupCheckList GCL ON GCL.GroupId = SG.Id
		WHERE S.Id = @topStage AND GCL.Id NOT IN (SELECT ICLM.CheckListId FROM InterviewCheckListMapping ICLM WHERE InterviewId = @InterviewId) )

		IF(@countIntersection > 0)
			BEGIN
				SELECT TOP 1 * FROM #temp ORDER BY SEQUENCE DESC
			 END
			ELSE IF (@countIntersection = 0)
			BEGIN
				IF EXISTS(SELECT 1 FROM STAGES WHERE Sequence > (SELECT TOP 1 Sequence FROM #temp ORDER BY SEQUENCE DESC))
					SELECT TOP 1 S.StageName, S.Id, S.Sequence FROM STAGES S WHERE Sequence > (SELECT TOP 1 Sequence FROM #temp ORDER BY SEQUENCE DESC)
				ELSE
				   SELECT TOP 1 * FROM #temp ORDER BY SEQUENCE DESC
			END
	END
	ELSE
	BEGIN
	  SELECT TOP 1 S.StageName, S.Id, S.Sequence FROM STAGES S 
	  Where S.InterviewTypeId = (Select InterviewTypeId from InterviewDetails Where InterviewDetailId  = @InterviewId) ORDER BY S.Sequence 
	END
END



GO
