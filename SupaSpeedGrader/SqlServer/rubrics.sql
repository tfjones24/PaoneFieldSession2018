﻿/*************************************************************
	LGE - this script was generated by Sql Server 2008
		  and tested on Sql Server 2014 Community Edition
		  then modified and tested on Sql Server 2016 Community Edition
		  
		  my database is named [oauth2test], if you use a different name
		  adjust the USE statement accordingly
*************************************************************/

USE [oauth2test]
GO

/****** Object:  Table [dbo].[rubrics]   ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rubrics]') AND type in (N'U'))
DROP TABLE [dbo].[rubrics]
GO

USE [oauth2test]
GO

/****** Object:  Table [dbo].[rubrics]    ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[rubrics](
	[id]   [integer] PRIMARY KEY,
	[name] [text] NOT NULL,
	[json] [text] NOT NULL,
	[rubricCols] [text],
	[rubricRows] [text],
	[questionCount] [text]
)

GO


