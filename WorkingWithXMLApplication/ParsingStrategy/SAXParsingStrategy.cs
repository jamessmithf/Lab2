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
                                reader.Read(); // Move to text node
                                if (currentInstructor != null)
                                    currentInstructor.FullName = reader.Value.Trim();
                                else if (currentStudent != null)
                                    currentStudent.FullName = reader.Value.Trim();
                                break;

                            case "Student":
                                currentStudent = new Student
                                {
                                    Group = reader.GetAttribute("Group")
                                };
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
                                if (currentStudent != null && !string.IsNullOrWhiteSpace(currentStudent.FullName) && !string.IsNullOrWhiteSpace(currentStudent.Group))
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

            return schedule;
        }
    }
}
