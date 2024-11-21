using System.Text.RegularExpressions;

namespace WorkingWithXMLApplication.ParsingStrategy
{
    public static class ScheduleFilter
    {
        // Допоміжний метод, який спростить використання фільтрів
        private static string[] SplitIntoWords(string? input)
        {
            // Використовуємо регулярний вираз для пошуку слів і розділових знаків
            if (input != null)
            {
                var regex = new Regex(@"[\w'-]+|[^\w\s]+");
                return regex.Matches(input).Select(m => m.Value).ToArray();
            }
            else
                return new string[] { };
        }

        /// <summary>
        /// Returns true if the 'userFilter' has simmilar parts with 'attributesValue'
        /// </summary>
        private static bool IsSimilarParts(string? userFilter, string? attributesValue)
        {
            // Розбиваємо на слова і зводимо до нижнього регістру для порівняння
            var splitedUserFilter = SplitIntoWords(userFilter?.ToLower());
            var splitedAtributesValue = SplitIntoWords(attributesValue?.ToLower());

            foreach (string filterPart in splitedUserFilter)
            {
                foreach (string attributePart in splitedAtributesValue)
                {
                    // Перевіряємо, чи є filterPart префіксом attributePart
                    if (attributePart.StartsWith(filterPart))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Schedule FilterSchedule(Schedule schedule, 
            string? instructorName = null, 
            string? time = null, 
            string? courseTitle = null, 
            string? room = null,
            string? day = null)
        {
            // Якщо жоден критерій фільтрації не вказаний, повертаємо початковий розклад
            if (string.IsNullOrEmpty(instructorName) && string.IsNullOrEmpty(time) && string.IsNullOrEmpty(courseTitle) 
                && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(day))
            {
                return schedule;
            }

            // Відфільтруємо курси за критеріями, якщо вони вказані
            var filteredCourses = schedule.Courses.Where(course =>
            {
                bool matchesInstructor = string.IsNullOrEmpty(instructorName) || IsSimilarParts(instructorName, course.Instructor.FullName);
                bool matchesTime = string.IsNullOrEmpty(time) || (course.TimeInterval[0] <= TimeOnly.Parse(time.Trim()) & course.TimeInterval[1] >= TimeOnly.Parse(time.Trim()));
                bool matchesCourseName = string.IsNullOrEmpty(courseTitle) || IsSimilarParts(courseTitle, course.Title);
                bool matchesRoom = string.IsNullOrEmpty(room) || IsSimilarParts(room, course.Room);
                bool matchesDay = string.IsNullOrEmpty(day) || IsSimilarParts(day, course.Day);

                // Курс відповідає, якщо всі задані критерії задовольняють умову
                return matchesInstructor && matchesTime && matchesCourseName && matchesRoom && matchesDay;
            }).ToList();

            // Повертаємо новий об'єкт розкладу з відфільтрованими курсами
            return new Schedule { Courses = filteredCourses };
        }
    }
}
