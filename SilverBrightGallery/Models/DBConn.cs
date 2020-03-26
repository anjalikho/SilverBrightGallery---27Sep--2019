using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data;
using System.Configuration;


namespace SilverBrightGallery.Models
{
    public class DBConn
    {
        //
        /// <summary>
        /// Returns all products of category "Items"
        /// Return everything when Items=ALL
         

        public int AddUser(RegisterUser regUser)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRegUser", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paraFname = new SqlParameter();
                paraFname.ParameterName = "@UserName";
                paraFname.Value = regUser.UserName;
                cmd.Parameters.Add(paraFname);

                SqlParameter paraPassword = new SqlParameter();
                paraPassword.ParameterName = "@Password";
                paraPassword.Value = regUser.Password;
                cmd.Parameters.Add(paraPassword);

                SqlParameter paraResult = new SqlParameter();
                paraResult.ParameterName = "@ReturnCode";
                paraResult.SqlDbType = SqlDbType.Int;
                paraResult.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraResult);

                con.Open();
                cmd.ExecuteNonQuery();
                int result = (int)paraResult.Value;

                return result;

            }
        }
        public RegisterUser AuthenticateUser(string name, string password)
        {
            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spAuthenticateUser", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paraName = new SqlParameter("@UserName", name);
                SqlParameter paraPassword = new SqlParameter("@Password", password);

                cmd.Parameters.Add(paraName);
                cmd.Parameters.Add(paraPassword);

                con.Open();
                int UserId = (int)cmd.ExecuteScalar();

                if (UserId == -1)
                {
                    return null;
                }

                RegisterUser reg = new RegisterUser();
                reg.UserName = name;
                reg.Password = password;
                reg.Id = UserId;

                return reg;
            }

        }

        public IEnumerable<Product> Pro(string Items)
        {
            string spName = "spGetItem";
            if (Items == "ALL")
            {
                spName = "spGetCollectionImages";
            }
           else if(Items!="ALL")
            {
                spName = "spGetCategoryManufacturingImages";
            }
           
            string Connectionstring = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            List<Product> pro = new List<Product>();

            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                if (Items == "ALL")
                {
                    //SqlParameter paraItem = new SqlParameter();
                    //paraItem.ParameterName = "@Product_Categorey";
                    //paraItem.Value = Items;
                    //cmd.Parameters.Add(paraItem);
                }
                if(Items != "ALL")
                {
                    SqlParameter paraItem = new SqlParameter();
                    paraItem.ParameterName = "@Product_Category";
                    paraItem.Value = Items;
                    cmd.Parameters.Add(paraItem);                
                }

                if(Items == "MANUFACTURING_DATE")
                {
                    SqlParameter paraItem = new SqlParameter();
                    paraItem.ParameterName = "@ManufacturingDate";
                    paraItem.Value = Items;
                    cmd.Parameters.Add(paraItem);

                }

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Product pr = new Product();
                    pr.ID = Convert.ToInt32(rdr["Id"].ToString());
                    //pr.Items = rdr["Items"].ToString();
                    //pr.Period = rdr["Period"].ToString();
                    pr.Photo = rdr["PHOTO"].ToString();
                    pr.Price = Convert.ToInt32(rdr["Price"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("PRODUCT_DISCOUNT")))
                    {
                        pr.Discount = (float)Convert.ToDouble(rdr["PRODUCT_DISCOUNT"]);
                    }
                    pro.Add(pr);
                }
                return (pro);
            }
        }

        //RETURN IMAGES from the DB

        public IEnumerable<Product> getImages()
        {
            string Connectionstring = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            List<Product> pro = new List<Product>();
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                SqlCommand cmd = new SqlCommand("spGetImages", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Product pr = new Product();
                    pr.Photo = rdr["PHOTO"].ToString();
                    pr.ID = Convert.ToInt32(rdr["ID"].ToString());
                    pr.Categorey = rdr["PRODUCT_CATEGORY"].ToString();
                    pr.Price = Convert.ToInt32(rdr["PRICE"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("PRODUCT_DISCOUNT")))
                    {
                        pr.Discount = (float)Convert.ToDouble(rdr["PRODUCT_DISCOUNT"]);
                    }
                    pro.Add(pr);
                }
                return pro;
            }
        }

        public IEnumerable<Product>getDiscountImages()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            List<Product> pro = new List<Product>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDiscountItems",con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Product pr = new Product();
                    pr.Photo = rdr["PHOTO"].ToString();
                    pr.ID = Convert.ToInt32(rdr["ID"].ToString());
                    pr.Categorey = rdr["PRODUCT_CATEGORY"].ToString();
                    pr.Price = Convert.ToInt32(rdr["PRICE"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("PRODUCT_DISCOUNT")))
                    {
                        pr.Discount = (float)Convert.ToDouble(rdr["PRODUCT_DISCOUNT"]);
                    }
                    pro.Add(pr);
                }
                return pro;
            }           
        }

        public Product getProduct(int Id)
        {
            Product product = new Product();

            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetProductDetail", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlParameter paraId = new SqlParameter();
                paraId.ParameterName = "@ID";
                paraId.Value = Id;
                cmd.Parameters.Add(paraId);


                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    product.ID = Convert.ToInt32(rdr["ID"].ToString());
                    product.Price = Convert.ToInt32(rdr["Price"].ToString());
                    product.ProductName = rdr["PRODUCT_NAME"].ToString();
                    product.ProductDescription = rdr["PRODUCT_DESCRIPTION"].ToString();
                    product.ManufacturingDate = rdr["Manufacturing_Date"].ToString();
                    product.Photo = rdr["Photo"].ToString();
                }
            }

            return product;
        }

        public void SaveToCart(AddToCart cartItem)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveCart", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paraProductId = new SqlParameter();
                paraProductId.ParameterName = "PRODUCT_ID";
                paraProductId.Value = cartItem.Product_Id;
                cmd.Parameters.Add(paraProductId);

                SqlParameter paraQuantity = new SqlParameter();
                paraQuantity.ParameterName = "QUANTITY";
                paraQuantity.Value = cartItem.Quantity;
                cmd.Parameters.Add(paraQuantity);

                SqlParameter paraUserId = new SqlParameter();
                paraUserId.ParameterName = "@UserId";
                paraUserId.Value = cartItem.UserId;
                cmd.Parameters.Add(paraUserId);

                con.Open();
                cmd.ExecuteNonQuery();

            }
        }

        public void UpdateCart(AddToCart cartItem)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateCartQuantity", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paraProductId = new SqlParameter();
                paraProductId.ParameterName = "ProductId";
                paraProductId.Value = cartItem.Product_Id;
                cmd.Parameters.Add(paraProductId);

                SqlParameter paraQuantity = new SqlParameter();
                paraQuantity.ParameterName = "qunatity";
                paraQuantity.Value = cartItem.Quantity;
                cmd.Parameters.Add(paraQuantity);

                SqlParameter paraUserId = new SqlParameter();
                paraUserId.ParameterName = "userid";
                paraUserId.Value = cartItem.UserId;
                cmd.Parameters.Add(paraUserId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<ViewCart> CartView(int userid)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            List<ViewCart> cart = new List<ViewCart>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetCart", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paraId = new SqlParameter();
                paraId.ParameterName = "@UserId";
                paraId.Value = userid;
                cmd.Parameters.Add(paraId);

                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ViewCart cartProduct = new ViewCart();
                    cartProduct.Id = Convert.ToInt32(rdr["Id"]);
                    cartProduct.Photo = rdr["Photo"].ToString();
                    cartProduct.Price = Convert.ToInt32(rdr["Price"].ToString());
                    cartProduct.Quantity = Convert.ToInt32(rdr["Quantity"].ToString());
                    cartProduct.Total = (cartProduct.Price) * (cartProduct.Quantity);
                    cartProduct.ProductName = rdr["Product_Name"].ToString();
                    cartProduct.Product_Id = cartProduct.Id;
                    cart.Add(cartProduct);
                }
                return (cart);

            }

        }


        public void DeleteCart(int id)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeletefromCart", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paraId = new SqlParameter();
                paraId.ParameterName = "@ID";
                paraId.Value = id;
                cmd.Parameters.Add(paraId);

                con.Open();
                cmd.ExecuteNonQuery();

            }
        }

        //public RegisterUser getUserDetail(int id)
        //{
        //    RegisterUser regUser = new RegisterUser();

        //    string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        //    using (SqlConnection con = new SqlConnection(ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spGetRegisterUser", con);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        con.Open();

        //        SqlParameter paraId = new SqlParameter();
        //        paraId.ParameterName = "@UserId";
        //        paraId.Value = id;
        //        cmd.Parameters.Add(paraId);


        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            regUser.UserName = rdr["Name"].ToString();

        //        }
        //        return (regUser);

        //    }
        //}

    }
}
       
    

