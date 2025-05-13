using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace MyProject.Controllers
{
    public class HomeController : Controller
    {
        DBManager db = new DBManager();
        // GET: Home
        public ActionResult Index()
        {
            CartCount();//method to calculate cart items
            string query = "select top 12 * from tbl_category order by cname";
            DataTable category = db.ExecuteSelect(query);
            ViewBag.category = category;

            string query1 = "select top 12 *,((mrp-salerate)*100)/mrp from tbl_product order by((mrp-salerate)*100)/mrp desc";
            DataTable product1 = db.ExecuteSelect(query1);
            ViewBag.product1 = product1;

            string query2 = "select top 12 *,((mrp-salerate)*100)/mrp  from tbl_product order by pid desc";
            DataTable product2 = db.ExecuteSelect(query2);
            ViewBag.product2 = product2;
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Product(int? cid, int? scat)
        {
            string query1, query2;
            if (cid.HasValue)
            {
                if (scat.HasValue)
                {
                    query2 = "select *,((mrp-salerate)*100)/mrp from tbl_product where sub_cid=" + scat;
                }
                else
                {
                    query2 = "select *,((mrp-salerate)*100)/mrp from tbl_product where cid=" + cid;
                }
                query1 = "select * from tbl_subcategory where cid=" + cid;
                query2 = "select *,((mrp-salerate)*100)/mrp from tbl_product where cid=" + cid;
            }
            else
            {
                query1 = "select * from tbl_subcategory";
                query2 = "select *,((mrp-salerate)*100)/mrp from tbl_product";
            }
            ViewBag.subcat = db.ExecuteSelect(query1);
            ViewBag.product1 = db.ExecuteSelect(query2);
            return View();

        }

        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(string emailid, string password)
        {
            string query = "select * from tbl_user where emailid='" + emailid + "'and password='" + password + "'";
            DataTable dt = db.ExecuteSelect(query);
            if (dt.Rows.Count > 0)
            {
                Session["username"] = dt.Rows[0][0];
                Session["useremail"] = dt.Rows[0][1];
                return RedirectToAction("index");
            }
            else
            {
                return Content("<script>alert('Invalid credentials.');location.href='/home/signin'</script>");
            }
        }

        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(string name, string email, long? mobno, string password, string address, string landmark, string pincode)
        {
            string query = "insert into tbl_user values('" + name + "','" + email + "'," + mobno + ",'" + password + "','" + address + "','" + landmark + "','" + pincode + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt") + "') ";
            int result = db.ExecuteInsertUpdateDelet(query);
            if (result > 0)
            {
                return Content("<script>alert('Now! You are a registered user');location.href='/home/SignUp'</script>");
            }
            else
            {
                return Content("<script>alert('Error in registration');location.href='/home/SignUp'</script>");
            }
        }
        public ActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminLogin(string adminid, string password)
        {
            string query = "select * from tbl_adminlogin where adminid='" + adminid + "'and password='" + password + "' ";
            DBManager db = new DBManager();
            DataTable dt = db.ExecuteSelect(query);
            if (dt.Rows.Count > 0)
            {
                Session["admin"] = adminid;
                return RedirectToAction("index", "admin");
            }
            else
            {
                return Content("<script>alert('Id or Password is invalid');location.href='/home/AdminLogin'</script>");
            }
        }
        [CheckUserSession]
        [HttpPost]
        public ActionResult addtocart(int? pid, int? salerate, int? quantity)
        {
            string email = Session["useremail"].ToString();
            string query = "insert into tbl_cart values(" + pid + ",'" + email + "'," + quantity + "," + salerate * quantity + ",'" + DateTime.Now.ToString("yyy-MM-dd hh:mm tt") + "')";
            int result = db.ExecuteInsertUpdateDelet(query);
            if (result > 0)
            {
                return Content("<script>alert('Item added in cart.Explore more products.');location.href='/home/index'</script>");
            }
            else
            {
                return RedirectToAction("index");
            }
        }

        public ActionResult contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(string name, string email, long mobno, string message)
        {
            DBManager dm = new DBManager();
            int x = dm.ExecuteInsertUpdateDelet("isert into tbl_contact values('" + name + "'," + mobno + ",'" + email + "','" + message + "','" + DateTime.Now + "')");
            if (x > 0)
                Response.Write("<script>alert('Thanks for contacting with us...')</script>");
            else
                Response.Write("<script>alert('Do not ')");
            return View();
        }
        [CheckUserSession]
        public ActionResult MyCart()
        {
            if (Session["useremail"] != null)
            {
                string email = Session["useremail"].ToString();
                string query = "select tbl_product.*,tbl_cart.*from tbl_cart left join tbl_product on tbl_cart.pid=tbl_product.pid where tbl_cart.uid='" + email + "'";
                DataTable dt = db.ExecuteSelect(query);
                ViewBag.cart = dt;
                return View();
            }
            else
            {
                return Content("<script>alert('Please try again after some time');location.href='/home/signin'</script>");
            }

        }

        public ActionResult logout()
        {
            Session.RemoveAll();
            return Redirect("~/home/index");
        }
        [CheckUserSession]
        public void CartCount()
        {
            if (Session["useremail"] != null)
            {
                string email = Session["useremail"].ToString();
                string query = "select sum(quantity),sum(total) from tbl_cart where uid='" + email + "'";
                DataTable dt = db.ExecuteSelect(query);
                if (dt.Rows.Count > 0)
                {
                    string cart = dt.Rows[0][0].ToString() + "items<br/>&#8377;" + dt.Rows[0][1].ToString();
                    Session["cart"] = cart;
                }
            }
        }
        public ActionResult deleteitem(int? cid)
        {
            string query = "delete from tbl_cart where cid=" + cid;
            int result = db.ExecuteInsertUpdateDelet(query);
            if (result > 0)
            {
                CartCount();
                return Content("<script>alert('Item deleted');location.href='/home/mycart'</script>");
            }
            else
            {
                return Content("<script>alert('Item not deleted');location.href='/home/mycart'</script>");
            }
        }
        [CheckUserSession]
        public ActionResult myorder()
        {
            if (Session["useremail"] != null)
            {
                string email = Session["useremail"].ToString();

                string query = "select tbl_product.*, tbl_order.* from tbl_order left join tbl_product on tbl_order.pid=tbl_product.pid where tbl_order.uid='" + email + "'";
                DataTable dt = db.ExecuteSelect(query);
                ViewBag.order = dt;
                return View();
            }
            else
            {
                return Content("<script>alert('Please try again after some time');location.href='/home/signin'</script>");
            }

        }
        public ActionResult order()
        {
            string email = Session["useremail"].ToString();
            DateTime today = DateTime.Now;
            string query = "insert into tbl_order select*,'pending','" + today.ToString("yyy-MM-dd hh:mm tt") + "'from tbl_cart where uid='" + email + "'";
            int result = db.ExecuteInsertUpdateDelet(query);
            if(result>0)
            {
                string q2 = "delete from tbl_cart where uid='"+email+"'";
                db.ExecuteInsertUpdateDelet(q2);
                CartCount();
                return Content("<script>alert('Order Successful. You can check status from my order section');location.href='/home/myorder'</script>");
            }
            else
            {
                return Content("<script>alert('Error. Try again');location.href='/home/myorder'</script>");
            }
        }     
        
    }
    class CheckUserSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session["useremail"] == null)
            {
                filterContext.Result = new RedirectToRouteResult
                    (new System.Web.Routing.RouteValueDictionary
                    {
                        {"Controller","home" },
                        {"Action","SignIn" }
                    });
            }
        }

    }

}