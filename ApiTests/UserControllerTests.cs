using AutoMapper;
using Internship.Controllers;
using Internship.Models;
using Internship.Profiles;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace ApiTests
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly UserController _controller;
        public UserControllerTests()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg => 
            { 
                cfg.AddProfile(new UserProfile()); 
                cfg.AddProfile(new RoleProfile()); 
            })) ;
            DbConfig dbConfig = new DbConfig();
            _controller = new UserController(dbConfig.SqliteContext(), mapper);
        }

        //GetAll
        [Fact]
        public void GetUsers_RetunsOkResult()
        {
            var response = _controller.GetAll();
            response.Result.ShouldBeOfType<OkObjectResult>();   
        }

        //GetById
        [Fact]
        public void GetById_UserExists_ReturnOkResult()
        {
            var response = _controller.GetById(1);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetById_UserDoesntExists_ReturnNotFound()
        {
            var response = _controller.GetById(10);
            response.Result.ShouldBeOfType<NotFoundResult>();
        }
 
        //Create
        [Fact]
        public void Add_ValidObject_ReturnOkResult()
        {
            var user = new UserCreateUpdateDTO()
            {
                Username = "Test",
                Email = "test@example.com",
                Password = "password",
                RoleId = 2
            };
            var response = _controller.Create(user);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }
        
        [Fact]
        public void Add_InvalidObject_ReturnBadRequest()
        {
            var user = new UserCreateUpdateDTO()
            {
                Username = "user1",
                Email = "test@example.com",
                Password = "password",
                RoleId = 1
            };
            var response = _controller.Create(user);
            response.Result.ShouldBeOfType<BadRequestResult>();
        }

        //Delete
        [Fact]
        public void Remove_ExistingUser_ReturnOkResult()
        {
            //fails because it cant delete role id?
            var response = _controller.Delete(1);
            response.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public void Remove_NonExistingUser_ReturnNotFound()
        {
            var response = _controller.Delete(10);
            response.Result.ShouldBeOfType<NotFoundResult>();
        }

        //Update
        [Fact]
        public void Update_ValidObjectExistingUser_ReturnOkResult()
        {
            var user = new UserCreateUpdateDTO()
            {
                Username = "updated user",
                Email = "test@example.com",
                Password = "password",
                RoleId = 2
            };
            var response = _controller.Update(1,user);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public void Update_InvalidObject_ReturnBadRequest()
        {
            var user = new UserCreateUpdateDTO()
            {
                Username = "user3",
                Email = "test@example.com",
                Password = "password",
                RoleId = 2
            };
            var response = _controller.Update(1,user);
            response.Result.ShouldBeOfType<BadRequestResult>();
        }
    }
}