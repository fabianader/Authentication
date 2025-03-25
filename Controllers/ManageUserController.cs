using IdentityProject.Models.AppCustomEntities;
using IdentityProject.Models.ManageUserVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Controllers
{
    public class ManageUserController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManageUserController(RoleManager<ApplicationRole> roleManager, 
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()        
        
        {
            var model = _userManager.Users.Select(u => new UsersListVM()
            {
                UserId = u.Id, UserName = u.UserName, Email = u.Email, PhoneNumber = u.PhoneNumber
            }).ToList();

            return View(model);
        }
        
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var model = new EditUserVM()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            var roleslist = _roleManager.Roles.Select(r => new RolesVM()
            {
                RoleId = r.Id, RoleName = r.Name
            }).ToList();

            ViewBag.Roles = roleslist;
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserVM model, List<string> SelectedRoles)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if(user == null) 
                return NotFound();

            user.UserName = model.UserName;
            user.FullName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            await _userManager.AddToRolesAsync(user, SelectedRoles);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Index");
            }

            foreach(var Err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, Err.Description);
            }

            var Model = new EditUserVM()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };

            var rolesList = _roleManager.Roles.Select(r => new RolesVM()
            {
                RoleId = r.Id,
                RoleName = r.Name
            }).ToList();


            ViewBag.Roles = rolesList;
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);

            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchUsers(string SearchedUserName)
        {
            if (SearchedUserName == null)
				return RedirectToAction("Index");

			List<UsersListVM> ResultUsers = new List<UsersListVM>();

            var AllUsers = _userManager.Users.Select(u => new UsersListVM()
			{
				UserId = u.Id,
				UserName = u.UserName,
				Email = u.Email,
				PhoneNumber = u.PhoneNumber
			}).ToList();

            if (AllUsers == null)
                return NotFound();

            foreach (var user in AllUsers)
            {
                if (user.UserName.Contains(SearchedUserName))
                {
                    ResultUsers.Add(user);
                }
            }

            if (!ResultUsers.Any())
            {
                //ModelState.AddModelError(string.Empty, "کاربری یافت نشد");
                return RedirectToAction("Index");
            }

            return View("SearchedUsers", ResultUsers);
        }
    }
}
