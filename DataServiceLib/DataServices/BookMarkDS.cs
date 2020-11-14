﻿using DataServiceLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataServiceLib.DataServices
{
    public class BookMarkDS : IBookMarkDS
    {
        private readonly MoviesContext ctx = new MoviesContext();
        public bool AddMovieBookMark(string userName, string movieId)
        {
            return ctx.AddMovieBookMark(userName, movieId);
        }
        public bool RemoveMovieBookMark(string userName, string movieId)
        {
            return ctx.RemoveMovieBookMark(userName, movieId);
        }
        public bool AddNameBookMark(string userName, string personId)
        {
            return ctx.AddNameBookMark(userName, personId);
        }
        public bool RemoveNameBookMark(string userName, string personId)
        {
            return ctx.RemoveNameBookMark(userName, personId);
        }
        public IList<BookmarkMovies> GetUsersBookmarksTitles(string username, int page, int pagesize)
        {
            return ctx.bookmarkMovies.Where(u => u.UserName == username).Skip(page * pagesize).Take(pagesize).ToList();
        }

        public IList<BookmarkActors> GetUsersBookmarksActors(string username, int page, int pagesize)
        {
            return ctx.bookmarkActors.Where(u => u.UserName == username).Skip(page * pagesize).Take(pagesize).ToList();
        }
    }
}