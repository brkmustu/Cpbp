using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using System.Collections.Generic;
using System.Reflection;

namespace Cpbp.Tests
{
    [TestClass]
    public class CpbpModuleTests
    {
        Assembly[] assemblies = new Assembly[] { typeof(CpbpModuleTests).Assembly };
        string[] args = new string[] { "TestApplication", "--parameter1", "value1" };


        [TestMethod]
        public void SetOptions_RequirementsTest()
        {
            Dictionary<string, string> applicationParameters =
                new Dictionary<string, string>
                {
                    { args[1], args[2] }
                };

            var cpbpModule = new CpbpModule()
                .SetOptions(
                    x =>
                    {
                        x.Requirements(
                                assemblies: assemblies,
                                args: args
                            );
                    }
                );

            Assert.AreEqual(assemblies, cpbpModule.Assemblies);
            Assert.AreEqual(applicationParameters.ContainsKey(args[1]), cpbpModule.ApplicationParameters.ContainsKey(args[1]));
            Assert.AreEqual(applicationParameters.ContainsValue(args[2]), cpbpModule.ApplicationParameters.ContainsValue(args[2]));
        }


        [TestMethod]
        public void SetOptions_WithSpecificArgumentSeperatorTest()
        {
            string argumentSeperator = "/";

            var cpbpModule = new CpbpModule()
                .SetOptions(
                    x =>
                    {
                        x.Requirements(
                                assemblies: assemblies,
                                args: args
                            );
                        x.WithArgumentSeperator(argumentSeperator);
                    }
                );

            Assert.AreEqual(argumentSeperator, cpbpModule.ArgumentSeperator);
        }

        [TestMethod]
        public void SetOptions_WithDefaultArgumentSeperatorTest()
        {
            string defaultArgumentSeperator = "-";

            var cpbpModule = new CpbpModule()
                .SetOptions(
                    x =>
                    {
                        x.Requirements(
                                assemblies: assemblies,
                                args: args
                            );
                    }
                );

            Assert.AreEqual(defaultArgumentSeperator, cpbpModule.ArgumentSeperator);
        }

        [TestMethod]
        public void SetOptions_WithContainerTest()
        {
            Container container = new Container();

            container.Register(typeof(Models.TestModel));

            var cpbpModule = new CpbpModule()
                .SetOptions(
                    x =>
                    {
                        x.Requirements(
                                assemblies: assemblies,
                                args: args
                            );
                        x.WithContainer(container);
                    }
                );

            Assert.AreEqual(container, cpbpModule.IocContainer);
        }
    }
}
