# Cpbp
Command prompt boilerplate

Cpbp is a cli (command line interface) application infrastructure.

#### Simple Usage

To use Cpbp in a simple way, create an application section that inherits the ```Cpbp.Contracts.CpbpApplication``` class and add the codes that you want to execute in that application section.
You should create a ```... ApplicationHandler``` class that inherits the ```Cpbp.Dependency.ICpbpApplicationHandler``` interface and add your code here.

Finally, just inherit the ```Program``` class, the main class of your console application, from ```CpbpProgram``` and call the ```ProgramStart``` method to send ```string [] args``` as a parameter in the ```Main``` method below.
```ProgramStart(args, new System.Reflection.Assembly[] { typeof(Program).Assembly });```

#### Module Usage

You can examine the sample project for module usage. the relevant documentation will be added soon.

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
