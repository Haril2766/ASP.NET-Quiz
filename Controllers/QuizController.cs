using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using QuizeManagement.Models;
using static QuizeManagement.Models.UserModel;

namespace QuizeManagement.Controllers
{
    public class QuizController : Controller
    {
        public IConfiguration configuration;
        public QuizController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #region Quiz List
        public IActionResult QuizList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_Quiz_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion Quiz List

        #region Quiz Add
        public IActionResult QuizSave(QuizModel model)
        {
            if (ModelState.IsValid)
            {
                QuizUserDropDown();
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.QuizID == 0)
                {
                    command.CommandText = "PR_MST_Quiz_Insert";
                }
                else
                {
                    command.CommandText = "PR_MST_Quiz_Update";
                    command.Parameters.Add("@QuizID", SqlDbType.Int).Value = model.QuizID;
                }
                command.Parameters.Add("@QuizName", SqlDbType.VarChar).Value = model.QuizName;
                command.Parameters.Add("@TotalQuestions", SqlDbType.Int).Value = model.TotalQuestions;
                command.Parameters.Add("@QuizDate", SqlDbType.DateTime).Value = model.QuizDate;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = model. UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("QuizList");
            }
            QuizUserDropDown();
            return View("QuizAddEdit", model);
        }
        #endregion Quiz Add

        #region Quiz Edit
        public IActionResult QuizAddEdit(int QuizID)
        {
            QuizUserDropDown();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            SqlCommand command = Connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_Quiz_SelectByID";
            command.Parameters.AddWithValue("@QuizID", QuizID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(reader);
            QuizModel model = new QuizModel();
            foreach (DataRow row in datatable.Rows)
            {
                model.QuizName = @row["QuizName"].ToString();
                model.TotalQuestions = Convert.ToInt32(@row["TotalQuestions"]);
                model.QuizDate = Convert.ToDateTime(@row["QuizDate"]);
                model.UserID = Convert.ToInt32(@row["UserID"]);
            }
            return View("QuizAddEdit", model);
        }
        #endregion Quiz Edit    

        #region Quiz Delete
        public IActionResult QuizDelete(int QuizID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand Command = connection.CreateCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "PR_MST_Quiz_Delete";
                Command.Parameters.Add("@QuizID", SqlDbType.Int).Value = QuizID;
                Command.ExecuteNonQuery();
                return RedirectToAction("QuizList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("QuizList");
        }

        #endregion Quiz Delete

        #region User Dropdown
        public void QuizUserDropDown()
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
                        sqlCommand.CommandText = "PR_MST_Quiz_SelectAll"; // No parameters needed

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            DataTable data = new DataTable();
                            data.Load(sqlDataReader);

                            using (var package = new ExcelPackage())
                            {
                                var worksheet = package.Workbook.Worksheets.Add("QuizData");

                                // Add headers
                                worksheet.Cells[1, 1].Value = "QuizID";
                                worksheet.Cells[1, 2].Value = "QuizName";
                                worksheet.Cells[1, 3].Value = "TotalQuestions";
                                worksheet.Cells[1, 4].Value = "QuizDate";
                                worksheet.Cells[1, 5].Value = "UserName";
                                worksheet.Cells[1, 6].Value = "Created";
                                worksheet.Cells[1, 7].Value = "Modified";

                                // Add data
                                int row = 2;
                                foreach (DataRow item in data.Rows)
                                {

                                    worksheet.Cells[row, 1].Value = item["QuizID"];
                                    worksheet.Cells[row, 2].Value = item["QuizName"];
                                    worksheet.Cells[row, 3].Value = item["TotalQuestions"];
                                    worksheet.Cells[row, 4].Value = item["QuizDate"];
                                    worksheet.Cells[row, 5].Value = item["UserName"];
                                    //worksheet.Cells[row, 1].Value = item["CityID"];
                                    //worksheet.Cells[row, 2].Value = item["CityName"];
                                    //worksheet.Cells[row, 3].Value = item["STDCode"];
                                    worksheet.Cells[row, 6].Value = Convert.ToDateTime(item["Created"]).ToString("yyyy-MM-dd");
                                    worksheet.Cells[row, 7].Value = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd");

                                    row++;
                                }

                                // Convert package to stream
                                var stream = new MemoryStream();
                                package.SaveAs(stream);
                                stream.Position = 0;

                                string excelName = $"QuizData-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting data: " + ex.Message;
                return RedirectToAction("CityList");
            }
        }


        #endregion Execl Export
    }
}