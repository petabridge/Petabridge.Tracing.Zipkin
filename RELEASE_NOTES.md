
#### 0.3.0 June 12 2018 ####
* [Upgraded to Akka.NET v1.3.8](https://github.com/petabridge/Petabridge.Tracing.Zipkin/pull/42)
* [Upgraded to OpenTracing v0.1.2](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/38) - this is a major change that involves some breaking API changes.

#### 0.2.3 April 24 2018 ####
* [Adding support for external `IScopeManager` implementations to work inside `IZipkinSpanBuilder`](https://github.com/petabridge/Petabridge.Tracing.Zipkin/pull/32)

#### 0.2.2 April 23 2018 ####
* Upgraded to [Akka.NET v1.3.6](https://github.com/akkadotnet/akka.net/releases/tag/v1.3.6)
* [Fixed: Initial tags created by IZipkinSpanBuilder aren't actually passed to span](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/25)

#### 0.2.1 April 5 2018 ####
* Fixed a bug that could occur during sampling where a `NoOpSpanContext` could accidentally be added as a parent under some circumstances. In these instances, we now filter these illegal span types out.

#### 0.2.0 April 3 2018 ####
* Added support for sampling inside Petabridge.Tracing.Zipkin.
* Extracted `IZipkinSpan` and `IZipkinSpanContext` interfaces in order to make it easier to work with mocks and fakes during testing.

#### 0.1.1 March 26 2018 ####
Fixed the following issues:

* [HTTP Reporting actor must cancel its task upon termination](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/13)
* [Add mechanism to terminate the Zipkin HTTP Reporter actor upon Dispose](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/12)
* [Allow HttpReportingActor to consume logging settings from outside ActorSystem](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/11)

#### 0.1.0 March 26 2018 ####
Initial release of Petabridge.Tracing.Zipkin.

We built this driver in order to provide a commercially supported, production-grade [Zipkin](https://zipkin.io/) driver for .NET Core that supports the [OpenTracing](http://opentracing.io/) standards. We felt that the engineering quality on the other Zipkin drivers, including the official Zipkin C# driver, was amateur.

This driver is built to be extremely memory-efficient, thread-safe, and highly concurrent - it's powered by [Akka.NET](http://getakka.net/) actors under the hood.