using UKHO.ADDS.Mocks.Dashboard.Models;

namespace UKHO.ADDS.Mocks.Dashboard.Services
{
    public class DashboardPageService
    {
        private readonly DashboardPage[] _allPages = new[]
        {
            new DashboardPage { Name = "Home", Path = "/", Icon = "\ue88a" },
            new DashboardPage
            {
                Name = "Explorer",
                Path = "/_dashboard/explorer",
                Title = "",
                Description = "",
                Icon = "\ue0c6"
            },
            new DashboardPage
            {
                Name = "Traffic",
                Path = "/_dashboard/traffic",
                Title = "",
                Description = "",
                Icon = "\ue0c6"
            }
        };

        public IEnumerable<DashboardPage> Pages => _allPages;
        
        public DashboardPage FindCurrent(Uri uri)
        {
            IEnumerable<DashboardPage> Flatten(IEnumerable<DashboardPage> e)
            {
                return e.SelectMany(c => c.Children != null ? Flatten(c.Children) : new[] { c });
            }

            return Flatten(Pages)
                        .FirstOrDefault(example => example.Path == uri.AbsolutePath || $"/{example.Path}" == uri.AbsolutePath);
        }

        public string TitleFor(DashboardPage example)
        {
            if (example != null && example.Name != "Overview")
            {
                return example.Title ?? "";
            }

            return "";
        }

        public string DescriptionFor(DashboardPage example)
        {
            return example?.Description ?? "";
        }
    }
}
