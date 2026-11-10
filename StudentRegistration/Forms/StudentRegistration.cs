using StudentRegistration.BusinessLogic.Services;
using StudentRegistration.DataAccess.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentRegistration
{
    public partial class StudentRegistration : Form
    {
        private int selectedStudentID = 0;
        private readonly StudentService studentService;
        private readonly CourseService courseService;
        public StudentRegistration(StudentService studentService, CourseService courseService)
        {
            this.studentService = studentService;
            this.courseService = courseService;
            InitializeComponent();
        }

        private void StudentRegistration_Load(object sender, EventArgs e)
        {
            LoadCourses();
            LoadStudents();
            buttonSave.Text = "Save Student";
        }

        private void LoadCourses()
        {
            comboBoxCourse.DataSource = courseService.GetAllCourses();
            comboBoxCourse.DisplayMember = "CourseName";
            comboBoxCourse.ValueMember = "CourseID";
            comboBoxCourse.SelectedIndex = -1; // No selection by default
        }

        private void LoadStudents()
        {
            dataGridViewStudentDetails.RowTemplate.Height = 30;
            dataGridViewStudentDetails.ColumnHeadersHeightSizeMode =
    DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewStudentDetails.ColumnHeadersHeight = 30;

            dataGridViewStudentDetails.EnableHeadersVisualStyles = false;

            dataGridViewStudentDetails.ColumnHeadersDefaultCellStyle.BackColor =
                Color.AliceBlue;

            dataGridViewStudentDetails.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.AliceBlue;
            dataGridViewStudentDetails.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridViewStudentDetails.DataSource = studentService.GetAllStudents();

            dataGridViewStudentDetails.Columns["StudentID"].Visible = false;

            dataGridViewStudentDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewStudentDetails.ReadOnly = true;
            dataGridViewStudentDetails.AllowUserToAddRows = false;
            dataGridViewStudentDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (!dataGridViewStudentDetails.Columns.Contains("Delete"))
            {
                DataGridViewImageColumn deleteColumn =
                    new DataGridViewImageColumn();

                deleteColumn.Name = "Delete";
                deleteColumn.HeaderText = "Action";
                deleteColumn.Image = Properties.Resources.deleteIcon;
                deleteColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                deleteColumn.Width = 50;
                deleteColumn.ReadOnly = true;
                deleteColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                dataGridViewStudentDetails.Columns.Add(deleteColumn);
            }
        }

        private void dataGridViewStudentDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dataGridViewStudentDetails.Columns[e.ColumnIndex].Name != "Delete")
                return;

            int studentID = Convert.ToInt32(
                dataGridViewStudentDetails.Rows[e.RowIndex]
                .Cells["StudentID"].Value
            );

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this student?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                studentService.DeleteStudent(studentID);

                MessageBox.Show(
                    "Student deleted successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadStudents();
                ClearFields();
            }
        }

        private void ClearFields()
        {
            textBoxFirstName.Clear();
            textBoxLastName.Clear();
            textBoxAddress.Clear();

            dateTimePickerDateOfBirth.Value = DateTime.Today;
            comboBoxCourse.SelectedIndex = -1;

            textBoxFirstName.Focus();
            selectedStudentID = 0;

            dataGridViewStudentDetails.ClearSelection();
            buttonSave.Text = "Save Student";
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void buttonGenerateReport_Click_(object sender, EventArgs e)
        {
            StudentReportForm reportForm = new StudentReportForm(studentService);

            reportForm.ShowDialog();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                Student student = new Student();
                {
                    student.FirstName = textBoxFirstName.Text.Trim();
                    student.LastName = textBoxLastName.Text.Trim();
                    student.Address = textBoxAddress.Text.Trim();
                    student.DateOfBirth = dateTimePickerDateOfBirth.Checked
                            ? dateTimePickerDateOfBirth.Value.Date : (DateTime?)null;
                    student.CourseId = comboBoxCourse.SelectedValue == null ? 0 : Convert.ToInt32(comboBoxCourse.SelectedValue);
                };

                if (selectedStudentID == 0)
                {
                    studentService.RegisterStudent(student);

                    MessageBox.Show(
                    "Student registered successfully!",
                    "Sucess",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                    );
                }

                else
                {
                    student.StudentId = selectedStudentID;
                    studentService.UpdateStudent(student);

                    MessageBox.Show(
                    "Student updated successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                    );
                }

                LoadStudents();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void dataGridViewStudentDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore header clicks and invalid cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            // Ignore the Delete column
            if (dataGridViewStudentDetails.Columns[e.ColumnIndex].Name == "Delete")
                return;

            // Check whether the StudentID column exists
            if (!dataGridViewStudentDetails.Columns.Contains("StudentID"))
                return;

            object studentIdValue = dataGridViewStudentDetails.Rows[e.RowIndex]
                .Cells["StudentID"].Value;

            if (studentIdValue == null || studentIdValue == DBNull.Value)
                return;

            int selectedID = Convert.ToInt32(studentIdValue);

            Student student = studentService.GetStudentByID(selectedID);

            if (student == null)
                return;

            selectedStudentID = student.StudentId;

            textBoxFirstName.Text = student.FirstName;
            textBoxLastName.Text = student.LastName;
            textBoxAddress.Text = student.Address;

            if (student.DateOfBirth.HasValue)
            {
                dateTimePickerDateOfBirth.Value = student.DateOfBirth.Value;
                dateTimePickerDateOfBirth.Checked = true;
            }
            else
            {
                dateTimePickerDateOfBirth.Checked = false;
            }

            comboBoxCourse.SelectedValue = student.CourseId;

            buttonSave.Text = "Update Student";
        }
    }
}
