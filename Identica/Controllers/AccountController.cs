using Identica.Helpers;
using Identica.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Identica.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost(nameof(Register))]
        public async Task<IActionResult> Register([FromBody] UserModel userModel)
        {
            try
            {
                if (userModel is null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var oNewUser = new ApplicationUser()
                {
                    UserName = userModel.Username,
                    Email = userModel.Email
                };

                var existingUser = await _userManager.FindByEmailAsync(oNewUser.Email);

                if (existingUser != null)
                    return BadRequest(StringConstants.UserExists);

                var createResult = await _userManager.CreateAsync(oNewUser, userModel.Password);

                if (createResult.Succeeded)
                {

                    return Ok();
                }
                else
                {
                    AddErrorsToModelState(createResult);
                    return StatusCode(500, ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, StringConstants.ProcessingError);
            }
        }

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] UserModel oUserModel)
        {
            try
            {
                if (oUserModel is null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var exisitingUser = await _userManager.FindByEmailAsync(oUserModel.Email);

                if (exisitingUser is null)
                    return NotFound(StringConstants.UserNotFound);
                else
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(exisitingUser, oUserModel.Password, isPersistent: false, lockoutOnFailure: false);

                    if (signInResult.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(500, StringConstants.WrongPwdEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, StringConstants.ProcessingError);
            }
        }

        private void AddErrorsToModelState(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}