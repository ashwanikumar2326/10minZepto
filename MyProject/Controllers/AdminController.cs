using Antlr.Runtime.Misc;
using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Text;

namespace MyProject.Controllers
{
    [CheckSession]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Category()
        {
            DataTable dt = db.ExecuteSelect("select * from tbl_category order by cid desc");
            ViewBag.category = dt;
            return View();
        }
        DBManager db = new DBManager();
        [HttpPost]
        public ActionResult Category(string catname,HttpPostedFileBase caticon)
        {
            if(catname!=null && caticon!=null)
            {
                string query = "insert into tbl_category values('"+catname+"','"+caticon.FileName+"')";
                int result = db.ExecuteInsertUpdateDelet(query);
                if(result>0)
                {
                    caticon.SaveAs(Server.MapPath("/Content/caticons/") + caticon.FileName);
                    return Content("<script>alert('Data Added');location.href='/Admin/Category'</script>");
                }
                else
                {
                    return Content("<script>alert('Data not Added');location.href='/Admin/category'</script>");
                }
            }
            else
            {
                return Content("<script>alert('File all field properly');location.href='/Admin/category'</script>");
            }
            
        }
        public ActionResult Subcategory()
        {
            string query = "select * from tbl_category order by cname";
            DataTable dt = db.ExecuteSelect(query);
            ViewBag.subcat = dt;

            //select all sub category from table
            string command = "select sc.sid,c.cname, sc.subcat_name, sc.subcat_pic from tbl_subcategory sc left join tbl_category c on c.cid=sc.cid";
            DataTable subcategory = db.ExecuteSelect(command);
            ViewBag.subcategory = subcategory;
            return View();
        }
        [HttpPost]
        public ActionResult Subcategory(int category, string subcatname, HttpPostedFileBase subcaticon)
        {
            string query = "insert into tbl_subcategory values("+category+",'"+subcatname+"','"+subcaticon.FileName+"')";
            int result = db.ExecuteInsertUpdateDelet(query);
            if (result > 0)
            {
                subcaticon.SaveAs(Server.MapPath("/Content/subcatpic/") + subcaticon.FileName);
                return Content("<script>alert('Data Added');location.href='/Admin/subcategory'</script>");
            }
            else
            {
                return Content("<script>alert('Data not Added');location.href='/Admin/subcategory'</script>");
            }

        }

        public ActionResult Product()
        {
            string query = "select * from tbl_category order by cname";
            DataTable category = db.ExecuteSelect(query);
            ViewBag.category = category;

            string command = "select * from tbl_subcategory order by subcat_name";
            DataTable subcat = db.ExecuteSelect(command);
            ViewBag.subcat = subcat;

            string product = "select * from tbl_product order by pid desc";
            DataTable productdt = db.ExecuteSelect(product);
            ViewBag.product = productdt;
            return View();
        }
        //add product data in database
        [HttpPost]
        public ActionResult product(int? pcat,int? psubcat,string pname,string pdesc, string pmodel, string psize, int psalerate, int pmrp,HttpPostedFileBase picon)
        {
            string query = "insert into tbl_product values("+pcat+","+psubcat+",'"+pname+"','"+pdesc+"','"+pmodel+"',"+pmrp+","+psalerate+",'"+psize+"','"+picon.FileName+"','"+DateTime.Now.ToString("yyyy-MM-dd")+"')";
            int result = db.ExecuteInsertUpdateDelet(query);
            if(result>0)
            {
                picon.SaveAs(Server.MapPath("/Content/productpic/") + picon.FileName);
                return Content("<script>alert('Product Added');location.href='/Admin/product/'</script>");
            }
            else
            {
                return Content("<script>alert('Product Not Added');location.href='/Admin/product/'</script>");

            }
        }
        public ActionResult ProductManagement()
        {
            return View();
        }
        public ActionResult UserManagement()
        {
            string query = "select * from tbl_user order by regdate desc";
            DataTable dt = db.ExecuteSelect(query);
            ViewBag.Data = dt;
            return View();
        }
        public ActionResult CustomerSupport()
        {
            string query = "select * from tbl_contact order by sr desc";
            DataTable dt = db.ExecuteSelect(query);
            ViewBag.Data = dt;
            return View();
        }
        public ActionResult OrderManagement()
        {
            string query = "select o.*, p.title, u.name from tbl_order o left join tbl_product p on o.pid=p.pid left join tbl_user u on o.uid=u.email";
            DataTable dt = db.ExecuteSelect(query);
            ViewBag.Data = dt;
            return View();
        }
        public ActionResult ChangePassword()
        {
           
            return View();
        }
        public ActionResult Logout()
        {
            Session.Remove("admin");
            return RedirectToAction("adminlogin", "home");
           
        }
    }
    //Filter to check session before executing any action method of adminzone
    class CheckSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session["admin"]==null)
            {
                filterContext.Result = new RedirectToRouteResult
                    (new System.Web.Routing.RouteValueDictionary
                    {
                        {"Controller","home" },
                        {"Action","adminlogin" }
                    });
            }
        }

    }
}