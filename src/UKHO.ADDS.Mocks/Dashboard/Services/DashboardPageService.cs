using UKHO.ADDS.Mocks.Dashboard.Models;

namespace UKHO.ADDS.Mocks.Dashboard.Services
{
    public class DashboardPageService
    {
        private readonly DashboardPage[] _allPages =
        {
            new DashboardPage { Name = "Services", Path = "/" }, new DashboardPage
            {
                Name = "Explorer",
                Path = "/_dashboard/explorer",
                Title = "",
                Description = ""
            },
            new DashboardPage
            {
                Name = "Traffic",
                Path = "/_dashboard/traffic",
                Title = "",
                Description = ""
            },
            new DashboardPage
            {
                Name = "File systems",
                Path = "/_dashboard/filesystem",
                Title = "",
                Description = ""
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

        public string DescriptionFor(DashboardPage example) => example?.Description ?? "";
    }
}
