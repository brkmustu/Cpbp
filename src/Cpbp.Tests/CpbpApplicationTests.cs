using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cpbp.Tests
{
    [TestClass]
    public class CpbpApplicationTests
    {
        Assembly[] assemblies = new Assembly[] { typeof(CpbpApplicationTests).Assembly };


        [TestMethod]
        public void Application_ExecutingTest()
        {
            new CpbpModule()
                .SetOptions(
                    x =>
                    {
                        x.Requirements(
                                assemblies: assemblies,
                                args: new string[] { "TestApplication", "--p1", "v1" }
                            );
                    }
                )
                .GetProgram()
                .Run();

            Assert.AreEqual(true, Models.TestParameters.IsWorked);
        }
    }
}
