using DirectoryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public HttpResponseMessage Register(string name, string email, string password, string phone)
        {

            try
            {
                Company company = (from c in context.Company where c.Email == email select c).FirstOrDefault();

                byte[] passwordHash, passworSalt;

                if (company == null)
                {
                    CreatePassword(password, out passwordHash, out passworSalt);
                    if ((passwordHash != null || passworSalt != null))
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
                            PasswordSalt = passworSalt,

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
        public void Login([FromBody]string value)
        {
            Company company = new Company();
            company.Name = value;

            context.Company.Add(company);
            context.SaveChanges();


        }
    }
}
