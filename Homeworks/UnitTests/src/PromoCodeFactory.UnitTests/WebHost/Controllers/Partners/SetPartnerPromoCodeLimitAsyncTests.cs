using AutoFixture;
using FluentAssertions;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private static (PartnersController partnersController, Mock<IRepository<Partner>> repositoryMock) CreateSut(Partner partner)
        {
            Mock<IRepository<Partner>> partnersRepositoryMock = new();
            partnersRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => partner);

            return (new PartnersController(partnersRepositoryMock.Object), partnersRepositoryMock);
        }

        private static Partner GetPartner(Guid id, bool isActive, List<PartnerPromoCodeLimit> partnerPromoCodeLimits) 
            => new Partner()
            {
                Id = id,
                Name = "Test Partner",
                IsActive = isActive,
                NumberIssuedPromoCodes = 2,
                PartnerLimits = partnerPromoCodeLimits,
            };
        

        private static Partner GetInactivePartner(Guid id) => GetPartner(id, false, null);

        private static PartnerPromoCodeLimit GetActiveLimit()
            => new PartnerPromoCodeLimit
            {
                Id = new Fixture().Create<Guid>(),
                Limit = 100,
                CreateDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(10),
            };

        private static PartnerPromoCodeLimit GetCanceledLimit()
            => new PartnerPromoCodeLimit
            {
                Id = new Fixture().Create<Guid>(),
                Limit = 100,
                CreateDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(10),
                CancelDate = DateTime.UtcNow.AddDays(-5),
            };

        /// <summary>
        /// Test #1
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCode_WhenPartnerIsNull_ReturnNotFoundResult()
        {
            //arrange
            var sut = CreateSut(null).partnersController;

            //act
            var result = await sut.SetPartnerPromoCodeLimitAsync(new Fixture().Create<Guid>(), null);

            //assert
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.NotFoundResult>();
        }

        /// <summary>
        /// Test #2
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCode_WhenPartnerIsInactive_ReturnBadResult()
        {
            //arrange
            var id = new Fixture().Create<Guid>();
            var sut = CreateSut(GetInactivePartner(id)).partnersController;
            SetPartnerPromoCodeLimitRequest req = new() { Limit = 10 };

            //act
            var result = await sut.SetPartnerPromoCodeLimitAsync(id, req);

            //assert
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>();
        }

        /// <summary>
        /// Test #3
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCode_WhenExistsLimitIsCancaled_IssuedNumberNotZero()
        {
            //arrange
            var id = new Fixture().Create<Guid>();
            var existingLimit = GetCanceledLimit();
            var partner = GetPartner(id, true, [existingLimit]);
            var sut = CreateSut(partner).partnersController;
            SetPartnerPromoCodeLimitRequest req = new() { Limit = 50 };

            //act
            var result = await sut.SetPartnerPromoCodeLimitAsync(id, req);
            //assert
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().NotBe(0);
        }

        /// <summary>
        /// Test #4 and part of Test #3
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCode_WhenSetNewLimit_OldActiveLimitCanceled()
        {
            //arrange
            var id = new Fixture().Create<Guid>();
            var existingLimit = GetActiveLimit();
            var partner = GetPartner(id, true, [existingLimit]);
            var sut = CreateSut(partner).partnersController;
            SetPartnerPromoCodeLimitRequest req = new() { Limit = 50 };

            //act
            var result = await sut.SetPartnerPromoCodeLimitAsync(id, req);

            //assert
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(0);
            existingLimit.CancelDate.Should().NotBeNull();
        }

        /// <summary>
        /// Test #5
        /// </summary>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task SetPartnerPromoCode_WhenLimitLessOrEqualZero_ReturnBadResult(int limit)
        {
            //arrange
            var id = new Fixture().Create<Guid>();
            var sut = CreateSut(GetInactivePartner(id)).partnersController;

            SetPartnerPromoCodeLimitRequest req = new() { Limit = limit };

            //act
            var result = await sut.SetPartnerPromoCodeLimitAsync(id, req);

            //assert
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>();
        }

        /// <summary>
        /// Test #6
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCode_OnSuccessResult_NewLimitAdded()
        {
            //arrange
            var id = new Fixture().Create<Guid>();
            var existingLimit = GetActiveLimit();
            var partner = GetPartner(id, true, [existingLimit]);
            var sut = CreateSut(partner);
            SetPartnerPromoCodeLimitRequest req = new() { Limit = 50 };

            //act
            var result = await sut.partnersController.SetPartnerPromoCodeLimitAsync(id, req);

            //assert
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>();
            partner.PartnerLimits.Should().HaveCount(2);
            sut.repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Partner>()), Times.Once);
        }
    }
}