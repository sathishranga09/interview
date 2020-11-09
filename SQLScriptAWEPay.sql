USE [ActivityLog]


GO
/****** Object:  Table [dbo].[UserReg]    Script Date: 11/9/2020 7:34:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserReg](
	[UserID] [varchar](50) NOT NULL,
	[FullName] [varchar](100) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Phone] [varchar](20) NULL,
	[Age] [varchar](3) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO


GO
/****** Object:  StoredProcedure [dbo].[spDeleteUser]    Script Date: 11/9/2020 7:34:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[spDeleteUser]
(		
		@UserID	VARCHAR(100)
)
AS
BEGIN		
	IF EXISTS (SELECT TOP 1 FullName FROM UserReg WHERE UserID = @UserID)
	BEGIN
		DELETE FROM UserReg WHERE UserID = @UserID
		IF NOT EXISTS (SELECT TOP 1 FullName FROM UserReg WHERE UserID = @UserID)
			SELECT "Record Deleted Successfully" AS Result;
		ELSE
			SELECT "Record Not Deleted" AS Result;
	END
	ELSE
	SELECT "Please verify the UserID" AS Result;

	
END


GO
/****** Object:  StoredProcedure [dbo].[spInsertUser]    Script Date: 11/9/2020 7:34:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[spInsertUser]
(		
		@FullName VARCHAR(100),
		@Email	VARCHAR(100),
		@Phone	VARCHAR(20),
		@Age	VARCHAR(3)
)
AS
BEGIN	
	DECLARE @ID VARCHAR(50)
	SET @ID = NEWID(); 

	INSERT INTO UserReg(UserID,FullName,Email,Phone,Age)
	VALUES(@ID,@FullName,@Email,@Phone,@Age)

	SELECT @ID AS UserID;

END


GO
/****** Object:  StoredProcedure [dbo].[spSearchUser]    Script Date: 11/9/2020 7:34:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[spSearchUser]
(		
		@Email	VARCHAR(100),
		@Phone	VARCHAR(20)
)
AS
BEGIN	
 IF (@Email = '' AND @Phone = '')
	SELECT UserID,FullName,Email,Phone,Age FROM UserReg ORDER BY FullName
 ELSE
	SELECT UserID,FullName,Email,Phone,Age FROM UserReg WHERE (Email = @Email OR Phone = @Phone) ORDER BY FullName
END


GO
/****** Object:  StoredProcedure [dbo].[spUpdateUser]    Script Date: 11/9/2020 7:34:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[spUpdateUser]
(		
		@UserID VARCHAR(50),
		@FullName VARCHAR(100),
		@Email	VARCHAR(100),
		@Phone	VARCHAR(20),
		@Age	VARCHAR(3)
)
AS
BEGIN		
	
	IF EXISTS (SELECT TOP 1 FullName FROM UserReg WHERE UserID = @UserID)
	BEGIN
		UPDATE UserReg SET 
		FullName= @FullName,
		Email = @Email,
		Phone = @Phone,
		Age = @Age
		WHERE UserID = @UserID; 
		SELECT "Updated Successfully" AS Result;
	END
	ELSE
	BEGIN
		SELECT "Please verify the UserID" AS Result;
	END

END

GO