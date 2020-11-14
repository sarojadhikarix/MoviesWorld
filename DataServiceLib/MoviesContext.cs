﻿using DataServiceLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Scaffolding.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataServiceLib
{
    public class MoviesContext : DbContext
    {
        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        public DbSet<TitleBasics> titleBasics { get; set; }
        public DbSet<SearchHistory> searchHistories { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<UsersFavorite> favorites { get; set; }
        public DbSet<BookmarkMovies> bookmarkMovies { get; set; }
        public DbSet<BookmarkActors> bookmarkActors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            var config = new ConfigurationBuilder()
                    .AddJsonFile("config.json")
                    .AddEnvironmentVariables()
                    .Build();

            optionsBuilder.UseLoggerFactory(loggerFactory);
            optionsBuilder.UseNpgsql(config["connectionString"]);
        }

        //For the string_search
        public IList<StringSearch> Search(string keyword)
        {
            var searchResult = new List<StringSearch>();

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "string_search";
                command.Parameters.Add(new Npgsql.NpgsqlParameter("s", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = keyword });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("uname", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = "adam12" });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Id = Convert.ToString(reader["id"]);
                        var Title = Convert.ToString(reader["title"]);

                        searchResult.Add(new StringSearch()
                        {
                            Id = Id,
                            Title = Title
                        });
                    }
                }

                return searchResult;
            }
        }

        //For the exact_match
        public IList<StringSearch> ExactSearch(string firstKeyword, string secondKeyword)
        {
            var searchResult = new List<StringSearch>();

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "exact_match";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("firstkeyword", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = firstKeyword });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("secondkeyword", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = secondKeyword });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("uname", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = "adam12" });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Id = Convert.ToString(reader["id"]);
                        var Title = Convert.ToString(reader["title"]);

                        searchResult.Add(new StringSearch()
                        {
                            Id = Id,
                            Title = Title
                        });
                    }
                }

                return searchResult;
            }
        }

        //For the bestmatch
        public IList<BestSearch> BestSearch(string firstKeyword, string secondKeyword, string thirdKeyword)
        {
            var searchResult = new List<BestSearch>();

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "bestmatch";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("w1", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = firstKeyword });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("w2", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = secondKeyword });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("w3", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = thirdKeyword });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("uname", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = "adam12" });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Id = Convert.ToString(reader["id"]);
                        var Title = Convert.ToString(reader["title"]);
                        int? Rank = Convert.ToInt32(reader["rank"]);

                        searchResult.Add(new BestSearch()
                        {
                            Id = Id,
                            Rank = (int)Rank,
                            Title = Title
                        });
                    }
                }

                return searchResult;
            }
        }

        //For the similar_movies
        public IList<SimilarMovies> SimilarMovies(string movieTitle)
        {
            var similarMovies = new List<SimilarMovies>();

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "similar_movies";
                command.Parameters.Add(new Npgsql.NpgsqlParameter("movietitle", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = movieTitle });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Id = Convert.ToString(reader["id"]);
                        var Title = Convert.ToString(reader["title"]);
                        var Genres = Convert.ToString(reader["genres"]);
                        var StartYear = Convert.ToString(reader["startyear"]);

                        similarMovies.Add(new SimilarMovies()
                        {
                            Id = Id,
                            Title = Title,
                            Genres = Genres,
                            StartYear = StartYear
                        });
                    }
                }

                return similarMovies;
            }
        }

        //For the popular actors
        public IList<PopularActors> PopularActors()
        {
            var actors = new List<PopularActors>();

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "show_popular_actors";

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Id = Convert.ToString(reader["id"]);
                        var Name = Convert.ToString(reader["name"]);
                        float Rating = Convert.ToInt32(reader["rating"]);
                        int NumVotes = Convert.ToInt32(reader["numvotes"]);

                        actors.Add(new PopularActors()
                        {
                            Id = Id,
                            Name = Name,
                            Rating = Rating,
                            NumOfVotes = NumVotes
                        });
                    }
                }

                return actors;
            }
        }

        //For add movie to bookmark
        public bool AddMovieBookMark(string userName, string movieId )
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "add_to_bookmark_movie";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = userName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputtconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = movieId });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }

        //For remove movie from bookmark
        public bool RemoveMovieBookMark(string userName, string movieId)
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "remove_from_bookmark_movie";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = userName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputtconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = movieId });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }

        //For add name to bookmark
        public bool AddNameBookMark(string userName, string personId)
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "add_to_bookmark_names";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = userName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputnconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = personId });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }

        //For remove name from bookmark
        public bool RemoveNameBookMark(string userName, string personId)
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "remove_from_bookmark_names";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = userName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputnconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = personId });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }

        //For add movie to fav
        public bool AddMovieFavorite(string userName, string movieId)
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "add_to_fav";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = userName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputtconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = movieId });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }

        //For remove movie from fav
        public bool RemoveMovieFavorite(string userName, string movieId)
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "remove_from_fav_movie";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = userName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputtconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = movieId });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }


        //For rating a movie
        public bool AddMovieRating(MovieRatingDto ratingDto)
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "rating_function_titles";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = ratingDto.UserName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputtconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = ratingDto.MovieId });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputrating", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = ratingDto.Rating });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }

        //For remove movie rating
        public bool RemoveMovieRating(MovieRatingDto ratingDto)
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "remove_movie_rating";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = ratingDto.UserName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputtconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = ratingDto.MovieId });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputrating", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = ratingDto.Rating });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }



        //For rating an actor rating
        public bool AddActorRating(ActorRatingDto actorRatingDto)
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "rating_function_names";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = actorRatingDto.UserName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputnconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = actorRatingDto.PersonId });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputrating", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = actorRatingDto.Rating });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }

        //For remove actor rating
        public bool RemoveActorRating(ActorRatingDto actorRatingDto)
        {

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "remove_actor_rating";

                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputusername", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = actorRatingDto.UserName });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputnconst", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = actorRatingDto.PersonId });
                command.Parameters.Add(new Npgsql.NpgsqlParameter("inputrating", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = actorRatingDto.PersonId });

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                var reader = command.ExecuteScalar();


                return (bool)reader;
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //For title_basics table
            modelBuilder.Entity<TitleBasics>().ToTable("title_basics");
            modelBuilder.Entity<TitleBasics>().Property(x => x.Id).HasColumnName("tconst").HasColumnType("varchar");
            modelBuilder.Entity<TitleBasics>().Property(x => x.TitleType).HasColumnName("titletype");
            modelBuilder.Entity<TitleBasics>().Property(x => x.PrimaryTitle).HasColumnName("primarytitle");
            modelBuilder.Entity<TitleBasics>().Property(x => x.OriginalTitle).HasColumnName("originaltitle");
            modelBuilder.Entity<TitleBasics>().Property(x => x.IsAdult).HasColumnName("isadult");
            modelBuilder.Entity<TitleBasics>().Property(x => x.StartYear).HasColumnName("startyear");
            modelBuilder.Entity<TitleBasics>().Property(x => x.EndYear).HasColumnName("endyear");
            modelBuilder.Entity<TitleBasics>().Property(x => x.RuntimeMinutes).HasColumnName("runtimeminutes");
            modelBuilder.Entity<TitleBasics>().Property(x => x.Genres).HasColumnName("genres");

            //for search_history table
            modelBuilder.Entity<SearchHistory>().ToTable("search_history");
            modelBuilder.Entity<SearchHistory>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<SearchHistory>().Property(x => x.UserName).HasColumnName("username");
            modelBuilder.Entity<SearchHistory>().Property(x => x.Keywords).HasColumnName("string_search");
            modelBuilder.Entity<SearchHistory>().Property(x => x.SearchNumber).HasColumnName("search_number");

            //usersFavorites
            modelBuilder.Entity<UsersFavorite>().ToTable("favorites").HasNoKey();
            modelBuilder.Entity<UsersFavorite>().Property(x => x.UserName).HasColumnName("username");
            modelBuilder.Entity<UsersFavorite>().Property(x => x.MovieId).HasColumnName("tconst");

            

            //For users table
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<User>().Property(x => x.UserName).HasColumnName("username");
            modelBuilder.Entity<User>().Property(x => x.Name).HasColumnName("name");
            modelBuilder.Entity<User>().Property(x => x.Email).HasColumnName("email");
            modelBuilder.Entity<User>().Property(x => x.Password).HasColumnName("password");
            modelBuilder.Entity<User>().Property(x => x.Salt).HasColumnName("salt");

            //For bookmark movies
            modelBuilder.Entity<BookmarkMovies>().ToTable("bookmarks_movies").HasNoKey();
            modelBuilder.Entity<BookmarkMovies>().Property(x => x.UserName).HasColumnName("username");
            modelBuilder.Entity<BookmarkMovies>().Property(x => x.MovieId).HasColumnName("tconst");

            //For bookmark movies
            modelBuilder.Entity<BookmarkActors>().ToTable("bookmarks_names").HasNoKey();
            modelBuilder.Entity<BookmarkActors>().Property(x => x.UserName).HasColumnName("username");
            modelBuilder.Entity<BookmarkActors>().Property(x => x.PersonId).HasColumnName("nconst");
        }
    }
}