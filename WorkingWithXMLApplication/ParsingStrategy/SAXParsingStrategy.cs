using System.Xml;

namespace WorkingWithXMLApplication.ParsingStrategy
{
    public class SAXParsingStrategy : IParsingStrategy
    {
        public Schedule Parse(string selectedFilePath)
        {
            var schedule = new Schedule { Courses = new List<Course>() };

            Course? currentCourse = null;
            Instructor? currentInstructor = null;
            Student? currentStudent = null;

            using var reader = XmlReader.Create(selectedFilePath);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        HandleStartElement(reader, ref currentCourse, ref currentInstructor, ref currentStudent);
                        break;

                    case XmlNodeType.EndElement:
                        HandleEndElement(reader, ref schedule, ref currentCourse, ref currentInstructor, ref currentStudent);
                        break;
                }
            }

            return schedule;
        }

        private static void HandleStartElement(XmlReader reader, ref Course? currentCourse, ref Instructor? currentInstructor, ref Student? currentStudent)
        {
            switch (reader.Name)
            {
                case "Course":
                    currentCourse = new Course
                    {
                        Students = new List<Student>(),
                        Day = reader.GetAttribute("Day"),
                        Title = reader.GetAttribute("Title"),
                        Room = reader.GetAttribute("Room"),
                        ScheduleTime = reader.GetAttribute("ScheduleTime")
                    };
                    break;

                case "Instructor":
                    currentInstructor = new Instructor
                    {
                        Faculty = reader.GetAttribute("Faculty"),
                        Department = reader.GetAttribute("Department")
                    };
                    break;

                case "FullName":
                    reader.Read(); // Перехід до текстового вузла
                    var fullName = reader.Value.Trim();
                    if (currentInstructor != null)
                        currentInstructor.FullName = fullName;
                    else if (currentStudent != null)
                        currentStudent.FullName = fullName;
                    break;

                case "Student":
                    currentStudent = new Student
                    {
                        Group = reader.GetAttribute("Group")
                    };
                    break;
            }
        }

        private static void HandleEndElement(XmlReader reader, ref Schedule schedule, ref Course? currentCourse, ref Instructor? currentInstructor, ref Student? currentStudent)
        {
            switch (reader.Name)
            {
                case "Instructor":
                    if (currentCourse != null && currentInstructor != null)
                        currentCourse.Instructor = currentInstructor;
                    currentInstructor = null;
                    break;

                case "Student":
                    if (currentStudent != null &&
                        !string.IsNullOrWhiteSpace(currentStudent.FullName) &&
                        !string.IsNullOrWhiteSpace(currentStudent.Group))
                    {
                        currentCourse?.Students.Add(currentStudent);
                    }
                    currentStudent = null;
                    break;

                case "Course":
                    if (currentCourse != null)
                        schedule.Courses.Add(currentCourse);
                    currentCourse = null;
                    break;
            }
        }
    }
}
