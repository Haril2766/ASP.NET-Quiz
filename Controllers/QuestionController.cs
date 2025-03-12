using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using QuizeManagement.Models;
using static QuizeManagement.Models.UserModel;
using OfficeOpenXml;


namespace QuizeManagement.Controllers
{
    public class QuestionController : Controller
    {
        public IConfiguration configuration;

        public QuestionController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region Question Add

        public IActionResult QuestionSave(QuestionModel model)
        {
            if (ModelState.IsValid)
            {
                QuestionLevelDropDown();
                QuestionUserDropDown();
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if(model.QuestionID == 0)
                {
                    command.CommandText = "PR_MST_Question_Insert";
                }
                else
                {
                    command.CommandText = "PR_MST_Question_Update";
                    command.Parameters.Add("@QuestionID",SqlDbType.Int).Value = model.QuestionID;
                }
                command.Parameters.Add("@QuestionText", SqlDbType.VarChar).Value = model.QuestionText;
                command.Parameters.Add("@OptionA", SqlDbType.VarChar).Value = model.OptionA;
                command.Parameters.Add("@OptionB", SqlDbType.VarChar).Value = model.OptionB;
                command.Parameters.Add("@OptionC", SqlDbType.VarChar).Value = model.OptionC;
                command.Parameters.Add("@OptionD", SqlDbType.VarChar).Value = model.OptionD;
                command.Parameters.Add("@QuestionLevelID", SqlDbType.VarChar).Value = model.QuestionLevelID;
                command.Parameters.Add("@QuestionMarks", SqlDbType.Int).Value = model.QuestionMarks;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;
                command.Parameters.Add("@CorrectOption", SqlDbType.VarChar).Value = model.CorrectOption;
                
                command.ExecuteNonQuery();

                return RedirectToAction("QuestionList");
            }
            else
            {
                QuestionLevelDropDown();
                QuestionUserDropDown();
                return View("QuestionAddEdit", model);
            }
        }
        #endregion Question Add

        #region Question Edit
        public IActionResult QuestionAddEdit(int QuestionID)
        {
            QuestionLevelDropDown();
            QuestionUserDropDown();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            SqlCommand command = Connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_Question_SelectByID";
            command.Parameters.AddWithValue("@QuestionID",QuestionID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(reader);
            QuestionModel model = new QuestionModel();
            foreach (DataRow row in datatable.Rows)
            {
                model.QuestionID = Convert.ToInt32(@row["QuestionID"]);
                model.QuestionText = @row["QuestionText"].ToString();
                model.OptionA = @row["OptionA"].ToString();
                model.OptionB = @row["OptionB"].ToString();
                model.OptionC = @row["OptionC"].ToString();
                model.OptionD = @row["OptionD"].ToString();
                model.QuestionLevel = @row["QuestionLevel"].ToString();
                model.QuestionMarks = Convert.ToInt32(@row["QuestionMarks"]);
                model.CorrectOption = @row["CorrectOption"].ToString();
                model.QuestionLevelID = Convert.ToInt32(@row["QuestionLevelID"]);
                model.UserID = Convert.ToInt32(@row["UserID"]);
            }
            return View("QuestionAddEdit", model);
        }
        #endregion Question Edit

        #region Question level Dropdown
        public void QuestionLevelDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Dropdown_MST_QuestionLevel";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<QuestionLevelDropdownModel> list = new List<QuestionLevelDropdownModel>();
            foreach (DataRow data in dataTable.Rows)
            {
                QuestionLevelDropdownModel model = new QuestionLevelDropdownModel();
                model.QuestionLevelID = Convert.ToInt32(data["QuestionLevelID"]);
                model.QuestionLevel = data["QuestionLevel"].ToString();
                list.Add(model);
            }
            ViewBag.QuestionLevel = list;
        }
        #endregion Question Dropdown

        #region User Dropdown
        public void QuestionUserDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Dropdown_MST_User";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<UserDropdownModel> list = new List<UserDropdownModel>();
            foreach (DataRow data in dataTable.Rows)
            {
                UserDropdownModel model = new UserDropdownModel();
                model.UserID = Convert.ToInt32(data["UserID"]);
                model.UserName = data["UserName"].ToString();
                list.Add(model);
            }
            ViewBag.User = list;
        }
        #endregion User Dropdown

        #region Question List
        public IActionResult QuestionList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_Question_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion Question List

        #region Question Delete
        public IActionResult QuestionDelete(int QuestionID)
        {
            try { 
                    string connectionString = configuration.GetConnectionString("ConnectionString");
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    SqlCommand Command = connection.CreateCommand();
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.CommandText = "PR_MST_Question_Delete";
                    Command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = QuestionID;
                    Command.ExecuteNonQuery();
                    return RedirectToAction("QuestionList");
                }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("QuestionList");
        }
        #endregion Question Delete

        #region Execl Export

        public IActionResult ExportToExcel()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Required for EPPlus

                string connectionString = configuration.GetConnectionString("ConnectionString");

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "PR_MST_Question_SelectAll"; // No parameters needed

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            DataTable data = new DataTable();
                            data.Load(sqlDataReader);

                            using (var package = new ExcelPackage())
                            {
                                var worksheet = package.Workbook.Worksheets.Add("QuizData");

                                // Add headers
                                worksheet.Cells[1, 2].Value = "QuestionID";
                                worksheet.Cells[1, 3].Value = "QuestionText";
                                worksheet.Cells[1, 4].Value = "OptionA";
                                worksheet.Cells[1, 5].Value = "OptionB";
                                worksheet.Cells[1, 6].Value = "OptionC";
                                worksheet.Cells[1, 7].Value = "OptionD";
                                worksheet.Cells[1, 8].Value = "CorrectOption";
                                worksheet.Cells[1, 9].Value = "QuestionMarks";
                                //worksheet.Cells[1, 10].Value = "IsActive";
                                worksheet.Cells[1, 10].Value = "Created";
                                worksheet.Cells[1, 11].Value = "Modified";
                                worksheet.Cells[1, 12].Value = "QuestionLevel";
                                worksheet.Cells[1, 13].Value = "UserName";

                                // Add data
                                int row = 2;
                                foreach (DataRow item in data.Rows)
                                {

                                    worksheet.Cells[row, 2].Value = item["QuestionID"];
                                    worksheet.Cells[row, 3].Value = item["QuestionText"];
                                    worksheet.Cells[row, 4].Value = item["OptionA"];
                                    worksheet.Cells[row, 5].Value = item["OptionB"];
                                    worksheet.Cells[row, 6].Value = item["OptionC"];
                                    worksheet.Cells[row, 7].Value = item["OptionD"];
                                    worksheet.Cells[row, 8].Value = item["CorrectOption"];
                                    worksheet.Cells[row, 9].Value = item["QuestionMarks"];
                                    worksheet.Cells[row, 11].Value = Convert.ToDateTime(item["Created"]).ToString("yyyy-MM-dd");
                                    worksheet.Cells[row, 12].Value = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd");
                                    worksheet.Cells[row, 13].Value = item["QuestionLevel"];
                                    worksheet.Cells[row, 13].Value = item["UserName"];


                                    row++;
                                }

                                // Convert package to stream
                                var stream = new MemoryStream();
                                package.SaveAs(stream);
                                stream.Position = 0;

                                string excelName = $"QuestionData-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting data: " + ex.Message;
                return RedirectToAction("QuestionList");
            }
        }


        #endregion Execl Export
    }
}
