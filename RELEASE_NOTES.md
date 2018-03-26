#### 0.1.1 March 26 2018 ####
Fixed the following issues:

* [HTTP Reporting actor must cancel its task upon termination](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/13)
* [Add mechanism to terminate the Zipkin HTTP Reporter actor upon Dispose](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/12)
* [Allow HttpReportingActor to consume logging settings from outside ActorSystem](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/11)

#### 0.1.0 March 26 2018 ####
Initial release of Petabridge.Tracing.Zipkin.

We built this driver in order to provide a commercially supported, production-grade [Zipkin](https://zipkin.io/) driver for .NET Core that supports the [OpenTracing](http://opentracing.io/) standards. We felt that the engineering quality on the other Zipkin drivers, including the official Zipkin C# driver, was amateur.

This driver is built to be extremely memory-efficient, thread-safe, and highly concurrent - it's powered by [Akka.NET](http://getakka.net/) actors under the hood.