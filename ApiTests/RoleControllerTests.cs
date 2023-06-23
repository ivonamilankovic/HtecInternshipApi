using Internship.Models;
using AutoMapper;
using Internship.Controllers;
using Internship.Profiles;
using Shouldly;
using Microsoft.AspNetCore.Mvc;

namespace ApiTests
{
    public class RoleControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly RoleController _controller;
        public RoleControllerTests()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new RoleProfile())));
            DbConfig dbConfig = new DbConfig();
            _controller = new RoleController(dbConfig.SqliteContext(), mapper);
        }

        //GetAll
        [Fact]
        public void GetRoles_ReturnOkResult()
        {
            var response = _controller.GetAll();
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        //GetById
        [Fact]
        public void GetById_RoleExists_ReturnOkResult()
        {
            var response = _controller.GetById(1);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetById_RoleDoesntExist_ReturnNotFound()
        {
            var response = _controller.GetById(10);
            response.Result.ShouldBeOfType<NotFoundResult>();
        }

        //Create
        [Fact]
        public void Add_ValidObject_ReturnOkResult()
        {
            var role = new RoleDTO() 
            { 
                RoleName = "New role" 
            };
            var response = _controller.Create(role);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }
        
        [Fact]
        public void Add_InvalidObject_ReturnBadRequest()
        {
            var role = new RoleDTO() 
            {
                RoleName = "admin" 
            };
            var response = _controller.Create(role);
            response.Result.ShouldBeOfType<BadRequestResult>();
        }

        //Delete
        [Fact]
        public void Remove_ExistingRole_ReturnOkResult()
        {
            var response = _controller.Delete(1);
            response.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public void Remove_NonExistingRole_ReturnNotFound()
        {
            var response = _controller.Delete(10);
            response.Result.ShouldBeOfType<NotFoundResult>();
        }

        //Update
        [Fact]
        public void Update_ValidObjectExistingRole_ReturnOkResult()
        {
            var role = new RoleDTO()
            {
                RoleName = "Update role"
            };
            var response = _controller.Update(1, role);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public void Update_InvalidObjectExistingRole_ReturnBadRequest()
        {
            var role = new RoleDTO() 
            {
                RoleName = "admin"
            };
            var response = _controller.Update(2, role);
            response.Result.ShouldBeOfType<BadRequestResult>();
        }
        [Fact]
        public void Update_ValidObjectNonExistingRole_ReturnNotFound()
        {
            var role = new RoleDTO()
            {
                RoleName = "update role"
            };
            var response = _controller.Update(10, role);
            response.Result.ShouldBeOfType<NotFoundResult>();
        }
    }
}
