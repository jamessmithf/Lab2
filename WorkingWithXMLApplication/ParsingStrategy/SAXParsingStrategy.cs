using System.Xml;

namespace WorkingWithXMLApplication.ParsingStrategy
{
    public class SAXParsingStrategy : IParsingStrategy
    {
        public Schedule Parse(string selectedFilePath)
        {
            var schedule = new Schedule { Courses = new List<Course>() };
            Course currentCourse = null;
            Instructor currentInstructor = null;
            Student currentStudent = null;

            using (XmlReader reader = XmlReader.Create(selectedFilePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "Course":
                                currentCourse = new Course
                                {
                                    Students = new List<Student>()
                                };
                                break;
                            case "Title":
                                reader.Read(); // Move to the text node
                                if (currentCourse != null)
                                    currentCourse.Title = reader.Value.Trim();
                                break;
                            case "Instructor":
                                currentInstructor = new Instructor();
                                break;
                            case "FullName":
                                reader.Read();
                                if (currentInstructor != null)
                                    currentInstructor.FullName = reader.Value.Trim();
                                else if (currentStudent != null)
                                    currentStudent.FullName = reader.Value.Trim();
                                break;
                            case "Faculty":
                                reader.Read();
                                if (currentInstructor != null)
                                    currentInstructor.Faculty = reader.Value.Trim();
                                break;
                            case "Department":
                                reader.Read();
                                if (currentInstructor != null)
                                    currentInstructor.Department = reader.Value.Trim();
                                break;
                            case "Room":
                                reader.Read();
                                if (currentCourse != null)
                                    currentCourse.Room = reader.Value.Trim();
                                break;
                            case "ScheduleTime":
                                reader.Read();
                                if (currentCourse != null)
                                    currentCourse.ScheduleTime = reader.Value.Trim();
                                break;
                            case "Student":
                                currentStudent = new Student();
                                break;
                            case "Group":
                                reader.Read();
                                if (currentStudent != null)
                                    currentStudent.Group = reader.Value.Trim();
                                break;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        switch (reader.Name)
                        {
                            case "Instructor":
                                if (currentCourse != null)
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
            using (StreamWriter writer = new StreamWriter("C:\\Users\\vikto\\source\\repos\\Lab2\\WorkingWithXMLApplication\\Temp\\log.txt", false))
            {
                foreach (var course in schedule.Courses)
                {
                    foreach (var student in course.Students)
                    {
                        writer.WriteLine(student.FullName + " " + student.Group);
                    }
                }
            }

            return schedule;
        }
    }
}
