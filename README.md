# PaoneFieldSession2018
Quick Quiz Grader: a much better Canvas grading system extension that uses the Blackboard grading system setup. This LTI application leverages the Canvas API to get student submission and grades, and put back in Canvas new grades and feedback. This also includes a custom Rubric system to speed up grading of free response questions.

Deployment Notes, Tips, and Tricks:

Project has been verified to build correctly using Visual Studio 2017 (any edition) so long as all NuGet packages are present. 
Project has been verified to deploy correctly to IIS on Windows Server 2016 Datacenter edition, but should deploy correctly to any Windows Server 2016 version.
Project has been verified to work with SQL Server 2016 Community Edition hosted on the same instance of Windows Server 2016 as the IIS deployment.

To load the application into Canvas:
We recommend using Paste-XML when adding the application to Canvas. We used this site (https://www.edu-apps.org/build_xml.html) to build the XML. A few short notes:
- Name is the name that will appear in the side bar
- LTI launch URL should point to the public-access domain for the IIS site
- Launch Privacy MUST be set to Public
- Extenstions: select ONLY Course Navigation
  -LTI Launch URL MUST be https://the-app-url/Home/oauth2Request
  
Some configuration notes:
-Web.config has a few values that MUST be changed
  -oauth2ClientId: Must be the generated Developer Key ID from Canvas
  -oauth2ClientKey: Must be the generated Developer Key from Canvas
  -consumerKey: This is set when you paste the XML made above into Canvas. THIS MUST MATCH WHAT YOU SET IN CANVAS
  -consumerSecret: This is set when you paste the XML made above into Canvas. THIS MUST MATCH WHAT YOU SET IN CANVAS
  
  -oauth2: This is the connection string for the SQL server. This MUST match the connection settings for your target SQL Database
  
SQL Notes:
This has only been validated with SQL Server 2016. It likely will NOT work with other vendors' SQL solutions, but this DOES work with SQL Server 2016 Community, which is free. In the project folder, or the deployment folder, there is a folder titled SqlServer. All these scripts must be run on the SQL instance the deployed application will be accessing, and must have a database titled oauth2test to access. Make sure the application instance has access to create and drop tables on this database, since it will do that for every quiz it loads in.

It is recommended to clean the session state table (created in the stateCache.sql file) from any session not accessed in the last 24 hours. However, no such task exists in our project, and is recommended to schedule in your SQL server manager as a repeat task.

Access tokens should NEVER be cleared, to prevent cluttering of the application autorization list in a user's profile. Any and all maintainence of this table is done in-app.

Other notes:
As part of the workaround, the application will download CSV files from Canvas to the path C:\qqg_temp_data\quizID_report.csv. Make sure the application has full privledges for this directory alone, and sufficient space is given to this drive. Our test reports were typically sub-10kb in size, but larger quizzes with >50 students have been documented in the MB per CSV. 

These files can be cleaned daily, but no such task currently exists within this project. 
