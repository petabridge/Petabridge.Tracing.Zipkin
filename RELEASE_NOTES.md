#### 0.5.0 January 1 2019 ###
Added support for the [new B3 "single header" propagation format](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/66), among other things.

To use the new B3 "single header" format for outbound writes, you'll want to set the following property on the `ZipkinTracerOptions` class:

```
var tracer = new ZipkinTracer(new ZipkinTracerOptions(new Endpoint("AaronsAppKafka"),
                ZipkinKafkaSpanReporter.Create(new ZipkinKafkaReportingOptions(new[] {"localhost:19092"},
                    debugLogging: true))){ Propagation =  });
```

#### 0.4.0 August 28 2018 ###
Added support for Zipkin's built-in Kafka span reporting capabilities, for users who are already considering operating at that kind of large scale.

You can access the Kafka reporter via the following syntax:

```
var tracer = new ZipkinTracer(new ZipkinTracerOptions(new Endpoint("AaronsAppKafka"),
                ZipkinKafkaSpanReporter.Create(new ZipkinKafkaReportingOptions(new[] {"localhost:19092"},
                    debugLogging: true))));
```

The `ZipkinKafkaSpanReporter.Create` method will give you the ability to specify the Kafka endpoint, topic, and batching settings used by Petabridge.Tracing.Zipkin for reporting spans back to Zipkin via a Kafka topic. Normally, however, all you'll need to specify is just a set of endpoints for Kafka brokers - both Zipkin and the Petabridge.Tracing.Zipkin client use the "zipkin" topic by default.

**Other Changes**
Petabridge.Tracing.Zipkin also introduces the following changes:

* [BugFix: calling ITracer.Inject with a NoOp span context causes cast exception](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/56)
* [BugFix: B3Propagator.Extract always returns a span context even when one isn't present](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/55)
* [Upgraded to use Akka.NET v1.3.9](https://github.com/akkadotnet/akka.net/releases/tag/v1.3.9)

#### 0.3.2 July 30 2018 ####
* [Implemented some missing OpenTracing v0.1.2 APIs](https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/51).

#### 0.3.1 July 13 2018 ####
* [Added `ExternalSampling`](https://github.com/petabridge/Petabridge.Tracing.Zipkin/pull/46) - which allows the driver to accept sampling decisions made outside of the driver itself.
* [Added NBench performance specifications](https://github.com/petabridge/Petabridge.Tracing.Zipkin/pull/47) and [Dockerized integration tests](https://github.com/petabridge/Petabridge.Tracing.Zipkin/pull/41).


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