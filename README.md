# Statsd.Net #

A network service (daemon) that listens for metrics such as counters and timers and aggregates them for backend services.

This project is a .NET implementation of the [StatsD project by Etsy](https://github.com/etsy/statsd), which in turn is based on the work of [Cal Henderson at Flickr.](http://code.flickr.com/blog/2008/10/27/counting-timing/)

## Concepts ##

- *buckets*: Each metric is in its own "bucket". Buckets are created when used and do not need to be preconfigured.
- *values*: Each metric has a value. These are interpreted depending on modifiers, such as sampling intervals.
- *flush*: The service will flush a snapshot of aggregated metrics to an upstream backend service on a regular, configurable interval

## Architecture ##

- *frontend*: A mechanism for receiving packets to be processed by the middleware pipeline. The Statsd.Net service can have multiple frontends, such as Udp, Tcp, Websockets or cloud sources such as Azure EventHub.
- *middleware*: A chain of systems that receives a packet from the frontend for processing. The Statsd.Net Service is a middleware component. Other middleware can be plugged in, such as systems to capture received packets for storage or relaying to other targets.
- *backend*: Backends receive aggregated metrics from the Statsd.Net service. There can be multiple backends, such as a graphing service, storage services or other relay endpoints.
- *hosting*: Statsd.Net can be hosted anywhere, such as in a Console application, a Windows Service or in a cloud platform such as Azure Worker roles.

## Usage ##

Statsd.Net currently implements the protocol described by [Etsy's StatsD implementation](https://github.com/etsy/statsd).

Metrics are specified in the format:

    <metricName>:<value>|<metricType>[|@<sampleInterval>]

Statsd.Net implements the following metric types:

- *counters (c)*: Value is summed with the existing value in the bucket. The optional sampleInterval argument will scale the value.
- *gauges (g)*: Value replaces the existing one in the bucket. Values can have an option prefix of + or - which acts to modify the bucket value instead of replacing it.
- *sets (s)*: The value is added to the set. Sets contain unique values.
- *timer (ms)*: The value is treated as a timestamp. Note that histograms are not currently supported by Statsd.Net.

## Client ##

This project provides a simple strongly-typed client which allows you to send metrics from .NET. The client has a pluggable *ITransport* interface, which will allow the creation of factories for any frontend that metrics need to be sent to. 

An example of the client use is as follows:


    var statsdClient = await TcpClientFactory.CreateClient(new IPEndPoint(IPAddress.Loopback, 8125));
    statsdClient.AddCounter("testcounter", rng.Next(0, 1000));
    statsdClient.SetGauge("itemssent", itemsSent++);



## Status ##

Statsd.Net is in a very early alpha state. 

The following features are planned:

- *Histograms*: Fully implement the Timer metric type to maintain min/max/averages.

- *Azure*: A separate project to provide Azure hosting, frontend, middleware and backend components. This will include allowing EventHub to be used as a frontend source, a backend metrics target and as a middleware to provide an event stream for raw packets. 

- *ASP.NET*: A separate project to provide  an ASP.NET-based RESTful API and WebSocket frontend implementation.

- *Backends*: Provide various backends, such as Graphite, ElasticSearch, Redis, etc.

- *Improvements*: Other improvements, such as tracing and other debug/monitoring features.
