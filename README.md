# BouncingRectangles

This solution created to achieve full understanding of gRPC streaming callbacks (like WCF callbacks) and to complete the test task.

The original file with test task is in the \docs folder.

# Build

To build solution you need Visual Studio Tools 2022 or Visual Studio 2022.

In root folder:

*msbuild*

OR

*dotnet build*

# Structure

This solution contains 3 modules: Server, WPF and Client.

## BouncingRectangles.Server

ASP.NET Core 6 project. Contains the main logic for bouncing rectangles.

## BouncingRectangles.WPF

.NET Core 6 WPF project. Contains logic to visualize bouncing rectangles generating on the server.

## BouncingRectangles.Client

Has a link to .proto file and required for client project to visualize the results. That project produces some temporary files for clients like it made in Swagger.