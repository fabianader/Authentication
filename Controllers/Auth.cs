using IdentityProject.DTOs;
using IdentityProject.Models.AppCustomEntities;
using IdentityProject.Models.ViewModels;
using IdentityProject.Services;
using IdentityProject.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Common;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace IdentityProject.Controllers
{
    public class Auth : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public Auth(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        # region Register

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            string code = Tools.Tools.CodeGenerator(100000, 1000000).ToString();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "ثبت نام انجام نشد");
                return View(model);
            }

            var registerDTO = new RegisterDTO()
            {
                DTO_UserName = model.UserName,
                DTO_Age = model.Age,
                DTO_Email = model.Email,
                DTO_PhoneNumber = model.PhoneNumber,
                DTO_Password = model.Password
            };

            var result = await _userManager.CreateAsync(new ApplicationUser
            {
                UserName = registerDTO.DTO_UserName,
                FullName = registerDTO.DTO_UserName,
                Age = registerDTO.DTO_Age,
                Email = registerDTO.DTO_Email,
                PhoneNumber = registerDTO.DTO_PhoneNumber,
                RegisterGeneratedCode = code
            }, registerDTO.DTO_Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                    return View(model);
                }
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Tools.Tools.Encrypt(token);

            await _emailSender.SendEmailAsync(new EmailModel(user.Email, "Email Confirmation", code));

            TempData["EmailConfirmationToken"] = token;
            user.RegisterGeneratedCode = code;

            return RedirectToAction("EmailCode", "Auth", new { username = registerDTO.DTO_UserName }, Request.Scheme);
        }

        public IActionResult EmailCode(string username)
        {
            if (username == null)
                return BadRequest();

            string token = TempData["EmailConfirmationToken"] as string;
            if (string.IsNullOrEmpty(token))
                return BadRequest();

            EmailCodeVM model = new EmailCodeVM()
            {
                UserName = username,
                Token = token,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailCode(EmailCodeVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "خطایی رخ داد");
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return NotFound();

            model.Token = Tools.Tools.Decrypt(model.Token);
            
            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (result.Errors.Any())
            {
                ModelState.AddModelError(string.Empty, "ثبت نام با خطا مواجه شد");
                return View(model);
            }

            if (model.Code != user.RegisterGeneratedCode)
            {
                ModelState.AddModelError(string.Empty, "کد وارد شده نامعتبر است");
                return View(model);
            }

            user.EmailConfirmed = true;
            user.PhoneNumberConfirmed = true;

            return RedirectToAction("Login");
        }

        #endregion

        #region Log In

        public async Task<IActionResult> Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;
            var model = new LoginVM()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            //if (_signInManager.IsSignedIn(User))
            //{
            //    ModelState.AddModelError(string.Empty, "کاربر از قبل وارد شده است");
            //    return View(model);
            //}

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "ورود معتبر نیست");
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربری با مشخصات وارد شده یافت نشد");
                return View(model);
            }

            var result =
                await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "حساب شما قفل شده است");
                return View(model);
            }

            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "ورود کاربر مجاز نیست");
                return View(model);
            }

            //else if (result.RequiresTwoFactor)
            //    return RedirectToAction("TwoFactorLogin");

            ModelState.AddModelError(string.Empty, "تلاش برای ورود نامعتبر است");
            return View(model);
        }

        #endregion

        #region Log Out

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Forgot Password
        
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "اطلاعات وارد شده معتبر نمی باشند");
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربر یافت نشد");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = Tools.Tools.Encrypt(token);
            TempData["PasswordResetToken"] = token;


            string code = Tools.Tools.CodeGenerator(100000, 1000000).ToString();
            TempData["Code"] = code;
            await _emailSender.SendEmailAsync(new EmailModel(user.Email, "Password Change Operation", code));

            return RedirectToAction("ForgotPasswordEmailCode", new { username = model.UserName });
        }

        public IActionResult ForgotPasswordEmailCode(string username)
        {
            string token = TempData.Peek("PasswordResetToken") as string;
            if (string.IsNullOrEmpty(token))
                return BadRequest();

            ForgotPasswordEmailCodeVM model = new ForgotPasswordEmailCodeVM()
            {
                UserName = username,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPasswordEmailCode(ForgotPasswordEmailCodeVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "خطایی رخ داد");
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربری با این مشخصات یافت نشد");
                return View(model);
            }

            string code = TempData["Code"] as string;
            if (model.Code != code)
            {
                ModelState.AddModelError(string.Empty, "کد وارد شده نامعتبر است");
                return View(model);
            }

            string token = TempData["PasswordResetToken"] as string;
            // token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            // model.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            if ((Tools.Tools.Decrypt(model.Token)) != (Tools.Tools.Decrypt((token))))
            {
                ModelState.AddModelError(string.Empty, "توکن نامعتبر است");
                return View(model);
            }

            return RedirectToAction("ResetPassword", "Auth", new { username = model.UserName });
        }

        #endregion

        #region Reset Password

        public IActionResult ResetPassword(string username)
        {
            string token = TempData["PasswordResetToken"] as string;
            if (string.IsNullOrEmpty(token))
                return BadRequest();

            ResetPasswordVM model = new ResetPasswordVM()
            {
                UserName = username,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "خطایی رخ داد");
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربر یافت نشد");
                return View(model);
            }

            var token = Tools.Tools.Decrypt(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach(var Err in result.Errors)
                    ModelState.AddModelError(string.Empty, Err.Description);

                return View(model);
            }

            return RedirectToAction("Login", "Auth");
        }

        #endregion

        #region External Login

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Auth", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallBack(string remoteError = null, string returnUrl = null)
        {
            returnUrl = (returnUrl != null && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/"));

            var model = new LoginVM()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error: {remoteError}");
                return View("Login", model);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error: {remoteError}");
                return View("Login", model);
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            if (result.Succeeded)
                return Redirect(returnUrl);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if(email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    string userName = email.Split("@")[0];
                    user = new ApplicationUser()
                    {
                        UserName = userName,
                        FullName = userName,
                        Email = email,
                        PhoneNumber = "",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    await _userManager.CreateAsync(user);
                }

                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, false);

                return Redirect(returnUrl);
            }

            return View();  
        }

        #endregion

        #region Remote Validations

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyUserName(string username)
        {
            bool IsAny = await _userManager.Users.AnyAsync(u => u.UserName == username);

            if (!IsAny)
                return Json(true);

            return Json("نام کاربری مورد نظر از قبل ثبت شده است");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyEmail(string email)
        {
            bool IsAny = await _userManager.Users.AnyAsync(u => u.Email ==  email);

            if(!IsAny)
                return Json(true);

            return Json("ایمیل مورد نظر از قبل ثبت شده است");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyPhoneNumber(string phonenumber)
        {
            bool IsAny = await _userManager.Users.AnyAsync(u => u.PhoneNumber == phonenumber);

			if (!IsAny)
				return Json(true);

			return Json("شماره تلفن مورد نظر از قبل ثبت شده است");
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IsAgeValid(int age)
        {
			if (age > 0)
                return Json(true);

            return Json("سن نامعتبر است");
		}

		//     [HttpPost]
		//     [ValidateAntiForgeryToken]
		//     public IActionResult IsPhoneNumberValid(string phone)
		//     {
		//bool IsValid = Regex.Match(phone, @"^(\+[0-9]{9})$").Success;

		//         if (IsValid)
		//             return Json(true);

		//         return Json("شماره نامعتبر است");
		//     }

		#endregion
	}
}
