using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using QuizeManagement.Models;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using static QuizeManagement.Models.UserModel;

namespace QuizeManagement.Controllers
{
    public class QuizWiseQuestionController : Controller
    {
        public IConfiguration configuration;
        public QuizWiseQuestionController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region Quiz Wise Question List
        public IActionResult QuizWiseQuestionList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_QuizWiseQuestions_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion Quiz List


        #region Quiz Wise Question Add
        public IActionResult QuizWiseQuestionAdd()
        {
            return View();
        }
        #endregion Quiz Wise Question Add

        #region Temporary Quiz Wise Question Add
        public IActionResult QuizWiseQuestionAddTemp(QuizWiseQuestionModel model)
        {
            if (ModelState.IsValid)
            {
                QuizWiseQuestionUserDropDown();
                QuizWiseQuestionQuizDropDown();
                QuizWiseQuestionQuestionDropDown();
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.QuizWiseQuestionsID == 0)
                {
                    command.CommandText = "PR_MST_QuizWiseQuestions_Insert";
                }
                else
                {
                    command.CommandText = "PR_MST_QuizWiseQuestions_Update";
                    command.Parameters.Add("@QuizWiseQuestionsID", SqlDbType.Int).Value = model.QuizWiseQuestionsID;
                }
                command.Parameters.Add("@QuizID", SqlDbType.Int).Value = model.QuizID;
                command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = model.QuestionID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("QuizWiseQuestionList");
            }
            QuizWiseQuestionUserDropDown();
            QuizWiseQuestionQuizDropDown();
            QuizWiseQuestionQuestionDropDown();
            return View("QuizWiseQuestionAddTemp", model);
        }
        #endregion Temporary Quiz Wise Question Add

        #region Quiz Wise Question Edit
        public IActionResult QuizWiseQuestionEdit(int QuizWiseQuestionsID)
        {
            QuizWiseQuestionUserDropDown();
            QuizWiseQuestionQuizDropDown();
            QuizWiseQuestionQuestionDropDown();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            SqlCommand command = Connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_QuizWiseQuestions_SelectByID";
            command.Parameters.AddWithValue("@QuizWiseQuestionsID", QuizWiseQuestionsID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(reader);
            QuizWiseQuestionModel model = new QuizWiseQuestionModel();
            foreach (DataRow row in datatable.Rows)
            {
                model.QuestionID = Convert.ToInt32(@row["QuestionID"]);
                model.QuizID = Convert.ToInt32(@row["QuizID"]);
                model.UserID = Convert.ToInt32(@row["UserID"]);
                model.Created = Convert.ToDateTime(@row["Created"]);
                model.Modified = Convert.ToDateTime(@row["Modified"]);
            }
            return View("QuizWiseQuestionAddTemp", model);
        }
        #endregion Quiz Wise Question Edit

        #region Quiz Wise Question Delete
        public IActionResult QuizWiseQuestionDelete(int QuizWiseQuestionsID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                Console.Write(QuizWiseQuestionsID);
                SqlCommand Command = connection.CreateCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "PR_MST_QuizWiseQuestions_Delete";
                Command.Parameters.Add("@QuizWiseQuestionsID", SqlDbType.Int).Value = QuizWiseQuestionsID;
                Command.ExecuteNonQuery();
                return RedirectToAction("QuizWiseQuestionList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("QuizWiseQuestionList");
        }
        #endregion Quiz Wise Question Delete

        #region Quiz Wise Question User DropDown
        public void QuizWiseQuestionUserDropDown()
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
        #endregion Quiz Wise Question User DropDown

        #region Quiz Wise Question Quiz DropDown
        public void QuizWiseQuestionQuizDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Dropdown_MST_Quiz";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<QuizDropdownModel> list = new List<QuizDropdownModel>();
            foreach (DataRow data in dataTable.Rows)
            {
                QuizDropdownModel model = new QuizDropdownModel();
                model.QuizID = Convert.ToInt32(data["QuizID"]);
                model.QuizName = data["QuizName"].ToString();
                list.Add(model);
            }
            ViewBag.Quiz = list;
        }
        #endregion Quiz Wise Question Quiz DropDown

        #region Quiz Wise Question Question DropDown
        public void QuizWiseQuestionQuestionDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Dropdown_MST_Question";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<QuestionDropdownModel> list = new List<QuestionDropdownModel>();
            foreach (DataRow data in dataTable.Rows)
            {
                QuestionDropdownModel model = new QuestionDropdownModel();
                model.QuestionID = Convert.ToInt32(data["QuestionID"]);
                model.QuestionText = data["QuestionText"].ToString();
                list.Add(model);
            }
            ViewBag.Question = list;
        }
        #endregion Quiz Wise Question Question DropDown
    }
}
