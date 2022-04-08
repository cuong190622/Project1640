using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        public void Index_Staff_NoParameter_Staff()
        {
            //Arrage
            StaffController staffcontroller = new StaffController();

            //Action
            ViewResult result = staffcontroller.Index() as ViewResult;

            //Assert
            Assert.AreEqual(result.TempData["PageNo"], 1);
        }
        [TestMethod]
        public void Index_Staff_OneParameter_Staff()
        {
            //Arrage
            StaffController staffcontroller = new StaffController();

            //Action
            ViewResult result = staffcontroller.Index(1) as ViewResult;

            //Assert
            Assert.AreEqual(result.TempData["PageNo"], 1);
        }
        [TestMethod]
        public void Index_Staff_TwoParameter_Staff()
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
        public void CreateDepartment_DirectData()
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
        public void CreateCategory_DirectData()
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
        public void ShowCategory_Staff()
        {
            //Arrange
            StaffController staffcontroller = new StaffController();

            //ACt
            ViewResult result = staffcontroller.ShowCategory(2) as ViewResult;


            //Assert
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void FindEmailUserByCommentId_Staff()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            var result = staffcontroller.FindEmailUserByCommentId(1);

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void FindIdUserByEmail_Staff()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            var result = staffcontroller.FindIdUserByEmail(staffcontroller.FindEmailUserByCommentId(1));

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CheckDate_Staff()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            var result = staffcontroller.CheckDate();

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TopView_Staff()
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
/*        [TestMethod]
        public void CreateIdea_Staff_MethodGet()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //Act
            ViewResult result = staffcontroller.CreateIdea() as ViewResult;

            //Arrange
            //result.Area.ToString();
        }*/

/*        [TestMethod]
        public void CreateIdeaMethodPost()
        {
            var staffController = new StaffController();

            ViewResult result = staffController.CreateIdea() as ViewResult;


        }*/
        [TestMethod]
        public void ShowComment_Staff()
        {
            //Arrange
            StaffController staffcontroller = new StaffController();

            //ACt
            ViewResult result = staffcontroller.ShowComment(3) as ViewResult;


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
            var fakeHttpContext = new Mock<HttpContextBase>();
            var fakeIdentity = new GenericIdentity("staff1@gmail.com");
            var principal = new GenericPrincipal(fakeIdentity, null);

            fakeHttpContext.Setup(t => t.User).Returns(principal);
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);

            StaffController _requestController = new StaffController();

            //Set your controller ControllerContext with fake context
            _requestController.ControllerContext = controllerContext.Object;


            ViewResult result = _requestController.Like() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void TopLike_Staff()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)staffcontroller.TopLike();

            //Arrange

            Assert.AreEqual(redirectResult.RouteValues["Action"], "ViewIdea");
        }
        [TestMethod]
        public void LastComment_Staff()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)staffcontroller.LastIdea();

            //Arrange

            Assert.AreEqual(redirectResult.RouteValues["Action"], "ViewIdea");
        }
        [TestMethod]
        public void LastIdea_Staff()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)staffcontroller.LastIdea();

            //Arrange

            Assert.AreEqual(redirectResult.RouteValues["Action"], "ViewIdea");
        }
        [TestMethod]
        public void SendEmail_Staff()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            Task result = (Task)staffcontroller.SendEmail("123456789awdstk.mk@gmail.com", "Test message");

            //Arrange

            Assert.IsNotNull(result.IsCompleted);
        }

        [TestMethod]
        public void CreateCode_Staff()
        {
            //Arrange
            var staffcontroller = new StaffController();

            //ACt
            Task result = (Task)staffcontroller.CreateCode(staffcontroller.FindIdUserByEmail("staff1@gmail.com").ToString());

            //Arrange

            Assert.IsNotNull(result.IsCompleted);

        }
        [TestMethod]
        public void BlockTime_Staff()
        {
            var staffController = new StaffController();

            ViewResult result = staffController.BlockTime() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }

        [TestMethod]
        public void ShowAllCategory_Staff()
        {
            var staffController = new StaffController();

            ViewResult result = staffController.ShowAllCategory() as ViewResult;

            Assert.IsNotNull(result.Model);
        }
        [TestMethod]
        public void Filter_Staff()
        {
            var staffController = new StaffController();

            ViewResult result = staffController.Fiter() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void ValidationPassword_Fail_Staff()
        {
            var staffController = new StaffController();
            staffController.CustomValidationForPassword("abc", "abc", "abc", "941256", staffController.FindIdUserByEmail("staff1@gmail.com").ToString());

            Assert.IsNotNull(staffController.ModelState);
        }

        [TestMethod]
        public void Index_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
        }
/*        [TestMethod]
        public void CreateStaff_MethodGet_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.Createstaff() as ViewResult;

            Assert.IsNotNull(result);
        }*/
        [TestMethod]
        public void CreateStaff_MethodPost_Admin()
        {
            var adminController = new AdminController();

            var user = new UserInfo();
            user.Email = "test@gmail.com";
            user.DepartmentId = 2;
            user.DoB = DateTime.Now.ToString();
            user.PhoneNumber = "0124124214";
            

            Task task = (Task)adminController.Createstaff(user);

            Assert.IsNotNull(task.Id);
        }

        [TestMethod]
        public void ValidationStaffInfo_Fail_Admin()
        {
            var adminController = new AdminController();
            var user = new UserInfo();
            user.Email = null;

            adminController.CustomValidationStaff(user);

            Assert.IsFalse(adminController.ModelState.IsValid);
        }
        [TestMethod]
        public void ValidationStaffInfo_Success_Admin()
        {
            var adminController = new AdminController();
            var user = new UserInfo();
            user.Email = "test@gmail.com";

            adminController.CustomValidationStaff(user);

            Assert.IsTrue(adminController.ModelState.IsValid);
        }
        [TestMethod]
        public void ViewAccount_Staff()
        {
            var adminController = new AdminController();
            var staffController = new StaffController();

            ViewResult result = adminController.ViewAccount(staffController.FindIdUserByEmail("staff1@gmail.com").ToString()) as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void CreateManager_MethodPost_Admin()
        {
            var adminController = new AdminController();

            var user = new UserInfo();
            user.Email = "test@gmail.com";
            user.DepartmentId = 2;
            user.DoB = DateTime.Now.ToString();
            user.PhoneNumber = "0124124214";


            Task task = (Task)adminController.CreateManager(user);

            Assert.IsNotNull(task.Id);
        }
        [TestMethod]
        public void CreateCoor_MethodPost_Admin()
        {
            var adminController = new AdminController();

            var user = new UserInfo();
            user.Email = "test@gmail.com";
            user.DepartmentId = 2;
            user.DoB = DateTime.Now.ToString();
            user.PhoneNumber = "0124124214";


            Task task = (Task)adminController.CreateCoor(user);

            Assert.IsNotNull(task.Id);
        }
        [TestMethod]
        public void ValidationManagerInfo_Fail_Admin()
        {
            var adminController = new AdminController();
            var user = new UserInfo();
            user.Email = null;

            adminController.CustomValidationManager(user);

            Assert.IsFalse(adminController.ModelState.IsValid);
        }
        [TestMethod]
        public void ValidationManagerInfo_Success_Admin()
        {
            var adminController = new AdminController();
            var user = new UserInfo();
            user.Email = "test@gmail.com";

            adminController.CustomValidationManager(user);

            Assert.IsTrue(adminController.ModelState.IsValid);
        }
        [TestMethod]
        public void ValidationCoorInfo_Fail_Admin()
        {
            var adminController = new AdminController();
            var user = new UserInfo();
            user.Email = null;

            adminController.CustomValidationCoor(user);

            Assert.IsFalse(adminController.ModelState.IsValid);
        }
        [TestMethod]
        public void ValidationCoorInfo_Success_Admin()
        {
            var adminController = new AdminController();
            var user = new UserInfo();
            user.Email = "test@gmail.com";

            adminController.CustomValidationCoor(user);

            Assert.IsTrue(adminController.ModelState.IsValid);
        }
        [TestMethod]
        public void GetList_Admin()
        {
            //Arrange
            var admincontroller = new AdminController();

            //ACT
            var result = admincontroller.getList();

            //Arrange

            Assert.IsNotNull(result);
        }
       /* [TestMethod]
        public void EditAccount_Admin()
        {
            var adminController = new AdminController();
            var staffController = new StaffController();
            var context = new CMSContext();
            var store = new UserStore<UserInfo>(context);
            var manager = new UserManager<UserInfo>(store);

            var user = manager.FindByEmail("staff1@gmail.com");
            user.Age += 1;

            ViewResult Result = (ViewResult)adminController.EditAccount("123",user);

            //Arrange

            Assert.IsNotNull(Result.);

        }*/
       
        [TestMethod]
        public void Convert_Return_Existed_Admin()
        {
            var adminController = new AdminController();
            var database = new CMSContext();
            var result = adminController.Convert(database, "2,3");

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void Convert_Return_NotExisted_Admin()
        {
            var adminController = new AdminController();
            var database = new CMSContext();
            var result = adminController.Convert(database, "99,100");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateRole_Success_Admin()
        {
            var adminController = new AdminController();

            Task task = adminController.CreateRole("staff1@gmail.com", "staff");

            Assert.IsFalse(task.IsCompleted);
        }
        [TestMethod]
        public void CreateRole_Fail_Admin()
        {
            var adminController = new AdminController();

            Task task = adminController.CreateRole("staff1@gmail.com", "manager");

            Assert.IsNotNull(task.Id);
        }
        [TestMethod]
        public void IndexDepartment_Staff()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.IndexDepartment() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void CreateDepartment_MethodGet_Staff()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.CreateDepartment() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }

        [TestMethod]
        public void CreateDepartment_MethodPost_Staff()
        {
            var adminController = new AdminController();

            var _department = new Department();
            _department.Id = 99;
            _department.Name = "Test";
            _department.Description = "This is test";

            RedirectToRouteResult result = (RedirectToRouteResult)adminController.CreateDepartment(_department);

            Assert.IsTrue(result.RouteValues.Values.ToList().Contains("IndexDepartment"));
        }

        [TestMethod]
        public void EditDepartment_MethodGet_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.EditDepartment(2) as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void EditCategory_MethodGet_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.EditCategory(2) as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void EditSetdate_MethodGet_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.EditSetDate() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void EditSetdate_MethodPost_Admin()
        {
            var adminController = new AdminController();
            var date = new SetDate();
            date.StartDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            date.EndDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");

            RedirectToRouteResult result = (RedirectToRouteResult)adminController.EditSetDate(date);

            Assert.IsTrue(result.RouteValues.Values.ToList().Contains("Index"));
        }
        [TestMethod]
        public void Top5Like_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.Top5Like() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }

        [TestMethod]
        public void Top5Idea_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.Top5Idea() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }

        [TestMethod]
        public void TopLike_Admin()
        {
            var adminController = new AdminController();

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)adminController.TopLike();

            //Arrange

            Assert.AreEqual(redirectResult.RouteValues["Action"], "ViewIdea");
        }
        [TestMethod]
        public void TopView_Admin()
        {
            var adminController = new AdminController();

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)adminController.TopView();

            //Arrange

            Assert.AreEqual(redirectResult.RouteValues["Action"], "ViewIdea");
        }
        [TestMethod]
        public void EditCategory_MethodPost_Admin()
        {
            var adminController = new AdminController();
            var category = new Category();
            category.Description = "abc";
            category.Id = 2;
            category.Name = "abc";


            RedirectToRouteResult result = (RedirectToRouteResult)adminController.EditCategory(category.Id, category);

            Assert.IsTrue(result.RouteValues.Values.ToList().Contains("IndexCategory"));
        }
        [TestMethod]
        public void EditDepartment_MethodPost_Admin()
        {
            var adminController = new AdminController();
            var department = new Department();
            department.Description = "abc";
            department.Id = 2;
            department.Name = "abc";


            RedirectToRouteResult result = (RedirectToRouteResult)adminController.EditDepartment(department.Id, department);

            Assert.IsTrue(result.RouteValues.Values.ToList().Contains("IndexDepartment"));
        }
      
        [TestMethod]
        public void Filter_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.Filter() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void Filter2_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.Filter2() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void Filter3_Admin()
        {
            var adminController = new AdminController();

            ViewResult result = adminController.Filter3() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void ViewAllIdea_Admin_NoParameter()
        {
            //Arrage
            AdminController adminController = new AdminController();

            //Action
            ViewResult result = adminController.ViewAllIdea() as ViewResult;

            //Assert
            Assert.AreEqual(result.TempData["PageNo"], 1);
        }
        [TestMethod]
        public void ViewAllIdea_Admin_OneParameter()
        {
            //Arrage
            AdminController adminController = new AdminController();

            //Action
            ViewResult result = adminController.ViewAllIdea(1) as ViewResult;

            //Assert
            Assert.AreEqual(result.TempData["PageNo"], 1);
        }
        [TestMethod]
        public void ViewAllIdea_Admin_TwoParameter()
        {
            //Arrage
            AdminController adminController = new AdminController();

            //Action
            ViewResult result = adminController.ViewAllIdea(1, 1) as ViewResult;

            //Assert
            Assert.IsNotNull(result.Model);
        }
        [TestMethod]
        public void ViewAllIdea_Admin_ThreeParameter()
        {
            //Arrage
            AdminController adminController = new AdminController();

            //Action
            ViewResult result = adminController.ViewAllIdea(1, 1, "2") as ViewResult;

            //Assert
            Assert.IsNotNull(result.Model);
        }
        [TestMethod]
        public void ShowComment_Admin()
        {
            //Arrange
            AdminController adminController = new AdminController();

            //ACt
            ViewResult result = adminController.ShowComment(3) as ViewResult;


            //Assert
            Assert.IsNotNull(result.Model);
        }
        [TestMethod]
        public void ShowAllCategory_Admin()
        {
            //Arrange
            AdminController adminController = new AdminController();

            //ACt
            ViewResult result = adminController.ShowAllCategory() as ViewResult;


            //Assert
            Assert.IsNotNull(result.Model);
        }
        [TestMethod]
        public void DeleteIdea_Admin()
        {
            var adminController = new AdminController();
            RedirectToRouteResult result = (RedirectToRouteResult)adminController.DeleteIdea(1);

            Assert.IsTrue(result.RouteValues.Values.ToList().Contains("ViewAllIdea"));
        }
        public void TopIdea_Coor()
        {
            var coorController = new CoorController();

            ViewResult result = coorController.TopView() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void TopLike_Coor()
        {
            var coorController = new CoorController();

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)coorController.TopLike();

            //Arrange

            Assert.AreEqual(redirectResult.RouteValues["Action"], "ViewIdea");
        }
        public void LastComment_Coor()
        {
            //Arrange
            var coorController = new CoorController();

            //ACt
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)coorController.LastIdea();

            //Arrange

            Assert.AreEqual(redirectResult.RouteValues["Action"], "ViewIdea");
        }
        [TestMethod]
        public void LastIdea_Coor()
        {
            //Arrange
            var coorController = new CoorController();

            //ACt
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)coorController.LastIdea();

            //Arrange

            Assert.AreEqual(redirectResult.RouteValues["Action"], "ViewIdea");
        }
        [TestMethod]
        public void CreateCode_Coor()
        {
            //Arrange
            var coorcontroller = new CoorController();

            //ACt
            Task result = (Task)coorcontroller.CreateCode(coorcontroller.FindIdUserByEmail("staff1@gmail.com").ToString());

            //Arrange

            Assert.IsNotNull(result.IsCompleted);

        }
        [TestMethod]
        public void FindIdUserByEmail_Coor()
        {
            //Arrange
            var coorcontroller = new CoorController();

            //ACt
            var result = coorcontroller.FindIdUserByEmail("staff1@gmail.com");

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ShowAllCategory_Coor()
        {
            //Arrange
            var coorcontroller = new CoorController();

            ViewResult result = coorcontroller.ShowAllCategory() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void Filter_Coor()
        {
            //Arrange
            var coorcontroller = new CoorController();

            ViewResult result = coorcontroller.Filter() as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }
        [TestMethod]
        public void ShowComment_Coor()
        {
            //Arrange
            var coorcontroller = new CoorController();

            ViewResult result = coorcontroller.ShowComment(3) as ViewResult;

            Assert.IsNotNull(result.ViewData);
        }

        [TestMethod]
        public void DeleteAccount_Admin()
        {
            var adminController = new AdminController();
            var staffController = new StaffController();


            ViewResult result = adminController.deleteAccount("bb36095e-4f09-4dc9-bee5-b645b60e77f7") as ViewResult;

            Assert.IsNotNull(result.TempData["message"]);
        }

        [TestMethod]
        public void DeleteDepartment_Admin()
        {
            var adminController = new AdminController();

            RedirectToRouteResult result = (RedirectToRouteResult)adminController.DeleteDepartment(1);

            Assert.IsTrue(result.RouteValues.Values.ToList().Contains("IndexDepartment"));
        }
        [TestMethod]
        public void DeleteCategory_Admin()
        {
            var adminController = new AdminController();
            CreateCategory_DirectData();
            RedirectToRouteResult result = (RedirectToRouteResult)adminController.DeleteCategory(1);

            Assert.IsTrue(result.RouteValues.Values.ToList().Contains("IndexCategory"));
        }

    }
}
