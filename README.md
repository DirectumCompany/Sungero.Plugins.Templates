# Sungero.Plugins.Templates
В репозитории находятся шаблоны проектов для разработки плагина подписания к системе Directum RX.
Шаблоны проектов созданы в Microsoft Visual Studio 2017.

Базовая информация об использовании электронной подписи приведена в документации к системе.
Плагины подписания поддерживаются, начиная с Directum RX 3.3. Ограничения:
* Плагины подписания не поддерживаются в десктоп-клиенте.
* Клиентский плагин подписания поддерживается в:
  * Microsoft Windows 7 и выше
  * Microsoft Windows Server 2008 R2 и выше.
  * x64 версии Linux и MacOS, на которых можно запустить .NET Core версии 2.0 и выше (поддержка добавлена в Sungero 4.6.0.0090 и выше, 4.7.0.0097 и выше, 4.8.0.0060 и выше, 4.9 и выше) 


Совместимость с версией платформы Sungero:
* Ветка master - с Sungero 4.6.0.0090 и выше, 4.7.0.0097 и выше, 4.8.0.0060 и выше, 4.9 и выше.
* Ветка 4.8 - с Sungero 4.7 и до Sungero 4.8.0.0059 включительно.
* Ветка 4.6 - с Sungero 4.1 и до Sungero 4.6.0.0089 включительно.
* Ветка 4.0 - с Sungero 3.3.8.0019 и до Sungero 4.1.
* Ветка 3.3.8.0018 - с Sungero версии 3.3 до 3.3.8.0018 включительно.

## Как разработать плагин подписания
1. В проектах `ServerCryptographyPlugin`, `ClientCryptographyPlugin` измените имя сборки на свое (например, на `MyServerCryptographyPlugin`, `MyClientCryptographyPlugin`)

2. Сгенерируйте уникальный идентификатор (GUID) плагина (например на сайте https://www.guidgenerator.com/), пропишите его в свойстве `CryptographyPlugin.Id` класса серверного плагина и в файле `ClientPlugin.props` клиентского плагина;

3. Реализуйте **серверный плагин** (проект `ServerCryptographyPlugin`). Для этого: 
    * Реализуйте методы класса `Signer`: `SignData()`, `TryLoadPrivateKey()` и `VerifySignature()`. При необходимости модифицируйте остальные методы класса.
    * При необходимости модифицируйте методы класса `CryptographyPlugin`. Укажите нужный идентификатор алгоритма подписания в данном классе (поле `SignAlgorithmId`).
    * При необходимости создайте свой алгоритм хеширования с помощью класса `HashAlgorithm`.
    * Взаимодействие между классами описано в начале модуля `CryptographyPlugin.cs`.

4. Если планируется реализовать **плагин облачного подписания**, то:

    - Модифицируйте методы класса `CloudCryptographyPlugin` в проекте сервеного плагина.
    - Пропустите пункт реализации **клиентского плагина**.

5. Реализуйте **клиентский плагин** (проект `ClientCryptographyPlugin`). Для этого:

    * **Если требуется поддержка подписания на клиентских машинах с Linux и/или MacOS:**

      * В файле `ClientPlugin.props` клиентского плагина измените значение переменной `BuildLinuxPlugin` c `false` на `true` для Linux. Для MacOS, соответсвенно, измените значение переменной `BuildMacOsPlugin` c `false` на `true`. Возможна одновременная сборка пакетов для обеих целевых ОС. Для этого следует установить оба значения в `true`.

      * Скачайте с [сайта Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) актуальный пакет бинарных файлов `.NET Runtime` для целевой платформы. Целевыми платформами могут быть `Linux x64` и/или `MacOS x64`. Пример названия файла для Linux `dotnet-runtime-6.0.16-linux-x64.tar.gz`.  Для MacОS, соответственно, файл может называться `dotnet-runtime-6.0.16-osx-x64.tar.gz`.

        *Если `.NET Runtime` версии 6.x не поддерживается целевой платформой, то допустимо использование другой версии (2.0 и выше)*.

      * В зависимости от целевой ОС (Linux/MacOS) распакуйте, соответсвенно, содержимое файла(ов) в каталог `redist\linux-dotnet-runtime\` для Linux и/или `redist\macos-dotnet-runtime` для MacOS (после этого именно в нем, а не во вложенных каталогах, должны оказаться файл `dotnet`, каталог `host`  и каталог `shared`).

    * Модифицируйте методы класса `Signer`.

6. Соберите проект. В папке *out* в корне проекта появятся папки *Client* и *Server*, содержащие файлы клиентского и серверного плагинов соответственно.

7. Дополнительные библиотеки, требующие распространения вместе с плагином, включите в соответствующий проект. Они автоматически должны попасть в zip-архив. 

8. Подключите серверный плагин к Directum RX:
    * Создайте папку для хранения плагина, например, *D:\Plugins*. При обновлении системы Directum RX может изменяться содержимое ее папок. Поэтому рекомендуется создать отдельную папку.
    * В конфигурационных файлах *_ConfigSettings.yml* всех серверных компонент в параметре PLUGINS_ZIP_PATH укажите путь к папке с плагинами, например:  
    ```PLUGINS_ZIP_PATH: 'D:\Plugins'```
    * Скопируйте архив с серверным плагином в указанную папку (архив из папки `\out\Server\`)
    * Если необходимо передать дополнительные настройки в плагин, укажите их в тех же конфигурационных файлах *_ConfigSettings.yml*, где производилась настройка пути к папке с плагинами. Формат секции с настройками: 
      ```yaml
      PLUGINS:
        plugin:
          - '@id': '<ид_плагина>'
            '@exampleSetting': 'Example value'
            '@otherSetting': 'Other value'
      ```
      Чтение настроек в плагине выполняется в методе `CryptographyPlugin.ApplySettings()`.
      Плагин облачного подписания также является серверным плагином, поэтому подключается и настраивается аналогично.

9. При необходимости подключите клиентский плагин к Directum RX. Клиентский плагин используется в веб-агенте при работе веб-клиента Directum RX. Для подключения:
    Для Windows:
    * Скопируйте файлы из папки *out\Client* в папку плагинов веб-агента на сервере приложений, например, в  
    ```C:\inetpub\wwwroot\Client\content\WebAgent\plugins\```
    * Запустите утилиту *packages_manifest_updater.exe* из папки *PackagesManifestUpdater* веб-агента на сервере приложений, например, из  
    ```C:\inetpub\wwwroot\Client\content\WebAgent\PackagesManifestUpdater```

    Для Linux и MacOS:
    * Скопируйте файлы из папки *out\Client* в папку плагинов веб-агента на сервере приложений, например, в  
    ```\SungeroWebClient\content\WebAgent\plugins\```
    * Запустите утилиту *packages_manifest_updater.exe* из папки *PackagesManifestUpdater* веб-агента на сервере приложений, например, из  
    ```\SungeroWebClient\content\WebAgent\PackagesManifestUpdater```

    В разных версиях Directum RX путь может отличаться, ориентир - папка WebAgent.

## Особенности формирования и проверки электронной подписи в Directum RX
* Электронная подпись формируется в формате CAdES-BES.
* При подписании сертификатом в веб-агенте само подписание выполняется на стороне клиента, но данные для подписи (подписываемые атрибуты подписи) формируются на сервере приложений.
* Сервер приложений работает в 64-битном окружении, а веб-агент - в 32-битном. Это необходимо учитывать, если для работы плагинов нужны COM-компоненты и их регистрация.
* При необходимости подписание может быть выполнено на стороне сервера. В этом случае закрытый ключ должен быть доступен на сервере.
