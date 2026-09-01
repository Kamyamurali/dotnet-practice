using System.IO;
Console.WriteLine("FILE IO Demo ");

#region create and write to a file
// //like a book
// FileStream myFile = new FileStream("myFile.txt", FileMode.Create, FileAccess.Write);
// //like a pen
// StreamWriter myPen = new StreamWriter(myFile);
// //to write 
// myPen.WriteLine("this is my book ");
// myPen.WriteLine("i like to write");
// string hobby = "";
// Console.WriteLine("Enter your hobby: ");
// hobby = Console.ReadLine();
// myPen.WriteLine(hobby);

// //close the pen
// myPen.Close();
// //if pen not closed, data will not be written to the file and memory won't be released
// myFile.Close();
// Console.WriteLine("File created successfully");
#endregion

#region read from a file
FileStream myBook = new FileStream("myFile.txt", FileMode.Open, FileAccess.Read);
StreamReader myReader = new StreamReader(myBook);
Console.WriteLine(myReader.ReadToEnd());
myReader.Close();
myBook.Close();
#endregion