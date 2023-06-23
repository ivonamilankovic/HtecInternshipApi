using AutoMapper;
using Internship.Controllers;
using Internship.Mail;
using Internship.Models;
using Internship.Profiles;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace ApiTests
{
    public class AssignmentControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly AssignmentController _controller;
        public AssignmentControllerTests()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AssigneeProfile())));
            IMailer mailer =  new FakeMailer();
            DbConfig dbConfig = new DbConfig();
            _controller = new AssignmentController(dbConfig.SqliteContext(), mapper, mailer);
        }

        //GetAll
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GetAssignments_ReturnOkResult(bool mentorsWithAssignee)
        {
            var response = _controller.GetAll(mentorsWithAssignee);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }
        
        //GetByAssigneeId
        [Fact]
        public void GetByAssigneeId_AssigneeExistsAndHaveMentor_ReturnOkResult()
        {
            var response = _controller.GetByAssigneeId(1);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByAssigneeId_AssigneeDoesntExist_ReturnNotFound()
        {
            var response = _controller.GetByAssigneeId(10);
            response.Result.ShouldBeOfType<NotFoundResult>();
        }

        //GetByMentorId
        [Fact]
        public void GetByMentorId_MentorExists_ReturnOkResult()
        {
            var response = _controller.GetByMentorId(2);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByMentorId_MentornotExist_ReturnNotFound()
        {
            var response = _controller.GetByMentorId(10);
            response.Result.ShouldBeOfType<NotFoundResult>();
        }

        //SetAssigneeToMentor
        [Fact]
        public void SetAssigneeToMentor_ValidObject_ReturnOkResult()
        {
            var assignment = new MentorAssigneeDTO
            {
                MentorId = 1,
                AssigneeId = 2
            };
            var response = _controller.SetAssigneeToMentor(assignment);
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Theory]
        [InlineData(1,3)]
        [InlineData(1,1)]
        [InlineData(10,3)]
        [InlineData(1,10)]
        public void SetAssigneeToMentor_InvalidObject_ReturnBadRequest(int mentor, int assignee)
        {
            var assignment = new MentorAssigneeDTO
            {
                MentorId = mentor,
                AssigneeId = assignee
            };
            var response = _controller.SetAssigneeToMentor(assignment);
            response.Result.ShouldBeOfType<BadRequestResult>();
        }

        //ChangeAssignee
        [Theory]
        [InlineData(2, 3)]
        [InlineData(2, 2)]
        [InlineData(10, 2)]
        [InlineData(2, 10)]
        public void ChangeAssignee_InvalidObject_ReturnBadRequest(int mentor, int assignee)
        {
            var assignment = new MentorAssigneeDTO
            {
                MentorId = mentor,
                AssigneeId = assignee
            };
            var response = _controller.SetAssigneeToMentor(assignment);
            response.Result.ShouldBeOfType<BadRequestResult>();
        }

        //RemoveAssigneeFromMentor
        [Fact]
        public void RemoveAssigneeFromMentor_ValidObject_ReturnOkResult()
        {
            var response = _controller.RemoveAssigneeFromMentor(3);
            response.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public void RemoveAssigneeFromMentor_InvalidObject_ReturnNotFound()
        {
            var response = _controller.RemoveAssigneeFromMentor(10);
            response.Result.ShouldBeOfType<NotFoundResult>();
        }

        //RemoveMentorFromAssignee
        [Fact]
        public void RemoveMentorFromAssignee_ValidObject_ReturnOkResult()
        {
            var response = _controller.RemoveMentorFromAssignee(2);
            response.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public void RemoveMentorFromAssignee_InvalidObject_ReturnNotfound()
        {
            var response = _controller.RemoveMentorFromAssignee(10);
            response.Result.ShouldBeOfType<NotFoundResult>();
        }
    }
}
