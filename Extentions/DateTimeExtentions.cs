namespace Dating_App.Extentions;

public static class DateTimeExtentions
{
    public static int CalculateAge(this DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;

        if (dob.AddYears(age) > today.Date) age--;
        return age;
    }
}