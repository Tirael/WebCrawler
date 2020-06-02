[![Build status](https://img.shields.io/appveyor/ci/tirael/WebCrawler/master)](https://ci.appveyor.com/project/tirael/WebCrawler/branch/master)
[![CodeFactor](https://www.codefactor.io/repository/github/tirael/WebCrawler/badge/master)](https://www.codefactor.io/repository/github/tirael/WebCrawler/overview/master)

# Задание

Написать web-crawler - консольное приложение, которое на вход получает:
- url входа,
- степень параллелизма (кол-во одновременно обрабатываемых url'ов)

Результат сохраняет в файл в виде:
url: content-type, response length

Учитывать возможность выделения кода в компонент, который будет встраиваться в другие приложения и тестируемость.

# Реализация консольного приложения
Аргументы командной строки:
--urls 'url1' 'url2' --max-degree-of-parallelism 1 --output-file result.txt
где:
--urls - список url через пробел
--max-degree-of-parallelism - количество одновременно обрабатываемых url'ов
--output-file - файл с результатом