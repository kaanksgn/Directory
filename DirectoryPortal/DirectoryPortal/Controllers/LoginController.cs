using DirectoryPortal.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DirectoryPortal.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        // GET: Login
        [HttpPost]
        public string Login([Required]string email, [Required] string password)
        {
            

            if (!ModelState.IsValid)
            {
                return null;
            }


            try
            {

                string strLocalUrl = "http://localhost:50706";

                WebRequest webRequest = WebRequest.Create(strLocalUrl + "/api/Authanticate/Login?email="+ email+"&password="+ password);
                webRequest.Method = "GET";
                webRequest.ContentType = "application/json";

                webRequest.ContentLength = 0;
                WebResponse webResponse = webRequest.GetResponse();

                var serializer = new DataContractJsonSerializer(typeof(AuthorizedDto));


                if (serializer.ReadObject(webResponse.GetResponseStream()) is AuthorizedDto authorizedDto)
                {
                    Session["AuthorizedID"] = authorizedDto.ID.ToString();
                    Session["AuthorizedParentID"] = authorizedDto.ParentID.ToString();
                    Session["AuthorizedName"] = authorizedDto.Name.ToString();
                    Session["AuthorizedToken"] = authorizedDto.Token.ToString();
                    Session["AuthorizedRole"] = authorizedDto.Role.ToString();

                    if (authorizedDto.ImageURL != null && authorizedDto.ImageURL != "")
                        Session["AuthorizedImageURL"] = authorizedDto.ImageURL.ToString();


                    if (authorizedDto.Role == "Admin")
                    {
                        Session["AdminID"] = authorizedDto.ID.ToString();

                        if (Session["AdminID"].ToString() != null && Session["AuthorizedName"].ToString() != null && Session["AuthorizedToken"].ToString() != null) return "Admin/Dashboard";
                    }
                    else if (authorizedDto.Role == "Company")
                    {
                        Session["CompanyID"] = authorizedDto.ID.ToString();

                        if (Session["CompanyID"].ToString() != null && Session["AuthorizedName"].ToString() != null && Session["AuthorizedToken"].ToString() != null) return "Company/Dashboard";
                    }
                    else if (authorizedDto.Role == "Staff")
                    {
                        Session["CompanyID"] = authorizedDto.ParentID.ToString();
                        Session["StaffID"] = authorizedDto.ID.ToString();

                        if (Session["CompanyID"].ToString() != null && Session["StaffID"].ToString() != null && 
                            Session["AuthorizedName"].ToString() != null && Session["AuthorizedToken"].ToString() != null) return "Company/Dashboard";
                    }
                    
                        
                }

            }
            catch (Exception exc)
            {

                return null;
            }

            return null;
        }
    }
}