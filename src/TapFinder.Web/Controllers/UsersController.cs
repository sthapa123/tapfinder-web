﻿using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TapFinder.DataAccess.Entities;
using TapFinder.Web.Dtos.UserDtos;
using TapFinder.Web.Services;

namespace TapFinder.Web.Controllers
{
    [Authorize]
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        private readonly UsersService usersService;
        private readonly ImageSaver imageSaver;
        private readonly BadgesService badgesService;

        public UsersController(UsersService usersService, ImageSaver imageSaver, BadgesService badgesService)
        {
            this.usersService = usersService;
            this.imageSaver = imageSaver;
            this.badgesService = badgesService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> RegisterUser(UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await usersService.RegisterUser(dto))
            {
                return BadRequest();
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpHead]
        [Route("")]
        public async Task<IHttpActionResult> CheckUsernameAvailability([FromUri] string username)
        {
            if (await usersService.FindByName(username) == null)
            {
                return NotFound();
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpHead]
        [Route("")]
        public async Task<IHttpActionResult> CheckEmailAvailability([FromUri] string email)
        {
            if (await usersService.FindByEmail(email) == null)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpGet]
        [Route("{username}")]
        public IHttpActionResult GetUser(string username)
        {
            var user = usersService.GetUser(username);
            return Ok(user);
        }

        [HttpPut]
        [Route("image")]
        public IHttpActionResult ChangeImage(UserImageDto imageDto)
        {
            var path = imageSaver.SaveFile(imageDto.Image);
            usersService.SetImage(User.Identity.GetUserId<int>(), path);
            return Ok(new UserDto { ImagePath = path });
        }

        [HttpPut]
        [Route("password")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await usersService.ChangePassword(dto, User.Identity.GetUserId<int>()))
            {
                return BadRequest();
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("external")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await usersService.VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            User user = await usersService.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;
            if (hasRegistered)
            {
                return BadRequest("External user is already registered");
            }

            user = usersService.GetNewUser(model.UserName, model.Email);

            IdentityResult result = await usersService.CreateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join("\n", result.Errors));
            }
            badgesService.AddInitialBadge(user);
            var info = new ExternalLoginInfo()
            {
                DefaultUserName = model.UserName,
                Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
            };

            result = await usersService.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join("\n", result.Errors));
            }

            var accessTokenResponse = usersService.GenerateLocalAccessTokenResponse(model.UserName, user.Id);
            return Ok(accessTokenResponse);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("token")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {
            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            {
                return BadRequest("Provider or external access token is not sent");
            }

            var verifiedAccessToken = await usersService.VerifyExternalAccessToken(provider, externalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            User user = await usersService.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));
            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }
            var accessTokenResponse = usersService.GenerateLocalAccessTokenResponse(user.UserName, user.Id);
            return Ok(accessTokenResponse);
        }

    }
}
