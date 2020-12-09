![.NET Core](https://github.com/BooTheHamster/iznakurnoz.Bot/workflows/.NET%20Core/badge.svg)

# iznakurnoz.Bot
Бот для telegram для домашнего сервера на .Net Core.


#### Возможные проблемы и способы их устранения:
* При инициализации OmniSharp в Visual Studio Code возникает ошибка: "The SDK 'Microsoft.NET.Sdk.Web' specified could not be found."
> Создать переменную среды **MSBuildSDKsPath** в которой указать путь до Net Core SDK, например, "C:\Program Files\dotnet\sdk\3.1.401\Sdks".
