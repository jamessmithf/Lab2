using System.Xml.Linq;

namespace WorkingWithXMLApplication.ParsingStrategy
{
    public class LINQParsingStrategy : IParsingStrategy
    {
        public Schedule Parse(string SelectedFilePath)
        {
            XDocument xml = XDocument.Load(SelectedFilePath);
            var schedule = new Schedule { Courses = new List<Course>() };

            foreach (var courseElement in xml.Descendants("Course"))
            {
                var course = new Course
                {
                    Title = (string)courseElement.Element("Title"),
                    Instructor = new Instructor
                    {
                        FullName = (string)courseElement.Element("Instructor")?.Element("FullName"),
                        Faculty = (string)courseElement.Element("Instructor")?.Element("Faculty"),
                        Department = (string)courseElement.Element("Instructor")?.Element("Department")
                    },
                    Room = (string)courseElement.Element("Room"),
                    ScheduleTime = (string)courseElement.Element("ScheduleTime"),
                    Students = courseElement.Descendants("Student").Select(s => new Student
                    {
                        FullName = (string)s.Element("FullName"),
                        Group = (string)s.Element("Group")
                    }).ToList()
                };
                schedule.Courses.Add(course);

            }

            return schedule;
        }
    }
}
