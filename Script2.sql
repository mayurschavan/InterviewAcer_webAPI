/****** Object:  Table [dbo].[ForgotPassword]    Script Date: 7/1/2018 11:52:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForgotPassword](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[OTP] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ForgotPassword] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ForgotPassword]  WITH CHECK ADD  CONSTRAINT [FK_ForgotPassword_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ForgotPassword] CHECK CONSTRAINT [FK_ForgotPassword_AspNetUsers]
GO
