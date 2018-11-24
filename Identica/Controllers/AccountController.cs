using Identica.Helpers;
using Identica.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Identica.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// An action to Register a user
        /// No Roles used
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
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

        /// <summary>
        /// An action to login a user
        /// </summary>
        /// <param name="oUserModel"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Action used to Register an Applicant
        /// First Register the User and then add a role to it
        /// </summary>
        /// <param name="oUserModel"></param>
        /// <returns></returns>
        [HttpPost(nameof(RegisterApplicant))]
        public async Task<IActionResult> RegisterApplicant([FromBody] UserModel oUserModel)
        {
            try
            {
                if (oUserModel is null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var oExistingApplicant = await _userManager.FindByEmailAsync(oUserModel.Email);

                if (oExistingApplicant != null)
                    return BadRequest(StringConstants.UserExists);
                else
                {
                    var oApplicant = new ApplicationUser()
                    {
                        Email = oUserModel.Email,
                        UserName = oUserModel.Username
                    };

                    var registerUser = await _userManager.CreateAsync(oApplicant, oUserModel.Password);

                    if (registerUser.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(oApplicant, nameof(EnumConstants.RoleType.Applicant));
                        return Ok();
                    }
                    else
                    {
                        AddErrorsToModelState(registerUser);
                        return StatusCode(500, ModelState);
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