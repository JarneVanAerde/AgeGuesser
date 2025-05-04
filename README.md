## Summary

A simple application to demonstrate the new [`HybridCache`](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-9.0) introduced with .NET 9. This new caching mechanism aims to replace the `MemoryCache` and `DistributedCache`.

## Setup

Run the following commands:
1.  `docker pull redis`
2.  `docker run --name local-redis -p 6379:6379 -d redis`

Inside the [AgeGuesser.Api/AgeGuesser.Api.http](https://github.com/JarneVanAerde/AgeGuesser/blob/eedf4273b10d480221dadfa22df82cb2e0ad1837/AgeGuesser.Api/AgeGuesser.Api.http) you can call the endpoint for guessing your age by passing a name via the query parameter.

You're good to go 🎉
