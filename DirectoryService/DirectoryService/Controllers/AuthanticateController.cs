﻿using DirectoryService.Dtos;
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

        // GET api/Authanticate/Login?
        [AllowAnonymous]
        [HttpGet]
        [ActionName("Login")]
        public AuthorizedDto Login([Required]string email, [Required]string password)
        {

            if (!ModelState.IsValid) return null;

            try
            {
                Admin admin = (from c in context.Admin.AsNoTracking() where c.Email == email select c).FirstOrDefault();
                Company company = (from c in context.Company.AsNoTracking() where c.Email == email select c).FirstOrDefault();
                CompanyStaff staff = (from c in context.CompanyStaff.AsNoTracking() where c.Email == email select c).FirstOrDefault();

                if (admin == null && company == null && staff == null) return null;

                string strLocalUrl = "http://localhost:50706";

                WebRequest webRequest = WebRequest.Create(strLocalUrl + "/token");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                byte[] byteBody = new ASCIIEncoding().GetBytes("grant_type=password&username=" + email + "&password=" + password);

                webRequest.ContentLength = byteBody.Length;
                webRequest.GetRequestStream().Write(byteBody, 0, byteBody.Length);

                webRequest.GetRequestStream().Close();

                WebResponse webResponse = webRequest.GetResponse();

                var serializer = new DataContractJsonSerializer(typeof(AuthTokenDto));

                AuthTokenDto authTokenDto = serializer.ReadObject(webResponse.GetResponseStream()) as AuthTokenDto;

                if (admin != null)
                {
                    return new AuthorizedDto()
                    {
                        ID = admin.Id,
                        Name = admin.Name,
                        Email = admin.Email,
                        ImageURL = admin.ImageURL,
                        Role = "Admin",
                        Token = authTokenDto.access_token
                    };
                }
                else if (company != null)
                {
                    return new AuthorizedDto()
                    {
                        ID = company.Id,
                        Name = company.Name,
                        Email = company.Email,
                        ImageURL = company.ImageURL,
                        Role = "Company",
                        Token = authTokenDto.access_token
                    };
                }
                else if (staff != null)
                {
                    return new AuthorizedDto()
                    {
                        ID = staff.Id,
                        ParentID = staff.CompanyId,
                        Name = staff.Name,
                        Email = staff.Email,
                        ImageURL = staff.ImageURL,
                        Role = "Staff",
                        Token = authTokenDto.access_token
                    };

                }
                else { return null; }


            }
            catch (Exception)
            {

                return null;
            }




        }


        // GET api/Authanticate/Register?
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Register")]
        public HttpResponseMessage Register([Required]string name, [Required]string email, [Required]string password, [Required]string phone)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.InternalServerError, "Model state is not valid.");

            try
            {
                Company company = (from c in context.Company where c.Email == email select c).FirstOrDefault();


                if (company != null)
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Email is already exist!");
               
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

    }
}
