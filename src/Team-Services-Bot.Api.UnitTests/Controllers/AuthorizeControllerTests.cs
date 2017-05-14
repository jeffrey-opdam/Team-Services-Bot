﻿// ———————————————————————————————
// <copyright file="AuthorizeControllerTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the AuthorizeController.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using FluentAssertions;
    using Common;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [TestCategory(TestCategories.Unit)]
    public class AuthorizeControllerTests
    {
        [TestMethod]
        public async Task Authorize_A_Valid_LogOn()
        {
            var authenticationService = new Mock<IAuthenticationService>();
            var botService = new Mock<IBotService>();
            var profileService = new Mock<IVstsService>();

            var token = new OAuthToken();
            var profile = new Profile();
            var accounts = new List<Account>();
            var botData = new BotData();

            var controller = new AuthorizeController(
                botService.Object,
                authenticationService.Object,
                profileService.Object);

            const string code = "1234567890";
            const string state = "channel1;user1";

            authenticationService
                .Setup(a => a.GetToken(code))
                .ReturnsAsync(() => token);
            profileService
                .Setup(p => p.GetProfile(token))
                .ReturnsAsync(profile);
            profileService
                .Setup(p => p.GetAccounts(token, It.IsAny<Guid>()))
                .ReturnsAsync(accounts);
            botService
                .Setup(b => b.GetUserData("channel1", "user1"))
                .ReturnsAsync(botData);
            botService
                .Setup(b => b.SetUserData("channel1", "user1", botData))
                .Returns(Task.CompletedTask);

            var result = await controller.Index(code, string.Empty, state) as ViewResult;
            var profiles = botData.GetProfiles();

            result.Should().NotBeNull();
            profiles.Should().NotBeNull().And.HaveCount(1);
        }
    }
}