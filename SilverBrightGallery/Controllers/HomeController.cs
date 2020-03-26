using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SilverBrightGallery.Models;

namespace SilverBrightGallery.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home

        [HttpGet]
        public ActionResult Index()
        {
            RegisterUser registerUser = new RegisterUser();
            registerUser.UserName = "Guest User";
            registerUser.Id = 0;
            Session["Registration"] = registerUser;

            DBConn db = new DBConn();
            List<Product> Pro = db.getImages().ToList();
            return View(Pro);
        }
        
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            RegisterUser regUser = new RegisterUser();
            regUser.UserName = form["Name"];
            regUser.Password = form["Password"];

            DBConn db = new DBConn();
            RegisterUser registerUser = db.AuthenticateUser(regUser.UserName, regUser.Password);

            if (registerUser != null)
            {
                TempData["Message"] = "Login Successfull.";
                Session["Registration"] = registerUser;
                return RedirectToAction("Collection", "Home");
            }
            else
            {
                TempData["Message"] = "Login Unsuccessful." + "" + "Please try Again";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAccount(FormCollection form)
        {
            RegisterUser regUser = new RegisterUser();
            regUser.UserName = form["Name"];
            if (string.IsNullOrEmpty(regUser.UserName))
            {
                ModelState.AddModelError("Name", "Name is required");
            }
            regUser.Password = form["Password"];
            if (string.IsNullOrEmpty(regUser.Password))
            {
                ModelState.AddModelError("Password", "Password is required");
            }
            if (ModelState.IsValid)
            {
                DBConn db = new DBConn();
                int result = db.AddUser(regUser);
                if (result == -1)
                {
                    TempData["Message"] = "User already exist.Choose another name.";
                    string message = Convert.ToString(TempData["Message"]);
                    ViewBag.result = message;
                    return View();
                }

                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(regUser);

            }
        }

        [HttpGet]
        public ActionResult GuestUser()
        {
            RegisterUser GuestUser = new RegisterUser();
            GuestUser.UserName = "Guest";
            GuestUser.Id = 0;
            Session["GuestUserDetail"] = GuestUser;
            return View();
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(FormCollection form)
        {
            RegisterUser regUser = new RegisterUser();
            regUser.UserName = form["Name"];
            regUser.Password = form["Password"];

            DBConn db = new DBConn();
            RegisterUser registerUser = db.AuthenticateUser(regUser.UserName, regUser.Password);

            if (registerUser.Id != 0)
            {
                TempData["Message"] = "Login Successfull.";
                Session["Registration"] = registerUser;
                return RedirectToAction("Collection", "Home");
            }
            else
            {
                TempData["Message"] = "Login Unsuccessful." + "" + "Please try Again";
                return RedirectToAction("Index", "Home");
            }
        }

        //!(A + B) = !A & !B
        [HttpGet]
        public ActionResult Collection(string Items)
        {
            String value = "ALL";
            if (Items != null && !Items.Equals(""))
            {
                value = Items;
            }
           
            DBConn db = new DBConn();
            List<Product> pr = db.Pro(value).ToList();
            return View(pr);
        }

        [HttpPost]
        public ActionResult Collection(FormCollection form)
        {
            RegisterUser regUser = new RegisterUser();
            regUser.UserName = form["Name"];
            regUser.Password = form["Password"];

            DBConn db = new DBConn();
            RegisterUser registerUser = db.AuthenticateUser(regUser.UserName, regUser.Password);

            if (registerUser != null)
            {
                TempData["Message"] = "Login Successfull.";
                Session["Registration"] = registerUser;
                return RedirectToAction("Cart", "Home");
            }
            else
            {
                TempData["Message"] = "Login Unsuccessful." + "" + "Please try Again";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ProductDetail(int id)
        {
            DBConn db = new DBConn();
            Product product = db.getProduct(id);
            return View(product);
        }

        //need to add two action in  a function.

        [HttpPost]
        public ActionResult addCart(FormCollection form, int Id)
        {
            DBConn db = new DBConn();
            RegisterUser sessionReg = (RegisterUser)Session["Registration"];
            CartViewModel cartViewModel = null;
            if (sessionReg == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (sessionReg.Id == 0)
                {
                    //Its guest user.
                    //Get cart from session.
                    if (cartViewModel == null)
                    {
                        //If its not in session already, create and put new one.
                        cartViewModel = new CartViewModel();
                        cartViewModel.ListofCartItems = (List<ViewCart>)Session["ListofCartItems"];
                        if (cartViewModel.ListofCartItems == null)
                        {
                            cartViewModel.ListofCartItems = new List<ViewCart>();
                            Session["ListofCartItems"] = cartViewModel.ListofCartItems;
                        }
                    }

                    Product product = db.getProduct(Id);

                    ViewCart addCart = new ViewCart();
                    addCart.Quantity = Convert.ToInt32(form["QUANTITY"].ToString());
                    addCart.Product_Id = Id;
                    addCart.Id = sessionReg.Id;
                    addCart.Photo = product.Photo;
                    addCart.ProductName = product.ProductName;
                    addCart.Price = product.Price;
                    addCart.Total = product.Price * addCart.Quantity;
                    cartViewModel.ListofCartItems.Add(addCart);
                }
                else
                {
                    AddToCart addCart = new AddToCart();
                    addCart.Quantity = Convert.ToInt32(form["QUANTITY"].ToString());
                    addCart.Product_Id = Id;
                    addCart.UserId = sessionReg.Id;
                    db.SaveToCart(addCart);
                }
                return RedirectToAction("Cart", "Home");
            }
        }


        [HttpGet]
        public ActionResult Cart()
        {
            RegisterUser sessionReg = (RegisterUser)Session["Registration"];
            if (sessionReg == null)
            {
                return RedirectToAction("Index", "Home");
            }
            CartViewModel cartViewModel = new CartViewModel();
            if (sessionReg.Id == 0)
            {
                cartViewModel.ListofCartItems = (List<ViewCart>)Session["ListofCartItems"];
            }
            else
            {
                DBConn db = new DBConn();
                cartViewModel.ListofCartItems = db.CartView(sessionReg.Id).ToList();
            }
            List<ViewCart> viewList = cartViewModel.ListofCartItems;

            float sum = 0;
            if (viewList != null)
            {
                cartViewModel.CountCartItems = viewList.Count;
                for (int i = 0; i < viewList.Count; i++)
                {
                    //ViewCart cartItem = viewList[i];
                    //float q = cartItem.Total;
                    //sum = sum + q;
                    sum = sum + viewList[i].Quantity * viewList[i].Price;
                }
            }
            cartViewModel.TotalCost = sum;
            return View(cartViewModel);
        }

        [HttpPost]
        public ActionResult Cart(FormCollection form)
        {
            RegisterUser regUser = new RegisterUser();
            regUser.UserName = form["Name"];
            regUser.Password = form["Password"];

            DBConn db = new DBConn();
            RegisterUser registerUser = db.AuthenticateUser(regUser.UserName, regUser.Password);

            if (registerUser != null)
            {
                TempData["Message"] = "Login Successfull.";
                Session["Registration"] = registerUser;
                return RedirectToAction("Cart", "Home");
            }
            else
            {
                TempData["Message"] = "Login Unsuccessful." + "" + "Please try Again";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]  
        public ActionResult Checkout()
        {
            RegisterUser sessionReg = (RegisterUser)Session["Registration"];
            if (sessionReg == null)
            {
                return RedirectToAction("Index", "Home");
            }
            CheckoutViewModel checkoutViewModel = new CheckoutViewModel();

            if (sessionReg.Id != 0) {
                DBConn db = new DBConn();
                checkoutViewModel.ListofCartItems = db.CartView(sessionReg.Id).ToList();
            }
            else
            {
                checkoutViewModel.ListofCartItems = (List<ViewCart>)Session["ListofCartItems"];
            }

            List<ViewCart> viewList = checkoutViewModel.ListofCartItems;
            float sum = 0;
            if (viewList != null)
            {
                for (int i = 0; i < viewList.Count; i++)
                {
                    sum = sum + viewList[i].Total;
                }
            }
            checkoutViewModel.TotalCost = sum;
            checkoutViewModel.UserName = sessionReg.UserName;     
            return View(checkoutViewModel);
        }

        [HttpPost]
        public ActionResult goToCheckout(FormCollection form)
        {
            if ((RegisterUser)Session["Registration"] != null)
            {
                return RedirectToAction("Checkout", "Home");
            }
            else
            {
                return RedirectToAction("LogIn", "Home");
            }
        }

        public ActionResult PaymentDetail()
        {
            
            return View();
        }

        public ActionResult PaymentDetail1()
        {
            return View();
        }

        public ActionResult Delete(int id )
        {
            DBConn db = new DBConn();
            db.DeleteCart(id);
            return RedirectToAction("Cart", "Home");

        }

        public ActionResult UpdateCart(FormCollection formcollection )
        {
            string[] qty = formcollection.GetValues("quantity");
            string[] pid = formcollection.GetValues("Id");

            RegisterUser sessionReg = (RegisterUser)Session["Registration"];
            if (sessionReg == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (sessionReg.Id == 0)
            {
                List<ViewCart> viewList = (List<ViewCart>)Session["ListofCartItems"];

                foreach (ViewCart v in viewList)
                {
                    //find [v.Product_Id] in array UI->pid[]
                    // and put UI->qty[] in v.Quantity 
                    for (int p = 0; p < pid.Length; p++)
                    {
                        if (v.Product_Id == Int32.Parse(pid[p]))
                        {
                            v.Quantity = Int32.Parse(qty[p]);
                        }
                    }
                }
/*
                viewList.Clear();
                for (int p = 0; p < pid.Length; p++)
                {
                    ViewCart addCart = new ViewCart();
                    addCart.Product_Id = Int32.Parse(pid[p]);
                    addCart.Quantity = Int32.Parse(qty[p]);
                    //addCart.UserId = sessionReg.Id;
                    viewList.Add(addCart);
                }
*/
            }
            else
            {
                DBConn db = new DBConn();
                for (int v = 0; v < qty.Length; v++)
                {
                    AddToCart addCart = new AddToCart();
                    addCart.Product_Id = Int32.Parse(pid[v]);
                    addCart.Quantity = Int32.Parse(qty[v]);
                    addCart.UserId = sessionReg.Id;
                    db.UpdateCart(addCart);
                }
            }          
            return RedirectToAction("Cart", "Home");
        }

        public ActionResult NewthisWeek()
        {
            return View();
        }
        public ActionResult Discount()
        {
            DBConn db = new DBConn();
            List<Product> Pro = db.getDiscountImages().ToList();
            return View(Pro);
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}