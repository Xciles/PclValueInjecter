using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    [TestFixture]
    public class Noemata
    {
        [Test]
        public void Test()
        {
            var projects = new List<Project>();

            var bigprojects = new[] { new BigProject { Name = "Theta", Cost = 99, Percent = 20 },
                                      new BigProject { Name = "Phi", Cost = 18, Percent = 47 } };
            projects = ProjectMapper.Map(bigprojects).ToList();

            projects[0].Name.IsEqualTo(bigprojects[0].Name);
            projects[0].Cost.IsEqualTo((double)bigprojects[0].Cost);
            projects[0].Percent.IsEqualTo((double?)bigprojects[0].Percent);
            projects[1].Name.IsEqualTo(bigprojects[1].Name);
        }

        public class Project
        {
            public double Cost { get; set; }
            public double? Percent { get; set; }
            public string Name { get; set; }
        }

        public class BigProject
        {
            public int Cost { get; set; }
            public int Percent { get; set; }
            public string Name { get; set; }
        }

        
        public class IntToDouble : LoopValueInjection<int, double>
        {
            protected override double SetValue(int sourcePropertyValue)
            {
                return sourcePropertyValue;
            }
        }

        public class IntToNDouble : LoopValueInjection<int, double?>
        {
            protected override double? SetValue(int sourcePropertyValue)
            {
                return sourcePropertyValue;
            }
        }

        public static class ProjectMapper
        {
            public static IEnumerable<Project> Map(BigProject[] bigProjects)
            {
                foreach (var bigProject in bigProjects)
                {
                    var p = new Project();
                    p.InjectFrom(bigProject)
                        .InjectFrom<IntToDouble>(bigProject)
                        .InjectFrom<IntToNDouble>(bigProject);
                    yield return p;
                }
            }
        }
    }
}