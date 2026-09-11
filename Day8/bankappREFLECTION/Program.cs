using System.Reflection;

var myapp = Assembly.LoadFile("/Users/kamya/Desktop/dotnet-practice/Day8/bankLIB/bin/Debug/net10.0/bankLIB.dll");

Type[] myClasses = myapp.GetTypes();

for (int i = 0; i < myClasses.Length; i++)
{
    Console.WriteLine(i + " : " + myClasses[i].FullName);
}
Console.WriteLine("------------------------------------------");

Type myClass = myapp.GetType("bankLIB.MyMaths");

var obj = Activator.CreateInstance(myClass);

MethodInfo m = myClass.GetMethod("Add");

object[] parameters = new object[] { 10, 20 };

object result = m.Invoke(obj, parameters);

Console.WriteLine("Add(10, 20) = " + result);