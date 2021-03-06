﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCheck.Modele
{
    public enum Code
    {
        T0000001,
        T0000002,
        T0000003,
        T0000004,
        T0000005,
        T0000006,
        T0000007,
        T0000008,
        T0000009,
        T0000010,
        T0000011,
        T0000012,
        T0000013,
        T0000014,
        T0000015,
        T0000016,
        T0000017,
        T0000018,
        T0000019,
        T0000020,
        T0000021,
        T0000022,
        T0000023,
        T0000024,
        T0000025,
        T0000026,
        T0000027,
        T0000028,
        T0000029,
        T0000030,
        T0000031,
        T0000032,
        T0000033,
        T0000034,
        T0000035,
        T0000036,
        T0000037,
        T0000038,
        T0000039,
        T0000040,
        T0000041,
        T0000042,
        T0000043,
        T0000044,
        T0000045,
        T0000046,
        T0000047,
        T0000048,
        T0000049,
        T0000050,
        T0000051,
        T0000052,
        T0000053
    }
    public enum TypeMessage
    {
        Warning,
        Error,
        Debug
    }
    public class MyTyps
    {
        public MyTyps(string message) : this(message, TypeMessage.Warning, true)
        {

        }
        public MyTyps(string message, TypeMessage type) : this(message, type, true)
        {

        }
        public MyTyps(string message, bool isDesable) : this(message, TypeMessage.Warning, isDesable)
        {
        }
        public MyTyps(string message, TypeMessage type, bool isDesable)
        {
            Message = message;
            Type = type;
            IsDesable = isDesable;
        }
        public string Message { get; set; }
        public TypeMessage Type { get; set; }
        public bool IsDesable { get; set; }
    }
    public static class DictionaryMessage
    {
        static Dictionary<Code, MyTyps> data = new Dictionary<Code, MyTyps>()
        {
              { Code.T0000001,new MyTyps("Переменная '{0}' ни где не используется ")}
            , { Code.T0000002,new MyTyps("Длина меньше чем должна быть для переменной '{0}'")}
            , { Code.T0000003,new MyTyps("Переменная '{0}' уже объявлена")}
            , { Code.T0000004,new MyTyps("Переменная '{0}' или не объявлена или отсутствует параметр")}
            , { Code.T0000005,new MyTyps("Operand data type nvarchar is invalid for '{0}' operator.")}
            , { Code.T0000006,new MyTyps("System ParseError. Message: '{0}' Line: '{1}'")}
            , { Code.T0000007,new MyTyps("При вставки в таблицу '{0}' не указано полей")}
            , { Code.T0000008,new MyTyps("При вставки в таблицу '{0}' в качестве ресурса не рекомендуется использовать 'select *'")}
            , { Code.T0000009,new MyTyps("В выборке указано больше полей чем в целевой таблице '{0}'")}
            , { Code.T0000010,new MyTyps("В выборке указано меньше полей чем в целевой таблице '{0}'")}
            , { Code.T0000011,new MyTyps("Табличная переменная '{0}' ни где не используется")}
            , { Code.T0000012,new MyTyps("При скалярной выборки не может быть указано несколько полей")}
            , { Code.T0000013,new MyTyps("При скалярной выборки не может быть указано 'select *'")}
            , { Code.T0000014,new MyTyps("Указанный алиас '{0}' не найден")}
            , { Code.T0000015,new MyTyps("Указанный табличная переменная '{0}' не объявлена")}
            , { Code.T0000016,new MyTyps("Указанный временная таблица '{0}' не объявлена")}
            , { Code.T0000017,new MyTyps("Указанный алиас '{0}' уже существует для таблицы '{1}'")}
            , { Code.T0000018,new MyTyps("Указанный название для with '{0}' уже существует")}
            , { Code.T0000019,new MyTyps("Так как у всех таблиц есть алиас, то для указанного поля '{0}' тоже следует указать алиас")}
            , { Code.T0000020,new MyTyps("При сравнении поля '{0}' с NULL указано '{1}', а должно быть IS NULL")}
            , { Code.T0000021,new MyTyps("Поле '{0}' не принимает значение NULL условие не корректно")}
            , { Code.T0000022,new MyTyps("С сервера по объекту '{0}' ожидалось получить таблиц, но получен тип '{1}'")}
            //, { Code.T0000023,new MyTyps("Обект '{0}' с сервера не получен, возможно отсутствует")}
            , { Code.T0000023,new MyTyps("Таблица '{0}' не существует")}
            , { Code.T0000024,new MyTyps("Ошбики при обращении к серверу по таблице '{1}': '{0}'")}
            , { Code.T0000025,new MyTyps("Вероятно ошибка указания сравнения одной и той же таблицы - '{0}'")}
            , { Code.T0000026,new MyTyps("Вероятно ошибка указания сравнения одной и той же колонки - '{0}'")}
            , { Code.T0000027,new MyTyps("Ссылка на таблицу не может быть более чем из 2 частей - '{0}'")}
          //, { Code.T0000028,new MyTyps("Таблицы '{0}' на сервере не существует")}
            , { Code.T0000029,new MyTyps("Указанного поля '{1}' для таблицы '{0}' не найдено")}
            , { Code.T0000030,new MyTyps("Указанное поле '{0}' без алиас не найдено ни в одной из используемых таблиц")}
            , { Code.T0000031,new MyTyps("При сравнении не указан алиас для одного из полей '{0}'")}
            , { Code.T0000032,new MyTyps("Сравнение одного и того же поля '{0}'")}
            , { Code.T0000033,new MyTyps("Указанное поля '{0}' существует в более чем одной таблице")}
            , { Code.T0000034,new MyTyps("При скалярной выборке желательно указать TOP 1")}
            , { Code.T0000035,new MyTyps("При скалярной выборке в аргументе TOP должно быть указано одно значение, а указано '{0}'")}
            , { Code.T0000036,new MyTyps("При скалярной выборке в аргументе TOP указан тип '{0}' вместо целого числа ")}
            , { Code.T0000037,new MyTyps("При скалярной выборке отсутствует условие")}
            , { Code.T0000038,new MyTyps("Табличная переменная {0} уже объявлена")}
            , { Code.T0000039,new MyTyps("Временная таблица {0} уже была создана")}
            , { Code.T0000040,new MyTyps("Таблица {0} создается повторно")}
            , { Code.T0000041,new MyTyps("Перед созданием новой таблицы {0} отсутствует удаление")}
            , { Code.T0000042,new MyTyps("Все параметры для функции object_id должны быть текстовыми")}
            , { Code.T0000043,new MyTyps("Не удалось получить идентификатор из функции object_id параметра - '{0}' ")}
            , { Code.T0000044,new MyTyps("При удалении объекта '{0}' отсутствует проверка на существование ")}
            , { Code.T0000045,new MyTyps("Параметр '{0}' нигде не используется")}
            , { Code.T0000046,new MyTyps("После Alter table '{0}' для Update следует разделить на Batch")}
            , { Code.T0000047,new MyTyps("Для агрегатной функции {0} не стоит использовать константное значение {1}")}
            , { Code.T0000048,new MyTyps("Указанная таблица '{0}' не существует")}
            , { Code.T0000049,new MyTyps("Указанное поле '{0}' отсутствует в таблице '{1}'")}
            , { Code.T0000050,new MyTyps("Таблица или указанный алиас '{0}' для update отсутствует в выборке from")}
            , { Code.T0000051,new MyTyps("В предложении Order by не слудует использовать константы")}
            , { Code.T0000052,new MyTyps("При Update для целевой таблицы следует использовать алиас '{0}'")}

        };
        public static MyTyps GetMessage(Code Code)
        {
            return data[Code];
        }

    }
}
