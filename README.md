# Cpbp
Command prompt boilerplate

Cpbp is a cli (command line interface) application infrastructure.

#### Firstly

To use Cpbp in a simple way, create an application section that inherits the ```Cpbp.CpbpApplication``` class and add the codes that you want to execute in that application section.

    public class FirstApplication : Cpbp.CpbpApplication
    {
    }

You should create a ```... ApplicationHandler``` class that inherits the ```Cpbp.Core.ICpbpApplicationHandler``` interface and add your code here.

    public class FirstApplicationHandler : Cpbp.Core.ICpbpApplicationHandler<FirstApplication>
    {
        public void Handle(FirstApplication application)
        {
            Console.WriteLine("Hello from first application.");
        }
    }

### You can easily run your console application using fluent interface

            new CpbpModule()
                .SetOptions(
                    x =>
                    {
                        x.Requirements(
                                assemblies: assemblies,
                                args: args
                            );
                    }
                )
                .GetProgram()
                .Run();


