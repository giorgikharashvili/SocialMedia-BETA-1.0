using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Models;
using SocialMedia.Repositories;
using SocialMedia.ViewModels;
using SocialMedia.Constants;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using Microsoft.AspNetCore.Components;


namespace SocialMedia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IRepository<ReelPostings> _reelPostingRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private IWebHostEnvironment _webHostEnvironment;
        public HomeController(IRepository<ReelPostings> repository, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
                _reelPostingRepository = repository;
                _userManager = userManager;
                _webHostEnvironment = webHostEnvironment;
        }

        

        public async Task<IActionResult> Index()
        {
            var reelPostings = await _reelPostingRepository.GetAllAsync();
            var userId = _userManager.GetUserId(User);
            ViewBag.CurrentUserId = userId;

            foreach(var reel in reelPostings)
            {
               var user = await _userManager.FindByIdAsync(reel.UserId);
               if(user != null)
                {
                    reel.User = user;
                }
            }

            return View(reelPostings);
        }

        [Authorize(Roles = "Admin,User")]
        public IActionResult Post()
        {
            return View();
        }
        [HttpPost] // gadavxedo detalurad
        [Authorize(Roles ="Admin,User")]
        public async Task<IActionResult> Post(ReelPostingViewModel reelPostingVm, IFormFile imageFile)
        {
            string uniqueFileName = null; // Ensure it's initialized
            var filePath = string.Empty;
            string relativePath = string.Empty;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                // Ensure uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

               

                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // asavebs atvirtul surats!
               if(imageFile is not null)
                {
                  try
                    {
                        using (var stream = imageFile.OpenReadStream())
                        {
                            var fileStream = System.IO.File.Create(filePath);

                            await stream.CopyToAsync(fileStream);
                            fileStream.Close();
                        }
                    } catch(Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }

                // Store only the relative path in the database
                 relativePath = Path.Combine("/uploads/" + uniqueFileName);
            }

            ModelState.Clear(); // Reset validation errors
            

            if (ModelState.IsValid)
            {
                var reelPostings = new ReelPostings
                {
                    Title = reelPostingVm.Title,
                    Description = reelPostingVm.Description,
                    UserId = _userManager.GetUserId(User),
                    ImagePath = relativePath,
                };

                await _reelPostingRepository.AddAsync(reelPostings);
                return RedirectToAction(nameof(Index));
            }

            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(modelError.ErrorMessage); // Log errors
            }

           
            


            return View(reelPostingVm);
        }


        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int id)
        {
            var reelPostings = await _reelPostingRepository.GetByIdAsync(id);

            if(reelPostings == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if (!User.IsInRole(Roles.Admin) && reelPostings.UserId != userId)
            {
                return Forbid();
            }


            await _reelPostingRepository.DeleteAsync(reelPostings.Id);

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
