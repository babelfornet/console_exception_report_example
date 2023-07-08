using System;
using Babel.Licensing;

class Program
{
    static BabelReporting reporting = new BabelReporting();

    // Assign an user key to allow this client to report exceptions
    static readonly string UserKey = "QGA6G-MTATL-2M798-4706W";

    static void Main()
    {
        // Configure Babel Reporting Service URL
        reporting.Configuration.ServiceUrl = "http://127.0.0.1:5005";

        reporting.BeforeSendReport += (s, e) => {

            // Set report custom properties
            e.Report.Properties.Add("cmdline", Environment.CommandLine);
            e.Report.Properties.Add("username", Environment.UserName);
        };

        while (true)
        {
            Console.WriteLine("Choose an option:");

            Console.WriteLine("1. DivideByZeroException");
            Console.WriteLine("2. IndexOutOfRangeException");
            Console.WriteLine("3. ArgumentNullException");
            Console.WriteLine("4. OutOfMemoryException");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    TryThrowException(() => new DivideByZeroExceptionWrapper().NestedMethod1());
                    break;
                case "2":
                    TryThrowException(() => new IndexOutOfRangeExceptionWrapper().NestedMethod1());
                    break;
                case "3":
                    TryThrowException(() => new ArgumentNullExceptionWrapper().NestedMethod1(null));
                    break;
                case "4":
                    TryThrowException(() => new OutOfMemoryExceptionWrapper().NestedMethod1());
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            Console.WriteLine();
        }
    }

    static async void TryThrowException(Action throwException)
    {
        try
        {
            throwException();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception thrown:");
            Console.WriteLine(ex.StackTrace);

            reporting.SendExceptionReport(UserKey, ex);
        }
    }
}

class DivideByZeroExceptionWrapper
{
    public void NestedMethod1()
    {
        NestedMethod2();
    }

    public void NestedMethod2()
    {
        NestedMethod3();
    }

    public void NestedMethod3()
    {
        int numerator = 10;
        int denominator = 0;
        int result = numerator / denominator; // Throws DivideByZeroException
    }
}

class IndexOutOfRangeExceptionWrapper
{
    public void NestedMethod1()
    {
        NestedMethod2(new int[5]);
    }

    public void NestedMethod2(int[] array)
    {
        NestedMethod3(array, 10);
    }

    public void NestedMethod3(int[] array, int index)
    {
        int value = array[index]; // Throws IndexOutOfRangeException
    }
}

class ArgumentNullExceptionWrapper
{
    public void NestedMethod1(string? value)
    {
        NestedMethod2(value);
    }

    public void NestedMethod2(string? value)
    {
        NestedMethod3(value?.Length);
    }

    public void NestedMethod3(int? length)
    {
        // Throws ArgumentNullException if value is null
        if (length == null)
            throw new ArgumentNullException(nameof(length));
    }
}

class OutOfMemoryExceptionWrapper
{
    public void NestedMethod1()
    {
        NestedMethod2();
    }

    public void NestedMethod2()
    {
        NestedMethod3();
    }

    public void NestedMethod3()
    {
        byte[] data = new byte[int.MaxValue]; // Throws OutOfMemoryException
    }
}
