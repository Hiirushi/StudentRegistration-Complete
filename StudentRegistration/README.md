*Student Registration System*

Features 
- Student registration
- View students
- Update student details
- Delete student records
- Generate student reports

Technologies used 
- Framework: .Net Framework 4.8
- Database: Microsoft SQL Server
- Database Library: Microsoft.Data.SqlClient
- Reporting: RDLC (ReportViewer)

Project Structure
- The application follows a layered architecture to separate responsibilities.

Database Setup

Create the Database

`CREATE DATABASE STUREG;`

Use Database
`USE STUREG`

Create Course Table

`CREATE Table Courses(
	CourseID INT IDENTITY(1,1) PRIMARY KEY,
	CourseName VARCHAR(100) NOT NULL UNIQUE
);`

Create Student Table

```
CREATE Table Students(
	StudentID INT IDENTITY(1,1) PRIMARY KEY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	Address VARCHAR(255) NULL,
	DateOfBirth DATE NULL,
	CourseID INT NOT NULL

	CONSTRAINT FK_Students_Courses
		FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);
```

Insert sample courses 

`
INSERT INTO Courses (CourseName)
VALUES
    ('Information Technology'),
    ('Software Engineering'),
    ('Computer Science'),
    ('Information Systems'),
    ('Business Management');
`

Configure the Connection String 
- Update App.Config with local SQL server instance

