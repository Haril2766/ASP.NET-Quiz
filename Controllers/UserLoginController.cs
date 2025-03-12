using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using QuizeManagement.Models;

namespace QuizeManagement.Controllers
{
    public class UserLoginController : Controller
    {
            private IConfiguration Configuration;

            public UserLoginController(IConfiguration _configuration)
            {
                Configuration = _configuration;
            }

            public IActionResult Index()
            {
                return View("UserLogin");
            }

            [HttpPost]
            public IActionResult Login(UserLoginModel userLoginModel)
            {
                string ErrorMsg = string.Empty;

                if (string.IsNullOrEmpty(userLoginModel.UserName))
                {
                    ErrorMsg += "User Name is Required";
                }

                if (string.IsNullOrEmpty(userLoginModel.Password))
                {
                    ErrorMsg += "<br/>Password is Required";
                }

                if (ModelState.IsValid)
                {
                    SqlConnection conn = new SqlConnection(this.Configuration.GetConnectionString("ABConnectionString"));
                    conn.Open();

                    SqlCommand objCmd = conn.CreateCommand();

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.CommandText = "PR_MST_User_Login";
                    objCmd.Parameters.AddWithValue("@UserName", userLoginModel.UserName);
                    objCmd.Parameters.AddWithValue("@Password", userLoginModel.Password);

                    SqlDataReader objSDR = objCmd.ExecuteReader();

                    DataTable dtLogin = new DataTable();

                    // Check if Data Reader has Rows or not
                    // if row(s) does not exists that means either username or password or both are invalid.
                    if (!objSDR.HasRows)
                    {
                        TempData["ErrorMessage"] = "Invalid User Name or Password";
                        return RedirectToAction("Index", "UserLogin");
                    }
                    else
                    {
                        dtLogin.Load(objSDR);

                        //Load the retrived data to session through HttpContext.
                        foreach (DataRow dr in dtLogin.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                            HttpContext.Session.SetString("MobileNo", dr["MobileNo"].ToString());
                            HttpContext.Session.SetString("Email", dr["Email"].ToString());
                            HttpContext.Session.SetString("Password", dr["Password"].ToString());
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = ErrorMsg;
                    return RedirectToAction("Index", "SEC_User");
                }
            }

            [HttpPost]
            public IActionResult Logout()
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }

       
        }
    }

