using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Entities;
using Todo.Enums;
using Todo.Storage.Context;

namespace Todo.Data;

public static class DbSeeder
{
    private static readonly Random _random = new Random();
    
    private static readonly string[] FirstNames = 
    {
        "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda", "David", "Elizabeth",
        "William", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen",
        "Christopher", "Nancy", "Daniel", "Lisa", "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra",
        "Donald", "Ashley", "Steven", "Kimberly", "Paul", "Emily", "Andrew", "Donna", "Joshua", "Michelle",
        "Kenneth", "Dorothy", "Kevin", "Carol", "Brian", "Amanda", "George", "Melissa", "Edward", "Deborah"
    };

    private static readonly string[] LastNames = 
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
        "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
        "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
        "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
        "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts"
    };

    private static readonly string[] CategoryNames = 
    {
        "Work", "Personal", "Projects", "Learning", "Health", "Finance", "Shopping", "Travel", "Home", "Family",
        "Career", "Fitness", "Hobbies", "Social", "Volunteer", "Creative", "Technical", "Business", "Education", "Entertainment"
    };

    private static readonly string[] TagNames = 
    {
        "Urgent", "Important", "LowPriority", "HighPriority", "Research", "Development", "Design", "Testing", "Documentation",
        "Meeting", "Planning", "Review", "Analysis", "Implementation", "Deployment", "Maintenance", "Support", "Training",
        "Consultation", "Collaboration", "Innovation", "Optimization", "Refactoring", "Debugging", "Monitoring", "Security",
        "Performance", "Scalability", "Accessibility", "Usability", "Compliance", "Audit", "Backup", "Recovery", "Migration",
        "Integration", "Automation", "Workflow", "Process", "Strategy", "Leadership", "Management", "Communication"
    };

    private static readonly string[] ActivityTitles = 
    {
        "Complete project documentation", "Review pull requests", "Update database schema", "Fix critical bug",
        "Implement new feature", "Conduct team meeting", "Prepare presentation", "Analyze user feedback",
        "Optimize application performance", "Write unit tests", "Deploy to production", "Monitor system health",
        "Refactor legacy code", "Design user interface", "Create API endpoints", "Implement authentication",
        "Set up CI/CD pipeline", "Conduct code review", "Update dependencies", "Write technical specifications",
        "Design database architecture", "Implement caching strategy", "Create user documentation", "Conduct security audit",
        "Optimize database queries", "Implement error handling", "Set up logging system", "Create monitoring dashboard",
        "Conduct load testing", "Implement backup strategy", "Design RESTful API", "Create mobile responsive design",
        "Implement real-time features", "Set up analytics tracking", "Conduct user testing", "Write API documentation",
        "Implement search functionality", "Create data visualization", "Set up email notifications", "Implement file upload",
        "Design notification system", "Create admin panel", "Implement role-based access", "Conduct performance testing",
        "Optimize images and assets", "Implement lazy loading", "Create progress tracking", "Set up webhook handlers"
    };

    private static readonly string[] ActivityDescriptions = 
    {
        "Complete this task by the end of the week with proper documentation",
        "Review and provide feedback on the submitted changes",
        "Update the schema to support new requirements",
        "Fix the critical issue affecting production users",
        "Implement the new feature as per specifications",
        "Discuss project progress and blockers with the team",
        "Prepare slides for the upcoming client presentation",
        "Analyze user feedback and identify improvement areas",
        "Improve application response time and reduce latency",
        "Write comprehensive unit tests for the module",
        "Deploy the latest changes to production environment",
        "Monitor system metrics and ensure stability",
        "Refactor old code to improve maintainability",
        "Design intuitive and user-friendly interface",
        "Create RESTful endpoints for client consumption",
        "Implement secure authentication and authorization",
        "Set up automated build and deployment pipeline",
        "Review code changes and ensure quality standards",
        "Update project dependencies to latest versions",
        "Write detailed technical specifications",
        "Design scalable and efficient database structure",
        "Implement caching to improve performance",
        "Create comprehensive user guides and tutorials",
        "Conduct thorough security review and testing",
        "Optimize slow database queries and indexes",
        "Implement proper error handling and logging",
        "Set up centralized logging for all services",
        "Create real-time monitoring and alerting dashboard",
        "Test system performance under high load",
        "Implement automated backup and recovery",
        "Design scalable API architecture",
        "Ensure mobile responsiveness across devices",
        "Implement real-time data synchronization",
        "Set up analytics and user behavior tracking",
        "Conduct usability testing with real users",
        "Write comprehensive API documentation",
        "Implement advanced search and filtering",
        "Create interactive data visualization charts",
        "Set up automated email notification system",
        "Implement secure file upload functionality",
        "Design flexible notification system",
        "Create administrative control panel",
        "Implement role-based access control",
        "Test system performance under various conditions",
        "Optimize images and static assets for faster loading",
        "Implement lazy loading for better performance",
        "Create progress tracking and reporting system",
        "Set up webhook handlers for external integrations"
    };

    private static readonly string[] CommentMessages = 
    {
        "Making good progress on this task", "Need to review the requirements again", "This is almost complete",
        "Waiting for approval from stakeholders", "Started working on this yesterday", "Priority needs to be reassessed",
        "Dependencies are blocking progress", "Adding this to the sprint backlog", "Documentation needs to be updated",
        "Testing revealed some issues", "Performance improvements needed", "Security review completed successfully",
        "Ready for code review", "Deployment scheduled for next week", "User feedback has been incorporated",
        "This feature is working as expected", "Need to coordinate with other teams", "Estimates were accurate",
        "Technical debt identified in this area", "Refactoring improved code quality", "Integration testing passed",
        "Monitoring shows stable performance", "Backup procedures verified", "Migration completed successfully",
        "Load testing results are positive", "Error handling is robust", "Logging provides good visibility",
        "Dashboard is helpful for tracking", "Email notifications are working", "File upload functionality tested",
        "Notification system is responsive", "Admin panel is user-friendly", "Access control is secure",
        "Performance metrics are within SLA", "Optimization reduced response time by 50%", "Lazy loading improved UX",
        "Progress tracking is accurate", "Webhook handlers are reliable", "Code quality is high",
        "Testing coverage is comprehensive", "Documentation is thorough", "User feedback is positive",
        "Requirements are clear", "Timeline is realistic", "Resources are allocated appropriately",
        "Risks have been mitigated", "Communication is effective", "Collaboration is smooth"
    };

    public static async Task SeedDataAsync(ApplicationDbContext context, IPasswordHasher<UserEntity> passwordHasher)
    {
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("Database already contains data. Skipping seeding.");
            return;
        }

        Console.WriteLine("Starting database seeding with 50 records per entity...");

        var users = await SeedUsersAsync(context, passwordHasher, 50);
        var categories = await SeedCategoriesAsync(context, users, 50);
        var tags = await SeedTagsAsync(context, users, 50);
        var activities = await SeedActivitiesAsync(context, users, categories, tags, 50);
        await SeedCommentsAsync(context, users, activities, 50);

        Console.WriteLine("Database seeding completed successfully.");
    }

    private static async Task<List<UserEntity>> SeedUsersAsync(ApplicationDbContext context, IPasswordHasher<UserEntity> passwordHasher, int count)
    {
        Console.WriteLine($"Seeding {count} Users...");
        
        var users = new List<UserEntity>();
        var usedEmails = new HashSet<string>();

        for (int i = 0; i < count; i++)
        {
            string firstName, lastName, email;
            do
            {
                firstName = FirstNames[_random.Next(FirstNames.Length)];
                lastName = LastNames[_random.Next(LastNames.Length)];
                email = $"{firstName.ToLower()}.{lastName.ToLower()}{_random.Next(100, 999)}@example.com";
            } while (usedEmails.Contains(email));

            usedEmails.Add(email);

            var tempUser = new UserEntity();
            var user = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = firstName,
                MiddleName = _random.Next(10) > 7 ? GetRandomMiddleName() : null,
                LastName = lastName,
                Email = email,
                PasswordHash = passwordHasher.HashPassword(tempUser, "Password123!"),
                CreatedOn = DateTime.UtcNow.AddDays(-_random.Next(1, 365))
            };

            users.Add(user);
        }

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"Seeded {users.Count} users.");
        return users;
    }

    private static async Task<List<CategoryEntity>> SeedCategoriesAsync(ApplicationDbContext context, List<UserEntity> users, int count)
    {
        Console.WriteLine($"Seeding {count} Categories...");
        
        var categories = new List<CategoryEntity>();

        for (int i = 0; i < count; i++)
        {
            var category = new CategoryEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[_random.Next(users.Count)].Id,
                Name = CategoryNames[_random.Next(CategoryNames.Length)] + (_random.Next(10) > 5 ? $" {_random.Next(1, 10)}" : ""),
                CreatedOn = DateTime.UtcNow.AddDays(-_random.Next(1, 365))
            };

            categories.Add(category);
        }

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"Seeded {categories.Count} categories.");
        return categories;
    }

    private static async Task<List<TagEntity>> SeedTagsAsync(ApplicationDbContext context, List<UserEntity> users, int count)
    {
        Console.WriteLine($"Seeding {count} Tags...");
        
        var tags = new List<TagEntity>();

        for (int i = 0; i < count; i++)
        {
            var tag = new TagEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[_random.Next(users.Count)].Id,
                Name = TagNames[_random.Next(TagNames.Length)],
                CreatedOn = DateTime.UtcNow.AddDays(-_random.Next(1, 365))
            };

            tags.Add(tag);
        }

        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"Seeded {tags.Count} tags.");
        return tags;
    }

    private static async Task<List<ActivityEntity>> SeedActivitiesAsync(ApplicationDbContext context, List<UserEntity> users, List<CategoryEntity> categories, List<TagEntity> tags, int count)
    {
        Console.WriteLine($"Seeding {count} Activities...");
        
        var activities = new List<ActivityEntity>();
        var statuses = Enum.GetValues<ActivityStatus>();
        var priorities = Enum.GetValues<ActivityPriority>();

        for (int i = 0; i < count; i++)
        {
            var status = statuses[_random.Next(statuses.Length)];
            var createdDate = DateTime.UtcNow.AddDays(-_random.Next(1, 365));
            DateTime? startedOn = status == ActivityStatus.InProgress || status == ActivityStatus.Completed 
                ? createdDate.AddDays(_random.Next(0, 5)) 
                : null;
            DateTime? endedOn = status == ActivityStatus.Completed 
                ? startedOn?.AddHours(_random.Next(1, 48)) 
                : null;

            var activity = new ActivityEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[_random.Next(users.Count)].Id,
                Title = ActivityTitles[_random.Next(ActivityTitles.Length)],
                Description = ActivityDescriptions[_random.Next(ActivityDescriptions.Length)],
                Status = status,
                Priority = priorities[_random.Next(priorities.Length)],
                CategoryId = categories[_random.Next(categories.Count)].Id,
                StartedOn = startedOn,
                EndedOn = endedOn,
                CreatedOn = createdDate
            };

            // Add random tags (0-3 tags per activity)
            var tagCount = _random.Next(0, 4);
            var availableTags = tags.OrderBy(t => _random.Next()).Take(tagCount);
            foreach (var tag in availableTags)
            {
                activity.Tags.Add(tag);
            }

            activities.Add(activity);
        }

        await context.Activities.AddRangeAsync(activities);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"Seeded {activities.Count} activities.");
        return activities;
    }

    private static async Task SeedCommentsAsync(ApplicationDbContext context, List<UserEntity> users, List<ActivityEntity> activities, int count)
    {
        Console.WriteLine($"Seeding {count} Comments...");
        
        var comments = new List<CommentEntity>();

        for (int i = 0; i < count; i++)
        {
            var comment = new CommentEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[_random.Next(users.Count)].Id,
                ActivityId = activities[_random.Next(activities.Count)].Id,
                Message = CommentMessages[_random.Next(CommentMessages.Length)],
                CreatedOn = DateTime.UtcNow.AddDays(-_random.Next(1, 30))
            };

            comments.Add(comment);
        }

        await context.Comments.AddRangeAsync(comments);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"Seeded {comments.Count} comments.");
    }

    private static string GetRandomMiddleName()
    {
        var middleNames = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T" };
        return middleNames[_random.Next(middleNames.Length)];
    }
}
