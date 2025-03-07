using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using QuizeManagement.Models;

namespace QuizeManagement.Controllers
{
    public class UsersController : Controller
    {
        public IConfiguration configuration;

        public UsersController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult UserAddEdit(int UserID)
        {

            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            SqlCommand command = Connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_User_SelectByID";
            command.Parameters.AddWithValue("@UserID", UserID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(reader);
            UserModel model = new UserModel();
            foreach (DataRow row in datatable.Rows)
            {
                model.UserID = Convert.ToInt32(row["ID"]);
                model.UserName = @row["UserName"].ToString();
                //model.QuestionID = Convert.ToInt32(@row["QuestionID"]);
                //model.QuestionText = @row["QuestionText"].ToString();
                //model.OptionA = @row["OptionA"].ToString();
                //model.OptionB = @row["OptionB"].ToString();
                //model.OptionC = @row["OptionC"].ToString();
                //model.OptionD = @row["OptionD"].ToString();
                //model.QuestionLevel = @row["QuestionLevel"].ToString();
                //model.QuestionMarks = Convert.ToInt32(@row["QuestionMarks"]);
                //model.CorrectOption = @row["CorrectOption"].ToString();
                //model.QuestionLevelID = Convert.ToInt32(@row["QuestionLevelID"]);
                //model.UserID = Convert.ToInt32(@row["UserID"]);
            }
            return View("UserAddEdit", model);
        }

        #region User List
        public IActionResult UsersView()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_User_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion User List

        #region User Register
        public IActionResult Register(UserModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_MST_User_Insert";
                command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = model.UserName;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = model.Email;
                command.Parameters.Add("@Password", SqlDbType.VarChar).Value = model.Password;
                command.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = model.Mobile;
                command.ExecuteNonQuery();

                return RedirectToAction("DashboardView", "Dashboard");
            }
            return View("Register");
        }

        #endregion User Register

        #region User Login
        public IActionResult Login(UserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand command = sqlConnection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_MST_User_Login";
                    command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = model.UserName;
                    command.Parameters.Add("@Password", SqlDbType.VarChar).Value = model.Password;
                    SqlDataReader sqlDataReader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        }
                        return RedirectToAction("DashboardView", "Dashboard");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Users");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return View("Login");
        }

        #endregion User Login

        #region User Delete
        public IActionResult UserDelete(int UserID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand Command = connection.CreateCommand();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "PR_MST_User_Delete";
                Command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                Command.ExecuteNonQuery();
                return RedirectToAction("UserView");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("UsersView");
        }
        #endregion User Delete
    }
}
