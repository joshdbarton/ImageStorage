using ImageStorage.Models;
using ImageStorage.Models.ViewModels;
using ImageStorage.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStorage.Controllers
{
    public class PersonController : Controller
    {
        private IImageRepository _imageRepo;
        private IPersonRepository _personRepo;
        
        public PersonController(IImageRepository imageRepo, IPersonRepository personRepo)
        {
            _imageRepo = imageRepo;
            _personRepo = personRepo;
        }
        // GET: PersonController
        public ActionResult Index()
        {
            var persons = _personRepo.GetPersons();
            return View(persons);
        }

        // GET: PersonController/Details/5
        public ActionResult Details(int id)
        {
            var person = _personRepo.GetPersonById(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // GET: PersonController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PersonController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonFormViewModel vm)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await vm.ImageUpload.CopyToAsync(memoryStream);

                    // Upload the file if less than 2 MB
                    if (memoryStream.Length == 0)
                    {
                        vm.Person.ImageId = null;
                    }
                    else if (memoryStream.Length < 2097152)
                    {

                       vm.Person.ImageId = _imageRepo.CreateImage(memoryStream.ToArray());
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                        return View(vm);
                    }
                    
                    _personRepo.CreatePerson(vm.Person);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return View(vm);
            }
        }

        // GET: PersonController/Edit/5
        public ActionResult Edit(int id)
        {
            var person = _personRepo.GetPersonById(id);
            if (person == null)
            {
                return NotFound();
            }

            return View(new PersonFormViewModel { Person = person });
        }

        // POST: PersonController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PersonFormViewModel vm)
        {

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await vm.ImageUpload.CopyToAsync(memoryStream);

                    // Upload the file if less than 2 MB
                    if (memoryStream.Length < 2097152 && memoryStream.Length > 0)
                    {
                        var imageId = vm.Person.ImageId;
                        vm.Person.ImageId = _imageRepo.CreateImage(memoryStream.ToArray());
                        _personRepo.UpdatePerson(vm.Person);
                        _imageRepo.DeleteImage(imageId);
                    }
                    else if (memoryStream.Length > 0)
                    {
                        ModelState.AddModelError("File", "The file is too large. 2MB max");
                        return View(vm);
                    }
                    else
                    {
                        _personRepo.UpdatePerson(vm.Person);
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return View(vm);
            }
            
        }

        // GET: PersonController/Delete/5
        public ActionResult Delete(int id)
        {
            var person = _personRepo.GetPersonById(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: PersonController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Person person)
        {
            try
            {
                var personToDelete = _personRepo.GetPersonById(id);
                if (personToDelete == null)
                {
                    return NotFound();
                }
                var imageId = personToDelete.ImageId;
                _personRepo.DeletePerson(id);
                _imageRepo.DeleteImage(imageId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Image(int id)
        {
            var stream = _imageRepo.GetImageStreamById(id);
            if (stream != null)
            {
                return File(stream, "image/jpeg", $"image_{id}.jpg");
            }

            return NotFound();
        }

        public ActionResult RemoveImage(int id)
        {
            var person = _personRepo.GetPersonById(id);
            var imageId = person.ImageId;
            person.ImageId = null;
            _personRepo.UpdatePerson(person);
            _imageRepo.DeleteImage(imageId);

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
