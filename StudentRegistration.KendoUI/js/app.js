$(document).ready(function () {

    const apiBaseUrl = "https://localhost:44381";

    // notification
    var notification = $("#notification").kendoNotification({
        position: {
            pinned: true,
            top: 20,
            right: 20
        },
        autoHideAfter: 3000,
        stacking: "down"
    })
        .data("kendoNotification");

    //course Datasource
    var courseDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: apiBaseUrl + "/api/courses",
                type: "GET",
                dataType: "json"
            }
        },

        schema: {
            parse: function (response) {
                console.log("Courses API response:", response);
                return $.map(response, function (course) {
                    return {
                        CourseId: course.CourseID,
                        CourseName: course.CourseName
                    };
                });
            }
        },
        error: function (e) {
            console.error("course loading error:", e);
            notification.show(
                "unable to load courses",
                "error"
            );
        }
    });

    $("#firstName").kendoTextBox({
        placeholder: "Enter first name"
    });

    $("#lastName").kendoTextBox({
        placeholder: "Enter last name"
    });

    $("#address").kendoTextBox({
        placeholder: "Enter address"
    });

    $("#dateOfBirth").kendoDatePicker({
        format: "dd/MM/yyyy",
        max: new Date()
    });

    $("#course").kendoDropDownList({
        optionLabel: "Select a course...",
        dataTextField: "CourseName",
        dataValueField: "CourseId",
        dataSource: courseDataSource
    });

    $("#registerButton").kendoButton({
        themeColor: "primary"
    });

    $("#viewStudentReportBtn").kendoButton({
        themeColor: "primary",
        icon: "file-pdf"
    });

    $("#clearButton").kendoButton();

    //validator
    var validator = $("#studentForm").kendoValidator().data("kendoValidator");

    //register student
    $("#studentForm").on("submit", function (e) {
        e.preventDefault();

        console.log("Register button clicked!");

        if (!validator.validate()) {
            console.log("Form validation failed!");
            return;
        }

        var courseId = $("#course")
            .data("kendoDropDownList")
            .value();

        var dateOfBirth = $("#dateOfBirth")
            .data("kendoDatePicker")
            .value();

        var student = {
            firstName: $("#firstName").val(),
            lastName: $("#lastName").val(),
            address: $("#address").val(),
            dateOfBirth: dateOfBirth ? kendo.toString(dateOfBirth, "yyyy-MM-dd") : null,
            courseId: parseInt(courseId, 10)
        };

        console.log("Student Details: ", student);
        $.ajax({
            url: apiBaseUrl + "/api/students",
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(student),

            success: function (response) {
                console.log("Registration successful:", response);
                notification.show(
                    response.message || "Student registered succesfully",
                    "success"
                );

                clearForm();

                studentsDataSource.read();
            },
            error: function (xhr) {
                console.error("Registeration error:", xhr);

                var errorMessage = xhr.responseText || "Unable to register student";

                notification.show(
                    errorMessage,
                    "error"
                );
            }
        });
    });

    //clear form
    function clearForm() {
        $("#firstName").data("kendoTextBox").value("");
        $("#lastName").data("kendoTextBox").value("");
        $("#address").data("kendoTextBox").value("");
        $("#dateOfBirth").data("kendoDatePicker").value(null);
        $("#course").data("kendoDropDownList").value("");
        validator.hideMessages();

    }

    $("#clearButton").on("click", function () {
        clearForm();
    });

    // student DataSource
    var studentsDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: apiBaseUrl + "/api/students",
                type: "GET",
                dataType: "json"
            },

            destroy: {
                url: function (data) {
                    return apiBaseUrl + "/api/students/" + data.StudentID;
                },
                type: "DELETE",
                dataType: "json"
            }
        },

        schema: {
            parse: function (response) {

                return response.map(function (student) {
                    return {
                        StudentID: student.StudentID,
                        FullName: student["Full Name"],
                        Address: student.Address,
                        DateOfBirth: student["Date of Birth"],
                        Course: student.Course,
                        CourseID: student.CourseID
                    };
                });
            },

            model: {
                id: "StudentID",
                fields: {
                    StudentID: { type: "number", editable: false },
                    FullName: { type: "string" },
                    Address: { type: "string" },
                    DateOfBirth: { type: "date" },
                    Course: { type: "string" },
                    CourseID: { type: "number" }
                }
            }
        },
        pageSize: 5,

        error: function (e) {
            console.error("Student DataSource error:", e);

            if (e.xhr) {
                console.error("HTTP status:", e.xhr.status);
                console.error("Response:", e.xhr.responseText);
            }

            notification.show(
                "Unable to delete student. Please try again.",
                "error"
            );
        }
    });

    $("#editFirstName").kendoTextBox({
        placeholder: "Enter first name"
    });

    $("#editLastName").kendoTextBox({
        placeholder: "Enter last name"
    });

    $("#editAddress").kendoTextBox({
        placeholder: "Enter address"
    });

    $("#editDateOfBirth").kendoDatePicker({
        format: "dd/MM/yyyy",
        max: new Date()
    });

    $("#editCourse").kendoDropDownList({
        optionLabel: "Select a course...",
        dataTextField: "CourseName",
        dataValueField: "CourseId",
        dataSource: courseDataSource
    });

    //edit window
    var editWindow = $("#editStudentWindow").kendoWindow({
        title: "Edit Student",
        width: "500px",
        modal: true,
        visible: false,
        resizable: false,
        actions: ["Close"]
    }).data("kendoWindow");

    //kendo grid
    $("#studentsGrid").kendoGrid({

        dataSource: studentsDataSource,

        pageable: {
            refresh: true,
            pageSizes: [5, 10, 20],
            buttonCount: 5
        },

        sortable: true,

        columns: [
            {
                field: "FullName",
                title: "Full Name",
            },
            {
                field: "Address",
                title: "Address",
            },
            {
                field: "DateOfBirth",
                title: "Date of Birth",
                format: "{0:dd/MM/yyyy}"
            },
            {
                field: "Course",
                title: "Course",
            },
            {
                title: "Actions",
                width: 180,
                command: [
                    {
                        name: "editStudent",
                        text: "Edit",
                        click: function (e) {
                            e.preventDefault();

                            var dataItem = this.dataItem(
                                $(e.currentTarget).closest("tr")
                            );

                            console.log("Selected student:", dataItem);
                            openEditWindow(dataItem);
                        }
                    },
                    {
                        name: "deleteStudent",
                        text: "Delete",
                        click: function (e) {
                            e.preventDefault();

                            var dataItem = this.dataItem(
                                $(e.currentTarget).closest("tr")
                            );

                            console.log("Delete button clicked!");
                            console.log("Student ID:", dataItem.StudentID);

                            if (!confirm("Are you sure you want to delete this student?")) {
                                return;
                            }

                            $.ajax({
                                url: apiBaseUrl + "/api/students/" + dataItem.StudentID,
                                type: "DELETE",
                                dataType: "json",

                                success: function (response) {
                                    console.log("Delete response:", response);

                                    notification.show(
                                        "Student deleted successfully!",
                                        "success"
                                    );

                                    studentsDataSource.read();
                                },

                                error: function (xhr) {
                                    console.error("Delete failed!");
                                    console.error("Status:", xhr.status);
                                    console.error("Response:", xhr.responseText);

                                    notification.show(
                                        "Unable to delete student.",
                                        "error"
                                    );
                                }
                            });
                        }
                    }
                ]
            }
        ]
    });

    function openEditWindow(student) {
        $.ajax({
            url: apiBaseUrl + "/api/students/" + student.StudentID,
            type: "GET",
            dataType: "json",

            success: function (response) {

                console.log("Student details:", response);

                $("#editStudentId").val(response.StudentId || response.StudentID);

                $("#editFirstName").data("kendoTextBox").value(response.FirstName);
                $("#editLastName").data("kendoTextBox").value(response.LastName);
                $("#editAddress").data("kendoTextBox").value(response.Address);

                $("#editDateOfBirth").data("kendoDatePicker").value(
                    response.DateOfBirth ? new Date(response.DateOfBirth) : null
                );

                $("#editCourse").data("kendoDropDownList").value(
                    response.CourseId
                );

                editWindow.center().open();
            },

            error: function (xhr) {
                console.error("Unable to load student:", xhr);
                notification.show("Unable to load student details.", "error");
            }
        });
    }

    $("#updateStudentButton").kendoButton({
        themeColor: "primary"
    });

    $("#updateStudentButton").on("click", function () {
        var studentId = $("#editStudentId").val();

        var firstName = $("#editFirstName").data("kendoTextBox").value();

        var lastName = $("#editLastName").data("kendoTextBox").value();

        var address = $("#editAddress").data("kendoTextBox").value();

        var dateOfBirth = $("#editDateOfBirth").data("kendoDatePicker").value();

        var courseId = $("#editCourse").data("kendoDropDownList").value();

        if (!firstName || !lastName || !courseId) {
            notification.show("Please fill in all fields.",
            "error"
            );
            return;
        }

        var student = {
            FirstName: firstName,
            LastName: lastName,
            Address: address,
            DateOfBirth: kendo.toString(dateOfBirth, "yyyy-MM-dd"),
            CourseId: parseInt(courseId, 10)
        };

        $.ajax({
            url: apiBaseUrl + "/api/students/" + studentId,
            type: "PUT",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(student),

            success: function (response) {
                notification.show(
                    response.message || "Student updated successfully",
                    "success"
                );

                editWindow.close();
                studentsDataSource.read();
            },

            error: function (xhr) {
                notification.show(
                    xhr.responseJSON?.Message ||
                    "Unable to update student",
                    "error"
                );
            }
        })
    });

    $("#cancelEditButton").kendoButton();

    $("#cancelEditButton").on("click", function () {
        editWindow.close();
    });

    $("#viewStudentReportBtn").on("click", function () {
        var reportUrl = apiBaseUrl.replace(/\/$/, "") +
            "/api/reports/student-details";

        window.open(reportUrl, "_blank");
    });
});