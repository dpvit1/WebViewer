1) Установить python 3.9, MSVC 22 с .Net, dotnet 9.0, pip install pythonnet
2) Сделать копию appsetting.json, новый файл с именем appsettings.Development.json. Установить пути до установленного python 3.9
3) Открыть Solution в MSVC 22, провести сборку проекта в Debug.
4) Собрать RGKGLTF, скопировать ее + бинарники RGK в bin/Debug..., т.е в место, откуда запускается API.exe.
5) Запустить проект под отладкой, перейти на http://localhost:5146/swagger и пробовать

## Предварительные действия по настройке проекта и запуск апи
1. ```cd .\app``` установить pythonnet ```pip install pythonnet```
2. добавить библиотеки ```RGK.dll``` и ```RGKPY.pyd``` в ```.\hello_from_python_lib\hello_from_python_lib```
3. ```cd .\hello_from_python_lib``` и выполнить ```pip install .```
4. копировать ```RGK.dll``` и ```RGKPY.pyd``` в ```.\hello_from_python_lib\build\lib\hello_from_python_lib\``` || todo: автоматизировать процессе сборки или предложить поставку в виде либы которая уже содержит данные файлы
5. ```cd .\api\``` создать файл ```appsettings.Development.json``` по аналогии с ```appsettings.json``` указать путь до dll интерпритатора\
6. Запустить проект ```dotnet run --launch-profile "http"```

публикация win:
```dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true```
публикация linux:
```dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true```

## Запуск клиента:
./nginx/
Открыть терминал ```start nginx```
для остановки ```nginx -s stop```
перейти на http://localhost:21999



## Публикация на стенд stestdocs@10.168.250.83
1. собрать апи
  ```dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true```
  ВНИМАНИЕ: не копирует rgkDist из publish скопировать руками
2. собрать клиента
   ```npm run storybook:build```

(доступ на папку 1 раз ```sudo chmod 777 /etc/nginx/sites-available/rgk-storybook/```)
3. скопировать публикацию в на стенд:
   ```scp -r "C:\Projects\RGK\RGK.Viewer-Integration\client\storybook-static\*" stestdocs@10.168.250.83:/etc/nginx/sites-available/rgk-storybook/```
4. конфиг клиента тут: ```/etc/nginx/conf.d/rgk-storibook.conf```
6. скопировать публикацию сервера на стенд:
   ```scp -r "C:\Projects\RGK\RGK.Viewer-Integration\api\bin\Release\net8.0\linux-x64\publish\*" stestdocs@10.168.250.83:~/RGK/api/```
7. Перейти ```~/RGK/api/```
7. давть доступ ```chmod +x API```
8. запустить ```./API```

сайт доступен по адресу: http://10.168.250.83:3006


<!-- 2. Установить виртуальное окружение ```python -m venv rgk_aspnet_converter```
1. Активировать виртуальное окружение ```rgk_aspnet_converter\Scripts\activate```
2. 
3. установить либу в виртуальное окружение ```pip install .``` --> 
<!-- 2. ```cd .\app``` установить pythonnet ```pip install pythonnet``` -->