using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaPOS.API.Controllers;
using SistemaPOS.Application.Queries.Sedes;

namespace SistemaPOS.API.Test.Controllers
{
    public class SedeControllerTest
    {
        private readonly Mock<ISedeQueries> _queriesMock;
        private readonly SedeController _sut;

        public SedeControllerTest()
        {
            _queriesMock = new Mock<ISedeQueries>(MockBehavior.Strict);
            _sut = new SedeController(_queriesMock.Object);
        }

        [Fact]
        public async Task GetAll_Should_Return_200_Ok_With_Data_And_Call_Query_Once()
        {
            // Arrange
            var expected = new List<SedeDto>
            {
                new SedeDto(  1,  "Cali", "123","312555", "ACTIVA" , [] ),
                new SedeDto(  2,  "Popayán", "123", "3222555", "ACTIVA", [] ),
};

            _queriesMock
                .Setup(q => q.GetAllAsync())
                .ReturnsAsync(expected);

            // Act
            var result = await _sut.GetAll();

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            ok.Value.Should().BeAssignableTo<IEnumerable<SedeDto>>()
                .Which.Should().BeEquivalentTo(expected);

            _queriesMock.Verify(q => q.GetAllAsync(), Times.Once);
            _queriesMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAll_Should_Return_200_Ok_With_Empty_List()
        {
            // Arrange
            _queriesMock
                .Setup(q => q.GetAllAsync())
                .ReturnsAsync(new List<SedeDto>());

            // Act
            var result = await _sut.GetAll();

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IEnumerable<SedeDto>>()
                .Which.Should().BeEmpty();

            _queriesMock.Verify(q => q.GetAllAsync(), Times.Once);
            _queriesMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAll_Should_Bubble_Exception_When_Query_Fails()
        {
            // Arrange
            _queriesMock
                .Setup(q => q.GetAllAsync())
                .ThrowsAsync(new System.Exception("DB down"));

            // Act & Assert
            await FluentActions
                .Invoking(() => _sut.GetAll())
                .Should().ThrowAsync<System.Exception>()
                .WithMessage("*DB down*");

            _queriesMock.Verify(q => q.GetAllAsync(), Times.Once);
            _queriesMock.VerifyNoOtherCalls();
        }
    }
}

