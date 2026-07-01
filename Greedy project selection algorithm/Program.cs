class Program
{
    public class Project
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Profit { get; set; }

        public Project(DateTime startDate, DateTime endDate, double profit)
        {
            StartDate = startDate;
            EndDate = endDate;
            Profit = profit;
        }
    }

    static void Main() // Space: O(N) | Time: O(N^2)
    {
        Project project1 = new Project(new DateTime(2026, 1, 1), new DateTime(2026, 1, 15), 1000);
        Project project2 = new Project(new DateTime(2026, 2, 1), new DateTime(2026, 2, 20), 1500);
        Project project3 = new Project(new DateTime(2026, 3, 1), new DateTime(2026, 3, 10), 800);
        Project project4 = new Project(new DateTime(2026, 1, 1), new DateTime(2026, 1, 15), 2000);

        List<Project> projects = [project1, project2, project3, project4]; // Space: O(N) | Time: O(1)

        List<Project> bestProjects = SelectMaxProfitProjects(projects); // Space: O(N) | Time: O(N^2)

        Console.WriteLine("Selected Projects:");
        foreach (Project project in bestProjects) // Time: O(K) where K <= N
        {
            Console.WriteLine($"Start Date: {project.StartDate.ToShortDateString()}, End Date: {project.EndDate.ToShortDateString()}, Profit: {project.Profit}");
        } 
    }

    // Space: O(N) auxiliary | Time: O(N^2) worst-case
    static List<Project> SelectMaxProfitProjects(List<Project> projects)
    {
        // LINQ sorting takes O(N log N) time and allocates an O(N) auxiliary list
        var sortedProjects = projects.OrderByDescending(p => p.Profit).ToList(); // Space: O(N) | Time: O(N log N)

        List<Project> bestProjects = new List<Project>(); 

        foreach (Project project in sortedProjects) // Loop runs N times -> Time: O(N)
        {
            // Nested conflict check takes O(K) time based on the current size of bestProjects
            if (!HasTimeConflict(project, bestProjects)) 
            {
                bestProjects.Add(project); 
            }
        }

        return bestProjects;
    }

    // Space: O(1) | Time: O(K) where K is the number of accepted projects
    static bool HasTimeConflict(Project project, List<Project> bestProjects)
    {
        foreach (Project approved in bestProjects) // Loop runs K times
        {
            // Logical validation evaluating chronological interval overlap
            if (project.StartDate < approved.EndDate && project.EndDate > approved.StartDate) 
            {
                return true; 
            }
        }
        return false; 
    }
}