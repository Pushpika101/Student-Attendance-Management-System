Imports MySql.Data.MySqlClient
Imports System.IO

Module Program

    Dim connectionString As String = "server=localhost;user=root;password=;database=attendpro_db;"

    Sub Main(args As String())
        Console.WriteLine("ATTENDPRO - Login")
        Console.Write("Username: ")
        Dim username As String = Console.ReadLine()

        Console.Write("Password: ")
        Dim password As String = Console.ReadLine()

        Dim role As String = Login(username, password)

        If role <> "" Then
            Console.WriteLine("Login successful.")
            Console.WriteLine("Role: " & role)

            ShowMenu(role)
        Else
            Console.WriteLine("Invalid username or password.")
        End If

        Console.ReadLine()
    End Sub

    Function Login(username As String, password As String) As String
        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "SELECT role FROM users WHERE username=@username AND password=@password"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    cmd.Parameters.AddWithValue("@password", password)

                    Dim result = cmd.ExecuteScalar()

                    If result IsNot Nothing Then
                        Return result.ToString()
                    Else
                        Return ""
                    End If
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Database error: " & ex.Message)
            Return ""
        End Try
    End Function

    Sub AddStudent()
        Console.WriteLine()
        Console.WriteLine("Add New Student")

        Console.Write("Student ID: ")
        Dim studentId As String = Console.ReadLine()

        Console.Write("Full Name: ")
        Dim fullName As String = Console.ReadLine()

        Console.Write("Course Year: ")
        Dim courseYear As String = Console.ReadLine()

        Console.Write("Contact Info: ")
        Dim contactInfo As String = Console.ReadLine()

        If studentId = "" Or fullName = "" Then
            Console.WriteLine("Student ID and Full Name cannot be empty.")
            Return
        End If

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "INSERT INTO students(student_id, full_name, course_year, contact_info) VALUES(@id, @name, @year, @contact)"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", studentId)
                    cmd.Parameters.AddWithValue("@name", fullName)
                    cmd.Parameters.AddWithValue("@year", courseYear)
                    cmd.Parameters.AddWithValue("@contact", contactInfo)

                    cmd.ExecuteNonQuery()
                    Console.WriteLine("Student added successfully.")
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error adding student: " & ex.Message)
        End Try
    End Sub

    Sub AddCourse()
        Console.WriteLine()
        Console.WriteLine("Add New Course")

        Console.Write("Course ID: ")
        Dim courseId As String = Console.ReadLine()

        Console.Write("Course Name: ")
        Dim courseName As String = Console.ReadLine()

        Console.Write("Lecturer Name: ")
        Dim lecturerName As String = Console.ReadLine()

        Console.Write("Hall Number: ")
        Dim hallNumber As String = Console.ReadLine()

        If courseId = "" Or courseName = "" Then
            Console.WriteLine("Course ID and Course Name cannot be empty.")
            Return
        End If

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "INSERT INTO courses(course_id, course_name, lecturer_name, hall_number) VALUES(@id, @name, @lecturer, @hall)"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", courseId)
                    cmd.Parameters.AddWithValue("@name", courseName)
                    cmd.Parameters.AddWithValue("@lecturer", lecturerName)
                    cmd.Parameters.AddWithValue("@hall", hallNumber)

                    cmd.ExecuteNonQuery()
                    Console.WriteLine("Course added successfully.")
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error adding course: " & ex.Message)
        End Try
    End Sub

    Sub MarkAttendance()
        Console.WriteLine()
        Console.WriteLine("Mark Attendance")

        Console.Write("Student ID: ")
        Dim studentId As String = Console.ReadLine()

        Console.Write("Course ID: ")
        Dim courseId As String = Console.ReadLine()

        Console.Write("Date (YYYY-MM-DD): ")
        Dim attendanceDate As String = Console.ReadLine()

        Dim status As String = ""

        Do
            Console.Write("Status (Present/Absent/Late): ")
            status = Console.ReadLine()

            If Not IsValidStatus(status) Then
                Console.WriteLine("Invalid status. Please enter Present, Absent, or Late.")
            End If

        Loop While Not IsValidStatus(status)

        status = status.Trim()
        
        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "INSERT INTO attendance(student_id, course_id, attendance_date, status) VALUES(@student_id, @course_id, @date, @status)"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@student_id", studentId)
                    cmd.Parameters.AddWithValue("@course_id", courseId)
                    cmd.Parameters.AddWithValue("@date", attendanceDate)
                    cmd.Parameters.AddWithValue("@status", status)

                    cmd.ExecuteNonQuery()
                    Console.WriteLine("Attendance marked successfully.")
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error marking attendance: " & ex.Message)
        End Try
    End Sub

    Sub ViewAttendanceReport()
        Console.WriteLine()
        Console.WriteLine("Attendance Report")

        Console.Write("Student ID: ")
        Dim studentId As String = Console.ReadLine()

        Console.Write("Course ID: ")
        Dim courseId As String = Console.ReadLine()

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim totalQuery As String = "SELECT COUNT(*) FROM attendance WHERE student_id=@student_id AND course_id=@course_id"
                Dim presentQuery As String = "SELECT COUNT(*) FROM attendance WHERE student_id=@student_id AND course_id=@course_id AND status='Present'"

                Dim totalClasses As Integer
                Dim presentClasses As Integer

                Using cmd As New MySqlCommand(totalQuery, conn)
                    cmd.Parameters.AddWithValue("@student_id", studentId)
                    cmd.Parameters.AddWithValue("@course_id", courseId)
                    totalClasses = Convert.ToInt32(cmd.ExecuteScalar())
                End Using

                Using cmd As New MySqlCommand(presentQuery, conn)
                    cmd.Parameters.AddWithValue("@student_id", studentId)
                    cmd.Parameters.AddWithValue("@course_id", courseId)
                    presentClasses = Convert.ToInt32(cmd.ExecuteScalar())
                End Using

                If totalClasses = 0 Then
                    Console.WriteLine("No attendance records found.")
                Else
                    Dim percentage As Double = (presentClasses / totalClasses) * 100

                    Console.WriteLine("Total Classes: " & totalClasses)
                    Console.WriteLine("Present Classes: " & presentClasses)
                    Console.WriteLine("Attendance Percentage: " & percentage.ToString("0.00") & "%")

                    If percentage < 80 Then
                        Console.WriteLine("Warning: Attendance is below 80%.")
                    Else
                        Console.WriteLine("Attendance status is satisfactory.")
                    End If
                End If
            End Using

        Catch ex As Exception
            Console.WriteLine("Error generating report: " & ex.Message)
        End Try
    End Sub

    Sub ShowMenu(role As String)
        Dim choice As String = ""

        Do
            Console.WriteLine()
            Console.WriteLine("===== ATTENDPRO MENU =====")

            If role.ToLower() = "admin" Then
                Console.WriteLine("1. Add Student")
                Console.WriteLine("2. Add Course")
                Console.WriteLine("3. View Students")
                Console.WriteLine("4. View Courses")
                Console.WriteLine("5. Mark Attendance")
                Console.WriteLine("6. View Attendance Report")
                Console.WriteLine("7. Delete Student")
                Console.WriteLine("8. Update Student")
                Console.WriteLine("9. Delete Course")
                Console.WriteLine("10. Update Course")
                Console.WriteLine("11. Add User")
                Console.WriteLine("12. View Users")
                Console.WriteLine("13. Export Attendance Report")
                Console.WriteLine("14. Low Attendance Report")
                Console.WriteLine("15. Date-wise Attendance Report")
                Console.WriteLine("16. Exit")
            Else
                Console.WriteLine("1. View Students")
                Console.WriteLine("2. View Courses")
                Console.WriteLine("3. Mark Attendance")
                Console.WriteLine("4. View Attendance Report")
                Console.WriteLine("5. Date-wise Attendance Report")
                Console.WriteLine("6. Low Attendance Report")
                Console.WriteLine("7. Export Attendance Report")
                Console.WriteLine("8. Exit")
            End If

            Console.Write("Enter your choice: ")
            choice = Console.ReadLine()

            If role.ToLower() = "admin" Then
                Select Case choice
                    Case "1"
                        AddStudent()
                    Case "2"
                        AddCourse()
                    Case "3"
                        ViewStudents()
                    Case "4"
                        ViewCourses()
                    Case "5"
                        MarkAttendance()
                    Case "6"
                        ViewAttendanceReport()
                    Case "7"
                        DeleteStudent()
                    Case "8"
                        UpdateStudent()
                    Case "9"
                        DeleteCourse()
                    Case "10"
                        UpdateCourse()
                    Case "11"
                        AddUser()
                    Case "12"
                        ViewUsers()
                    Case "13"
                        ExportAttendanceReport()
                    Case "14"
                        LowAttendanceReport()
                    Case "15"
                        DateWiseAttendanceReport()
                    Case "16"
                        Console.WriteLine("Exiting system...")
                    Case Else
                        Console.WriteLine("Invalid choice.")
                End Select
            Else
                Select Case choice
                    Case "1"
                        ViewStudents()
                    Case "2"
                        ViewCourses()
                    Case "3"
                        MarkAttendance()
                    Case "4"
                        ViewAttendanceReport()
                    Case "5"
                        DateWiseAttendanceReport()
                    Case "6"
                        LowAttendanceReport()
                    Case "7"
                        ExportAttendanceReport()
                    Case "8"
                        Console.WriteLine("Exiting system...")
                    Case Else
                        Console.WriteLine("Invalid choice.")
                End Select
            End If

        Loop While Not ((role.ToLower() = "admin" And choice = "16") Or (role.ToLower() <> "admin" And choice = "8"))
    End Sub

    Sub ViewStudents()
        Console.WriteLine()
        Console.WriteLine("Student List")

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "SELECT student_id, full_name, course_year, contact_info FROM students"

                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Console.WriteLine(reader("student_id") & " | " &
                                            reader("full_name") & " | " &
                                            reader("course_year") & " | " &
                                            reader("contact_info"))
                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error loading students: " & ex.Message)
        End Try
    End Sub

    Sub ViewCourses()
        Console.WriteLine()
        Console.WriteLine("Course List")

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "SELECT course_id, course_name, lecturer_name, hall_number FROM courses"

                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Console.WriteLine(reader("course_id") & " | " &
                                            reader("course_name") & " | " &
                                            reader("lecturer_name") & " | " &
                                            reader("hall_number"))
                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error loading courses: " & ex.Message)
        End Try
    End Sub

    Sub DeleteStudent()
        Console.WriteLine()
        Console.WriteLine("Delete Student")

        Console.Write("Enter Student ID to delete: ")
        Dim studentId As String = Console.ReadLine()

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                ' First delete attendance records linked to this student
                Dim deleteAttendanceQuery As String = "DELETE FROM attendance WHERE student_id=@id"

                Using cmd As New MySqlCommand(deleteAttendanceQuery, conn)
                    cmd.Parameters.AddWithValue("@id", studentId)
                    cmd.ExecuteNonQuery()
                End Using

                ' Then delete student
                Dim deleteStudentQuery As String = "DELETE FROM students WHERE student_id=@id"

                Using cmd As New MySqlCommand(deleteStudentQuery, conn)
                    cmd.Parameters.AddWithValue("@id", studentId)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        Console.WriteLine("Student deleted successfully.")
                    Else
                        Console.WriteLine("Student not found.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error deleting student: " & ex.Message)
        End Try
    End Sub

    Sub UpdateStudent()
        Console.WriteLine()
        Console.WriteLine("Update Student Details")

        Console.Write("Enter Student ID to update: ")
        Dim studentId As String = Console.ReadLine()

        Console.Write("New Full Name: ")
        Dim fullName As String = Console.ReadLine()

        Console.Write("New Course Year: ")
        Dim courseYear As String = Console.ReadLine()

        Console.Write("New Contact Info: ")
        Dim contactInfo As String = Console.ReadLine()

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "UPDATE students SET full_name=@name, course_year=@year, contact_info=@contact WHERE student_id=@id"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", studentId)
                    cmd.Parameters.AddWithValue("@name", fullName)
                    cmd.Parameters.AddWithValue("@year", courseYear)
                    cmd.Parameters.AddWithValue("@contact", contactInfo)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        Console.WriteLine("Student updated successfully.")
                    Else
                        Console.WriteLine("Student not found.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error updating student: " & ex.Message)
        End Try
    End Sub

    Sub DeleteCourse()
        Console.WriteLine()
        Console.WriteLine("Delete Course")

        Console.Write("Enter Course ID to delete: ")
        Dim courseId As String = Console.ReadLine()

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                ' First delete attendance records linked to this course
                Dim deleteAttendanceQuery As String = "DELETE FROM attendance WHERE course_id=@id"

                Using cmd As New MySqlCommand(deleteAttendanceQuery, conn)
                    cmd.Parameters.AddWithValue("@id", courseId)
                    cmd.ExecuteNonQuery()
                End Using

                ' Then delete course
                Dim deleteCourseQuery As String = "DELETE FROM courses WHERE course_id=@id"

                Using cmd As New MySqlCommand(deleteCourseQuery, conn)
                    cmd.Parameters.AddWithValue("@id", courseId)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        Console.WriteLine("Course deleted successfully.")
                    Else
                        Console.WriteLine("Course not found.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error deleting course: " & ex.Message)
        End Try
    End Sub

    Sub UpdateCourse()
        Console.WriteLine()
        Console.WriteLine("Update Course Details")

        Console.Write("Enter Course ID to update: ")
        Dim courseId As String = Console.ReadLine()

        Console.Write("New Course Name: ")
        Dim courseName As String = Console.ReadLine()

        Console.Write("New Lecturer Name: ")
        Dim lecturerName As String = Console.ReadLine()

        Console.Write("New Hall Number: ")
        Dim hallNumber As String = Console.ReadLine()

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "UPDATE courses SET course_name=@name, lecturer_name=@lecturer, hall_number=@hall WHERE course_id=@id"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", courseId)
                    cmd.Parameters.AddWithValue("@name", courseName)
                    cmd.Parameters.AddWithValue("@lecturer", lecturerName)
                    cmd.Parameters.AddWithValue("@hall", hallNumber)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        Console.WriteLine("Course updated successfully.")
                    Else
                        Console.WriteLine("Course not found.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error updating course: " & ex.Message)
        End Try
    End Sub

    Sub AddUser()
        Console.WriteLine()
        Console.WriteLine("Create New User Account")

        Console.Write("Username: ")
        Dim username As String = Console.ReadLine()

        Console.Write("Password: ")
        Dim password As String = Console.ReadLine()

        Console.Write("Role (admin/lecturer): ")
        Dim role As String = Console.ReadLine()

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "INSERT INTO users(username, password, role) VALUES(@username, @password, @role)"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    cmd.Parameters.AddWithValue("@password", password)
                    cmd.Parameters.AddWithValue("@role", role)

                    cmd.ExecuteNonQuery()
                    Console.WriteLine("User account created successfully.")
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error creating user account: " & ex.Message)
        End Try
    End Sub

    Sub ViewUsers()
        Console.WriteLine()
        Console.WriteLine("User Account List")

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "SELECT user_id, username, role FROM users"

                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Console.WriteLine(reader("user_id") & " | " &
                                              reader("username") & " | " &
                                              reader("role"))
                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error loading users: " & ex.Message)
        End Try
    End Sub

    Sub ExportAttendanceReport()
        Console.WriteLine()
        Console.WriteLine("Export Attendance Report to CSV")

        Console.Write("Course ID: ")
        Dim courseId As String = Console.ReadLine()

        Dim fileName As String = "attendance_report_" & courseId & ".csv"

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String =
                    "SELECT a.student_id, s.full_name, a.course_id, c.course_name, a.attendance_date, a.status " &
                    "FROM attendance a " &
                    "INNER JOIN students s ON a.student_id = s.student_id " &
                    "INNER JOIN courses c ON a.course_id = c.course_id " &
                    "WHERE a.course_id=@course_id " &
                    "ORDER BY a.attendance_date"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@course_id", courseId)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        Using writer As New StreamWriter(fileName)
                            writer.WriteLine("Student ID,Full Name,Course ID,Course Name,Date,Status")

                            While reader.Read()
                                writer.WriteLine(reader("student_id") & "," &
                                                reader("full_name") & "," &
                                                reader("course_id") & "," &
                                                reader("course_name") & "," &
                                                Convert.ToDateTime(reader("attendance_date")).ToString("yyyy-MM-dd") & "," &
                                                reader("status"))
                            End While
                        End Using
                    End Using
                End Using
            End Using

            Console.WriteLine("Report exported successfully: " & fileName)

        Catch ex As Exception
            Console.WriteLine("Error exporting report: " & ex.Message)
        End Try
    End Sub

    Sub LowAttendanceReport()
        Console.WriteLine()
        Console.WriteLine("Low Attendance Report - Below 80%")

        Console.Write("Course ID: ")
        Dim courseId As String = Console.ReadLine()

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String =
                    "SELECT s.student_id, s.full_name, " &
                    "COUNT(a.attendance_id) AS total_classes, " &
                    "SUM(CASE WHEN a.status='Present' THEN 1 ELSE 0 END) AS present_classes, " &
                    "(SUM(CASE WHEN a.status='Present' THEN 1 ELSE 0 END) / COUNT(a.attendance_id)) * 100 AS percentage " &
                    "FROM attendance a " &
                    "INNER JOIN students s ON a.student_id = s.student_id " &
                    "WHERE a.course_id=@course_id " &
                    "GROUP BY s.student_id, s.full_name " &
                    "HAVING percentage < 80"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@course_id", courseId)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        Dim found As Boolean = False

                        While reader.Read()
                            found = True
                            Console.WriteLine(reader("student_id") & " | " &
                                              reader("full_name") & " | Total: " &
                                              reader("total_classes") & " | Present: " &
                                              reader("present_classes") & " | " &
                                              Convert.ToDouble(reader("percentage")).ToString("0.00") & "%")
                        End While

                        If Not found Then
                            Console.WriteLine("No students below 80% attendance.")
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error generating low attendance report: " & ex.Message)
        End Try
    End Sub

    Sub DateWiseAttendanceReport()
        Console.WriteLine()
        Console.WriteLine("Date-wise Attendance Report")

        Console.Write("Course ID: ")
        Dim courseId As String = Console.ReadLine()

        Console.Write("Date (YYYY-MM-DD): ")
        Dim attendanceDate As String = Console.ReadLine()

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String =
                    "SELECT a.student_id, s.full_name, c.course_name, a.attendance_date, a.status " &
                    "FROM attendance a " &
                    "INNER JOIN students s ON a.student_id = s.student_id " &
                    "INNER JOIN courses c ON a.course_id = c.course_id " &
                    "WHERE a.course_id=@course_id AND a.attendance_date=@date " &
                    "ORDER BY s.full_name"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@course_id", courseId)
                    cmd.Parameters.AddWithValue("@date", attendanceDate)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        Dim found As Boolean = False

                        While reader.Read()
                            found = True
                            Console.WriteLine(reader("student_id") & " | " &
                                              reader("full_name") & " | " &
                                              reader("course_name") & " | " &
                                              Convert.ToDateTime(reader("attendance_date")).ToString("yyyy-MM-dd") & " | " &
                                              reader("status"))
                        End While

                        If Not found Then
                            Console.WriteLine("No attendance records found for this date.")
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine("Error generating date-wise report: " & ex.Message)
        End Try
    End Sub

    Function IsValidStatus(status As String) As Boolean
        status = status.Trim().ToLower()

        Return status = "present" Or status = "absent" Or status = "late"
    End Function

End Module