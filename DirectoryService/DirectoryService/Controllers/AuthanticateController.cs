using DirectoryService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Http;

namespace DirectoryService.Controllers
{
    [Authorize]
    public class AuthanticateController : ApiController
    {

        private readonly Model1Container context;
        public AuthanticateController() { this.context = new Model1Container(); }

        // GET api/values/5
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Register")]
        public HttpResponseMessage Register([Required]string name, [Required]string email, [Required]string password, [Required]string phone)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.InternalServerError, "Model state is not valid.");
            
            try
            {
                Company company = (from c in context.Company where c.Email == email select c).FirstOrDefault();

                

                if (company == null)
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePassword(password, out passwordHash, out passwordSalt);
                    if ((passwordHash != null || passwordSalt != null))
                    {
                        Company newCompany = new Company()
                        {
                            Name = name,
                            Email = email,
                            Phone = phone,
                            Status = 1,
                            CreatorUID = 0,
                            CreatorIP = "::1",
                            CreatorRole = "Company",
                            CreationDate = DateTime.Now,
                            PasswordHash = passwordHash,
                            PasswordSalt = passwordSalt,

                        };
                        context.Company.Add(newCompany);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception exc)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exc.Message);
            }


            return Request.CreateResponse(HttpStatusCode.Accepted);
        }


        private void CreatePassword(string passw, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passw));
            }

        }
        // POST api/values
            [AllowAnonymous]
            [HttpPost]
            [ActionName("Login")]
        public void Login([Required]string email, [Required]string password)
        {
            string strLocalUrl = "";

            WebRequest webRequest = WebRequest.Create("/token");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            byte[] byteBody = new ASCIIEncoding().GetBytes("grant_type=password=&username=EmailEx&password=PasswordEx");

            webRequest.ContentLength = byteBody.Length;
            webRequest.GetRequestStream().Write(byteBody, 0, byteBody.Length);

            webRequest.GetRequestStream().Close();

            var serializer = new DataContractJsonSerializer(typeof());

        }
    }
}
