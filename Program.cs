using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using UUIDNext;
using static Project.Utils;

namespace Project;

class Program
{
  static void Main()
  {
    /*─────────────────────────────────────╮
    │ Creating the database context object │
    ╰─────────────────────────────────────*/
    var databaseContext = new DatabaseContext();

    /*───────────────────────────╮
    │ Creating the server object │
    ╰───────────────────────────*/
    var server = new HttpListener();
    server.Prefixes.Add("http://*:5000/");
    server.Start();

    Console.WriteLine("Server started. Listening for requests...");
    Console.WriteLine("Main page on http://localhost:5000/website/index.html");

    /*─────────────────────────╮
    │ Processing HTTP requests │
    ╰─────────────────────────*/
    while (true)
    {
      /*────────────────────────────╮
      │ Waiting for an HTTP request │
      ╰────────────────────────────*/
      var serverContext = server.GetContext();
      var response = serverContext.Response;

      try
      {
        /*────────────────────────╮
        │ Handeling file requests │
        ╰────────────────────────*/
        serverContext.ServeFiles();

        /*───────────────────────────╮
        │ Handeling custome requests │
        ╰───────────────────────────*/
        HandleRequests(serverContext, databaseContext);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        Console.WriteLine(e.StackTrace);
      }

      /*───────────────────────────────╮
      │ Saving changes to the database │
      ╰───────────────────────────────*/
      databaseContext.SaveChanges();

      /*───────────────────────────────────╮
      │ Sending the response to the client │
      ╰───────────────────────────────────*/
      response.Close();
    }
  }

  static void HandleRequests(HttpListenerContext serverContext, DatabaseContext databaseContext)
  {
    var request = serverContext.Request;
    var response = serverContext.Response;

    string absPath = request.Url!.AbsolutePath;

    if (absPath == "/signUp")
    {
      (string username, string password) = request.GetBody<(string, string)>();

      var userId = Uuid.NewDatabaseFriendly(Database.SQLite).ToString();

      var user = new User(userId, username, password);
      databaseContext.Users.Add(user);

      response.Write(userId);
    }

    else if (absPath == "/autoLogIn")
    {
      string userId = request.GetBody<string>();

      User user = databaseContext.Users.Find(userId)!;

      response.Write(new { username = user.Username });
    }

    else if (absPath == "/logIn")
    {
      (string username, string password) = request.GetBody<(string, string)>();

      User user = databaseContext.Users.First(
        u => u.Username == username && u.Password == password
      )!;

      response.Write(user.Id);
    }

    else if (absPath == "/addBook")
    {
      (string title, string imageSource, string description) =
        request.GetBody<(string, string, string)>();

      Book book = new Book(title, imageSource, description);

      databaseContext.Books.Add(book);
    }

    else if (absPath == "/getBook")
    {
      int bookId = request.GetBody<int>();

      Book book = databaseContext.Books.Find(bookId)!;

      var data = new
      {
        title = book.Title,
        imageSource = book.ImageSource,
        description = book.Description,
      };

      response.Write(data);
    }

    else if (absPath == "/getPreviews")
    {
      var previews = databaseContext.Books.Select(
        book => new
        {
          id = book.Id,
          title = book.Title,
          imageSource = book.ImageSource,
        }
      ).ToArray();

      response.Write(previews);
    }

    else if (absPath == "/getIsFavorite")
    {
      (string userId, int bookId) = request.GetBody<(string, int)>();

      bool isFavorite = databaseContext.Favorites.Any(
        f => f.UserId == userId && f.BookId == bookId
      );

      response.Write(isFavorite);
    }

    else if (absPath == "/addToFavorites")
    {
      (string userId, int bookId) = request.GetBody<(string, int)>();

      Favorite userFavorite = new Favorite(userId, bookId);

      databaseContext.Favorites.Add(userFavorite);
    }

    // else if (absPath == "/getFavorites") {
    //   string userId = request.GetBody<string>();

    //   Favorite[] favorites = [.. databaseContext.Favorites.Where(f => f.UserId == userId)];
    // }

    else if (absPath == "/removeFromFavorites")
    {
      (string userId, int bookId) = request.GetBody<(string, int)>();

      Favorite favorite = databaseContext.Favorites.First(
        f => f.UserId == userId && f.BookId == bookId
      );

      databaseContext.Favorites.Remove(favorite);
    }
  }
}

class DatabaseContext : DbContextWrapper
{
  public DbSet<User> Users { get; set; }
  public DbSet<Book> Books { get; set; }
  public DbSet<Favorite> Favorites { get; set; }

  public DatabaseContext() : base("Database") { }
}

class User(string id, string username, string password)
{
  [Key]
  public string Id { get; set; } = id;
  public string Username { get; set; } = username;
  public string Password { get; set; } = password;
  public int[] Favorites = [];
}

class Book(string title, string imageSource, string description)
{
  [Key]
  public int Id { get; set; }
  public string Title { get; set; } = title;
  public string ImageSource { get; set; } = imageSource;
  public string Description { get; set; } = description;
}

class Favorite(string userId, int bookId)
{
  [Key]
  public int Id { get; set; }

  public string UserId { get; set; } = userId;
  public int BookId { get; set; } = bookId;

  [ForeignKey("UserId")]
  public User? User { get; set; }

  [ForeignKey("BookId")]
  public Book? Book { get; set; }
}
