# KumulosSdkXamarin

Kumulos provides tools to build and host backend storage for apps, send push notifications, view audience and behavior analytics, and report on adoption, engagement and performance.

## Get Started with NuGet

Add the SDK package to your project via the package manager console by entering `Install-Package Com.Kumulos`.

After installation, you can now import & initialize the SDK with:

```csharp
using KumulosSDK.DotNet;
using KumulosSDK.DotNet.Abstractions;

var config = Kumulos.CurrentConfig.AddKeys("YOUR_API_KEY", "YOUR_SECRET_KEY");
Kumulos.Current.Initialize(config);
```

For more information on integrating the Xamarin SDK with your project, please see the [Kumulos Xamarin integration guide](https://docs.kumulos.com/integration/xamarin).

## Contributing

Pull requests are welcome for any improvements you might wish to make. If it's something big and you're not sure about it yet, we'd be happy to discuss it first. You can either file an issue or drop us a line to [support@kumulos.com](mailto:support@kumulos.com).

## License

This project is licensed under the MIT license with portions licensed under the BSD 2-Clause license. See our LICENSE file and individual source files for more information.
