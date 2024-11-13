namespace WorkingWithXMLApplication
{
    public class Schedule
    {
        public List<Course> Courses { get; set; }
    }

    public class Course
    {
        public string Title { get; set; }
        public Instructor Instructor { get; set; }
        public string Room { get; set; }
        public string ScheduleTime { get; set; }
        public List<Student> Students { get; set; }
    }

    public class Instructor
    {
        public string FullName { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
    }

    public class Student
    {
        public string FullName { get; set; }
        public string Group { get; set; }
    }
}
