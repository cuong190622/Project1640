using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project1640;
using Project1640.Controllers;
using Project1640.EF;
using Project1640.Models;

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
        public void Index_OneParameter()
        {
            //Arrage
            StaffController staffcontroller = new StaffController();

            //Action
            ViewResult result = staffcontroller.Index(1) as ViewResult;

            //Assert
            Assert.AreEqual(result.TempData["PageMax"], 1);
        }
        [TestMethod]
        public void Index_TwoParameter()
        {
            //Arrage
            StaffController staffcontroller = new StaffController();

            //Action
            ViewResult result = staffcontroller.Index(1, 1) as ViewResult;

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


    }
}
