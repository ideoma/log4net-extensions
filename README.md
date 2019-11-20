# Petabyte Log4net Extensions

Extends log4net with MemoryMappedAppender for fast and reliable logging.

## Getting Started

Step 1. Install nuget package ```log4net.Petabyte.Extensions```

Step 2. Configure MemoryMappedAppender the same way as RollingFileAppender configured

```xml
<appender name="MemoryMappedAppender" type="log4net.Petabyte.MemoryMappedAppender,log4net.Petabyte.Extensions">
    <mappingBufferSize value="2MB"/>
    <file value="logfile" />
    <appendToFile value="true" />
    <rollingStyle value="Composite" />
    <datePattern value="yyyyMMdd" />
    <maxSizeRollBackups value="10" />
    <maximumFileSize value="10MB" />
    <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" />
    </layout>
    <root>
        <level value="INFO" />
        <appender-ref ref="MemoryMappedAppender" />
    </root>
</appender>

```

Note additional parameter mappingBufferSize, see more about it in Configuration section

Step 3. Use the logging in the usual way
```C#
var log = LogManager.GetLogger(GetType());
log.Info("message");
```

### Why Memory Mapped
In short - it is faster. 

When every line of logging hits disk eventialy it becomes a bottleneck 

### Prerequisites

What things you need to install the software and how to install them

```
Give examples
```

### Installing

A step by step series of examples that tell you how to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Billie Thompson** - *Initial work* - [PurpleBooth](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
