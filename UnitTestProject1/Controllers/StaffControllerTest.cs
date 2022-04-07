using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project1640;
using Project1640.Controllers;
using Project1640.EF;
using Project1640.Models;
using Xunit;

namespace UnitTestProject1.Controllers
{
    [TestClass]
    public class StaffControllerTest
    {
        [TestInitialize]
        public void Intialize()
        {
            
        }

        [TestMethod]
        public void Index_Staff_NoParameter()
        {
            //Arrage
            StaffController staffcontroller = new StaffController();

            //Action
            ViewResult result = staffcontroller.Index() as ViewResult;

            //Assert
            Assert.AreEqual(result.TempData["PageNo"], 1);
        }
        [TestMethod]
        public void Index_Staff_OneParameter()
        {
            //Arrage
            StaffController staffcontroller = new StaffController();

            //Action
            ViewResult result = staffcontroller.Index(1) as ViewResult;

            //Assert
            Assert.AreEqual(result.TempData["PageNo"], 1);
        }
        [TestMethod]
        public void Index_Staff_TwoParameter()
        {
            //Arrage
            StaffController staffcontroller = new StaffController();

            //Action
            ViewResult result = staffcontroller.Index(1, 1) as ViewResult;

            //Assert
            Assert.IsNotNull(result.Model);
        }
        [TestMethod]
        public void Index_Staff_ThreeParameter()
        {
            //Arrage
            StaffController staffcontroller = new StaffController();

            //Action
            ViewResult result = staffcontroller.Index(1, 1, "2") as ViewResult;

            //Assert
            Assert.IsNotNull(result.Model);
        }
        [TestMethod]
        public void CreateDepartment()
        {
            //Arrange
            var _department = new Department();
            _department.Name = "Business";
            _department.Description = "None!";
            var dbContext = new CMSContext();
            //Act
            var result = dbContext.Department.Add(_department);

            //Assert
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public void CreateCategory()
        {
            //Arrange
            var _category = new Category();
            _category.Name = "Game";
            _category.Description = "None!";
            var dbContext = new CMSContext();
            //Act
            var result = dbContext.Category.Add(_category);

            //Assert
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void ShowCategory()
        {
            //Arrange
            StaffController staffcontroller = new StaffController();

            //ACt
            ViewResult result = staffcontroller.ShowCategory(2) as ViewResult;


            //Assert
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void FindEmailUserByCommentId()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            var result = staffcontroller.FindEmailUserByCommentId(1);

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void FindIdUserByEmail()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            var result = staffcontroller.FindIdUserByEmail(staffcontroller.FindEmailUserByCommentId(1));

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CheckDate()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            var result = staffcontroller.CheckDate();

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TopView()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)staffcontroller.TopLike();

            //Arrange
            
            Assert.AreEqual(redirectResult.RouteValues["Action"], "ViewIdea");
        }
        [TestMethod]
        public void GetList_Staff()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACT
            var result = staffcontroller.getList();

            //Arrange

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void CreateIdea_Staff_MethodGet()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //Act
            ViewResult result = staffcontroller.CreateIdea() as ViewResult;

            //Arrange
            //result.Area.ToString();
        }

        [TestMethod]
        public void CreateIdeaMethodPost()
        {
            var staffController = new StaffController();

            ViewResult result = staffController.CreateIdea() as ViewResult;


        }
        [TestMethod]
        public void ShowComment()
        {
            //Arrange
            StaffController staffcontroller = new StaffController();

            //ACt
            ViewResult result = staffcontroller.ShowComment(2) as ViewResult;


            //Assert
            Assert.IsNotNull(result.Model);
        }
        [TestMethod]
        public async void ShowUser()
        {
            //Arrange
            StaffController staffcontroller = new StaffController();

            //ACt
            ViewResult result = staffcontroller.ShowUser(await staffcontroller.FindIdUserByEmail("staff1@gmail.com")) as ViewResult;


            //Assert
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void LikeMethodGet()
        {
            //Arrange
            StaffController staffcontroller = new StaffController();
            var identity = new GenericIdentity("staff1@gmail.com");
            var context = new Mock<HttpContextBase>();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "82d29734-8de2-4371-88b1-f7ddc9128a01"));
            var principal = new GenericPrincipal(identity, new[] { "user" });
            context.Setup(s => s.User).Returns(principal);



            ViewResult result = staffcontroller.Like() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
       
    }
}
