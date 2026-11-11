// api base url
const apiBaseUrl = "https://localhost:44381";

let editingStudentID = null;

let allStudents = [];
let currentPage = 1;
const recordsPerPage = 5;

// run when the page is ready
$(document).ready(function () {
    loadStudents();
    loadCourses();
});

//fetches students
function loadStudents() {
    $.ajax({
        url: apiBaseUrl + "/api/students",
        type: "GET",
        dataType: "json",

        success: function (students) {

            allStudents = students;

            currentPage = 1;

            displayStudents();
            displayPagination();
        },

        error: function (xhr, status, error) {
            console.error("Error loading students:", error);
            showToast("Unable to load students.", "error");
        }
    });
}

function displayStudents() {
    $("#studentTableBody").empty();

    const filteredStudents = getFilteredStudents();

    const startIndex = (currentPage - 1) * recordsPerPage;

    const paginatedStudents = filteredStudents.slice(
        startIndex,
        startIndex + recordsPerPage
    );

    //empty results
    if (paginatedStudents.length === 0) {
        $("#studentTableBody").html(`
            <tr>
                <td colspan="5" class="text-center">
                    No students found
                </td>
            </tr>
        `);
        return;
    }

    $.each(paginatedStudents, function (index, student) {
        let row = `
            <tr>
                <td>${student["Full Name"]}</td>
                <td>${student.Address}</td>
                <td>${student["Date of Birth"] ? student["Date of Birth"].substring(0, 10) : ""}</td>
                <td>${student.Course}</td>
                
                <td>
                    <div class="d-flex justify-content-center gap-2">
                       <button type="button"
                            class="btn btn-sm btn-outline-primary btn-edit"
                            data-id="${student.StudentID}"
                            title="Edit Student">
                            <i class="bi bi-pencil-square"></i>
                        </button>
                        <button type="button"
                                class="btn btn-sm btn-outline-danger btn-delete"
                                data-id="${student.StudentID}"
                                title="Delete Student">
                                <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
        $("#studentTableBody").append(row);
    });
}

function displayPagination() {
    $("#pagination").empty();

    const filteredStudents = getFilteredStudents();

    const totalPages = Math.ceil(
        filteredStudents.length / recordsPerPage
    );

    if (totalPages <= 1) {
        return;
    }

    // prev button
    $("#pagination").append(`
        <li class="page-item ${currentPage === 1 ? "disabled" : ""}">
            <button class="page-link" data-page="${currentPage - 1}">
                Previous
            </button>
        </li>
    `);

    //numbered page buttons
    for (let i = 1; i <= totalPages; i++) {
        $("#pagination").append(`
            <li class="page-item ${currentPage === i ? "active" : ""}">
                <button class="page-link" data-page="${i}">
                    ${i}
                </button>
            </li>
        `);
    }

    //next button
    $("#pagination").append(`
        <li class="page-item ${currentPage === totalPages ? "disabled" : ""}">
        <button class="page-link" data-page="${currentPage + 1}">
            Next
        </button>
        </li>
    `);
}

// handle pagination clicks
$(document).on("click", "#pagination .page-link", function () {
    const page = Number($(this).data("page"));

    const totalPages = Math.ceil(
        getFilteredStudents().length / recordsPerPage
    );

    if (page >= 1 && page <= totalPages) {
        currentPage = page;
        displayStudents();
        displayPagination();
    }
});

// search and filtering
function getFilteredStudents() {

    const searchText = ($("#searchStudent").val() || "")
        .toLowerCase().trim();

    const selectedCourse = $("#filterCourse").val();

    return allStudents.filter(function (student) {

        const fullName = (student["Full Name"] || "").toLowerCase();
        const address = (student.Address || "").toLowerCase();

        const matchesSearch =
            fullName.includes(searchText) ||
            address.includes(searchText);

        const matchesCourse =
            selectedCourse === "0" ||
            String(student.CourseID) === selectedCourse;

        return matchesSearch && matchesCourse;
    });
}


$("#searchStudent, #filterCourse").on("input change", function () {
    currentPage = 1;

    displayStudents();
    displayPagination();
});

// clear search and filter
$("#btnClearFilters").on("click", function () {
    $("#searchStudent").val("");
    $("#filterCourse").val("0");

    currentPage = 1;

    displayStudents();
    displayPagination();
});

function showToast(message, type = "success") {

    const toastElement = document.getElementById("appToast");

    if (!toastElement) {
        console.error("Toast element not found.");
        return;
    }

    toastElement.classList.remove("bg-success", "bg-danger");
    toastElement.classList.add(
        type === "success" ? "bg-success" : "bg-danger"
    );

    $("#toastMessage").text(message);

    const toast = bootstrap.Toast.getOrCreateInstance(toastElement, {
        delay: 3000
    });

    toast.show();
}


// reset button
$("#btnReset").on("click", function () {
    $("#studentForm")[0].reset();

    editingStudentID = null;

    $("#btnSubmit").text("Register Student");
});

// load all courses
function loadCourses() {
    $.ajax({
        url: apiBaseUrl + "/api/courses",
        type: "GET",
        dataType: "json",

        success: function (courses) {
            $("#course").empty();
            $("#filterCourse").empty();

            $("#course").append(
                '<option value="">Select a course</option>'
            );

            $("#filterCourse").append(
                '<option value="0">All Courses</option>'
            );

            $.each(courses, function (index, course) {
                $("#course").append(
                    `<option value="${course.CourseID}">
                        ${course.CourseName}
                    </option>`
                );

                $("#filterCourse").append(
                    `<option value="${course.CourseID}">
                        ${course.CourseName}
                    </option>`
                );

            });
        },

        error: function (xhr, status, error) {
            console.error("Error loading courses:", error);
            showToast("Unable to load courses.", error);
        }
    });
}

// Register or update a student
$(document).on("submit", "#studentForm", function (e) {
    e.preventDefault();

    const student = {
        FirstName: $("#firstName").val(),
        LastName: $("#lastName").val(),
        Address: $("#address").val(),
        DateOfBirth: $("#dateOfBirth").val() || null,
        CourseID: parseInt($("#course").val()),
    };

    let url = apiBaseUrl + "/api/students";
    let method = "POST";

    if (editingStudentID !== null) {
        url += "/" + editingStudentID;
        method = "PUT";
    }

    $.ajax({
        url: url,
        type: method,
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(student),

        success: function (response) {
            showToast(
                response.message || response.Message ||
                "Student saved successfully.",
                "success"
            );

            $("#studentForm")[0].reset();

            editingStudentID = null;

            $("#studentForm").find("button[type='submit']")
                .text("Register Student");

            $("#studentTableBody").empty();
            loadStudents();
        },

        error: function (xhr) {
            console.error(xhr.responseText);
            
            const errorMessage =
                xhr.responseJSON?.Message ||
                xhr.responseJSON?.message ||
                "Unable to save student details";

            showToast(errorMessage, "error");
        }
    });
});

// delete a student
$(document).on("click", ".btn-delete", function () {
    const studentID = $(this).data("id");

    if (!confirm("Are you sure you want to delete this student?")) {
        return;
    }

    $.ajax({
        url: apiBaseUrl + "/api/students/" + studentID,
        type: "DELETE",

        success: function (response) {

            showToast(
                response.message || response.Message ||
                "Student deleted successfully.",
                "success"
            );
            loadStudents();
        },

        error: function (xhr) {
            console.error(xhr.responseText);
            showToast(
                xhr.responseJSON?.Message ||
                "Unable to delete student.",
                "error"
            );
        }
    });
});

// edit a student
$(document).on("click", ".btn-edit", function () {
    const studentID = $(this).data("id");

    $.ajax({
        url: apiBaseUrl + "/api/students/" + studentID,
        type: "GET",
        dataType: "json",

        success: function (student) {
            editingStudentID = studentID;

            $("#firstName").val(student.FirstName);
            $("#lastName").val(student.LastName);
            $("#address").val(student.Address);
            $("#dateOfBirth").val(student.DateOfBirth ? student.DateOfBirth.substring(0, 10) : "");
            $("#course").val(student.CourseId).trigger("change");

            $("#studentForm").find("button[type='submit']")
                .text("Update Student");

            //scrolls to the form
            $("html, body").animate({
                scrollTop: $("#studentForm").offset().top - 100
            }, 500);
        },

        error: function (xhr) {
            console.error("Edit error:",xhr.responseText);
            showToast("Unable to load students.", "error");
        }
    });
});

// generate student report
$("#btnGenerateReport").on("click", function () {
    const reportWindow = window.open("", "_blank");

    if (!reportWindow) {
        alert("Please allow pop-ups to generate the report");
        return;
    }

    $.ajax({
        url: apiBaseUrl + "/api/students",
        type: "GET",
        dataType: "json",

        success: function (students) {
            generateStudentReport(students, reportWindow);
        },

        error: function (xhr) {
            reportWindow.close();
            console.error(xhr.responseText);
            showToast("Unable to generate student report.", "error");
        }
    });
});

function generateStudentReport(students, reportWindow) {

    // Convert special characters to HTML-safe text
    function escapeHtml(value) {
        return String(value ?? "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    // Generate table rows
    let tableRows = "";

    $.each(students, function (index, student) {

        const fullName = student["Full Name"] || "";
        const address = student.Address || "";
        const dateOfBirth = student["Date of Birth"]
            ? student["Date of Birth"].substring(0, 10)
            : "";

        const course = student.Course || "";

        tableRows += `
            <tr>
                <td>${escapeHtml(fullName)}</td>
                <td>${escapeHtml(address)}</td>
                <td>${escapeHtml(dateOfBirth)}</td>
                <td>${escapeHtml(course)}</td>
            </tr>
        `;
    });

    // Current report generation date
    const generatedDate = new Date().toLocaleDateString();

    // Create the report HTML
    const reportHTML = `
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">

            <title>Student Registration Report</title>

            <style>
                body {
                    font-family: Arial, sans-serif;
                    margin: 40px;
                    color: #333;
                }

                .report-header {
                    text-align: center;
                    margin-bottom: 30px;
                }

                .report-header h1 {
                    margin-bottom: 8px;
                    color: #2c5f8a;
                }

                .report-info {
                    margin-bottom: 25px;
                    display: flex;
                    justify-content: space-between;
                }

                table {
                    width: 100%;
                    border-collapse: collapse;
                    border: 1px solid #d6e4f0;
                }

                th, td {
                    border: 1px solid #d6e4f0;
                    padding: 12px;
                    text-align: left;
                }

                th {
                    background-color: #2c5f8a;
                    color: white;
                    font-weight: 600;
                }

                tbody tr:nth-child(even) {
                    background-color: #eef5fb;
                }

                tbody tr:nth-child(odd) {
                    background-color: #ffffff;
                }

                tbody tr:hover {
                    background-color: #dceaf5;
                }

                .print-button {
                    background-color: #2c5f8a;
                    color: white;
                    border: none;
                    padding: 10px 20px;
                    border-radius: 5px;
                    cursor: pointer;
                    margin-bottom: 25px;
                }

                .print-button:hover {
                    background-color: #214969;
                }

                @media print {
                    .print-button {
                        display: none;
                    }

                    /* Repeat table headers on every printed page */
                    thead {
                        display: table-header-group;
                    }

                    tr {
                        page-break-inside: avoid;
                    }

                    /* Preserve table colours when printing */
                    * {
                        -webkit-print-color-adjust: exact !important;
                        print-color-adjust: exact !important;
                    }
                     @page {
                        @bottom-center {
                            content: "Page " counter(page) " of " counter(pages);
                            font-size: 10px;
                            color: #666;
                        }
                    }
                }
            </style>

        </head>

        <body>

            <button class="print-button" onclick="window.print()">
                Print Report
            </button>

            <div class="report-header">
                <h1>Student Registration Report</h1>
                <p>Registered Students Details</p>
            </div>

            <div class="report-info">
                <strong>Total Students: ${students.length}</strong>
                <span>Generated Date: ${escapeHtml(generatedDate)}</span>
            </div>

            <table>
                <thead>
                    <tr>
                        <th>Full Name</th>
                        <th>Address</th>
                        <th>Date of Birth</th>
                        <th>Course</th>
                    </tr>
                </thead>

                <tbody>
                    ${tableRows}
                </tbody>
            </table>

        </body>
        </html>
    `;

    // Display the report in the new tab
    reportWindow.document.open();
    reportWindow.document.write(reportHTML);
    reportWindow.document.close();
}
