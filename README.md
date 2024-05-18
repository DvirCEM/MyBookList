# הרצת הפרוייקט

על מנת להריץ את השרת מבלי להצטרך לפתוח את סביבת הפיתוח כמנהל, ניתן לאפשר גישה לפורט רצוי (במקרה שלנו, פורט 5000) בעזרת פתיחת שורת המשימות כמנהל והרצת הפקודה:

```
netsh http add urlacl url=http://*:5000/ user=Everyone
```
לאחר מכן נפתח את סביבת הפיתוח ונריץ את הפרוייקט בעזרת הפקודה:
```
dotnet run
```
# שמירת ושליפת מידע מה-Database
![Untitled-2023-10-12-1102](https://github.com/DvirCEM/MyBookList/assets/167659124/27df0da7-988b-4e88-9d19-655800e3027a)


# מערכת יצירת המשתמש \ כניסה למשתמש

![329591193-f4fd23a9-a07e-41b3-a72b-e94b45b2dd05](https://github.com/DvirCEM/MyBookList/assets/167659124/ab0deaeb-3813-45ee-91d6-8956a6dff7c1)

# שמירת רשימת מידע ב-Database
הדרך האינטואיטיבית לשמירה של רשימת מידע כמו ספרים אהובים על כל משתמש או רשימת מצרכים על כל מתכון היא שימוש במערכים, אך ב-databases מסוג SQL המידע נשמר בטבלאות עם מספר שדות מוגדר לכל רשומה, לכן אנחנו לא יכולים לשמור מערכים.

## דפוס One-to-Many
במקום, נשתמש בדפוס בשם One-to-Many.
לדוגמה, אם אנחנו רוצים לאחסן מתכונים, כאשר לכל מתכון יש מצרכים משלו, נכתוב זאת בצורה כזו:

```
class Recipe(string name, string imageSource, string description) {
  [Key]
  public int Id { get; set; }
  public string Name { get; set; } = name;
  public string ImageSource { get; set; } = imageSource;
  public string description { get; set; } = description;
}

class Ingredient(int recipeId, string name, float litersAmount) {
  [Key]
  public int Id { get; set; }
  public string Name { get; set; } = name;
  public float LitersAmount { get; set; } = litersAmount;

  public int RecipeId { get; set; } = recipeId;

  [ForeignKey("RecipeId")]
  public Recipe? Recipe { get; set; }
}
```


המצרכים מאוחסנים בטבלה נפרדת, כך שמה שמקשר בין המצרך למתכון אליו הוא שייך הוא המפתח של המתכון.

שימו לב שמעבר להוספת המפתח של המתכון כשדה למצרך, אנחנו גם מגדירים אותו כמפתח זר (Foreign Key) בעזרת השורות:

סימון שדה כמפתח זר מגדירה ל-database שניתן למחוק את הרשומה באופן אוטומטי כאשר הרשומה המקורית שמחזיקה במפתח נמחקת.

## דפוס Many-to-Many
במקרה כמו איחסון רשימת ספרים מועדפים לכל משתמש, כאשר כל ספר מועדף יכול להופיע ברשימת המועדפים של מספר משתמשים, יש להשתמש בדפוס Many-to-Many:
```
class User(string id, string username, string password)
{
  [Key]
  public string Id { get; set; } = id;
  public string Username { get; set; } = username;
  public string Password { get; set; } = password;
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
```
כיוון שלכל משתמש יכולים להיות מספר ספרים אהובים, ולכל ספר יכולים להיות מספר משתמשים שסימנו אותו כאהוב, עלינו להוסיף טבלה שלישית המכילה גם את המפתח של המשתמש וגם את המפתח של הספר כמפתחות זרים.
