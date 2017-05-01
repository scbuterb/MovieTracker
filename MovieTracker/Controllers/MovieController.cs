using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MovieTracker.Data;
using MovieTracker.Models;
using MovieTracker.Services;

namespace MovieTracker.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly IRepository<Genre> _genreRepository;
        private readonly IMembershipService _membershipService;
        private readonly IRepository<Movie> _movieRepository;

        public MovieController(IRepository<Movie> movieRepository, IRepository<Genre> genreRepository, IMembershipService membershipService)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _membershipService = membershipService;
        }

        public ActionResult Index()
        {
            MembershipUser user = _membershipService.GetUser(User.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("Register", "Account");
            }

            var userKey = (Guid) user.ProviderUserKey;

            IEnumerable<Movie> movies = _movieRepository.Find(m => m.aspnet_UsersUserId == userKey);

            return View(movies);
        }

        public ActionResult Detail(int id)
        {
            MembershipUser user = _membershipService.GetUser(User.Identity.Name);

            Movie movie = _movieRepository.Find(m => m.Id == id).FirstOrDefault();
            if (movie == null)
            {
                return RedirectToAction("MoveNotFound");
            }
            else if (!IsValidUserForMovie(movie.aspnet_UsersUserId.ToString(), user))
            {
                return RedirectToAction("MoveNotFound");
            }

            return View(movie);
        }

        public ActionResult Edit(int id)
        {
            MembershipUser user = _membershipService.GetUser(User.Identity.Name);

            Movie movie = _movieRepository.Find(m => m.Id == id).FirstOrDefault();

            if (movie == null)
            {
                return RedirectToAction("MoveNotFound");
            }
            else if (!IsValidUserForMovie(movie.aspnet_UsersUserId.ToString(), user))
            {
                return RedirectToAction("MoveNotFound");
            }

            IEnumerable<Genre> genres = _genreRepository.GetAll();

            var editMovieViewModel = new EditMovieViewModel
                                         {
                                             Directors = movie.Directors,
                                             Genres = new SelectList(genres, "Id", "Name"),
                                             Name = movie.Name,
                                             Rating = movie.Rating,
                                             Stars = movie.Stars,
                                             Writers = movie.Writers,
                                             Id = id,
                                             GenreId = movie.GenreId
                                         };


            return View(editMovieViewModel);
        }

        public ActionResult ReturnMovieFromFriend(int movieId)
        {
            MembershipUser user = _membershipService.GetUser(User.Identity.Name);
            Movie movie = _movieRepository.Find(m => m.Id == movieId).Single();

            movie.LentToName = "";
            movie.LentToDate = null;  

            _movieRepository.Save();

            var userKey = (Guid)user.ProviderUserKey;

            IEnumerable<Movie> movies = _movieRepository.Find(m => m.aspnet_UsersUserId == userKey);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveMovieLendTo(int movieId, string lendToName, string lendToDate)
        {
            Movie movie = _movieRepository.Find(m => m.Id == movieId).Single();

            List<string> _errorMessages = new List<string>();
            if (IsValidBorrowMovie(lendToName, lendToDate, ref _errorMessages))
            {          
                movie.LentToName = lendToName;
                movie.LentToDate = null;
                if (!string.IsNullOrWhiteSpace(lendToDate))
                {
                    DateTime _lendToDate = DateTime.Now;
                    if (DateTime.TryParse(lendToDate, out _lendToDate))
                    {
                        movie.LentToDate = _lendToDate;
                    }
                }

                _movieRepository.Save();
            }
            else
            {
                foreach (string _errorMsg in _errorMessages)
                {
                    ModelState.AddModelError("Movie", _errorMsg);
                }
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditMovieViewModel editMovieViewModel)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user = _membershipService.GetUser(User.Identity.Name);

                Movie movie = _movieRepository.Find(m => m.Id == editMovieViewModel.Id).FirstOrDefault();

                if (movie == null)
                {
                    return RedirectToAction("MoveNotFound");
                }
                else if (!IsValidUserForMovie(movie.aspnet_UsersUserId.ToString(), user))
                {
                    return RedirectToAction("MoveNotFound");
                }

                movie.Directors = editMovieViewModel.Directors;
                movie.GenreId = editMovieViewModel.GenreId;
                movie.Name = editMovieViewModel.Name;
                movie.Rating = editMovieViewModel.Rating;
                movie.Stars = editMovieViewModel.Stars;
                movie.Writers = editMovieViewModel.Writers;

                _movieRepository.Save();
                return RedirectToAction("Index");
            }
            IEnumerable<Genre> genres = _genreRepository.GetAll();
            editMovieViewModel.Genres = new SelectList(genres, "Id", "Name");

            return View(editMovieViewModel);
        }        

        public ActionResult Add()
        {
            IEnumerable<Genre> genres = _genreRepository.GetAll();

            var editMovieViewModel = new EditMovieViewModel
                                         {
                                             Genres = new SelectList(genres, "Id", "Name"),
                                         };

            return View(editMovieViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(EditMovieViewModel editMovieViewModel)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user = _membershipService.GetUser(User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("Register", "Account");
                }
                var userKey = (Guid) user.ProviderUserKey;

                var movie = new Movie
                                {
                                    Directors = editMovieViewModel.Directors,
                                    GenreId = editMovieViewModel.GenreId,
                                    Name = editMovieViewModel.Name,
                                    Rating = editMovieViewModel.Rating,
                                    Stars = editMovieViewModel.Stars,
                                    Writers = editMovieViewModel.Writers,
                                    aspnet_UsersUserId = userKey
                                };

                _movieRepository.Add(movie);
                _movieRepository.Save();
                return RedirectToAction("Index");
            }
            IEnumerable<Genre> genres = _genreRepository.GetAll();
            editMovieViewModel.Genres = new SelectList(genres, "Id", "Name");

            return View(editMovieViewModel);
        }

        public ActionResult Delete(int id)
        {
            MembershipUser user = _membershipService.GetUser(User.Identity.Name);

            Movie movie = _movieRepository.Find(m => m.Id == id).FirstOrDefault();

            if (movie == null)
            {
                return RedirectToAction("MoveNotFound");
            }
            else if (!IsValidUserForMovie(movie.aspnet_UsersUserId.ToString(), user))
            {
                return RedirectToAction("MoveNotFound");
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Movie movie)
        {
            MembershipUser user = _membershipService.GetUser(User.Identity.Name);
            if (!IsValidUserForMovie(movie.aspnet_UsersUserId.ToString(), user))
            {
                return RedirectToAction("MoveNotFound");
            }

            Movie movieToDelete = _movieRepository.Find(m => m.Id == movie.Id).FirstOrDefault();
            _movieRepository.Delete(movieToDelete);
            _movieRepository.Save();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public PartialViewResult ClearRating(int movieId)
        {
            Movie movie = _movieRepository.Find(m => m.Id == movieId).Single();
            movie.ClearRating();

            _movieRepository.Save();

            return PartialView("MovieRatingControl", movie);
        }

        [HttpPost]
        public PartialViewResult AddRating(int movieId, short rating)
        {
            Movie movie = _movieRepository.Find(m => m.Id == movieId).Single();

            movie.Rating = rating;

            _movieRepository.Save();

            return PartialView("MovieRatingControl", movie);
        }

        public ActionResult MoveNotFound()
        {
            return View();
        }

        #region Private Helper Methods
        private static bool IsValidBorrowMovie(string lendToName, string lendToDate, ref List<string> errorMessages)
        {
            bool _isValid = true;

            if (string.IsNullOrWhiteSpace(lendToName))
            {
                _isValid = false;
                errorMessages.Add("Lend To Name is required.");
            }

            DateTime _tempDate;
            if (!DateTime.TryParse(lendToDate, out _tempDate))
            {
                _isValid = false;
                errorMessages.Add("Lend To Date is invalid.");
            }

            return _isValid;
        }

        /// <summary>
        /// This helper method will compare the moveUserID against the MembershipUser.ProviderUserKey
        /// to ensure the movie is being retrieved for the correct owner.
        /// </summary>
        /// <param name="movieUserID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static bool IsValidUserForMovie(string movieUserID, MembershipUser user)
        {
            bool _isValid = false;

            if (movieUserID == ((Guid)user.ProviderUserKey).ToString())
            {
                _isValid = true;
            }

            return _isValid;
        }
        #endregion
    }
}